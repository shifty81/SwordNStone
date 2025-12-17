# Build Validation Checklist

This checklist helps prevent metadata errors and build failures. **Follow this checklist whenever you:**
- Add new features
- Modify existing features
- Add or modify .ci.cs files
- Update project dependencies
- Before committing changes
- Before creating a pull request

## Quick Start

Run the automated validation script before building:

**Windows:**
```cmd
pre-build-validation.bat
```

**Linux/Mac:**
```bash
./pre-build-validation.sh
```

## Manual Validation Steps

### 1. Project File Validation ✓

**Before adding/removing any source files:**

- [ ] All new .cs and .ci.cs files are added to the appropriate .csproj file
- [ ] Removed files are also removed from .csproj files
- [ ] Project files (.csproj) are well-formed XML (no unclosed tags)
- [ ] Build action is set correctly (Compile for .cs files)

**How to verify:**
```cmd
# Windows
pre-build-validation.bat

# Linux/Mac
./pre-build-validation.sh
```

**Common mistake:** Adding a new file in the file system but forgetting to include it in the .csproj file leads to CS0246 "type or namespace not found" errors.

### 2. NuGet Package Validation ✓

**Before building:**

- [ ] NuGet packages are restored
- [ ] protobuf-net (v2.1.0) is in packages/ directory
- [ ] OpenTK (v2.0.0) is in packages/ directory
- [ ] No missing package warnings in build output

**How to fix:**
```cmd
# Restore all packages
nuget restore SwordAndStone.sln

# Verify packages exist
dir packages\protobuf-net.2.1.0
dir packages\OpenTK.2.0.0
```

**Common mistake:** Cloning the repo and building without running `nuget restore` first leads to "referenced component could not be found" warnings and metadata errors.

### 3. CiTo Transpilation Validation ✓

**Before modifying .ci.cs files:**

- [ ] No complex nested expressions in IntRef.Create() calls
- [ ] Array indexing operations extracted to intermediate variables when used with IntRef.Create()
- [ ] No unsupported 'ref' parameters in method calls (CiTo limitation)
- [ ] All method calls have matching parentheses and semicolons
- [ ] No lambda expressions with complex logic (CiTo limitation)

**Example of what to avoid:**
```csharp
// ❌ BAD - Complex nested array expression
game.Draw2dTexture(texture, x, y, w, h, 
    IntRef.Create(dataItems.TextureIdForInventory()[blockType]),  // CiTo may fail
    params...);

// ✅ GOOD - Extract to intermediate variable
int textureId = dataItems.TextureIdForInventory()[blockType];
game.Draw2dTexture(texture, x, y, w, h, 
    IntRef.Create(textureId),
    params...);
```

**How to verify:**
Run BuildCito.bat/sh manually to check for errors:
```cmd
# Windows
BuildCito.bat

# Linux/Mac
./BuildCito.sh
```

**Common mistake:** Using complex expressions directly in IntRef.Create() causes "Expected CiClassPtrType, got CiArrayPtrType" errors.

### 4. Build Order Validation ✓

**Correct build order:**

1. **First:** SwordAndStoneLib project (depends on: ScriptingApi, NuGet packages)
2. **Second:** SwordAndStone project (depends on: SwordAndStoneLib)
3. **Third:** SwordAndStoneServer project (depends on: SwordAndStoneLib)

**How to verify:**
```cmd
# Build library first
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug

# Verify DLL was created
dir SwordAndStoneLib\bin\Debug\SwordAndStoneLib.dll

# Then build main project
msbuild SwordAndStone\SwordAndStone.csproj /p:Configuration=Debug
```

**Common mistake:** Building SwordAndStone before SwordAndStoneLib completes leads to CS0006 "Metadata file 'SwordAndStoneLib.dll' could not be found".

### 5. Code Quality Checks ✓

**Before committing:**

- [ ] No CS0108 warnings (member hiding without 'new' keyword)
- [ ] No CS0649 warnings (unassigned fields) unless intentional
- [ ] No CS0219 warnings (unused variables)
- [ ] No CS0414 warnings (unused private fields)
- [ ] No CS0618 warnings (obsolete member usage)
- [ ] Code follows existing formatting conventions

**How to verify:**
```cmd
# Build with verbose warnings
msbuild SwordAndStone.sln /p:Configuration=Debug /v:normal

# Check for warnings in output
```

**Automated fix for CS0108 in generated code:**
The BuildCito scripts automatically apply fixes to Packet.Serializer.ci.cs for CS0108 warnings.

### 6. Generated Files Validation ✓

**After running BuildCito:**

- [ ] Packet.Serializer.ci.cs was generated
- [ ] CS0108 fix was applied (contains "public new int GetType()")
- [ ] cito/output/JsTa directory was created
- [ ] No transpilation errors in console output

**How to verify:**
```cmd
# Check for generated file
dir Packet.Serializer.ci.cs

# Check for CS0108 fix
findstr /C:"public new int GetType" Packet.Serializer.ci.cs
```

### 7. Dependency Validation ✓

**When adding new dependencies:**

- [ ] Update packages.config with new package
- [ ] Update .csproj with package reference
- [ ] Run nuget restore
- [ ] Document the new dependency in README.md or BUILD.md

