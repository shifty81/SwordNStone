# Quick Start: Metadata Error Prevention

## TL;DR - The 3-Step Process

### Before ANY changes:
```bash
./pre-build-validation.sh    # Linux/Mac
pre-build-validation.bat      # Windows
```

### Before committing:
```bash
# Install pre-commit hook (one-time)
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit

# The hook runs automatically on git commit
```

### After building:
```bash
./post-build-validation.sh Debug    # Linux/Mac
post-build-validation.bat Debug     # Windows
```

## What This Prevents

✅ **CS0006:** Metadata file 'SwordAndStoneLib.dll' could not be found  
✅ **CiTo Errors:** BuildCito.bat exited with code -1  
✅ **Type Errors:** The type or namespace name '...' could not be found  
✅ **Missing Packages:** Referenced component 'protobuf-net' could not be found  

## When to Use

### Use Pre-Build Validation:
- ✓ After pulling changes from Git
- ✓ Before starting a new feature
- ✓ After modifying .ci.cs files
- ✓ When build errors occur
- ✓ Before committing

### Use Post-Build Validation:
- ✓ After building
- ✓ Before running the application
- ✓ Before creating a release
- ✓ To verify DLLs are generated

### Use Pre-Commit Hook:
- ✓ Always (it runs automatically)

## Common Scenarios

### Scenario 1: Adding a New Feature

```bash
# 1. Validate starting state
./pre-build-validation.sh

# 2. Write your code
# Add new files to .csproj as you create them

# 3. Test build
msbuild SwordAndStone.sln /p:Configuration=Debug

# 4. Validate output
./post-build-validation.sh Debug

# 5. Commit (pre-commit hook runs automatically)
git add .
git commit -m "Add new feature"
```

### Scenario 2: Fixing a Build Error

```bash
# 1. Run pre-build validation to diagnose
./pre-build-validation.sh

# 2. Address errors/warnings shown
# - Restore NuGet packages if needed
# - Add missing files to .csproj
# - Fix CiTo syntax issues

# 3. Clean and rebuild
msbuild SwordAndStone.sln /t:Clean
msbuild SwordAndStone.sln /p:Configuration=Debug

# 4. Verify fix worked
./post-build-validation.sh Debug
```

### Scenario 3: Modifying .ci.cs Files

```bash
# 1. Before editing, check current state
./pre-build-validation.sh

# 2. Edit the .ci.cs file
# REMEMBER: Extract complex expressions to variables
# Example:
#   int textureId = array[index];
#   IntRef.Create(textureId);  # Not: IntRef.Create(array[index])

# 3. Test immediately
./BuildCito.sh  # Test transpilation
msbuild SwordAndStoneLib/SwordAndStoneLib.csproj

# 4. If successful, validate
./post-build-validation.sh Debug
```

## What Each Script Does

### pre-build-validation.sh/bat
**Duration:** ~5 seconds  
**Checks:**
- Project files are valid XML
- NuGet packages are present
- Build tools exist (CiTo.exe, CodeGenerator.exe)
- All .ci.cs files are in .csproj
- No complex CiTo expressions

**Output:** Errors (must fix) and Warnings (should fix)

### post-build-validation.sh/bat
**Duration:** ~2 seconds  
**Checks:**
- DLLs were generated (SwordAndStoneLib.dll, etc.)
- EXEs were created (SwordAndStone.exe)
- NuGet DLLs are in output directories
- Generated files exist

**Output:** Verification that build succeeded

### .git-hooks/pre-commit
**Duration:** <1 second  
**Checks:**
- Modified .ci.cs files are in .csproj
- Modified .csproj files are valid XML
- No complex CiTo expressions in changes

**Output:** Blocks commit if errors found

## Interpreting Results

### ✅ Green (OK/Success)
All checks passed. Proceed with confidence.

### ⚠️ Yellow (Warning)
Issue detected but not critical. Review and fix if relevant.

