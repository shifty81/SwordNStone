# Standardized UI Framework - Golden UI System

## Overview

This document describes the standardized UI framework for Manic Digger, based on the "GOLDEN UI - BIGGER THAN EVER EDITION" asset sheet (`ui_big_pieces.png`). All GUI implementations should use this consistent style moving forward.

## Design Philosophy

**Key Principle:** All UI components (frames, buttons, bars, slots) should use the same golden/brown ornate border style with dark gray backgrounds for consistency and visual cohesion.

### Color Palette
- **Primary Border:** Golden/Brown (#A06428, #C07830)
- **Dark Gray Background:** #323232, #505050
- **Highlight/Active:** Brighter golden (#FFD700)
- **Health Bar:** Red gradient
- **Oxygen/Mana Bar:** Blue gradient

## Architecture

### GuiFrameRenderer.ci.cs

Central utility class that provides standardized rendering methods for all UI components. This ensures consistency across all GUI implementations.

**Location:** `/SwordAndStoneLib/Client/Mods/GuiFrameRenderer.ci.cs`

### Extracted Assets

All UI pieces are extracted from `ui_big_pieces.png` into individual files in:
**Location:** `/data/local/gui/golden/`

**Total Assets:** 35 individual PNG files

## Component Catalog

### 1. Frames

#### Small Frame
- **File:** `frame_small.png`
- **Size:** 96x64 pixels
- **Usage:** Small panels, tooltips
- **Method:** `GuiFrameRenderer.DrawFrame(game, x, y, width, height, FRAME_SMALL)`

#### Large Ornate Frame
- **File:** `frame_ornate.png`
- **Size:** 128x96 pixels
- **Usage:** Unit frames, character panels, large UI elements
- **Method:** `GuiFrameRenderer.DrawFrame(game, x, y, width, height, FRAME_LARGE_ORNATE)`

#### Circular Frame
- **File:** `frame_circular.png`
- **Size:** 128x128 pixels
- **Usage:** Minimap, circular portraits
- **Method:** `GuiFrameRenderer.DrawCircularFrame(game, x, y, size)`

### 2. Buttons

All buttons use the golden border style with three states:

#### Button States
- **Normal:** `button_normal.png` (56x32)
- **Hover:** `button_hover.png` (56x32)
- **Pressed:** `button_pressed.png` (56x32)

#### Long Buttons (for wider UI elements)
- **Normal:** `button_long_normal.png` (120x32)
- **Hover:** `button_long_hover.png` (120x32)
- **Pressed:** `button_long_pressed.png` (120x32)

**Usage:**
```csharp
GuiFrameRenderer.DrawButton(game, x, y, width, height, GuiFrameRenderer.BUTTON_NORMAL);
GuiFrameRenderer.DrawButton(game, x, y, width, height, GuiFrameRenderer.BUTTON_HOVER);
GuiFrameRenderer.DrawButton(game, x, y, width, height, GuiFrameRenderer.BUTTON_PRESSED);
```

### 3. Progress/Health Bars

Bars use golden borders with colored fills:

#### Bar Components
- **Left Cap:** `bar_left.png` (24x32)
- **Middle Sections:** Various colored middles for gradient effect
- **Right Cap:** `bar_right.png` (24x32)
- **Full Red Bar:** `bar_full_red.png` (192x32) - for health
- **Full Blue Bar:** `bar_full_blue.png` (192x32) - for oxygen/mana

**Usage:**
```csharp
// Red bar (health)
GuiFrameRenderer.DrawProgressBar(game, x, y, width, height, progress, 0);

// Blue bar (oxygen/mana)
GuiFrameRenderer.DrawProgressBar(game, x, y, width, height, progress, 1);
```

### 4. Inventory Slots

Slots for action bars and inventory grids:

#### Slot States
- **Normal:** `slot_normal.png` (48x48)
- **Active/Selected:** `slot_active.png` (48x48)
- **Highlight/Hover:** `slot_highlight.png` (48x48)

**Usage:**
```csharp
// Normal slot
GuiFrameRenderer.DrawSlot(game, x, y, size, false);

// Active/selected slot
GuiFrameRenderer.DrawSlot(game, x, y, size, true);
```

### 5. Portrait Borders

Circular borders for character portraits:

- **File:** `portrait_border.png` (128x128)
- **Usage:** Player portraits, target portraits, circular displays

**Usage:**
```csharp
GuiFrameRenderer.DrawPortraitBorder(game, x, y, size);
```

### 6. Additional Assets

- **Action Bar Section:** `actionbar_section.png` (160x48)
- **Dark Panel:** `panel_dark.png` (96x56)
- **Medium Panel:** `panel_medium.png` (96x56)
- **Decorative Circles:** Various sizes for UI embellishment

## Implementation Examples

### Unit Frames (GuiWoWUnitFrames.ci.cs)

**Before:**
```csharp
// Used custom WoW-style textures
game.Draw2dBitmapFile("local/gui/wow/player_frame.png", x, y, width, height);
```

**After (Standardized):**
```csharp
// Uses standardized golden frame
GuiFrameRenderer.DrawFrame(game, x, y, width, height, GuiFrameRenderer.FRAME_LARGE_ORNATE);
```

**Health Bars:**
```csharp
// Before: Custom gradient drawing
DrawBar(game, barX, barY, barWidth, barHeight, healthProgress, 
    Game.ColorFromArgb(255, 0, 150, 0), 
    Game.ColorFromArgb(255, 0, 255, 0));

// After: Standardized golden bar
GuiFrameRenderer.DrawProgressBar(game, barX, barY, barWidth, barHeight, healthProgress, 0);
```

### Action Bars (GuiWoWActionBars.ci.cs)

**Slots:**
```csharp
// Before: Custom button textures
string buttonTexture = "local/gui/wow/button_normal.png";
if (state == hover) buttonTexture = "local/gui/wow/button_hover.png";
game.Draw2dBitmapFile(buttonTexture, x, y, size, size);

// After: Standardized slots
bool isHighlighted = (i == game.ActiveMaterial);
GuiFrameRenderer.DrawSlot(game, x, y, size, isHighlighted);
```

### Minimap (GuiWoWMinimap.ci.cs)

**Circular Frame:**
```csharp
// Before: Custom minimap border
game.Draw2dBitmapFile("local/gui/wow/minimap_border.png", x, y, size, size);

// After: Standardized circular frame
GuiFrameRenderer.DrawCircularFrame(game, x, y, size);
```

## Standards for Future Implementations

### Rule 1: Always Use GuiFrameRenderer

All new GUI code must use `GuiFrameRenderer` methods instead of drawing frames manually.

**❌ Don't Do This:**
```csharp
game.Draw2dTexture(game.WhiteTexture(), x, y, width, height, null, 0, 
    Game.ColorFromArgb(255, 100, 100, 100), false);
```

**✅ Do This:**
```csharp
GuiFrameRenderer.DrawFrame(game, x, y, width, height, GuiFrameRenderer.FRAME_SMALL);
```

### Rule 2: Use Consistent Asset Paths

All golden UI assets are in `/data/local/gui/golden/`. Use the constants defined in `GuiFrameRenderer`:

```csharp
internal const string GOLDEN_UI_PATH = "local/gui/golden/";
```

### Rule 3: Frame Type Selection

Choose the appropriate frame type for your UI element:

- **Small UI elements, tooltips:** `FRAME_SMALL`
- **Character panels, unit frames, large UI:** `FRAME_LARGE_ORNATE`
- **Minimap, circular displays:** `FRAME_CIRCULAR`

### Rule 4: Bar Type Selection

Use the correct bar type for different stats:

- **Health, damage:** Red bar (`barType = 0`)
- **Oxygen, mana, stamina:** Blue bar (`barType = 1`)

### Rule 5: Slot States

Properly indicate slot states:

- **Normal/inactive:** `highlighted = false`
- **Selected/active:** `highlighted = true`
- Consider hover states in the future

## Migration Guide

### Converting Existing GUI Code

**Step 1:** Identify frame drawing code
```csharp
// Find patterns like:
game.Draw2dBitmapFile("custom_frame.png", ...);
game.Draw2dTexture(game.WhiteTexture(), ...); // for backgrounds
```

**Step 2:** Replace with GuiFrameRenderer
```csharp
GuiFrameRenderer.DrawFrame(game, x, y, width, height, frameType);
```

**Step 3:** Update bar rendering
```csharp
// Replace custom bar code with:
GuiFrameRenderer.DrawProgressBar(game, x, y, width, height, progress, barType);
```

**Step 4:** Update slots/buttons
```csharp
GuiFrameRenderer.DrawSlot(game, x, y, size, highlighted);
```

**Step 5:** Test scaling
Ensure your UI scales properly with `game.Scale()` factor.

## Benefits of Standardization

### 1. Visual Consistency
All UI elements share the same golden/brown ornate style, creating a cohesive visual identity.

### 2. Maintainability
Changes to the UI style only need to be made in one place (the asset files), not in every GUI component.

### 3. Code Reuse
`GuiFrameRenderer` eliminates duplicate frame-drawing code across multiple GUI implementations.

### 4. Easier Development
New GUI features can be implemented faster using pre-made, tested components.

### 5. Asset Management
All UI assets are organized in one location (`/data/local/gui/golden/`), making them easy to find and update.

## Asset Creation Guidelines

If you need to create new UI assets that match this style:

### Color Requirements
- **Border:** Golden/brown tones (#A06428, #C07830, #804020)
- **Background:** Dark gray (#323232, #404040, #505050)
- **Highlights:** Brighter gold (#FFD700, #E0B000)

### Style Requirements
- Ornate, slightly embossed borders
- Subtle gradient on borders (darker on bottom, lighter on top)
- Dark gray inner fill area
- Rounded corners where appropriate
- Consistent border thickness (4-6 pixels)

### Technical Requirements
- PNG format with alpha transparency
- Power-of-2 dimensions preferred (32, 48, 64, 96, 128, 256)
- Anti-aliased edges
- Suitable for scaling (avoid too much fine detail)

## Troubleshooting

### Issue: Frames Look Stretched
**Solution:** Ensure you're using the correct frame type for your size requirements. Very large frames may need a custom size variant.

### Issue: Assets Not Loading
**Solution:** Verify the file path. All assets should be in `/data/local/gui/golden/` and referenced as `local/gui/golden/filename.png`.

### Issue: Colors Don't Match
**Solution:** Use the standardized bar types (0=red, 1=blue) instead of custom colors. For custom colors, you may need to create new bar assets.

### Issue: Slots Don't Show Active State
**Solution:** Make sure you're passing the correct `highlighted` parameter to `DrawSlot()`.

## Future Enhancements

### Potential Additions
1. **9-Slice Rendering:** Proper corner/edge rendering for frames that scale better
2. **Additional Bar Colors:** Green bars, yellow bars, multi-segment bars
3. **Animated Elements:** Glowing active states, pulsing highlights
4. **Themed Variants:** Dark theme, light theme, different color schemes
5. **Button Sizes:** More size variants for different use cases

### Extension Pattern

To add new frame types, follow this pattern:

1. Extract or create the asset in `/data/local/gui/golden/`
2. Add a constant to `GuiFrameRenderer`:
   ```csharp
   internal const int FRAME_MY_NEW_TYPE = 3;
   ```
3. Update `DrawFrame()` method to handle the new type
4. Document the new frame type in this file

## Version History

- **v1.0** (December 2025) - Initial standardized UI framework
  - Created GuiFrameRenderer utility class
  - Extracted 35 assets from ui_big_pieces.png
  - Migrated WoW GUI components to use standardized frames
  - Established golden UI as the standard style

## Credits

- **Original Asset Sheet:** "GOLDEN UI - BIGGER THAN EVER EDITION" (ui_big_pieces.png)
- **Framework Design:** Standardized UI System
- **Implementation:** GitHub Copilot with community feedback

## License

This UI framework follows the same license as Manic Digger. See main LICENSE file.

---

**Remember:** All future GUI implementations should use `GuiFrameRenderer` and the golden UI assets for consistency!
