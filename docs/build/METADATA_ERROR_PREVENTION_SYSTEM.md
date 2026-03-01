# Metadata Error Prevention System

## Overview

This document describes the comprehensive system implemented to prevent recurring metadata errors in the SwordNStone project. These errors (CS0006, missing DLLs, build failures) have been a persistent issue when adding features or making changes.

## Problem Statement

The recurring pattern:
1. Developer adds a new feature or modifies code
2. Build fails with metadata errors (CS0006: Metadata file 'SwordAndStoneLib.dll' could not be found)
3. Additional cascading errors: missing types, missing dependencies
4. Root cause is often one of:
   - New .ci.cs file not added to .csproj
   - CiTo transpilation error due to complex syntax
   - NuGet packages not restored
   - Build order dependency issue

## Solution Components

### 1. Pre-Build Validation Scripts

**Files:**
- `pre-build-validation.bat` (Windows)
- `pre-build-validation.sh` (Linux/Mac)

**Purpose:** Validate project state BEFORE building to catch issues early.

**Checks performed:**
- ✅ Project files (.csproj) are well-formed XML
- ✅ NuGet packages (protobuf-net, OpenTK) are restored
- ✅ Required build tools (CiTo.exe, CodeGenerator.exe) exist
- ✅ All .ci.cs files are referenced in project files
- ✅ No common CiTo syntax issues (complex nested expressions)
- ✅ Previous build artifacts exist (if applicable)
- ✅ Generated files have required fixes applied

**Usage:**
```bash
# Run before every build
./pre-build-validation.sh    # Linux/Mac
pre-build-validation.bat      # Windows
```

### 2. Enhanced Build Scripts

**Files:**
- `BuildCito.bat` (Windows)
- `BuildCito.sh` (Linux/Mac)

**Enhancements:**
- Step-by-step progress reporting ([1/5], [2/5], etc.)
- Pre-requisite validation before execution
- Detailed error messages with suggestions
- Exit codes for CI integration
- Automatic CS0108 fix application
- File count reporting

**Benefits:**
- Failures are immediately obvious
- Error messages include helpful hints
- Scripts fail fast with clear diagnostics

### 3. Post-Build Validation Scripts

**Files:**
- `post-build-validation.bat` (Windows)
- `post-build-validation.sh` (Linux/Mac)

**Purpose:** Verify build completed successfully and all outputs exist.

**Checks performed:**
- ✅ SwordAndStoneLib.dll was generated
- ✅ SwordAndStone.exe was created
- ✅ NuGet package DLLs are in output directories
- ✅ Debug symbols (.pdb) exist
- ✅ Generated files exist and have required fixes
- ✅ obj/ directories confirm compilation occurred

**Usage:**
```bash
# Run after building
./post-build-validation.sh Debug    # Linux/Mac
post-build-validation.bat Debug      # Windows
```

### 4. Pre-Commit Hooks

**Files:**
- `.git-hooks/pre-commit`
- `.git-hooks/README.md`

**Purpose:** Automatically validate changes before allowing commit.

**Checks performed:**
- ✅ Modified .ci.cs files are in .csproj
- ✅ New .cs files are in .csproj (with warning)
- ✅ Modified .csproj files are valid XML
- ✅ Complex CiTo expressions trigger warnings
- ✅ Reminds to run full validation

**Installation:**
```bash
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

### 5. GitHub Actions CI Validation

**File:** `.github/workflows/build-validation.yml`

**Jobs:**
1. **validate-project-files:** Ensures all source files are in project files
2. **check-code-quality:** Scans for common code issues
3. **check-dependencies:** Validates NuGet configuration and build tools
4. **lint-config:** Validates XML file structure
5. **summary:** Reports overall status

**Benefits:**
- Catches errors before merge
- Prevents broken code from entering main branch
- Provides detailed error messages in PR checks
- Runs automatically on every push and PR

### 6. Comprehensive Documentation

**Files:**
- `BUILD_VALIDATION_CHECKLIST.md` - Complete validation procedures
- `BUILD.md` - Updated with validation workflow
- `BUILD_ERROR_PREVENTION.md` - Existing prevention strategies
- `BUILD_TROUBLESHOOTING.md` - Existing troubleshooting guide
- `.git-hooks/README.md` - Hook installation instructions

**Purpose:** Ensure all developers understand the process.

### 7. Enhanced EditorConfig

**File:** `.editorconfig`

**Additions:**
- Rules for metadata-related errors (CS0246, CS0006)
- Special section for .ci.cs files with CiTo limitations noted
- Stricter warnings and error levels

### 8. Fixed Immediate Issue

**File:** `SwordAndStoneLib/Client/Mods/GuiWoWUnitFrames.ci.cs`

**Problem:** 
```csharp
// This caused: Expected CiClassPtrType, got CiArrayPtrType
IntRef.Create(dataItems.TextureIdForInventory()[blockType])
```

**Solution:**
```csharp
// Extract to intermediate variable
int textureId = dataItems.TextureIdForInventory()[blockType];
IntRef.Create(textureId)
```

## Standard Workflow (Mandatory)

### For Every Feature/Change:

**1. Before Starting:**
```bash
# Ensure environment is ready
./pre-build-validation.sh
nuget restore SwordAndStone.sln  # if warnings about packages
```

**2. While Developing:**
- Add new .cs/.ci.cs files to .csproj **immediately**
- Extract complex expressions in CiTo code to variables
- Follow existing code patterns
- Test build frequently

**3. Before Committing:**
```bash
# Validate changes
./pre-build-validation.sh