**Example warnings:**
- NuGet packages not found → Run `nuget restore`
- Mono not found → Install if building on Linux/Mac
- Debug symbols not found → Not critical for release builds

### ❌ Red (Error)
Critical issue that will cause build failure. **Must fix before proceeding.**

**Example errors:**
- .ci.cs file not in .csproj → Add to project file
- CiTo.exe not found → Ensure repo is complete
- Project file invalid XML → Fix XML syntax

## Quick Fixes

### "NuGet packages not found"
```bash
nuget restore SwordAndStone.sln
# Or on Linux/Mac:
mono nuget.exe restore SwordAndStone.sln
```

### "File not found in .csproj"
1. Open `SwordAndStoneLib/SwordAndStoneLib.csproj` in editor
2. Find similar `<Compile Include="..." />` lines
3. Add your file:
```xml
<Compile Include="Client\Mods\YourNewFile.ci.cs" />
```

### "Complex nested expression in IntRef.Create"
Change from:
```csharp
IntRef.Create(dataItems.TextureIdForInventory()[blockType])
```

To:
```csharp
int textureId = dataItems.TextureIdForInventory()[blockType];
IntRef.Create(textureId);
```

## Getting Help

If validation fails and you're not sure why:

1. **Read the error message carefully** - They include hints
2. **Check documentation:**
   - [BUILD_VALIDATION_CHECKLIST.md](BUILD_VALIDATION_CHECKLIST.md) - Complete procedures
   - [BUILD_TROUBLESHOOTING.md](BUILD_TROUBLESHOOTING.md) - Specific errors
   - [METADATA_ERROR_PREVENTION_SYSTEM.md](METADATA_ERROR_PREVENTION_SYSTEM.md) - System overview
3. **Run with verbose output:**
   ```bash
   ./pre-build-validation.sh 2>&1 | tee validation.log
   ```
4. **Check GitHub Actions** - CI logs may have more details
5. **Open an issue** - Include validation.log output

## Best Practices

### DO ✅
- Run validation before every build
- Install the pre-commit hook
- Fix errors immediately (don't accumulate)
- Add files to .csproj as soon as you create them
- Extract complex expressions to variables in .ci.cs files
- Clean build regularly

### DON'T ❌
- Skip validation ("it will probably work")
- Bypass pre-commit hook with `--no-verify`
- Commit with known errors
- Use complex nested expressions in CiTo code
- Ignore warnings (they often become errors later)
- Leave build in broken state

## Automation Ideas

### Make it even easier:

**1. Add to your shell profile:**
```bash
# Add to ~/.bashrc or ~/.zshrc
alias build-validate='./pre-build-validation.sh && msbuild SwordAndStone.sln /p:Configuration=Debug && ./post-build-validation.sh Debug'
```

**2. Create a build script:**
```bash
#!/bin/bash
# build-all.sh
set -e
./pre-build-validation.sh
msbuild SwordAndStone.sln /t:Clean
nuget restore SwordAndStone.sln
msbuild SwordAndStone.sln /p:Configuration=Debug
./post-build-validation.sh Debug
echo "✅ Build complete and validated!"
```

**3. IDE integration:**
Configure your IDE to run validation before build.

## Success Metrics

You're doing it right if:
- ✅ Builds succeed on first try
- ✅ No metadata errors (CS0006)
- ✅ CI passes without intervention
- ✅ Validation takes <10 seconds total
- ✅ You catch errors before committing

## One-Time Setup

For new team members or fresh clones:

```bash
# 1. Clone repository
git clone https://github.com/shifty81/SwordNStone.git
cd SwordNStone

# 2. Install pre-commit hook
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit

# 3. Restore packages
nuget restore SwordAndStone.sln

# 4. Validate and build
./pre-build-validation.sh
msbuild SwordAndStone.sln /p:Configuration=Debug
./post-build-validation.sh Debug

# Done! You're ready to develop.
```

---

**Remember:** These scripts are your friends. They catch errors **before** they become problems. Use them liberally!
