#!/bin/bash
# Build validation script for SwordNStone project
# Run this before committing to catch common issues

set -e

echo "=========================================="
echo "SwordNStone Build Validation"
echo "=========================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

errors=0
warnings=0

# Function to print colored messages
error() {
    echo -e "${RED}❌ ERROR: $1${NC}"
    errors=$((errors + 1))
}

warning() {
    echo -e "${YELLOW}⚠️  WARNING: $1${NC}"
    warnings=$((warnings + 1))
}

success() {
    echo -e "${GREEN}✅ $1${NC}"
}

info() {
    echo "ℹ️  $1"
}

# 1. Check project file completeness
echo "1. Checking project file references..."
for csproj in $(find . -name "*.csproj" -not -path "*/bin/*" -not -path "*/obj/*"); do
    info "Checking $csproj..."
    dir=$(dirname "$csproj")
    
    # Find all .cs files
    while IFS= read -r csfile; do
        # Skip generated and system files
        if [[ "$csfile" == *"Packet.Serializer"* ]] || \
           [[ "$csfile" == *"AssemblyInfo"* ]] || \
           [[ "$csfile" == *"obj/"* ]] || \
           [[ "$csfile" == *"bin/"* ]] || \
           [[ "$csfile" == *"Assets.ci.cs"* ]]; then
            continue
        fi
        
        filename=$(basename "$csfile")
        
        # Check if file is in csproj
        if ! grep -q "$filename" "$csproj"; then
            error "$csfile not found in $csproj"
        fi
    done < <(find "$dir" -name "*.cs" -o -name "*.ci.cs" 2>/dev/null | grep -v "/bin/" | grep -v "/obj/")
done

if [ $errors -eq 0 ]; then
    success "All source files are referenced in project files"
fi
echo ""

# 2. Check for common code issues
echo "2. Checking for common code issues..."

# Check for unused variables (simple pattern)
info "Checking for potential unused variables..."
unused_vars=$(grep -rn "int .* = .*;" --include="*.cs" --include="*.ci.cs" \
    --exclude-dir=bin --exclude-dir=obj --exclude-dir=packages \
    | grep -v "return" | grep -v "//" | wc -l)

if [ "$unused_vars" -gt 0 ]; then
    warning "Found $unused_vars potential unused variable declarations"
fi

# Check for TODO comments
info "Checking for TODO/FIXME comments..."
todo_count=$(grep -rn "TODO\|FIXME" --include="*.cs" --include="*.ci.cs" \
    --exclude-dir=bin --exclude-dir=obj --exclude-dir=packages \
    2>/dev/null | wc -l || echo "0")

if [ "$todo_count" -gt 0 ]; then
    warning "Found $todo_count TODO/FIXME comments"
fi

success "Code quality checks completed"
echo ""

# 3. Check for required files
echo "3. Checking required configuration files..."

check_file() {
    if [ -f "$1" ]; then
        success "$1 exists"
    else
        warning "$1 not found"
    fi
}

check_file ".editorconfig"
check_file ".gitignore"
check_file "README.md"
echo ""

# 4. Validate XML files
echo "4. Validating XML configuration files..."
xml_valid=true
for xml in $(find . -name "*.csproj" -o -name "*.config" -o -name "*.sln" \
    -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/packages/*"); do
    if command -v xmllint &> /dev/null; then
        if ! xmllint --noout "$xml" 2>/dev/null; then
            error "Invalid XML: $xml"
            xml_valid=false
        fi
    fi
done

if [ "$xml_valid" = true ]; then
    success "All XML files are valid"
fi
echo ""

# 5. Check for NuGet packages
echo "5. Checking NuGet package configuration..."
if [ -d "packages" ]; then
    success "Packages directory exists"
else
    warning "Packages directory not found - run 'nuget restore' or build in Visual Studio"
fi

for pkg_config in $(find . -name "packages.config" -not -path "*/bin/*" -not -path "*/obj/*"); do
    info "Found package config: $pkg_config"
done
echo ""

# 6. Summary
echo "=========================================="
echo "Validation Summary"
echo "=========================================="
if [ $errors -gt 0 ]; then
    echo -e "${RED}❌ Found $errors error(s)${NC}"
fi

if [ $warnings -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Found $warnings warning(s)${NC}"
fi

if [ $errors -eq 0 ] && [ $warnings -eq 0 ]; then
    echo -e "${GREEN}✅ All checks passed!${NC}"
fi

echo ""
echo "Next steps:"
echo "  1. Fix any errors before committing"
echo "  2. Review warnings and address if needed"
echo "  3. Run full build in Visual Studio or with msbuild"
echo "  4. Run tests to ensure functionality"

exit $errors
