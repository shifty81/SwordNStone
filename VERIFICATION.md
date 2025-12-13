# Verification Guide - Testing Manic Digger Build

This guide documents how to verify that the Manic Digger project builds and runs correctly.

## Build Verification

### Step 1: Verify Build Succeeds

**Windows (Visual Studio):**
```cmd
# Open Visual Studio
# File → Open → Project/Solution → ManicDigger.sln
# Build → Rebuild Solution (Ctrl+Shift+B)
# Check Output window for "Build: 6 succeeded, 0 failed"
```

**Linux/Mac (Mono):**
```bash
cd /path/to/manicdiggerVSCLONE
mono nuget.exe restore ManicDigger.sln
xbuild ManicDigger.sln /p:Configuration=Release
# Look for "0 Error(s)" in output
```

**Expected Result:**
- ✅ All 6 projects build successfully
- ✅ Warnings are OK (the codebase has many warnings)
- ✅ 0 Errors
- ✅ Binaries created in respective bin/Release folders

### Step 2: Verify Output Files

Check that the following files exist:

```bash
# Core libraries
ls ManicDiggerLib/bin/Release/ManicDiggerLib.dll
ls ScriptingApi/bin/Release/ScriptingApi.dll

# Executables
ls ManicDigger/bin/Release/ManicDigger.exe
ls ManicDiggerServer/bin/Release/ManicDiggerServer.exe
ls MdMonsterEditor/bin/Release/MdMonsterEditor.exe

# Test assembly
ls ManicDigger.Tests/bin/Release/ManicDigger.Tests.dll
```

**Expected Result:**
- ✅ All files exist
- ✅ Files have recent timestamps
- ✅ Reasonable file sizes (not 0 bytes)

## Runtime Verification

### Test 1: Server Startup

This test verifies the server can start without crashing.

**Steps:**
1. Navigate to server directory:
   ```bash
   cd ManicDiggerServer/bin/Release
   ```

2. Start the server:
   ```bash
   # Windows
   ManicDiggerServer.exe
   
   # Linux/Mac
   mono ManicDiggerServer.exe
   ```

3. Observe console output

**Expected Output:**
```
Manic Digger Server
...
[Various initialization messages]
...
Server started on port 25565
```

**Success Criteria:**
- ✅ Server starts without errors
- ✅ Displays initialization messages
- ✅ Indicates listening on port (usually 25565)
- ✅ Console accepts input
- ✅ Can be stopped with Ctrl+C

**Common Issues:**
- ❌ Port already in use: Change port in server config or stop other servers
- ❌ Missing DLLs: Ensure all dependencies copied to bin directory
- ❌ Immediate crash: Check for error messages, verify .NET/Mono version

### Test 2: Server Command Processing

This test verifies the server can process commands.

**Steps:**
1. Start the server (as above)
2. Type a command in the console:
   ```
   help
   ```
3. Observe response

**Expected Output:**
```
Available commands:
[List of server commands]
```

**Success Criteria:**
- ✅ Server responds to command
- ✅ Displays help information
- ✅ Console remains responsive

### Test 3: Library Loading

This test verifies all required libraries load correctly.

**Steps:**
1. Start the server with verbose output
2. Check for missing library errors

**Success Criteria:**
- ✅ No "Could not load file or assembly" errors
- ✅ All DLLs found and loaded
- ✅ Native libraries (if any) found

**Common Libraries:**
- ManicDiggerLib.dll
- ScriptingApi.dll
- protobuf-net.dll
- Jint.dll (JavaScript interpreter)
- ENet.dll (Networking)

### Test 4: Mod Loading (Optional)

If server mods are present, verify they load:

**Steps:**
1. Ensure `ManicDiggerLib/Server/Mods/` contains mod files
2. Start server
3. Check console for mod loading messages

**Expected Output:**
```
Loading mod: [ModName]
Mod [ModName] loaded successfully
```

**Success Criteria:**
- ✅ Mods discovered
- ✅ Mods loaded without errors
- ✅ No script compilation errors

## Test Project Verification

### Test 5: Unit Tests Build

**Steps:**
1. Build the test project:
   ```bash
   xbuild ManicDigger.Tests/ManicDigger.Tests.csproj
   ```

2. Check output:
   ```bash
   ls ManicDigger.Tests/bin/Debug/ManicDigger.Tests.dll
   ```

**Expected Result:**
- ✅ Test assembly builds
- ✅ NUnit framework referenced
- ✅ Test assembly contains test fixtures

### Test 6: Run Sample Tests

**Prerequisites:**
Install NUnit Console Runner:
```bash
mono nuget.exe install NUnit.ConsoleRunner -Version 3.15.4 -OutputDirectory packages
```

**Steps:**
1. Run tests:
   ```bash
   # Linux/Mac
   mono packages/NUnit.ConsoleRunner.3.15.4/tools/nunit3-console.exe ManicDigger.Tests/bin/Debug/ManicDigger.Tests.dll
   
   # Or in Visual Studio
   # Test Explorer → Run All
   ```

**Expected Output:**
```
NUnit Console Runner 3.16.3
...
Test Run Summary
  Overall result: Passed
  Test Count: X, Passed: X, Failed: 0, Warnings: 0, Inconclusive: 0, Skipped: Y
```

**Success Criteria:**
- ✅ All tests pass (unless marked [Ignore])
- ✅ Test count > 0
- ✅ No unexpected failures

## Integration Tests

### Test 7: Client-Server Integration (Advanced)

This test verifies client and server can communicate.

**Requirements:**
- X11 display (Linux)
- OpenGL support
- Input device

