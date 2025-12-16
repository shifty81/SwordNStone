# GUI Tile Template Guide

## Overview

This guide provides the standard sizes and formats for creating GUI tile templates that properly align with the Sword&Stone GUI system.

## Standard Tile Sizes

Based on the existing GUI system analysis, here are the recommended tile sizes:

### Core UI Elements (Base Size: 8px Grid)

All GUI elements in Sword&Stone use an **8-pixel grid system** for alignment. This means all dimensions should be multiples of 8 pixels.

```
Base Grid Unit: 8px
All dimensions: Multiples of 8 (8, 16, 24, 32, 40, 48, 56, 64, 96, 128...)
```

### Button Sizes

**Small Button:**
- Size: `56x32` pixels
- Use for: Action buttons, tool buttons, small UI controls
- Format: PNG with RGBA
- States: Normal, Hover, Pressed (3 separate files)

**Long Button:**
- Size: `120x32` pixels  
- Use for: Menu buttons, confirm/cancel buttons
- Format: PNG with RGBA
- States: Normal, Hover, Pressed (3 separate files)

**Custom Button:**
- Width: Multiple of 8px (minimum 48px, maximum 256px)
- Height: 32px (standard) or 40px (large)
- Follow 8px grid for all dimensions

### Frame Sizes

**Small Frame:**
- Size: `96x64` pixels
- Use for: Small panels, tooltips, mini-dialogs
- Border width: 8px on all sides
- Inner content area: 80x48 pixels

**Large Ornate Frame:**
- Size: `128x96` pixels
- Use for: Character preview, important UI panels
- Border width: 8-12px (decorative)
- Inner content area: ~104x72 pixels

**Circular Frame:**
- Size: `128x128` pixels
- Use for: Portraits, minimap, circular UI elements
- Border width: 8-12px
- Inner circle diameter: ~104px

**Custom Frame:**
- Minimum: 64x64 pixels
- Maximum: 512x512 pixels (full-screen panels)
- All dimensions: Multiples of 8
- Border: 8-16px recommended

### Panel Sizes

**Panel Dark (Background):**
- Size: `96x56` pixels (base tile)
- Use for: Panel backgrounds that tile or stretch
- Format: PNG with alpha for proper blending
- Tiling: Can be tiled for larger areas

**Panel Medium:**
- Size: `96x56` pixels (base tile)
- Use for: Standard UI backgrounds
- Can be stretched or tiled

**Large Assembled Panels:**
- Inventory: 345x430 pixels
- Crafting: 345x335 pixels  
- Menu Medium: 195x85 pixels
- Menu Long: 360x270 pixels

### Slot Sizes

**Inventory/Hotbar Slot:**
- Size: `48x48` pixels
- Use for: Item slots, ability slots
- Format: PNG with RGBA
- States: Normal, Highlight, Active

**Small Slot:**
- Size: `32x32` pixels
- Use for: Mini-inventory, compact UI

**Large Slot:**
- Size: `64x64` pixels
- Use for: Featured items, large inventory

### Bar Sizes

**Bar Segment (Tiled):**
- Size: `24x32` pixels
- Use for: Health bars, mana bars (tiles horizontally)
- Types: Left cap, Middle (repeating), Right cap

**Full Bar:**
- Size: `192x32` pixels
- Use for: Complete bar (non-tiling)
- Can be 8 segments of 24px each

**Custom Bar:**
- Width: Multiple of 24px (24, 48, 72, 96, 120, 144, 168, 192, 216, 240)
- Height: 32px (standard)

### Decoration Elements

**Small Circle:**
- Size: `24x24` pixels
- Use for: Small icons, dots, indicators

**Medium Circle:**
- Size: `32x32` pixels
- Use for: Medium decorations, status icons

**Large Circle:**
- Size: `48x48` pixels
- Use for: Large decorations, emphasis

## 9-Slice Scaling (Border-Box Method)

For scalable UI elements that need to maintain crisp borders, use 9-slice scaling:

### 9-Slice Template Structure

```
┌─────┬─────────┬─────┐
│ TL  │   T     │ TR  │  Top row (fixed height)
├─────┼─────────┼─────┤
│  L  │ Center  │  R  │  Middle row (stretches vertically)
├─────┼─────────┼─────┤
│ BL  │   B     │ BR  │  Bottom row (fixed height)
└─────┴─────────┴─────┘
```

**Recommended 9-Slice Dimensions:**

