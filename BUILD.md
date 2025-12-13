# Building and Testing Sword&Stone

This document provides instructions for building and testing the Sword&Stone project using Visual Studio and command-line tools.

## Prerequisites

### Windows - Visual Studio
- **Visual Studio 2012 or later** (tested with VS 2022)
- **.NET Framework 4.5** (included with Visual Studio)
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
   cd /path/to/manicdiggerVSCLONE
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
- **MdMonsterEditor**: Monster model editor tool

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

#### Issue: Missing .NET Framework 4.5
**Solution:** Install .NET Framework 4.5 Developer Pack from Microsoft or use Mono on Linux/Mac

#### Issue: Missing NuGet packages
**Solution:** Run `nuget restore` or right-click solution in VS and select "Restore NuGet Packages"

#### Issue: OpenTK.dll not found
**Solution:** Ensure NuGet packages are restored. OpenTK 2.0.0 should be in packages/OpenTK.2.0.0/

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
2. **Build** the solution to check for compilation errors
3. **Run manual tests** to verify functionality
4. **Build Release** when ready to distribute
5. **Use build scripts** to create distribution packages

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
