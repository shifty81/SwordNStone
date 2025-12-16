# Building and Testing Sword&Stone

This document provides instructions for building and testing the Sword&Stone project using Visual Studio and command-line tools.

## Prerequisites

### Windows - Visual Studio
- **Visual Studio 2012 or later** (tested with VS 2022)
- **.NET Framework 4.8** (included with Visual Studio)
- The solution should open directly in Visual Studio

### Windows - Command Line
- **MSBuild** (included with Visual Studio or .NET Framework SDK)
- **NuGet CLI** (for restoring packages)

### Linux/Mac - Mono
- **Mono 6.8+** with development tools
- **xbuild** (included with mono-complete)
- **wget** or **curl** (for downloading NuGet)

## Building the Project

### Option 1: Visual Studio (Recommended for Windows)

1. **Open the solution:**
   ```
   Open SwordAndStone.sln in Visual Studio
   ```

2. **Restore NuGet packages:**
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"
   - Visual Studio should automatically restore packages on build

3. **Build the solution:**
   - Select Build → Build Solution (Ctrl+Shift+B)
   - Or select Build → Rebuild Solution for a clean build

4. **Build configurations:**
   - **Debug**: For development and debugging (includes symbols)
   - **Release**: For optimized production builds
   - **FastBuild**: Quick build configuration for rapid iteration

### Option 2: Command Line (Windows)

1. **Navigate to the project directory:**
   ```cmd
   cd /path/to/SwordNStone
   ```

2. **Restore NuGet packages:**
   ```cmd
   nuget restore SwordAndStone.sln
   ```

3. **Build the solution:**
   ```cmd
   msbuild SwordAndStone.sln /p:Configuration=Debug
   ```
   
   Or for Release:
   ```cmd
   msbuild SwordAndStone.sln /p:Configuration=Release
   ```

### Option 3: Linux/Mac with Mono

1. **Install Mono (Ubuntu/Debian):**
   ```bash
   sudo apt-get update
   sudo apt-get install -y mono-complete
   ```

2. **Download NuGet:**
   ```bash
   wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
   ```

3. **Restore packages:**
   ```bash
   mono nuget.exe restore SwordAndStone.sln
   ```

4. **Build with xbuild:**
   ```bash
   xbuild SwordAndStone.sln /p:Configuration=Debug
   ```

## Project Structure

The solution contains the following projects:

- **SwordAndStone**: Main game client application (WinExe)
- **SwordAndStoneLib**: Core game library containing game logic
- **SwordAndStoneServer**: Dedicated server application (Console)
- **ScriptingApi**: Server-side scripting API for mods
- **SwordAndStoneMonsterEditor**: Monster model editor tool

## Build Output

After building, the compiled binaries will be located in:
```
<ProjectName>/bin/<Configuration>/
```

For example:
- `SwordAndStone/bin/Debug/SwordAndStone.exe` - Game client
- `SwordAndStoneServer/bin/Debug/SwordAndStoneServer.exe` - Server
- `SwordAndStoneLib/bin/Debug/SwordAndStoneLib.dll` - Core library

## Running the Application

### Game Client
```bash
# Windows
SwordAndStone\bin\Debug\SwordAndStone.exe

# Linux/Mac with Mono
mono SwordAndStone/bin/Debug/SwordAndStone.exe
```

### Server
```bash
# Windows
SwordAndStoneServer\bin\Debug\SwordAndStoneServer.exe

# Linux/Mac with Mono
mono SwordAndStoneServer/bin/Debug/SwordAndStoneServer.exe
```

## Testing

### Manual Testing

The project currently uses manual testing. Here's how to verify the build:

#### Test 1: Server Startup
1. Navigate to the server build directory:
   ```bash
   cd SwordAndStoneServer/bin/Debug
   ```

2. Start the server:
   ```bash
   # Windows
   SwordAndStoneServer.exe
   
   # Linux/Mac
   mono SwordAndStoneServer.exe
   ```

3. **Expected output:**
   - Server should start without errors
   - Console should show initialization messages
   - Server should listen on default port (usually 25565)

4. **Success criteria:**
   - No crash on startup
   - Server displays "Ready" or similar message
   - Can be stopped gracefully with Ctrl+C

#### Test 2: Client Startup
1. Ensure you have the data folder in the working directory
2. Navigate to the client build directory:
   ```bash
   cd SwordAndStone/bin/Debug
   ```

3. Start the client:
   ```bash
   # Windows
   SwordAndStone.exe
   
   # Linux/Mac (requires X11 display)
   mono SwordAndStone.exe
   ```

4. **Expected output:**
   - Main menu should appear
   - Options for singleplayer, multiplayer, settings
   - No crash on startup

5. **Success criteria:**
   - Application launches and displays main menu
   - UI is responsive
   - Can navigate menus without errors

#### Test 3: Library Integration
The SwordAndStoneLib.dll is the core library used by both client and server. It's automatically tested when:
- Server starts successfully (uses lib)
- Client starts successfully (uses lib)
- No missing dependency errors occur

### Common Build Issues

#### Issue: Missing .NET Framework 4.8
**Solution:** Install .NET Framework 4.8 Developer Pack from Microsoft or use Mono on Linux/Mac

#### Issue: Missing NuGet packages
**Solution:** Run `nuget restore` or right-click solution in VS and select "Restore NuGet Packages"

**Important:** Before building for the first time, you MUST restore NuGet packages. The packages folder is not included in the repository.