For a `96x64` frame:
- Corner size: `16x16` pixels (TL, TR, BL, BR)
- Edge size: Variable width/height
- Center: Stretches to fill

**Template Structure:**
```
Total: 96x64 pixels
Corners: 16x16 each (4 corners)
Top/Bottom edges: 64x16 (stretch horizontally)
Left/Right edges: 16x32 (stretch vertically)
Center: 64x32 (stretch both directions)
```

### Creating a 9-Slice Template

1. **Corner Regions** (16x16 each):
   - Top-Left: (0, 0) to (16, 16)
   - Top-Right: (80, 0) to (96, 16)
   - Bottom-Left: (0, 48) to (16, 64)
   - Bottom-Right: (80, 48) to (96, 64)

2. **Edge Regions**:
   - Top: (16, 0) to (80, 16) - Tiles horizontally
   - Bottom: (16, 48) to (80, 64) - Tiles horizontally
   - Left: (0, 16) to (16, 48) - Tiles vertically
   - Right: (80, 16) to (96, 48) - Tiles vertically

3. **Center Region**:
   - Center: (16, 16) to (80, 48) - Tiles both directions

## Tiling Patterns

### Horizontal Tiling (Bars)

**Bar Middle Segment:**
- Size: `24x32` pixels
- Repeats: Horizontally
- Seamless: Left and right edges must match
- Usage: Health bars, progress bars

**Creating Seamless Tiles:**
1. Design must repeat perfectly on left/right edges
2. Test by placing tiles side-by-side
3. No visible seams should appear

### Vertical Tiling (Rarely Used)

- Height: 32px or 48px segments
- Width: Fixed (96px typical)
- Used for tall panels that extend downward

## Pixel-Perfect Alignment Tips

### 1. Use the 8px Grid

All positioning and sizing should align to 8px grid:
```
✅ Good: 0, 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96...
❌ Bad:  5, 13, 27, 35, 43, 51, 63...
```

### 2. Border Widths

Standard border widths:
- Thin border: 4px
- Medium border: 8px
- Thick border: 12px
- Decorative border: 16px

### 3. Padding

Inner padding (content to border):
- Minimal: 4px
- Standard: 8px
- Comfortable: 12px
- Spacious: 16px

### 4. Element Spacing

Space between UI elements:
- Tight: 4px
- Normal: 8px
- Relaxed: 16px
- Section: 24px or 32px

## Creating New GUI Assets

### Recommended Workflow

1. **Choose Base Size**
   - Determine which category: Button, Frame, Panel, Slot
   - Use standard sizes or multiples of 8

2. **Design in Layers**
   - Background layer
   - Border/frame layer
   - Decoration layer
   - Highlight layer (for states)

3. **Export Settings**
   - Format: PNG
   - Color mode: RGBA (with alpha channel)
   - Bit depth: 32-bit
   - Compression: Standard PNG

4. **Test Alignment**
   - Place asset in game at various positions
   - Verify it aligns with 8px grid
   - Check borders are crisp

5. **Create Variants**
   - Normal state
   - Hover state (lighter, +20-40 brightness)
   - Pressed/Active state (darker or golden tint)

## Common Size Templates

### Quick Reference Chart

```
Element Type          Width    Height   Notes
─────────────────────────────────────────────────────────
Small Button          56       32       Standard action button
Long Button           120      32       Menu button
Small Frame           96       64       Tooltip, small dialog
Large Frame           128      96       Important panel
Circular Frame        128      128      Portrait, minimap
Panel (tile)          96       56       Background tile
Slot                  48       48       Inventory/hotbar
Bar segment           24       32       Health/mana bar
Full bar              192      32       Complete bar
Small icon            24       24       Small decoration
Medium icon           32       32       Status icon
Large icon            48       48       Large item
Portrait              128      128      Character portrait
Minimap               140      240      Complete minimap UI
Inventory panel       345      430      Full inventory
```

## File Naming Conventions

Use descriptive names that indicate size and purpose:

```
button_small_normal.png        (56x32)
button_small_hover.png         (56x32)
button_small_pressed.png       (56x32)

button_long_normal.png         (120x32)
button_long_hover.png          (120x32)
button_long_pressed.png        (120x32)

frame_small.png                (96x64)
frame_large_ornate.png         (128x96)
frame_circular.png             (128x128)

panel_dark.png                 (96x56)
panel_medium.png               (96x56)

slot_normal.png                (48x48)
slot_highlight.png             (48x48)
slot_active.png                (48x48)

bar_left.png                   (24x32)
bar_middle_red.png             (24x32)
bar_right.png                  (24x32)
```