# Clean build
msbuild SwordAndStone.sln /t:Clean
msbuild SwordAndStone.sln /p:Configuration=Debug

# Verify output
./post-build-validation.sh Debug

# If all pass, commit
git add .
git commit -m "Your message"
# Pre-commit hook runs automatically
```

**4. After Push:**
- Check GitHub Actions for validation status
- Address any CI failures immediately

## Enforcement

### Automated Enforcement:
1. **Pre-commit hook:** Blocks commits with obvious errors
2. **GitHub Actions:** Fails CI if validation fails
3. **Build scripts:** Exit with error codes for CI

### Team Enforcement:
1. **Code Review:** PRs must pass all validation checks
2. **Documentation:** Clear instructions in BUILD.md
3. **Team Agreement:** Everyone follows the checklist

## Benefits

### Immediate Benefits:
- ✅ Catches missing project references before build
- ✅ Detects CiTo syntax errors before transpilation
- ✅ Validates NuGet packages are restored
- ✅ Prevents commits with known issues

### Long-term Benefits:
- ✅ Reduces "it works on my machine" problems
- ✅ Faster CI builds (fewer failures)
- ✅ Less time debugging metadata errors
- ✅ Smoother onboarding for new developers
- ✅ More reliable releases

## Metrics for Success

Track these metrics to measure effectiveness:

1. **Build Failure Rate:** 
   - Before: High (metadata errors every few changes)
   - Target: Near zero for metadata-related issues

2. **Time to Fix Build:**
   - Before: Hours (debugging cascading errors)
   - Target: Minutes (validation identifies root cause)

3. **CI Success Rate:**
   - Before: Frequent failures on push
   - Target: >95% success rate

4. **Developer Satisfaction:**
   - Before: Frustration with recurring errors
   - Target: Confidence in build system

## Troubleshooting the Validation System

### "Validation script not found"
```bash
# Ensure you're in repo root
cd /path/to/SwordNStone
ls -la pre-build-validation.sh

# Make executable (Linux/Mac)
chmod +x pre-build-validation.sh
```

### "Pre-commit hook not working"
```bash
# Reinstall hook
cp .git-hooks/pre-commit .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit

# Verify installation
ls -la .git/hooks/pre-commit
```

### "False positive warnings"
The validation scripts may show warnings that are not actual issues. Use judgment:
- **Errors:** Must be fixed
- **Warnings:** Review and fix if relevant
- **Info:** Informational only

### "Validation takes too long"
Most validations complete in <5 seconds. If slower:
- Check disk I/O (slow HDD)
- Large number of .ci.cs files
- Antivirus scanning scripts

## Future Enhancements

Potential improvements:

1. **IDE Integration:**
   - Visual Studio extension for validation
   - VS Code task for validation
   - Real-time validation as you type

2. **Automated Fixes:**
   - Auto-add files to .csproj
   - Auto-format code
   - Auto-fix common issues

3. **Better Error Messages:**
   - Link to relevant documentation
   - Suggest specific fixes
   - Show examples

4. **Performance Monitoring:**
   - Track validation speed
   - Optimize slow checks
   - Cache results when possible

## Maintenance

### Keep It Updated:
- Review validation rules when adding new dependencies
- Update documentation when process changes
- Gather feedback from team on pain points

### Continuous Improvement:
- Add new checks as new error patterns emerge
- Refine existing checks based on false positives
- Optimize for speed without sacrificing accuracy

## Conclusion

This system transforms metadata errors from a recurring frustration to a preventable issue. By validating **before** building and committing, we catch errors early when they're easiest to fix.

**Key Principle:** Prevention is better than diagnosis.

The system is:
- ✅ Comprehensive (covers all common causes)
- ✅ Automated (minimal manual intervention)
- ✅ Fast (seconds to validate)
- ✅ Documented (clear instructions)
- ✅ Enforced (CI + hooks + code review)

**Adopt this system as the standard workflow for all changes moving forward.**

---

**Questions or Issues?**
- See [BUILD_VALIDATION_CHECKLIST.md](BUILD_VALIDATION_CHECKLIST.md) for detailed procedures
- See [BUILD_TROUBLESHOOTING.md](BUILD_TROUBLESHOOTING.md) for specific error solutions
- Open a GitHub issue for validation system problems