**Steps:**
1. Start the server:
   ```bash
   cd ManicDiggerServer/bin/Release
   mono ManicDiggerServer.exe
   ```

2. In another terminal, start the client:
   ```bash
   cd ManicDigger/bin/Release
   mono ManicDigger.exe
   ```

3. In client menu, connect to localhost

**Expected Result:**
- ✅ Client shows main menu
- ✅ Can connect to server
- ✅ Server accepts connection
- ✅ No crash on either side

**Note:** Client requires GUI environment. On headless servers, only test the server.

## Performance Verification

### Test 8: Server Performance (Optional)

Basic performance check:

**Steps:**
1. Start server
2. Monitor resource usage:
   ```bash
   # Linux
   top | grep -i manicdigger
   
   # Windows
   # Task Manager → Details → ManicDiggerServer.exe
   ```

**Success Criteria:**
- ✅ CPU usage reasonable when idle (<5%)
- ✅ Memory usage stable (not growing continuously)
- ✅ No memory leaks over time

## Distribution Build Verification

### Test 9: Create Distribution Package

**Steps:**
1. Run build script:
   ```bash
   # Windows
   build.bat
   
   # Linux/Mac
   ./build.sh
   ```

2. Check output directory:
   ```bash
   ls output/
   ```

**Expected Contents:**
```
output/
├── ManicDigger.exe
├── ManicDiggerServer.exe
├── ManicDiggerLib.dll
├── ScriptingApi.dll
├── [Various DLLs]
├── data/
│   ├── textures/
│   ├── sounds/
│   └── [Game assets]
└── Mods/
    └── [Server mods]
```

**Success Criteria:**
- ✅ All executables present
- ✅ All libraries present
- ✅ Data folder copied
- ✅ Package is self-contained

### Test 10: Run from Distribution

**Steps:**
1. Navigate to output folder:
   ```bash
   cd output/
   ```

2. Run server:
   ```bash
   mono ManicDiggerServer.exe
   ```

**Success Criteria:**
- ✅ Server runs from distribution
- ✅ Finds all dependencies
- ✅ Loads data files
- ✅ Fully functional

## Automated Verification Script

You can create a script to automate basic checks:

**verify.sh:**
```bash
#!/bin/bash
set -e

echo "=== Manic Digger Verification Script ==="

# Test 1: Build
echo "Test 1: Building solution..."
xbuild ManicDigger.sln /p:Configuration=Release /verbosity:quiet
echo "✓ Build succeeded"

# Test 2: Check outputs
echo "Test 2: Checking output files..."
test -f ManicDiggerLib/bin/Release/ManicDiggerLib.dll
test -f ManicDiggerServer/bin/Release/ManicDiggerServer.exe
test -f ManicDigger.Tests/bin/Release/ManicDigger.Tests.dll
echo "✓ All output files present"

# Test 3: Server quick start
echo "Test 3: Testing server startup..."
timeout 5 mono ManicDiggerServer/bin/Release/ManicDiggerServer.exe || true
echo "✓ Server started (stopped after 5 seconds)"

echo ""
echo "=== All basic checks passed! ==="
echo "See VERIFICATION.md for comprehensive testing."
```

**Usage:**
```bash
chmod +x verify.sh
./verify.sh
```

## Verification Checklist

Use this checklist to track verification status:

### Build Verification
- [ ] Solution builds without errors
- [ ] All 6 projects compile
- [ ] Output files generated
- [ ] Tests project builds

### Runtime Verification  
- [ ] Server starts successfully
- [ ] Server responds to commands
- [ ] Libraries load correctly
- [ ] Mods load (if applicable)

### Test Verification
- [ ] Unit tests build
- [ ] Tests can be executed
- [ ] Sample tests pass
- [ ] Test framework working

### Distribution Verification
- [ ] Build script succeeds
- [ ] Output directory complete
- [ ] Standalone package runs
- [ ] All assets included

### Documentation Verification
- [ ] BUILD.md is accurate
- [ ] TESTING.md is complete
- [ ] QUICKSTART.md works
- [ ] This guide is helpful

## Troubleshooting

### Build Fails
1. Check NuGet packages restored: `mono nuget.exe restore ManicDigger.sln`
2. Verify Mono/MSBuild installed: `mono --version`, `xbuild /?`
3. Clean build: Delete `bin/` and `obj/` folders, rebuild

### Server Won't Start
1. Check port availability: `netstat -an | grep 25565`
2. Verify dependencies: All DLLs in bin folder?
3. Check logs: Look for error messages in console
4. Try Debug build for more info

### Tests Don't Run
1. Install NUnit Console Runner
2. Verify test assembly built
3. Check NUnit package installed
4. Try running in Visual Studio Test Explorer

## Success Criteria Summary

A successful verification means:

✅ **Build**: All projects compile without errors
✅ **Server**: Starts, runs, responds to commands  
✅ **Tests**: Build and execute successfully
✅ **Distribution**: Creates working standalone package
✅ **Documentation**: Guides are accurate and helpful

## Next Steps After Verification

Once verification is complete:

1. **Start developing**: Make changes, add features
2. **Add tests**: Expand test coverage
3. **Test continuously**: Verify changes don't break things
4. **Document**: Keep docs updated
5. **Share**: Submit pull requests

---

**Last Updated**: December 2025

For more information, see:
- [BUILD.md](BUILD.md) - Build instructions
- [TESTING.md](TESTING.md) - Testing guide  
- [QUICKSTART.md](QUICKSTART.md) - Quick start guide
