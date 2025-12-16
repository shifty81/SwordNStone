# Build Troubleshooting Guide

## Common Build Errors and Solutions

This guide addresses common build errors you may encounter when building Sword&Stone.

---

## Error 1: "Expected RightParenthesis, got Id" (PixelArtEditor.ci.cs line 325)

### Problem
```
Error: Expected RightParenthesis, got Id
File: PixelArtEditor.ci.cs, Line: 325
```

### Solution
**FIXED** in latest commit. The issue was using `ref` parameters with `ColorPicker.GetColorComponents()`. Changed to inline bit operations.

If you still see this error, pull the latest changes:
```bash
git pull origin copilot/implement-ui-theme-editor
```

---

## Error 2: "The referenced component 'protobuf-net' could not be found"

### Problem
```
Warning: The referenced component 'protobuf-net' could not be found.
Project: SwordAndStoneLib
```

### Solution
**Restore NuGet Packages**

#### Method 1: Visual Studio (Recommended)
1. Open `SwordAndStone.sln` in Visual Studio
2. Right-click on Solution in Solution Explorer
3. Click **"Restore NuGet Packages"**
4. Wait for restoration to complete
5. Rebuild solution

#### Method 2: Command Line
```cmd
# Navigate to solution directory
cd C:\GIT PROJECTS\SwordNStone

# Restore packages
nuget restore SwordAndStone.sln

# Or if nuget.exe is not in PATH:
.\nuget.exe restore SwordAndStone.sln
```

#### Method 3: Download NuGet CLI
If you don't have nuget.exe:
```cmd
# Download NuGet.exe
curl -o nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe

# Then restore
nuget.exe restore SwordAndStone.sln
```

---

## Error 3: "The referenced component 'OpenTK' could not be found"

### Problem
```
Warning: The referenced component 'OpenTK' could not be found.
Project: SwordAndStone
```

### Solution
Same as Error 2 - **Restore NuGet Packages**

The packages required are:
- `OpenTK` version 2.0.0 (for SwordAndStone project)
- `protobuf-net` version 2.1.0 (for SwordAndStoneLib project)

After NuGet restore, these will be in:
```
packages/
├── OpenTK.2.0.0/
└── protobuf-net.2.1.0/
```

---

## Error 4: "Metadata file 'SwordAndStoneLib.dll' could not be found"

### Problem
```
Error CS0006: Metadata file 'C:\GIT PROJECTS\SwordNStone\SwordAndStoneLib\bin\Debug\SwordAndStoneLib.dll' could not be found
Project: SwordAndStone
```

### Root Cause
This happens when `SwordAndStoneLib` project fails to build, usually because:
1. NuGet packages not restored (protobuf-net missing)
2. CiTo transpilation failed
3. Build order issue

### Solution

**Step 1: Restore NuGet Packages**
```cmd
nuget restore SwordAndStone.sln
```

**Step 2: Clean Solution**
In Visual Studio:
- Build → Clean Solution

Or command line:
```cmd
msbuild SwordAndStone.sln /t:Clean
```

**Step 3: Build SwordAndStoneLib First**
In Visual Studio:
- Right-click `SwordAndStoneLib` project
- Click **"Build"**

Or command line:
```cmd
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug
```

**Step 4: Build Entire Solution**
```cmd
msbuild SwordAndStone.sln /p:Configuration=Debug
```

---

## Error 5: "BuildCito.bat exited with code -1"

### Problem
```
Error: The command "cd "C:\GIT PROJECTS\SwordNStone\"
cmd /c ""C:\GIT PROJECTS\SwordNStone\BuildCito.bat""" exited with code -1.
Project: SwordAndStoneLib
```

### Root Cause
The CiTo transpiler (converts .ci.cs files to .cs files) failed. This can happen due to:
1. Syntax errors in .ci.cs files
2. CiTo.exe not found
3. Path with spaces causing issues
4. CiTo version compatibility

### Solution

**Check CiTo.exe Exists**
```cmd
dir CiTo.exe
```
Should show the file in the root directory.

**Check BuildCito.bat**
Open `BuildCito.bat` and verify the path is correctly quoted:
```batch
@echo off
cd /d "%~dp0"
CiTo.exe -l cs SwordAndStoneLib\SwordAndStoneLib.csproj
```

**Manual CiTo Run**
Try running CiTo manually:
```cmd
cd C:\GIT PROJECTS\SwordNStone
CiTo.exe -l cs SwordAndStoneLib\SwordAndStoneLib.csproj
```

Look for specific error messages about which .ci.cs file has issues.

**Check Recent .ci.cs Changes**
If the error started after recent changes, check syntax in:
- `SwordAndStoneLib/Client/MainMenu/PixelArtEditor.ci.cs`
- `SwordAndStoneLib/Client/MainMenu/ThemeEditor.ci.cs`
- `SwordAndStoneLib/Client/Misc/ThemeCanvas.ci.cs`

**Common CiTo Syntax Issues**
- ❌ `ref` parameters in method calls: `Method(ref a, ref b)`
- ✅ Use inline operations instead: `a = (value >> 24) & 0xFF;`
- ❌ Complex lambda expressions
- ✅ Keep expressions simple

---

## Error 6: ".NET Framework 4.8 not found"

### Problem
```
Error: The reference assemblies for .NETFramework,Version=v4.8 were not found.
```

### Solution
**Install .NET Framework 4.8 Developer Pack**

Download and install from:
https://dotnet.microsoft.com/download/dotnet-framework/net48

Make sure to install the **Developer Pack**, not just the runtime.

