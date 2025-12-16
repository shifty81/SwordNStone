# Build Error Prevention Guide

This document outlines strategies to prevent the types of build warnings and errors that were recently fixed.

## 1. Missing Project References

### Problem
Files existed in the repository but weren't included in the `.csproj` file, causing CS0246 errors.

### Solutions

#### A. Use Glob Patterns in .csproj (Recommended for .NET Core/5+)
```xml
<ItemGroup>
  <!-- Include all .cs files in Client/Mods directory -->
  <Compile Include="Client\Mods\*.ci.cs" />
</ItemGroup>
```

#### B. Pre-commit Hook to Validate
Create `.git/hooks/pre-commit`:
```bash
#!/bin/bash
# Check for .cs files not in .csproj

for csproj in $(find . -name "*.csproj"); do
  dir=$(dirname "$csproj")
  
  # Find all .cs files
  for csfile in $(find "$dir" -name "*.cs" -o -name "*.ci.cs"); do
    rel_path=$(realpath --relative-to="$dir" "$csfile")
    
    # Check if file is referenced in csproj
    if ! grep -q "$rel_path" "$csproj"; then
      echo "WARNING: $csfile not found in $csproj"
      exit 1
    fi
  done
done
```

#### C. CI/CD Build Validation
Add to your CI pipeline (e.g., `.github/workflows/build.yml`):
```yaml
name: Build Validation

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '4.8.x'
      
      - name: Restore packages
        run: nuget restore SwordAndStone.sln
      
      - name: Build solution
        run: msbuild SwordAndStone.sln /p:Configuration=Release /warnaserror
        
      - name: Run tests
        run: dotnet test --no-build --verbosity normal
```

## 2. Enum and Constant Naming Mismatches

### Problem
`Packet_ClientIdEnum.GameResolution` should have been `Packet_ClientIdEnum.ClientGameResolution`

### Solutions

#### A. Use Code Generation with Validation
Ensure your code generator (CodeGenerator.exe) creates consistent naming:
```csharp
// In code generator template
foreach (var enumValue in protoEnum.Values)
{
    // Ensure prefix consistency
    string enumName = protoEnum.Name.StartsWith("Client") 
        ? "Client" + enumValue.Name 
        : enumValue.Name;
}
```

#### B. Add Unit Tests for Generated Code
```csharp
[Test]
public void TestPacketEnumExists()
{
    // Verify enum values exist
    var enumType = typeof(Packet_ClientIdEnum);
    Assert.IsTrue(Enum.IsDefined(enumType, "ClientGameResolution"));
}
```

#### C. Use Static Analysis
Add to `.editorconfig`:
```ini
[*.cs]
# Require consistent naming
dotnet_diagnostic.IDE1006.severity = error
```

## 3. Unassigned Field Warnings (CS0649)

### Problem
Many fields were flagged as never assigned, but they were intentionally set via reflection or serialization.

### Solutions

#### A. Document Intent with Attributes (Preferred)
```csharp
using System.Diagnostics.CodeAnalysis;

public class Entity 
{
    // Assigned via serialization/reflection
    [SuppressMessage("CodeQuality", "CS0649", Justification = "Assigned via protobuf deserialization")]
    internal Packet_ServerPlayerStats playerStats;
}
```

#### B. Use Constructor or Property Initialization
```csharp
public class ConnectedPlayer
{
    // Initialize with default values
    internal int id = 0;
    internal string name = string.Empty;
    internal int ping = 0;
}
```

#### C. Global Suppression for Generated Code
Add to `GlobalSuppressions.cs`:
```csharp
[assembly: SuppressMessage("CodeQuality", "CS0649", 
    Scope = "namespaceanddescendants", 
    Target = "~N:SwordAndStone.Generated")]
```

## 4. Obsolete Method Usage (CS0618)

### Problem
Calling deprecated methods like `GetShadowRatioOld()` instead of `GetShadowRatio()`

### Solutions

#### A. Enable Code Analysis Warnings as Errors
In `.csproj`:
```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <!-- Or specific warnings -->
  <WarningsAsErrors>CS0618</WarningsAsErrors>
</PropertyGroup>
```

#### B. Use Roslyn Analyzers
Install `Microsoft.CodeAnalysis.NetAnalyzers`:
```bash
dotnet add package Microsoft.CodeAnalysis.NetAnalyzers
```

#### C. Automated Refactoring
Use tools like Resharper or Rider with "Fix all issues in solution" before commit.

## 5. Unused Variables and Fields (CS0219, CS0414)

### Problem
Variables declared but never used, causing code bloat and confusion.

### Solutions

#### A. Enable IDE Suggestions
In `.editorconfig`:
```ini
[*.cs]
# Unused variables
dotnet_diagnostic.CS0219.severity = error
dotnet_diagnostic.CS0414.severity = error

# IDE analyzer for unused code
dotnet_diagnostic.IDE0051.severity = warning # Unused private members
dotnet_diagnostic.IDE0052.severity = warning # Unused private members
```

#### B. Use Code Cleanup on Save
Configure your IDE to:
- Remove unused imports
- Remove unused variables
- Format code on save

#### C. Pre-commit Linting
Add to `.git/hooks/pre-commit`:
```bash
#!/bin/bash
# Run Roslyn analyzers
dotnet format --verify-no-changes --verbosity diagnostic
if [ $? -ne 0 ]; then
  echo "Code formatting issues detected. Run 'dotnet format' to fix."
  exit 1
fi
```

