#!/bin/bash
# Post-Build Validation Script for SwordNStone
# This script verifies the build completed successfully and all required outputs exist
# Run this after building to verify metadata and DLLs are properly generated

echo "========================================"
echo "SwordNStone Post-Build Validation"
echo "========================================"
echo ""

ERRORS=0
WARNINGS=0
CONFIGURATION=${1:-Debug}

echo "Validating build for Configuration: $CONFIGURATION"
echo ""

# 1. Check SwordAndStoneLib output
echo "[1/6] Checking SwordAndStoneLib output..."
if [ -f "SwordAndStoneLib/bin/$CONFIGURATION/SwordAndStoneLib.dll" ]; then
    echo "  ✓ OK: SwordAndStoneLib.dll found"
else
    echo "  ✗ ERROR: SwordAndStoneLib.dll not found"
    echo "  Path: SwordAndStoneLib/bin/$CONFIGURATION/SwordAndStoneLib.dll"
    ERRORS=$((ERRORS + 1))
fi

if [ -f "SwordAndStoneLib/bin/$CONFIGURATION/SwordAndStoneLib.dll.mdb" ] || \
   [ -f "SwordAndStoneLib/bin/$CONFIGURATION/SwordAndStoneLib.pdb" ]; then
    echo "  ✓ OK: Debug symbols found"
else
    echo "  ⚠ WARNING: Debug symbols not found"
    WARNINGS=$((WARNINGS + 1))
fi
echo ""

# 2. Check ScriptingApi output
echo "[2/6] Checking ScriptingApi output..."
if [ -f "ScriptingApi/bin/$CONFIGURATION/ScriptingApi.dll" ]; then
    echo "  ✓ OK: ScriptingApi.dll found"
else
    echo "  ✗ ERROR: ScriptingApi.dll not found"
    ERRORS=$((ERRORS + 1))
fi
echo ""

# 3. Check SwordAndStone main executable
echo "[3/6] Checking SwordAndStone executable..."
if [ -f "SwordAndStone/bin/$CONFIGURATION/SwordAndStone.exe" ]; then
    echo "  ✓ OK: SwordAndStone.exe found"
else
    echo "  ✗ ERROR: SwordAndStone.exe not found"
    echo "  Path: SwordAndStone/bin/$CONFIGURATION/SwordAndStone.exe"
    ERRORS=$((ERRORS + 1))
fi
echo ""

# 4. Check NuGet package DLLs are copied
echo "[4/6] Checking NuGet package dependencies..."
if [ -f "SwordAndStoneLib/bin/$CONFIGURATION/protobuf-net.dll" ]; then
    echo "  ✓ OK: protobuf-net.dll copied to output"
else
    echo "  ⚠ WARNING: protobuf-net.dll not in output directory"
    echo "  May cause runtime errors"
    WARNINGS=$((WARNINGS + 1))
fi

if [ -f "SwordAndStone/bin/$CONFIGURATION/OpenTK.dll" ]; then
    echo "  ✓ OK: OpenTK.dll copied to output"
else
    echo "  ⚠ WARNING: OpenTK.dll not in output directory"
    echo "  May cause runtime errors"
    WARNINGS=$((WARNINGS + 1))
fi
echo ""

# 5. Check generated files
echo "[5/6] Checking generated files..."
if [ -f "Packet.Serializer.ci.cs" ]; then
    echo "  ✓ OK: Packet.Serializer.ci.cs generated"
    
    # Check for CS0108 fix
    if grep -q "public new int GetType" "Packet.Serializer.ci.cs"; then
        echo "  ✓ OK: CS0108 fix applied"
    else
        echo "  ⚠ WARNING: CS0108 fix not applied"
        echo "  You may see CS0108 warnings in build output"
        WARNINGS=$((WARNINGS + 1))
    fi
else
    echo "  ⚠ WARNING: Packet.Serializer.ci.cs not found"
    echo "  May be generated during build"
    WARNINGS=$((WARNINGS + 1))
fi

if [ -f "cito/output/JsTa/SwordAndStone.js" ]; then
    echo "  ✓ OK: JavaScript transpilation output found"
else
    echo "  ℹ INFO: JavaScript output not found (may be disabled)"
fi
echo ""

# 6. Check for common build issues in output
echo "[6/6] Checking for common build warnings..."

# Check if obj directories exist (successful compilation)
if [ -d "SwordAndStoneLib/obj/$CONFIGURATION" ]; then
    echo "  ✓ OK: SwordAndStoneLib compiled (obj directory exists)"
else
    echo "  ⚠ WARNING: SwordAndStoneLib obj directory not found"
    WARNINGS=$((WARNINGS + 1))
fi

if [ -d "SwordAndStone/obj/$CONFIGURATION" ]; then
    echo "  ✓ OK: SwordAndStone compiled (obj directory exists)"
else
    echo "  ⚠ WARNING: SwordAndStone obj directory not found"
    WARNINGS=$((WARNINGS + 1))
fi
echo ""

# Summary
echo "========================================"
echo "Post-Build Validation Summary"
echo "========================================"
if [ $ERRORS -gt 0 ]; then
    echo "❌ ERRORS: $ERRORS"
    echo "Build did NOT complete successfully!"
fi
if [ $WARNINGS -gt 0 ]; then
    echo "⚠️  WARNINGS: $WARNINGS"
    echo "Build completed but with issues"
fi

if [ $ERRORS -eq 0 ]; then
    if [ $WARNINGS -eq 0 ]; then
        echo "✅ All checks passed! Build successful."
        echo ""
        echo "Output locations:"
        echo "  Library: SwordAndStoneLib/bin/$CONFIGURATION/SwordAndStoneLib.dll"
        echo "  Executable: SwordAndStone/bin/$CONFIGURATION/SwordAndStone.exe"
    else
        echo "✅ Build completed with warnings"
    fi
else
    echo "❌ Build validation FAILED!"
    echo "Common causes:"
    echo "  1. Pre-build event failed (BuildCito.sh)"
    echo "  2. Missing NuGet packages"
    echo "  3. Compilation errors"
    echo "  4. Missing project references"
    echo ""
    echo "Run ./pre-build-validation.sh to diagnose"
fi

echo ""
if [ $ERRORS -gt 0 ]; then
    exit 1
fi
exit 0
