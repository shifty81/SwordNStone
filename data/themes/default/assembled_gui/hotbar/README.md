# Hotbar Assets

This directory contains the hotbar UI assets for the game.

## Files

### hotbar_10slot.png
- **Dimensions**: 598x66 pixels
- **Description**: Complete hotbar template with 10 inventory slots
- **Design**: Bronze/orange bordered slots with dark grey backgrounds
- **Usage**: Used as the background for the main hotbar UI in-game

### hotbar_3slot.png
- **Dimensions**: 160x40 pixels
- **Description**: Original 3-slot hotbar extracted from Assembled_Gui_Pieces.PNG
- **Purpose**: Reference design for creating extended hotbar versions

## Design Specifications

### Colors
- **Bronze/Orange Border**: RGB(200, 120, 40) - Medium bronze
- **Bronze Highlight**: RGB(220, 150, 60) - Light bronze/orange
- **Dark Bronze**: RGB(160, 90, 30) - Outer border
- **Slot Background**: RGB(60, 55, 50) - Dark grey

### Layout
- **Slot Size**: 48x48 pixels (inner area)
- **Border Thickness**: 4 pixels
- **Slot Spacing**: 2 pixels between slots
- **Rivet Size**: 10 pixels diameter
- **Total Slots**: 10 (numbered 1-9, 0)

### Style
- Steampunk/industrial theme with bronze metallic borders
- Decorative rivets at corners
- Consistent with the Assembled_Gui_Pieces.PNG aesthetic
- Matches other GUI elements (capsule bars, minimap, inventory panels)

## Integration

The hotbar is integrated via:
- **Main File**: `data/hotbar.png` (copy of hotbar_10slot.png)
- **Code**: `SwordAndStoneLib/Client/Mods/GuiHotbar.ci.cs`
- **Method**: `DrawCustomHotbar()` renders the hotbar background image

## Related Assets

- `data/themes/default/assembled_gui/bars/slot_normal.png` - Default slot appearance
- `data/themes/default/assembled_gui/bars/slot_hover.png` - Hovered slot appearance
- `data/themes/default/assembled_gui/bars/slot_active.png` - Active/selected slot appearance

These individual slot images have been updated to match the hotbar template design.

## Source

Extracted and extended from `Assembled_Gui_Pieces.PNG` in the repository root, which contains the complete GUI component sheet for the game.