## 6. Missing NuGet Packages

### Problem
`protobuf-net` and `OpenTK` not found after fresh clone.

### Solutions

#### A. Automatic Package Restore (Recommended)
Enable in Visual Studio: Tools → Options → NuGet Package Manager → General → "Allow NuGet to download missing packages"

#### B. Add Package Restore to Build Scripts
Create `build.bat`:
```batch
@echo off
echo Restoring NuGet packages...
nuget restore SwordAndStone.sln

echo Building solution...
msbuild SwordAndStone.sln /p:Configuration=Release

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b 1
)

echo Build successful!
```

#### C. Document in README
Add to `README.md`:
```markdown
## Building the Project

1. Clone the repository
2. Restore NuGet packages: `nuget restore SwordAndStone.sln`
3. Build using Visual Studio or: `msbuild SwordAndStone.sln`
```

#### D. Use PackageReference Instead of packages.config
Migrate to SDK-style projects for automatic restore:
```xml
<ItemGroup>
  <PackageReference Include="protobuf-net" Version="2.1.0" />
  <PackageReference Include="OpenTK" Version="2.0.0" />
</ItemGroup>
```

## 7. Inconsistent Code Hiding (CS0108)

### Problem
Member hiding without explicit `new` keyword.

### Solutions

#### A. Enforce Warning as Error
```xml
<PropertyGroup>
  <WarningsAsErrors>CS0108</WarningsAsErrors>
</PropertyGroup>
```

#### B. Code Review Checklist
Add to `CONTRIBUTING.md`:
- [ ] No unintentional member hiding
- [ ] Use `new` keyword when intentionally hiding
- [ ] Consider using `override` if overriding is intended

## 8. Recommended IDE Setup

### Visual Studio Settings
1. **Enable all code analysis**: Project Properties → Code Analysis → Enable all rules
2. **Treat warnings as errors**: Build → Treat warnings as errors → All
3. **Code cleanup on save**: Tools → Options → Text Editor → C# → Code Style → Enable

### VS Code / Omnisharp Settings
Add to `.vscode/settings.json`:
```json
{
  "csharp.format.enable": true,
  "editor.formatOnSave": true,
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.enableEditorConfigSupport": true
}
```

## 9. Continuous Integration Best Practices

### GitHub Actions Example
```yaml
name: CI Build

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
      
    - name: Restore packages
      run: nuget restore SwordAndStone.sln
      
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
      
    - name: Build with warnings as errors
      run: msbuild SwordAndStone.sln /p:Configuration=Release /p:TreatWarningsAsErrors=true
      
    - name: Run tests
      run: dotnet test --configuration Release --no-build
      
    - name: Upload build artifacts
      uses: actions/upload-artifact@v2
      with:
        name: build-artifacts
        path: |
          **/bin/Release/**
```

## 10. Development Workflow Recommendations

### Daily Development
1. **Pull latest changes**: `git pull`
2. **Restore packages**: Automatic in Visual Studio or `nuget restore`
3. **Build before commit**: Ensure clean build
4. **Run code cleanup**: Format and remove unused code
5. **Review warnings**: Address all warnings before PR

### Before Creating PR
1. ✅ Build succeeds with zero warnings
2. ✅ All tests pass
3. ✅ Code formatted according to `.editorconfig`
4. ✅ No unused code (variables, fields, methods)
5. ✅ All files included in `.csproj`
6. ✅ Documentation updated if needed

### Code Review Checklist
- [ ] No new warnings introduced
- [ ] All project files properly referenced
- [ ] Deprecated APIs not used
- [ ] Appropriate use of `#pragma` for intentional suppressions
- [ ] NuGet packages documented if added

## 11. EditorConfig for Consistency

Create `.editorconfig` in root:
```ini
root = true

[*.cs]
# Code style rules
dotnet_sort_system_directives_first = true
dotnet_style_qualification_for_field = false:warning
dotnet_style_qualification_for_property = false:warning

# Warning levels
dotnet_diagnostic.CS0108.severity = error  # Member hiding
dotnet_diagnostic.CS0219.severity = error  # Unused variable
dotnet_diagnostic.CS0414.severity = error  # Unused field
dotnet_diagnostic.CS0618.severity = error  # Obsolete member
dotnet_diagnostic.CS0649.severity = warning # Unassigned field

# Naming conventions
dotnet_naming_rule.private_members_with_underscore.severity = suggestion
dotnet_naming_rule.private_members_with_underscore.symbols = private_fields
dotnet_naming_rule.private_members_with_underscore.style = prefix_underscore

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.prefix_underscore.capitalization = camel_case
dotnet_naming_style.prefix_underscore.required_prefix = _
```

## Summary

Implementing these practices will:
- ✅ Catch missing project references automatically
- ✅ Prevent enum naming mismatches
- ✅ Reduce false-positive warnings
- ✅ Eliminate unused code
- ✅ Ensure consistent code style
- ✅ Automate package restoration
- ✅ Improve code quality through CI/CD

**Priority Implementation Order:**
1. Set up CI/CD with build validation (catches 80% of issues)
2. Add `.editorconfig` for consistency
3. Enable "Treat Warnings as Errors" in projects
4. Add pre-commit hooks for validation
5. Document build process in README
