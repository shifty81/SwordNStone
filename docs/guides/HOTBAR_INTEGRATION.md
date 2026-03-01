# Hotbar.png Integration Guide

## Overview

The `ModGuiHotbar.ci.cs` module is designed to integrate the `hotbar.png` image when it becomes available. This document explains how the integration works and what to do once the image is added to the repository.

## Current Status

**Status**: ✅ Ready for Integration  
**Acknowledged**: hotbar.png will be provided  
**Module**: `SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs`

## What to Do When hotbar.png is Available

### Step 1: Add the Image

Place `hotbar.png` in the appropriate location:

```
/home/runner/work/SwordNStone/SwordNStone/data/themes/default/assembled_gui/hotbar/hotbar.png
```

Or alternatively in the root:
```
/home/runner/work/SwordNStone/SwordNStone/hotbar.png
```

### Step 2: Update the Path

In `GuiHotbar.ci.cs`, update the `CheckHotbarImageExists` method:

**Before**:
```csharp
bool CheckHotbarImageExists(Game game)
{
    // Check if hotbar.png exists in theme directory
    // This will be true once hotbar.png is added to the repository
    return false; // Placeholder - will be updated when image is available
}
```

**After**:
```csharp
bool CheckHotbarImageExists(Game game)
{
    // Check if hotbar.png exists
    useCustomHotbarImage = true; // Set flag to use custom image
    return true;
}
```

### Step 3: Update the Path in DrawCustomHotbar

In `GuiHotbar.ci.cs`, update the `DrawCustomHotbar` method:

**Before**:
```csharp
void DrawCustomHotbar(Game game, int startX, int startY, int slotSize, int spacing)
{
    // Use the custom hotbar.png image when available
    string hotbarPath = "hotbar.png"; // Will be moved to proper location
    int hotbarWidth = slotCount * slotSize + (slotCount - 1) * spacing + 20;
    int hotbarHeight = slotSize + 20;
    
    game.Draw2dBitmapFile(hotbarPath, startX - 10, startY - 10, hotbarWidth, hotbarHeight);
}
```

**After** (adjust path as needed):
```csharp
void DrawCustomHotbar(Game game, int startX, int startY, int slotSize, int spacing)
{
    // Use the custom hotbar.png image
    string hotbarPath = "data/themes/default/assembled_gui/hotbar/hotbar.png";
    int hotbarWidth = slotCount * slotSize + (slotCount - 1) * spacing + 20;
    int hotbarHeight = slotSize + 20;
    
    game.Draw2dBitmapFile(hotbarPath, startX - 10, startY - 10, hotbarWidth, hotbarHeight);
}
```

### Step 4: Test

1. Build the project
2. Run the game
3. Verify hotbar displays correctly
4. Check that slots align properly
5. Verify hover and selection states work

## Expected Behavior

### Without hotbar.png (Current)
- Hotbar renders with default golden frame
- 10 individual slots drawn using slot_normal.png
- Golden border around entire hotbar
- Dark background panel
- Fully functional but uses assembled pieces

### With hotbar.png (Future)
- Hotbar uses single custom image as background
- Slots still rendered individually on top
- Custom hotbar design from provided image
- All functionality remains the same
- Better visual integration with provided design

## Design Specifications

When creating or verifying hotbar.png:

### Dimensions
- Width: ~600-650 pixels (for 10 slots)
- Height: ~70-80 pixels
- Should accommodate 10 slots of 50x50 pixels with spacing

### Layout
- 10 slot positions evenly spaced
- Golden steampunk theme consistent with other GUI
- Dark background in slot areas
- Ornate border/frame around entire hotbar

### Color Scheme
- Golden/bronze borders: RGB(184, 134, 11) or similar
- Dark grey background: RGB(40, 35, 30) or similar
- Should match Assembled_Gui_Pieces.PNG aesthetic

### Style Guidelines
- Steampunk/industrial theme
- Rivets, gears, or mechanical details optional
- Should complement existing capsule bars and minimap
- Border should be ~3-4 pixels thick

