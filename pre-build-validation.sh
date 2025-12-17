#!/bin/bash
# Pre-Build Validation Script for SwordNStone
# This script validates the project state before build to catch common metadata errors
# Run this before building or committing changes

# Note: We don't use 'set -e' because we want to collect all errors/warnings
# and exit only at the end with proper error count

echo "========================================"
echo "SwordNStone Pre-Build Validation"
echo "========================================"
echo ""

ERRORS=0
WARNINGS=0

# 1. Validate all .csproj files are well-formed XML
echo "[1/8] Validating project files..."
for proj in SwordAndStoneLib/SwordAndStoneLib.csproj \
            SwordAndStone/SwordAndStone.csproj \
            SwordAndStoneServer/SwordAndStoneServer.csproj \
            ScriptingApi/ScriptingApi.csproj; do
    if [ -f "$proj" ]; then
        if command -v xmllint &> /dev/null; then
            if xmllint --noout "$proj" 2>/dev/null; then
                echo "  ✓ OK: $proj"
            else
                echo "  ✗ ERROR: $proj has invalid XML"
                ERRORS=$((ERRORS + 1))
            fi
        else
            # Basic check without xmllint
            if grep -q "<Project" "$proj" && grep -q "</Project>" "$proj"; then
                echo "  ✓ OK: $proj (basic check)"
            else
                echo "  ✗ ERROR: $proj missing Project tags"
                ERRORS=$((ERRORS + 1))
            fi
        fi
    else
        echo "  ✗ ERROR: $proj not found"
        ERRORS=$((ERRORS + 1))
    fi
done
echo ""

# 2. Check for NuGet packages
echo "[2/8] Checking NuGet packages..."
if [ -d "packages/protobuf-net.2.1.0" ]; then
    echo "  ✓ OK: protobuf-net package found"
else
    echo "  ⚠ WARNING: protobuf-net package not found"
    echo "    Please run: nuget restore SwordAndStone.sln"
    echo "    Or: mono nuget.exe restore SwordAndStone.sln"
    WARNINGS=$((WARNINGS + 1))
fi

if [ -d "packages/OpenTK.2.0.0" ]; then
    echo "  ✓ OK: OpenTK package found"
else
    echo "  ⚠ WARNING: OpenTK package not found"
    echo "    Please run: nuget restore SwordAndStone.sln"
    echo "    Or: mono nuget.exe restore SwordAndStone.sln"
    WARNINGS=$((WARNINGS + 1))
fi
echo ""

# 3. Check for required build tools
echo "[3/8] Checking required build tools..."
if [ -f "CiTo.exe" ]; then
    echo "  ✓ OK: CiTo.exe found"
else
    echo "  ✗ ERROR: CiTo.exe not found in root directory"
    ERRORS=$((ERRORS + 1))
fi

if [ -f "CodeGenerator.exe" ]; then
    echo "  ✓ OK: CodeGenerator.exe found"
else
    echo "  ✗ ERROR: CodeGenerator.exe not found in root directory"
    ERRORS=$((ERRORS + 1))
fi

if [ -f "CitoAssets.exe" ]; then
    echo "  ✓ OK: CitoAssets.exe found"
else
    echo "  ⚠ WARNING: CitoAssets.exe not found in root directory"
    WARNINGS=$((WARNINGS + 1))
fi

# Check for Mono
if command -v mono &> /dev/null; then
    echo "  ✓ OK: Mono found ($(mono --version | head -n1))"
else
    echo "  ⚠ WARNING: Mono not found - required for building on Linux/Mac"
    echo "    Install with: sudo apt-get install mono-complete"
    WARNINGS=$((WARNINGS + 1))
fi
echo ""

# 4. Check for critical source files
echo "[4/8] Checking critical source files..."
if [ -f "Packet.proto" ]; then
    echo "  ✓ OK: Packet.proto found"
else
    echo "  ✗ ERROR: Packet.proto not found"
    ERRORS=$((ERRORS + 1))
fi

if [ -f "BuildCito.sh" ]; then
    echo "  ✓ OK: BuildCito.sh found"
    if [ -x "BuildCito.sh" ]; then
        echo "  ✓ OK: BuildCito.sh is executable"
    else
        echo "  ⚠ WARNING: BuildCito.sh is not executable"
        echo "    Run: chmod +x BuildCito.sh"
        WARNINGS=$((WARNINGS + 1))
    fi
else
    echo "  ✗ ERROR: BuildCito.sh not found"
    ERRORS=$((ERRORS + 1))
fi
echo ""

