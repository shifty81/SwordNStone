# Quick Start Guide - Sword&Stone Development

This guide will get you up and running with Sword&Stone development quickly.

## Prerequisites

Choose your platform:

### Windows
- **Visual Studio 2012 or later** (VS 2022 recommended)
- **.NET Framework 4.5** (included with Visual Studio)

### Linux/Mac
- **Mono 6.8+** complete package
- **Text editor** (VS Code, vim, etc.)

## 5-Minute Setup

### Windows with Visual Studio

1. **Clone the repository:**
   ```cmd
   git clone https://github.com/shifty81/SwordNStone.git
   cd SwordNStone
   ```

2. **Open in Visual Studio:**
   ```cmd
   ManicDigger.sln
   ```

3. **Build the solution:**
   - Press `Ctrl+Shift+B` or
   - Right-click solution → Build Solution

4. **Run the client:**
   - Set `ManicDigger` as startup project (right-click → Set as Startup Project)
   - Press `F5` to run with debugging
   - Or `Ctrl+F5` to run without debugging

5. **Run tests:**
   - Open Test Explorer (`Ctrl+E, T`)
   - Click "Run All"

**Done!** You're ready to develop.

### Linux/Mac with Mono

1. **Install Mono:**
   ```bash
   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install -y mono-complete
   
   # macOS with Homebrew
   brew install mono
   ```

2. **Clone the repository:**
   ```bash
   git clone https://github.com/shifty81/SwordNStone.git
   cd SwordNStone
   ```

3. **Download NuGet:**
   ```bash
   wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
   ```

4. **Restore packages:**
   ```bash
   mono nuget.exe restore ManicDigger.sln
   ```

5. **Build:**
   ```bash
   xbuild ManicDigger.sln /p:Configuration=Debug
   ```

6. **Run the server:**
   ```bash
   cd ManicDiggerServer/bin/Debug
   mono ManicDiggerServer.exe
   ```

**Done!** The server should start.

## Project Structure

```
SwordNStone/
├── ManicDigger/           # Game client (Windows GUI application)
├── ManicDiggerLib/        # Core game library (shared code)
├── ManicDiggerServer/     # Dedicated server application
├── ScriptingApi/          # Server-side modding API
├── MdMonsterEditor/       # Monster model editor tool
├── ManicDigger.Tests/     # Unit and integration tests
├── data/                  # Game assets (textures, sounds, etc.)
├── Lib/                   # Third-party native libraries
└── packages/              # NuGet packages (generated)
```

## Common Tasks

### Building

```bash
# Debug build (default)
xbuild ManicDigger.sln

# Release build
xbuild ManicDigger.sln /p:Configuration=Release

# Visual Studio: Ctrl+Shift+B
```

### Running

```bash
# Server
cd ManicDiggerServer/bin/Debug
mono ManicDiggerServer.exe

# Client (requires X11 on Linux)
cd ManicDigger/bin/Debug
mono ManicDigger.exe

# Visual Studio: Press F5
```

### Testing

```bash
# Visual Studio: Ctrl+E, T (Test Explorer)

# Command line (after installing NUnit.Console)
mono packages/NUnit.ConsoleRunner.*/tools/nunit3-console.exe ManicDigger.Tests/bin/Debug/ManicDigger.Tests.dll
```

### Creating a Distribution

```bash
# Windows
build.bat

# Linux/Mac
./build.sh
```

Output goes to `output/` directory.

## Making Your First Change

Let's add a simple feature to understand the workflow:

### Example: Add a Welcome Message

1. **Open the server code:**
   ```
   ManicDiggerLib/Server/Server.cs
   ```

2. **Find the player join handler** (around line 200-300):
   Look for methods handling player connections

3. **Add your message:**
   ```csharp
   // In the player join handler
   SendMessageToPlayer(playerId, "Welcome to My Custom Server!");
   ```

4. **Build the project:**
   ```bash
   xbuild ManicDigger.sln
   ```

5. **Test your change:**
   - Run the server
   - Connect with a client
   - Verify the welcome message appears

6. **Commit your change:**
   ```bash
   git add .
   git commit -m "Add welcome message to server"
   ```

## Development Workflow

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/my-new-feature
   ```

2. **Make your changes:**
   - Edit code in your preferred editor
   - Build frequently to catch errors early

3. **Test your changes:**
   - Run manual tests
   - Add automated tests if applicable
   - Verify nothing broke

4. **Commit and push:**
   ```bash
   git add .
   git commit -m "Descriptive commit message"
   git push origin feature/my-new-feature
   ```

5. **Create a pull request:**
   - Go to GitHub
   - Create PR from your branch
   - Wait for review

## Key Files to Know

### Configuration
- `ManicDigger.sln` - Solution file (open in VS)
- `*.csproj` - Project files
- `packages.config` - NuGet dependencies

### Build Scripts
- `build.bat` / `build.sh` - Create distribution
- `BuildCito.bat` / `BuildCito.sh` - Build Cito components

### Documentation
- `README.md` - Project overview
- `BUILD.md` - Detailed build instructions
- `TESTING.md` - Testing guide
- `QUICKSTART.md` - This file!

## Debugging

### Visual Studio

1. Set breakpoints (click left margin or F9)
2. Start debugging (F5)
3. Use debugging windows:
   - Locals (shows variable values)
   - Call Stack
   - Immediate Window (execute code)

### Mono

```bash
# Run with debugger
mono --debug ManicDiggerServer.exe

# Use logging
# Add Console.WriteLine() statements
# Check console output
```

## Getting Help

### Documentation
- See `BUILD.md` for detailed build instructions
- See `TESTING.md` for testing information

### Community
- **GitHub Issues**: Report bugs and request features

### Common Issues

**Q: Build fails with "missing reference"**
A: Run `nuget restore ManicDigger.sln`

**Q: Can't run client on Linux**
A: You need X11 display. Client is primarily for Windows.

**Q: Server won't start**
A: Check that port 25565 is available and not blocked by firewall

**Q: Changes not reflected**
A: Do a clean rebuild: Delete `bin/` and `obj/` folders, then rebuild

## Next Steps

Now that you're set up:

1. **Explore the code:**
   - Browse `ManicDiggerLib/` to understand core game logic
   - Look at `ScriptingApi/` to see the modding API
   - Check `ManicDigger.Tests/` for test examples

2. **Try the examples:**
   - Run the server and client
   - Test different game modes
   - Load some mods from `ManicDiggerLib/Server/Mods/`

3. **Make improvements:**
   - Fix a bug from GitHub Issues
   - Add a new feature
   - Improve documentation
   - Add tests

4. **Share your work:**
   - Create a pull request
   - Share on the forum
   - Help other developers

## Development Tips

1. **Build often**: Catch errors early
2. **Test frequently**: Verify changes work
3. **Use version control**: Commit regularly
4. **Write tests**: Prevent regressions
5. **Document code**: Help future maintainers
6. **Ask questions**: Community is helpful

## Resources

- **C# Documentation**: https://docs.microsoft.com/en-us/dotnet/csharp/
- **NUnit Testing**: https://docs.nunit.org/
- **Git Basics**: https://git-scm.com/doc
- **Visual Studio**: https://docs.microsoft.com/en-us/visualstudio/

---

**Happy coding!** Welcome to the Sword&Stone development community.

For more detailed information, see:
- [BUILD.md](BUILD.md) - Comprehensive build guide
- [TESTING.md](TESTING.md) - Testing documentation
- [README.md](README.md) - Project overview
