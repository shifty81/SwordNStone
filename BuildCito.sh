#!/bin/bash
# Enhanced BuildCito.sh with error handling and validation

set -e  # Exit on error

echo "========================================"
echo "BuildCito - CiTo Transpilation Script"
echo "========================================"

# 1. Validate prerequisites
echo "[1/5] Checking prerequisites..."
if [ ! -f "CodeGenerator.exe" ]; then
    echo "ERROR: CodeGenerator.exe not found"
    exit 1
fi
if [ ! -f "CiTo.exe" ]; then
    echo "ERROR: CiTo.exe not found"
    exit 1
fi
if [ ! -f "Packet.proto" ]; then
    echo "ERROR: Packet.proto not found"
    exit 1
fi
if ! command -v mono &> /dev/null; then
    echo "ERROR: Mono not found - required for building"
    echo "Install with: sudo apt-get install mono-complete"
    exit 1
fi
echo "OK: All required tools found"

# 2. Generate ProtoBuf files
echo "[2/5] Generating ProtoBuf files..."
mono CodeGenerator.exe Packet.proto
if [ $? -ne 0 ]; then
    echo "ERROR: CodeGenerator failed"
    exit 1
fi
echo "OK: ProtoBuf files generated"

# 3. Clean cito output directory
echo "[3/5] Cleaning output directory..."
if [ -d "cito/output" ]; then
    rm -rf cito/output
fi

# Create output directories
mkdir -p cito/output/JsTa
echo "OK: Output directories prepared"

# 4. Collect source files
echo "[4/5] Collecting source files..."
FILE_COUNT=$(find SwordAndStoneLib/Client SwordAndStoneLib/Common -name "*.ci.cs" 2>/dev/null | wc -l)
echo "OK: Found $FILE_COUNT source files"

# 5. Compile JavaScript files
echo "[5/5] Running CiTo transpilation..."
echo "  - Generating Assets..."
mono CitoAssets.exe data Assets.ci.cs
if [ $? -ne 0 ]; then
    echo "ERROR: CitoAssets failed"
    exit 1
fi

echo "  - Transpiling Assets.js..."
mono CiTo.exe -D CITO -D JS -D JSTA -l js-ta -o cito/output/JsTa/Assets.js Assets.ci.cs
if [ $? -ne 0 ]; then
    echo "ERROR: CiTo transpilation of Assets.js failed"
    echo "Check Assets.ci.cs for syntax errors"
    exit 1
fi

echo "  - Transpiling SwordAndStone.js..."
mono CiTo.exe -D CITO -D JS -D JSTA -l js-ta -o cito/output/JsTa/SwordAndStone.js \
    $(find SwordAndStoneLib/Client -name "*.ci.cs") \
    $(find SwordAndStoneLib/Common -name "*.ci.cs") \
    Packet.Serializer.ci.cs
if [ $? -ne 0 ]; then
    echo "ERROR: CiTo transpilation of SwordAndStone.js failed"
    echo "Check .ci.cs files for syntax errors"
    echo "Common issues:"
    echo "  - Complex nested expressions in IntRef.Create"
    echo "  - Unsupported 'ref' parameter usage"
    echo "  - Missing semicolons or mismatched braces"
    exit 1
fi
echo "OK: CiTo transpilation successful"

# Copy skeleton files
echo "Copying platform skeleton files..."
cp -r cito/platform/JsTa/* cito/output/JsTa/

# Fix CS0108 warnings in generated file by adding 'new' keyword to GetType() methods
# This is done AFTER CiTo transpilation because CiTo doesn't support the 'new' keyword
echo "Applying CS0108 fix to Packet.Serializer.ci.cs..."
sed -i 's/^\([[:space:]]*\)public int GetType()/\1public new int GetType()/' Packet.Serializer.ci.cs
if [ $? -ne 0 ]; then
    echo "WARNING: CS0108 fix failed - you may see compiler warnings"
fi

echo "========================================"
echo "BuildCito completed successfully"
echo "========================================"
exit 0
