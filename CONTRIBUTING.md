# Contributing to SwordNStone

Thank you for your interest in contributing to SwordNStone! This guide will help you get started and ensure your contributions are successful.

## Quick Start

1. **Fork and Clone:**
   ```bash
   git clone https://github.com/YOUR-USERNAME/SwordNStone.git
   cd SwordNStone
   ```

2. **Restore Packages:**
   ```bash
   nuget restore SwordAndStone.sln
   ```

3. **Build:**
   ```bash
   # Windows
   msbuild SwordAndStone.sln /p:Configuration=Debug
   
   # Linux/Mac
   xbuild SwordAndStone.sln
   ```

4. **Run Validation:**
   ```bash
   # Windows
   validate-build.bat
   
   # Linux/Mac
   ./validate-build.sh
   ```

## Before Committing - Checklist

Use this checklist before creating a pull request:

- [ ] **Code builds without warnings**
  - Run: `msbuild SwordAndStone.sln /p:TreatWarningsAsErrors=true`
  
- [ ] **Validation passes**
  - Run: `./validate-build.sh` or `validate-build.bat`
  
- [ ] **New files added to .csproj**
  - Check: Any new .cs files are referenced in the appropriate project file
  
- [ ] **No unused variables or fields**
  - Fix CS0219 (unused variables) and CS0414 (unused fields)
  
- [ ] **No obsolete API usage**
  - Fix CS0618 warnings by using recommended alternatives
  
- [ ] **Intentional field non-assignment documented**
  - Use `#pragma warning disable CS0649` with justification comments
  
- [ ] **Member hiding is intentional**
  - Use `new` keyword for CS0108 warnings when hiding is intentional
  
- [ ] **Code formatted consistently**
  - Modern IDEs will use `.editorconfig` automatically
  
- [ ] **Tests pass** (if applicable)
  - Run any existing tests
  
- [ ] **Documentation updated** (if needed)
  - Update README.md, BUILD.md, or other docs for new features

## Code Style Guidelines

The project uses `.editorconfig` for automatic formatting. Key points:

### Naming Conventions
```csharp
// Classes, methods, properties: PascalCase
public class MyClass { }
public void DoSomething() { }
public int MyProperty { get; set; }

// Private fields: camelCase with underscore prefix (optional)
private int _myField;

// Local variables: camelCase
int myVariable = 0;

// Constants: PascalCase
const int MaxValue = 100;
```

### Member Hiding
```csharp
// BAD - Unintentional hiding
public class Derived : Base
{
    Game game;  // CS0108 warning
}

// GOOD - Intentional hiding with 'new' keyword
public class Derived : Base
{
    new Game game;  // Clear intent
}
```

### Unused Code
```csharp
// BAD - Unused variables
int unused = 0;  // CS0219
UpdateValue();

// GOOD - Remove or use them
UpdateValue();
```

### Unassigned Fields
```csharp
// BAD - No documentation
internal string playerName;  // CS0649

// GOOD - Document intent
#pragma warning disable CS0649
internal string playerName;  // Assigned via protobuf deserialization
#pragma warning restore CS0649
```

### Obsolete APIs
```csharp
// BAD - Using deprecated method
GetShadowRatioOld(x, y, z, gx, gy, gz);  // CS0618

// GOOD - Use recommended alternative
GetShadowRatio(x, y, z);
```

## Common Issues and Solutions

### "File not found in .csproj"
**Solution:** Add the file to the appropriate .csproj:
```xml
<Compile Include="Client\Mods\MyNewFile.ci.cs" />
```

### "Missing NuGet package"
**Solution:** Restore packages:
```bash
nuget restore SwordAndStone.sln
```

### "Enum value not found"
**Solution:** Check Packet.proto for correct enum names. The file is regenerated during build.

### "Build succeeds but has warnings"
**Solution:** Treat warnings as errors during development:
```bash
msbuild SwordAndStone.sln /p:TreatWarningsAsErrors=true
```

## Pull Request Process

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/my-new-feature
   ```

2. **Make your changes:**
   - Follow code style guidelines
   - Add/update tests if applicable
   - Update documentation

3. **Run validation:**
   ```bash
   ./validate-build.sh
   ```

4. **Commit with clear messages:**
   ```bash
   git add .
   git commit -m "Add feature: description of what you added"
   ```

5. **Push and create PR:**
   ```bash
   git push origin feature/my-new-feature
   ```
   Then create a pull request on GitHub

6. **Address review feedback:**
   - CI checks must pass
   - Code review comments should be addressed
   - Keep commits clean and focused

## Development Tools

### Validation Scripts
- `validate-build.sh` / `validate-build.bat` - Pre-commit validation
- Checks project files, code quality, and configuration

### EditorConfig
- `.editorconfig` - Automatic code formatting
- Supported by Visual Studio, VS Code, Rider, and more

### CI/CD
- `.github/workflows/build-validation.yml` - Automated checks on every push
- Validates project structure and code quality

## What to Contribute

We welcome contributions in many forms:

### Code Contributions
- Bug fixes
- New features
- Performance improvements
- Code refactoring
- Tests

### Documentation
- README improvements
- Code comments
- Tutorials and guides
- API documentation

### Testing
- Manual testing and bug reports
- Automated test creation
- Performance testing

### Graphics and Assets
- Textures and models
- UI improvements
- Sound effects

## Getting Help

- **Build Issues:** See [BUILD.md](BUILD.md)
- **Error Prevention:** See [BUILD_ERROR_PREVENTION.md](BUILD_ERROR_PREVENTION.md)
- **GitHub Issues:** Report bugs or ask questions
- **Discussions:** Join community discussions on GitHub

## Code of Conduct

- Be respectful and constructive
- Help others learn and improve
- Focus on the code, not the person
- Keep discussions professional and on-topic

## License

By contributing, you agree that your contributions will be released into the public domain under the [Unlicense](http://unlicense.org), matching the project's existing license.

---

Thank you for contributing to SwordNStone! 🎮⚔️🗿