# 5. Validate that all .ci.cs files are included in SwordAndStoneLib.csproj
echo "[5/8] Validating .ci.cs file references in project..."
CI_FILES_MISSING=0
while IFS= read -r -d '' file; do
    filename=$(basename "$file")
    # Skip Packet.Serializer.ci.cs as it's generated
    if [ "$filename" != "Packet.Serializer.ci.cs" ]; then
        if ! grep -q "$filename" "SwordAndStoneLib/SwordAndStoneLib.csproj"; then
            echo "  ⚠ WARNING: $filename not found in SwordAndStoneLib.csproj"
            WARNINGS=$((WARNINGS + 1))
            CI_FILES_MISSING=1
        fi
    fi
done < <(find SwordAndStoneLib -name "*.ci.cs" -type f -print0)

if [ $CI_FILES_MISSING -eq 0 ]; then
    echo "  ✓ OK: All .ci.cs files referenced in project"
fi
echo ""

# 6. Check for common CiTo syntax issues
echo "[6/8] Scanning for common CiTo syntax issues..."
CITO_ISSUES=0

# Check for 'ref' parameters in method calls (basic check)
# Note: This is a simple heuristic and may have false positives
ref_check=$(grep -rn "(.*ref\s\+\w" SwordAndStoneLib/Client/*.ci.cs 2>/dev/null | grep -v "//" || true)
if [ -n "$ref_check" ]; then
    echo "  ⚠ WARNING: Found potential 'ref' parameter usage - may cause CiTo errors"
    echo "    CiTo has limited support for ref parameters"
    WARNINGS=$((WARNINGS + 1))
    CITO_ISSUES=1
fi

# Check for complex nested expressions with array indexing in IntRef.Create
# More specific pattern to avoid false positives
if grep -rn "IntRef\.Create([^)]*\[[^]]*\]" SwordAndStoneLib/Client/*.ci.cs 2>/dev/null | grep -v "//" ; then
    echo "  ⚠ WARNING: Found array indexing within IntRef.Create"
    echo "    Consider extracting to intermediate variables"
    echo "    Example: int val = array[index]; IntRef.Create(val);"
    WARNINGS=$((WARNINGS + 1))
    CITO_ISSUES=1
fi

if [ $CITO_ISSUES -eq 0 ]; then
    echo "  ✓ OK: No obvious CiTo syntax issues found"
fi
echo ""

# 7. Check if previous build artifacts exist
echo "[7/8] Checking for previous build artifacts..."
if [ -f "SwordAndStoneLib/bin/Debug/SwordAndStoneLib.dll" ]; then
    echo "  ✓ OK: Previous build found - incremental build possible"
else
    echo "  ℹ INFO: No previous build found - full build required"
fi
echo ""

# 8. Validate Packet.Serializer.ci.cs if it exists (should be generated)
echo "[8/8] Checking generated files..."
if [ -f "Packet.Serializer.ci.cs" ]; then
    # Check if it has the 'new' keyword fix applied
    if grep -q "public new int GetType" "Packet.Serializer.ci.cs"; then
        echo "  ✓ OK: Packet.Serializer.ci.cs exists and has CS0108 fix"
    else
        echo "  ⚠ WARNING: Packet.Serializer.ci.cs exists but may need CS0108 fix"
        WARNINGS=$((WARNINGS + 1))
    fi
else
    echo "  ℹ INFO: Packet.Serializer.ci.cs not found - will be generated during build"
fi
echo ""

# Summary
echo "========================================"
echo "Validation Summary"
echo "========================================"
if [ $ERRORS -gt 0 ]; then
    echo "❌ ERRORS: $ERRORS"
    echo "Build will likely FAIL - please fix errors first"
fi
if [ $WARNINGS -gt 0 ]; then
    echo "⚠️  WARNINGS: $WARNINGS"
    echo "Build may have issues - review warnings"
fi

if [ $ERRORS -eq 0 ]; then
    if [ $WARNINGS -eq 0 ]; then
        echo "✅ All checks passed! Ready to build."
    else
        echo "✅ Validation passed with warnings"
        echo "Consider running: mono nuget.exe restore SwordAndStone.sln"
    fi
fi

echo ""
echo "Next steps:"
echo "  1. Fix any ERRORS before building"
echo "  2. Address WARNINGS if possible"
echo "  3. Run: ./build.sh"
echo "  4. Or: xbuild SwordAndStone.sln /p:Configuration=Debug"
echo ""

if [ $ERRORS -gt 0 ]; then
    exit 1
fi
exit 0
