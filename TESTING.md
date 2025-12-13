# Testing Sword&Stone

This document describes how to run tests for the Sword&Stone project.

## Test Project

The solution includes a test project: **SwordAndStone.Tests**

This project contains:
- Sample unit tests demonstrating the testing framework
- Integration tests for verifying component interactions
- Examples showing how to add new tests

## Running Tests

### Visual Studio (Recommended)

1. **Open Test Explorer:**
   - View → Test Explorer (or Ctrl+E, T)

2. **Build the solution:**
   - This ensures tests are compiled
   - Build → Build Solution

3. **Run all tests:**
   - Click "Run All" in Test Explorer
   - Or use Test → Run → All Tests

4. **Run specific tests:**
   - Select tests in Test Explorer
   - Right-click → Run Selected Tests

5. **View results:**
   - Test Explorer shows pass/fail status
   - Click on a test to see details and output

### Visual Studio Code

1. **Install C# extension:**
   - Install the C# Dev Kit extension

2. **Open the folder:**
   - File → Open Folder → Select manicdiggerVSCLONE

3. **Run tests:**
   - Use the Testing sidebar
   - Or Command Palette → Test: Run All Tests

### Command Line - NUnit Console

To run tests from the command line, you need the NUnit Console Runner:

1. **Install NUnit Console Runner:**
   ```bash
   mono nuget.exe install NUnit.ConsoleRunner -Version 3.15.4 -OutputDirectory packages
   ```

2. **Run tests:**
   ```bash
   # Windows
   .\packages\NUnit.ConsoleRunner.3.15.4\tools\nunit3-console.exe ManicDigger.Tests\bin\Debug\ManicDigger.Tests.dll
   
   # Linux/Mac with Mono
   mono ./packages/NUnit.ConsoleRunner.3.15.4/tools/nunit3-console.exe ManicDigger.Tests/bin/Debug/ManicDigger.Tests.dll
   ```

3. **View results:**
   - Console output shows test results
   - XML report generated: TestResult.xml

### Command Line - dotnet test (Modern .NET)

If you migrate to modern .NET SDK-style projects:

```bash
dotnet test ManicDigger.sln
```

This automatically:
- Builds the solution
- Discovers tests
- Runs all tests
- Reports results

## Test Categories

Tests can be organized by category:

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **Performance Tests**: Verify performance characteristics

Run tests by category in Visual Studio:
- Test Explorer → Group By → Category
- Right-click category → Run

## Writing New Tests

### Basic Test Structure

```csharp
using NUnit.Framework;

namespace ManicDigger.Tests
{
    [TestFixture]
    public class MyTests
    {
        [Test]
        public void TestSomething()
        {
            // Arrange
            var input = "test";
            
            // Act
            var result = ProcessInput(input);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("expected", result);
        }
    }
}
```

### Test Attributes

- `[TestFixture]` - Marks a class as containing tests
- `[Test]` - Marks a method as a test
- `[SetUp]` - Runs before each test
- `[TearDown]` - Runs after each test
- `[TestCase(arg1, arg2, ...)]` - Parameterized test
- `[Category("CategoryName")]` - Categorize tests
- `[Ignore("Reason")]` - Skip a test

### Assertions

Common NUnit assertions:

```csharp
// Equality
Assert.AreEqual(expected, actual);
Assert.AreNotEqual(expected, actual);

// Boolean
Assert.IsTrue(condition);
Assert.IsFalse(condition);

// Null checks
Assert.IsNull(obj);
Assert.IsNotNull(obj);

// Collections
Assert.IsEmpty(collection);
Assert.IsNotEmpty(collection);
Assert.Contains(item, collection);

// Exceptions
Assert.Throws<ArgumentException>(() => MethodThatThrows());

// Numeric comparisons
Assert.Greater(actual, expected);
Assert.Less(actual, expected);
```

## Test Coverage

To measure test coverage:

### Visual Studio Enterprise

1. Test → Analyze Code Coverage for All Tests
2. View coverage results in Code Coverage Results window
3. Identify untested code paths