---

## Complete Build Process (Fresh Start)

If you're having multiple issues, follow this complete process:

### 1. Prerequisites Check
```cmd
# Check .NET Framework
dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8"

# Check Visual Studio installation
where msbuild
```

### 2. Clean Everything
```cmd
cd C:\GIT PROJECTS\SwordNStone

# Delete bin and obj directories
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"

# Delete packages (they'll be restored)
rd /s/q packages
```

### 3. Restore NuGet Packages
```cmd
nuget restore SwordAndStone.sln
```

Verify packages restored:
```cmd
dir packages\OpenTK.2.0.0
dir packages\protobuf-net.2.1.0
```

### 4. Build Solution
```cmd
msbuild SwordAndStone.sln /p:Configuration=Debug /v:detailed
```

The `/v:detailed` flag shows more information if errors occur.

### 5. Verify Output
```cmd
dir SwordAndStoneLib\bin\Debug\SwordAndStoneLib.dll
dir SwordAndStone\bin\Debug\SwordAndStone.exe
```

---

## Build in Visual Studio

### Recommended Steps

1. **Open Solution**
   - Open `SwordAndStone.sln` in Visual Studio

2. **Restore Packages**
   - Right-click solution → "Restore NuGet Packages"
   - Wait for completion (check Output window)

3. **Clean Solution**
   - Build → Clean Solution

4. **Build Solution**
   - Build → Build Solution (Ctrl+Shift+B)

5. **Check Build Output**
   - View → Output
   - Check "Build" in dropdown
   - Look for errors

6. **Run the Game**
   - Press F5 to debug
   - Or Ctrl+F5 to run without debugging

---

## Specific Project Errors

### SwordAndStoneLib Build Errors

**Most Common:**
- Missing protobuf-net → Restore NuGet packages
- CiTo transpilation failed → Check .ci.cs syntax
- Build order issue → Build this project first

**Fix:**
```cmd
nuget restore SwordAndStone.sln
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug
```

### SwordAndStone (Main Client) Build Errors

**Most Common:**
- Missing OpenTK → Restore NuGet packages
- SwordAndStoneLib.dll not found → Build SwordAndStoneLib first

**Fix:**
```cmd
nuget restore SwordAndStone.sln
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug
msbuild SwordAndStone\SwordAndStone.csproj /p:Configuration=Debug
```

---

## Verifying the Fix

After fixing build errors, verify everything works:

### 1. Check DLL Dependencies
```cmd
dir SwordAndStoneLib\bin\Debug\*.dll
dir SwordAndStone\bin\Debug\*.dll
```

Should see:
- SwordAndStoneLib.dll
- protobuf-net.dll
- OpenTK.dll

### 2. Run Tests (if available)
```cmd
cd SwordAndStone.Tests\bin\Debug
SwordAndStone.Tests.exe
```

### 3. Run the Game
```cmd
cd SwordAndStone\bin\Debug
SwordAndStone.exe
```

Should launch without errors.

---

## Getting More Help

### Log Files
Build logs are in:
```
SwordAndStoneLib\bin\Debug\build.log
SwordAndStone\bin\Debug\build.log
```

### Detailed Build Output
```cmd
msbuild SwordAndStone.sln /p:Configuration=Debug /v:diagnostic > build.log 2>&1
```

Then check `build.log` for detailed error information.

### Check GitHub Issues
Search for similar issues:
https://github.com/shifty81/SwordNStone/issues

### Ask for Help
If stuck, provide:
1. Full error message
2. Visual Studio version
3. .NET Framework version
4. Build log excerpt
5. What you've tried

---

## Recent Changes (Theme Editor Implementation)

If you're building after the theme editor implementation, note these new files:

**New Files:**
- `SwordAndStoneLib/Client/Misc/ThemeCanvas.ci.cs`
- `SwordAndStoneLib/Client/MainMenu/ThemeEditor.ci.cs`
- `SwordAndStoneLib/Client/Mods/UIThemeManager.ci.cs` (modified)
- `SwordAndStoneLib/Client/MainMenu.ci.cs` (modified)

**Fixed Issues:**
- Line 325 in PixelArtEditor.ci.cs (ref parameters)
- CiTo compatibility issues

**These should build without issues after latest commit.**

---

## Quick Reference

### Commands Cheat Sheet
```cmd
# Restore packages
nuget restore SwordAndStone.sln

# Clean
msbuild SwordAndStone.sln /t:Clean

# Build Debug
msbuild SwordAndStone.sln /p:Configuration=Debug

# Build Release
msbuild SwordAndStone.sln /p:Configuration=Release

# Build specific project
msbuild SwordAndStoneLib\SwordAndStoneLib.csproj /p:Configuration=Debug

# Run CiTo manually
CiTo.exe -l cs SwordAndStoneLib\SwordAndStoneLib.csproj
```

### Files to Check When Errors Occur
1. `packages.config` - NuGet dependencies
2. `*.csproj` - Project configuration
3. `BuildCito.bat` - CiTo transpiler script
4. `.ci.cs` files - Source files that get transpiled

---

**Need More Help?**

- Check [BUILD.md](BUILD.md) for general build instructions
- See GitHub Issues for known problems
- Ask on community Discord or forums

**Build Successfully?**

Great! Check out:
- [THEME_EDITOR_GUIDE.md](THEME_EDITOR_GUIDE.md) - Use the theme editor
- [QUICK_START_THEME_EDITOR.md](QUICK_START_THEME_EDITOR.md) - Quick start guide