#### Issue: "The referenced component 'OpenTK' could not be found"
**Cause:** OpenTK NuGet package (2.0.0) is not restored in the packages directory.
**Solution:** 
1. Restore NuGet packages using one of these methods:
   - Visual Studio: Right-click solution → "Restore NuGet Packages"
   - Command line (Windows): `nuget restore SwordAndStone.sln`
   - Command line (Linux/Mac): `mono nuget.exe restore SwordAndStone.sln`
2. Verify that `packages/OpenTK.2.0.0/lib/net20/OpenTK.dll` exists
3. If restoration fails, download manually from https://www.nuget.org/packages/OpenTK/2.0.0

#### Issue: "The referenced component 'protobuf-net' could not be found"
**Cause:** protobuf-net NuGet package (2.1.0) is not restored in the packages directory.
**Solution:**
1. Restore NuGet packages (same methods as above)
2. Verify that `packages/protobuf-net.2.1.0/lib/net45/protobuf-net.dll` exists
3. If restoration fails, download manually from https://www.nuget.org/packages/protobuf-net/2.1.0

#### Issue: "Metadata file 'SwordAndStoneLib.dll' could not be found"
**Cause:** This is usually a cascading error from missing packages in SwordAndStoneLib project.
**Solution:**
1. First, resolve any missing package references (OpenTK, protobuf-net)
2. Build SwordAndStoneLib project first: `msbuild SwordAndStoneLib/SwordAndStoneLib.csproj`
3. Then build the main SwordAndStone project

#### Issue: "Unknown symbol SetScreen" in ThemeEditor.ci.cs
**Status:** **FIXED** - This error has been resolved by updating ThemeEditor.ci.cs to use the correct method signatures and API calls matching the Screen base class.

#### Issue: BuildCito.bat fails with "Could not find file '~1'"
**Solution:** This was caused by spaces in the directory path and has been fixed. The PreBuildEvent now properly quotes the directory path.

#### Issue: Build warnings
**Note:** The project has many warnings but these don't prevent building or running. They are mostly unused variables and obsolete API usage.

## Adding Tests (Future Enhancement)

To add automated tests to this project:

1. **Add a test project:**
   - Create new Class Library project (e.g., SwordAndStone.Tests)
   - Add NUnit or xUnit NuGet package
   - Reference SwordAndStoneLib project

2. **Write unit tests:**
   ```csharp
   [TestFixture]
   public class GameTests
   {
       [Test]
       public void TestSomething()
       {
           // Test code here
           Assert.IsTrue(true);
       }
   }
   ```

3. **Run tests:**
   - Use Test Explorer in Visual Studio
   - Or command line: `nunit-console SwordAndStone.Tests.dll`

## Distribution Build

To create a distribution package, use the provided build scripts:

### Windows:
```cmd
build.bat
```

### Linux/Mac:
```bash
./build.sh
```

These scripts:
1. Build all projects in Release configuration
2. Copy binaries to `output/` directory
3. Include required data files and dependencies
4. Create a ready-to-distribute package

## Development Workflow

1. **Make changes** to source code in Visual Studio or your preferred editor
2. **Run validation** before committing (see Validation Tools below)
3. **Build** the solution to check for compilation errors
4. **Run manual tests** to verify functionality
5. **Build Release** when ready to distribute
6. **Use build scripts** to create distribution packages

## Validation Tools (NEW!)

To help prevent common build errors, the project now includes automated validation tools:

### Pre-Commit Validation Script

Run before committing to catch issues early:

**Windows:**
```cmd
validate-build.bat
```

**Linux/Mac:**
```bash
./validate-build.sh
```

These scripts check for:
- ✅ Missing project file references
- ✅ Unused variables and fields
- ✅ TODO/FIXME comments
- ✅ XML configuration validity
- ✅ Required configuration files
- ✅ NuGet package status

### EditorConfig

The project now includes `.editorconfig` for consistent code style across IDEs:
- Enforces code formatting rules
- Treats certain warnings as errors (CS0108, CS0219, CS0414, CS0618)
- Configures naming conventions
- Works with Visual Studio, VS Code, Rider, and other modern editors

### Continuous Integration

GitHub Actions workflow (`.github/workflows/build-validation.yml`) automatically validates:
- All source files are included in project files
- Code quality checks pass
- Configuration files are valid
- Build succeeds without warnings

### Best Practices

To avoid build errors in the future:

1. **Always run validation before committing:**
   ```bash
   ./validate-build.sh  # or validate-build.bat on Windows
   ```

2. **Enable all warnings in Visual Studio:**
   - Project Properties → Build → Treat warnings as errors → All

3. **Use the EditorConfig:**
   - Modern IDEs will automatically use `.editorconfig`
   - Ensures consistent code style across the team

4. **Add new files to .csproj:**
   - When creating new .cs files, add them to the appropriate .csproj
   - Or use wildcard patterns for automatic inclusion

5. **Keep packages updated:**
   - Run `nuget restore` after pulling changes
   - Check for package updates regularly

For detailed information on preventing build errors, see [BUILD_ERROR_PREVENTION.md](BUILD_ERROR_PREVENTION.md).

## Continuous Integration

The project includes `.travis.yml` for Travis CI integration. On each commit:
- Dependencies are restored
- All projects are built
- Build status is reported

## Additional Resources

- **GitHub Issues**: Report bugs and request features

## Getting Help

If you encounter build issues:
1. Check this document for common solutions
2. Verify all prerequisites are installed
3. Check the GitHub Issues page

---

**Last Updated**: December 2025