**How to add a package:**
```cmd
# Add package reference
nuget install PackageName -Version X.Y.Z -OutputDirectory packages

# Update .csproj manually or via Visual Studio
# Add <Reference Include="PackageName"> section
```

### 8. Clean Build Validation ✓

**Before final commit:**

- [ ] Perform a clean build
- [ ] Delete bin/ and obj/ directories
- [ ] Run nuget restore
- [ ] Build entire solution
- [ ] Verify all projects build successfully
- [ ] Run tests (if available)

**How to perform:**
```cmd
# Clean everything
for /d /r . %d in (bin,obj) do @if exist "%d" rd /s/q "%d"

# Restore packages
nuget restore SwordAndStone.sln

# Build solution
msbuild SwordAndStone.sln /p:Configuration=Debug

# Verify output
dir SwordAndStoneLib\bin\Debug\SwordAndStoneLib.dll
dir SwordAndStone\bin\Debug\SwordAndStone.exe
```

## CI/CD Integration

The GitHub Actions workflow automatically validates:
- Project file completeness
- XML file validity
- Code quality checks
- Configuration file presence

**See:** `.github/workflows/build-validation.yml`

## Pre-Commit Hook (Recommended)

Install the pre-commit hook to automatically validate before each commit:

**Windows:**
```cmd
copy .git-hooks\pre-commit .git\hooks\pre-commit
```

**Linux/Mac:**
```bash
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

## Troubleshooting Common Issues

### "Metadata file could not be found" (CS0006)

**Root causes:**
1. NuGet packages not restored → Run `nuget restore`
2. Dependency project failed to build → Check build output for errors in SwordAndStoneLib
3. Build order issue → Build SwordAndStoneLib first
4. CiTo transpilation failed → Check BuildCito.bat/sh output

**Fix:**
```cmd
# Clean and rebuild
msbuild SwordAndStone.sln /t:Clean
nuget restore SwordAndStone.sln
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug
msbuild SwordAndStone.sln /p:Configuration=Debug
```

### "BuildCito.bat exited with code -1"

**Root causes:**
1. CiTo syntax error in .ci.cs file
2. Missing CiTo.exe or CodeGenerator.exe
3. Complex expression in IntRef.Create()

**Fix:**
1. Run BuildCito.bat manually to see detailed error
2. Check recent changes to .ci.cs files
3. Extract complex expressions to intermediate variables
4. Review BuildCito.bat output for specific file and line number

### "Referenced component could not be found"

**Root causes:**
1. NuGet packages not restored
2. Package version mismatch
3. Corrupted packages folder

**Fix:**
```cmd
# Delete and restore packages
rmdir /s /q packages
nuget restore SwordAndStone.sln
```

### "Expected CiClassPtrType, got CiArrayPtrType"

**Root cause:**
Complex nested expression with array indexing in IntRef.Create()

**Fix:**
```csharp
// Before (causes error):
IntRef.Create(dataItems.TextureIdForInventory()[blockType])

// After (works):
int textureId = dataItems.TextureIdForInventory()[blockType];
IntRef.Create(textureId)
```

## Best Practices

### ✅ Do

- Run pre-build-validation before every build
- Extract complex expressions to intermediate variables
- Keep .csproj files synchronized with file system
- Restore NuGet packages after pulling changes
- Build SwordAndStoneLib before dependent projects
- Clean build before committing
- Test builds locally before pushing

### ❌ Don't

- Commit without running validation scripts
- Add files without updating .csproj
- Use complex nested expressions in CiTo code
- Build dependent projects before dependencies
- Ignore build warnings
- Commit with build errors
- Push untested changes

## Automation

To make this checklist automatic:

1. **Add to pre-commit hook:** Automatically validate before commit
2. **Add to CI/CD:** GitHub Actions runs validation on every push
3. **Add to IDE:** Configure Visual Studio/VS Code to run validation on save
4. **Add to build script:** Integrate validation into build.bat/sh

## Reference Documents

- [BUILD.md](BUILD.md) - Complete build instructions
- [BUILD_TROUBLESHOOTING.md](BUILD_TROUBLESHOOTING.md) - Detailed troubleshooting guide
- [BUILD_ERROR_PREVENTION.md](BUILD_ERROR_PREVENTION.md) - Prevention strategies
- [.github/workflows/build-validation.yml](.github/workflows/build-validation.yml) - CI validation

## Summary

**Before every change:**
1. ✓ Run pre-build-validation script
2. ✓ Verify NuGet packages are restored
3. ✓ Check .csproj includes all source files

**After making changes:**
1. ✓ Run BuildCito to test transpilation
2. ✓ Build SwordAndStoneLib first
3. ✓ Build entire solution
4. ✓ Check for warnings and errors
5. ✓ Perform clean build test

**Before committing:**
1. ✓ Run full validation checklist
2. ✓ Clean build from scratch
3. ✓ Document any new dependencies
4. ✓ Update relevant documentation

---

**Remember:** The metadata errors occur because of broken build dependencies. By following this checklist, we ensure all dependencies are met **before** building, preventing cascading failures.