### OpenCover (Command Line)

1. Install OpenCover:
   ```bash
   mono nuget.exe install OpenCover -Version 4.7.1221 -OutputDirectory packages
   ```

2. Run tests with coverage:
   ```bash
   packages\OpenCover.4.7.1221\tools\OpenCover.Console.exe `
     -target:"nunit3-console.exe" `
     -targetargs:"ManicDigger.Tests\bin\Debug\ManicDigger.Tests.dll" `
     -register:user `
     -output:coverage.xml
   ```

3. Generate HTML report with ReportGenerator

## Current Test Status

### Existing Tests

The ManicDigger.Tests project includes:

1. **SampleTests**
   - Basic assertion tests
   - String operation tests
   - Array operation tests
   - Exception handling tests
   - ScriptingApi availability tests

2. **UtilityTests**
   - Parameterized tests
   - Performance tests

These are demonstration tests showing the testing infrastructure. They should be expanded to test actual game functionality.

### Test Areas Needing Coverage

The following areas would benefit from automated tests:

1. **Game Logic**
   - Block placement and removal
   - Player movement and physics
   - Inventory management
   - Crafting system

2. **Multiplayer**
   - Client-server communication
   - Player synchronization
   - World state replication

3. **World Generation**
   - Terrain generation algorithms
   - Chunk loading/unloading
   - Map serialization

4. **Modding API**
   - Script execution
   - Mod loading
   - API functionality

5. **Rendering** (more challenging)
   - Texture loading
   - Model rendering
   - UI rendering

## Continuous Integration

Tests can be run automatically on each commit using CI services:

### GitHub Actions

Create `.github/workflows/test.yml`:

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '4.5'
      - name: Restore packages
        run: nuget restore ManicDigger.sln
      - name: Build
        run: msbuild ManicDigger.sln /p:Configuration=Debug
      - name: Test
        run: nunit3-console ManicDigger.Tests\bin\Debug\ManicDigger.Tests.dll
```

### Travis CI

The project already has `.travis.yml`. Add test execution:

```yaml
script:
  - xbuild ManicDigger.sln /p:Configuration=Debug
  - mono packages/NUnit.ConsoleRunner.3.16.3/tools/nunit3-console.exe ManicDigger.Tests/bin/Debug/ManicDigger.Tests.dll
```

## Best Practices

1. **Test Naming**: Use descriptive names
   - Good: `TestPlayerCanPlaceBlockWhenHoldingIt()`
   - Bad: `Test1()`

2. **Arrange-Act-Assert**: Structure tests clearly
   ```csharp
   // Arrange - Setup
   // Act - Execute
   // Assert - Verify
   ```

3. **One Assertion Per Test**: Keep tests focused
   - Each test should verify one behavior

4. **Independent Tests**: Tests shouldn't depend on each other
   - Use `[SetUp]` to initialize test state
   - Don't rely on test execution order

5. **Fast Tests**: Keep unit tests fast
   - Mock external dependencies
   - Avoid file I/O and network calls in unit tests

6. **Readable Tests**: Tests are documentation
   - Make intent clear
   - Use meaningful variable names

## Troubleshooting

### Tests Not Discovered

- Ensure project builds successfully
- Verify NUnit package is installed
- Check test methods have `[Test]` attribute
- Rebuild the solution

### Tests Fail to Run

- Check all dependencies are present
- Verify test project references necessary assemblies
- Look for exceptions in test output

### Tests Pass Locally But Fail in CI

- Check for environment-specific dependencies
- Verify file paths are not hardcoded
- Ensure tests don't depend on local state

## Additional Resources

- **NUnit Documentation**: https://docs.nunit.org/
- **Testing Best Practices**: https://docs.microsoft.com/en-us/dotnet/core/testing/
- **Mocking with Moq**: https://github.com/moq/moq4

---

**Note**: The test infrastructure is in place and ready to be expanded. As the project grows, add tests for new features and bug fixes to maintain quality and prevent regressions.