## Testing Your Templates

### In-Game Testing Checklist

- [ ] Asset loads without errors
- [ ] Aligns to 8px grid
- [ ] Borders are crisp (no blurring)
- [ ] Tiles seamlessly (if tileable)
- [ ] Scales properly (if scalable)
- [ ] Alpha channel works correctly
- [ ] Looks good at different resolutions
- [ ] Matches theme style

### Visual Testing

1. Place asset at (0, 0) - Should align perfectly
2. Place asset at (8, 8) - Should align perfectly
3. Place asset at (16, 16) - Should align perfectly
4. Any non-8px position should show misalignment

## Example Templates

### Creating a Custom Button (80x32)

```python
# Python with Pillow
from PIL import Image, ImageDraw

# Create base image
width = 80  # Multiple of 8
height = 32  # Standard button height
img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Draw border (8px)
border_color = (180, 140, 80, 255)  # Golden
draw.rectangle([(0, 0), (width-1, height-1)], 
               outline=border_color, width=2)

# Draw inner border
draw.rectangle([(4, 4), (width-5, height-5)], 
               outline=border_color, width=1)

# Fill center
fill_color = (60, 45, 30, 255)  # Dark brown
draw.rectangle([(8, 8), (width-9, height-9)], 
               fill=fill_color)

img.save('button_custom_normal.png')
```

### Creating a Tileable Bar Middle (24x32)

```python
from PIL import Image, ImageDraw

width = 24
height = 32
img = Image.new('RGBA', (width, height), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Ensure left and right edges match for seamless tiling
# Draw gradient or pattern that repeats perfectly

# Top border
draw.line([(0, 0), (width, 0)], fill=(200, 180, 100, 255), width=2)

# Bottom border  
draw.line([(0, height-1), (width, height-1)], 
          fill=(100, 80, 40, 255), width=2)

# Middle fill (must be seamless left-to-right)
for y in range(4, height-4):
    color_value = 180 - int((y / height) * 80)
    draw.line([(0, y), (width, y)], 
              fill=(color_value, color_value-20, 40, 255))

img.save('bar_middle_custom.png')
```

## Troubleshooting

### Asset Not Aligning

**Problem:** UI element appears offset or misaligned

**Solutions:**
- Verify dimensions are multiples of 8
- Check positioning code uses 8px increments
- Ensure PNG has no padding/margins
- Verify canvas size matches expected size

### Seams Visible in Tiling

**Problem:** Lines or gaps when tiles repeat

**Solutions:**
- Ensure left edge pixels match right edge pixels
- Ensure top edge pixels match bottom edge pixels
- Test tile by repeating 3x3 grid
- Use clone stamp tool for seamless edges

### Blurry Borders

**Problem:** Borders look blurred or anti-aliased

**Solutions:**
- Use nearest-neighbor scaling, not bilinear
- Ensure borders are on pixel boundaries
- Avoid fractional positions (use integers only)
- Check texture filtering settings

### Size Too Small/Large

**Problem:** Element appears wrong size in-game

**Solutions:**
- Check scale factor in rendering code
- Verify screen resolution handling
- Test on different screen sizes
- Adjust base size to match 8px grid

## Resources

### Design Tools

- **Aseprite** - Excellent for pixel art and UI
- **GIMP** - Free, supports all features needed
- **Photoshop** - Industry standard
- **Krita** - Free, good for digital painting
- **Paint.NET** - Simple, Windows-friendly

### Online Generators

- 9-slice grid generators
- Seamless pattern creators
- Color palette generators

### Testing

- Test in-game at 1920x1080, 1280x720, 2560x1440
- Test with UI scaling at 1.0x, 1.5x, 2.0x
- Test on different themes if available

## Conclusion

Follow these guidelines for pixel-perfect GUI alignment:

1. ✅ Use 8px grid for all dimensions
2. ✅ Multiple of 8 for width and height
3. ✅ Standard sizes when possible
4. ✅ PNG with RGBA format
5. ✅ Test alignment in-game
6. ✅ Create state variants (normal/hover/pressed)
7. ✅ Match existing golden steampunk theme

For questions or issues, see:
- [GUI_IMPLEMENTATION_SUMMARY.md](GUI_IMPLEMENTATION_SUMMARY.md)
- [ASSEMBLED_GUI_IMPLEMENTATION.md](ASSEMBLED_GUI_IMPLEMENTATION.md)