## Slot Positioning

The hotbar image should have clear areas for 10 slots:

```
[Border] [Slot1] [Space] [Slot2] [Space] ... [Slot10] [Border]
```

**Spacing**:
- Slot size: 50x50 pixels
- Inter-slot spacing: 4 pixels
- Border padding: 10 pixels left/right

**Total calculation**:
- 10 slots × 50px = 500px
- 9 gaps × 4px = 36px
- 2 borders × 10px = 20px
- **Total width**: ~556px minimum

## Code Structure

### Key Methods

**CheckHotbarImageExists()**
- Returns true if hotbar.png is available
- Sets `useCustomHotbarImage` flag
- Called during rendering to determine which method to use

**DrawCustomHotbar()**
- Renders the hotbar.png background image
- Scales to fit 10 slots + spacing + borders
- Only called when hotbar.png is available

**DrawDefaultHotbar()**
- Fallback rendering using assembled GUI pieces
- Draws background panel, border, and individual slots
- Used when hotbar.png is not available (current behavior)

**DrawHotbarItems()**
- Draws items in slots (works with both methods)
- Shows item textures and stack counts
- Independent of background rendering

**DrawSlotNumbers()**
- Draws 1-9, 0 numbers on slots
- Independent of background rendering
- Always visible

## Fallback Behavior

The module is designed with graceful fallback:

1. Check if hotbar.png exists
2. If yes: Use custom image
3. If no: Use assembled GUI pieces
4. Either way: Full functionality maintained

This ensures the game works correctly whether or not hotbar.png is provided.

## Testing Checklist

Once hotbar.png is integrated:

- [ ] Image loads without errors
- [ ] Slots align correctly with hotbar background
- [ ] All 10 slots are visible and properly spaced
- [ ] Items display correctly in slots
- [ ] Stack counts show properly
- [ ] Slot numbers (1-9, 0) are visible
- [ ] Hover effects work on slots
- [ ] Active slot highlighting works
- [ ] Mouse clicks select correct slots
- [ ] Number keys (1-9, 0) select correct slots
- [ ] Hotbar centers correctly on screen
- [ ] Hotbar scales properly at different resolutions
- [ ] Theme matches other GUI elements

## Quick Integration Steps

For a quick integration once hotbar.png is available:

1. **Add image to project**:
   ```bash
   cp hotbar.png data/themes/default/assembled_gui/hotbar/
   ```

2. **Update code** (2 changes in `GuiHotbar.ci.cs`):
   - Line ~78: `return true;` (was `return false`)
   - Line ~91: Update path to match image location

3. **Test**:
   ```bash
   # Build and run project
   # Verify hotbar displays correctly
   ```

4. **Commit**:
   ```bash
   git add data/themes/default/assembled_gui/hotbar/hotbar.png
   git add SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs
   git commit -m "Integrate hotbar.png image"
   ```

## Alternative Paths

If hotbar.png should be in a different location:

**Option 1**: Root directory
```csharp
string hotbarPath = "hotbar.png";
```

**Option 2**: Data directory
```csharp
string hotbarPath = "data/hotbar.png";
```

**Option 3**: GUI directory
```csharp
string hotbarPath = "data/gui/hotbar.png";
```

**Option 4**: Theme directory (recommended)
```csharp
string hotbarPath = "data/themes/default/assembled_gui/hotbar/hotbar.png";
```

Choose the path that matches your project's asset organization.

## Contact

If hotbar.png has specific requirements or dimensions that differ from these specifications, the code can be easily adjusted to accommodate them.

## Notes

- The module is fully functional without hotbar.png
- Integration is straightforward (2 line changes)
- No breaking changes to existing functionality
- Maintains backward compatibility
- Ready for immediate integration when image is available

---

**Last Updated**: 2025-12-14  
**Status**: Awaiting hotbar.png  
**Module Version**: 1.0
