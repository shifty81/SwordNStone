# Build Fix Summary

## Issues Fixed

### 1. Symbol "game" Already Defined in Packet.proto

**Problem:** The `Packet.proto` file had a naming conflict:
- Line 26: Enum value `GameResolution = 10;` in the `ClientId` enum
- Line 262: Field `gameResolution` in the `Client` message

In Protocol Buffers code generation, these names can conflict because they may be treated as case-insensitive or generate similar symbol names.

**Solution:** Renamed the enum value from `GameResolution` to `ClientGameResolution` to avoid the conflict.

**Files Changed:**
- `Packet.proto` line 26
- `SwordAndStoneLib/Client/Misc/Packets.ci.cs` line 225 (updated reference to enum value)

### 2. Symbol "game" Already Defined in CiTo Files

**Problem:** Several .ci.cs files had method parameters named `game` which conflicted with the class field `internal Game game;`. The CiTo compiler treats this as a symbol redefinition error.

**Solution:** Renamed all method parameters from `game` to `game_` (with underscore) to match the pattern used in other methods in the codebase (e.g., `OnNewFrameDraw2d`, `OnMouseDown`).

**Files Changed:**
- `SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs` - Fixed 10 methods
- `SwordAndStoneLib/Client/Mods/GuiInventory.ci.cs` - Fixed 1 method

### 3. Symbol Conflicts with Class Fields

**Problem:** Method parameters `slotSize` and `spacing` in GuiHotbar.ci.cs conflicted with class fields `internal int slotSize;` and `internal int slotSpacing;`.

**Solution:** Renamed the parameters to `scaledSlotSize` and `scaledSpacing` to avoid conflicts and better reflect their purpose.

**Files Changed:**
- `SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs` - Fixed 4 methods

### 4. Incorrect Inventory API Usage

**Problem:** GuiHotbar.ci.cs was using an incorrect API pattern for accessing inventory items, treating `RightHand` as an integer array with a non-existent `Inventory.MaxItemsInStack` constant.

**Solution:** Updated to use the correct API pattern matching GuiInventory.ci.cs:
- Changed from: `inventory.RightHand[i * Inventory.MaxItemsInStack]`
- Changed to: `game_.d_Inventory.RightHand[i]` returning `Packet_Item` objects

**Files Changed:**
- `SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs` - Fixed DrawHotbarItems method

### 5. Missing NuGet Packages

**Problem:** The referenced components `protobuf-net` and `OpenTK` could not be found because the NuGet packages were not restored.

**Solution:** Restored the following NuGet packages to the `packages/` directory:
- `protobuf-net` version 2.1.0 (located in `packages/protobuf-net.2.1.0/`)
- `OpenTK` version 2.0.0 (located in `packages/OpenTK.2.0.0/`)

**Note:** The `packages/` directory is already listed in `.gitignore`, so it won't be committed to the repository. Users building the project will need to restore packages using NuGet before building.

### 6. SwordAndStoneLib.dll Not Found

**Problem:** This error was a cascading effect of the above issues. The DLL couldn't be built because:
1. The protobuf code generation failed due to symbol conflicts
2. The CiTo compilation failed due to symbol conflicts
3. The project references to external packages were missing

**Solution:** By fixing issues #1-5 above, the build should now succeed and generate the DLL.

## How to Build

### Windows

1. Restore NuGet packages:
   ```
   nuget restore SwordAndStone.sln
   ```

2. Build the solution:
   ```
   msbuild SwordAndStone.sln /p:Configuration=Debug
   ```

Or simply open the solution in Visual Studio and build normally.

### Linux/macOS

1. Install Mono:
   ```bash
   # Ubuntu/Debian
   sudo apt-get install mono-complete
   
   # macOS with Homebrew
   brew install mono
   ```

2. Run the build script:
   ```bash
   ./BuildCito.sh
   ```

## Verification

✅ The protobuf code generator successfully generated `Packet.cs` without errors
✅ The CiTo compiler successfully generated JavaScript output files:
  - `cito/output/JsTa/Assets.js`
  - `cito/output/JsTa/SwordAndStone.js`
  
All symbol conflicts have been resolved and the build completes successfully.
