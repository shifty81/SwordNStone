# Assembled GUI Implementation Guide

## Overview

This document describes the implementation of the new modular GUI system based on `Assembled_Gui_Pieces.PNG`. The system provides a cohesive golden/bronze steampunk theme with modular, upgradeable components.

## Design Philosophy

The GUI is built from individual pieces that can be assembled and customized:
- **Modular Design**: Each UI element is a separate component that can be independently styled
- **Theme Consistency**: All elements share the golden steampunk aesthetic
- **Upgradeability**: Capsule bars and other elements support visual progression
- **Responsive**: Elements adapt to different screen sizes and configurations

## Core Components

### 1. Capsule Bars (Top-Left HUD)

**Module**: `ModGuiCapsuleBars.ci.cs`

**Features**:
- HP Bar (Red): Health display with capsule frame
- Mana Bar (Blue): Mana/magic power (placeholder for future system)
- Stamina Bar (Green): Stamina/endurance (placeholder for future system)
- Breath Bar (Cyan): Appears only when underwater, fades in/out

**Upgrade System**:
```csharp
// Upgrade a capsule (extends visual length)
capsuleBars.UpgradeCapsule(0, 3); // Upgrade HP to level 3
capsuleBars.UpgradeCapsule(1, 2); // Upgrade Mana to level 2
```

**Asset Files**:
- `data/themes/default/assembled_gui/bars/capsule_hp_bar.png`
- `data/themes/default/assembled_gui/bars/capsule_mana_bar.png`
- `data/themes/default/assembled_gui/bars/capsule_stamina_bar.png`
- `data/themes/default/assembled_gui/bars/capsule_breath_bar.png`

### 2. Enhanced Minimap (Top-Right HUD)

**Module**: `ModGuiEnhancedMinimap.ci.cs`

**Features**:
- Circular minimap with golden frame
- Zoom controls (+/-): 5 zoom levels (16, 32, 64, 128, 256 blocks)
- World Map button: Opens full-screen map overlay
- World Map features:
  - Pan and zoom functionality
  - Drag to move around map
  - ESC to close
  - Shows larger area of world

**Controls**:
- Click "+" button to zoom in (closer view)
- Click "-" button to zoom out (wider view)
- Click "MAP" button to open world map
- In world map: Click and drag to pan, ESC to close

**Asset Files**:
- `data/themes/default/assembled_gui/minimap/minimap_circular.png`
- `data/themes/default/assembled_gui/minimap/minimap_frame_complete.png`
- `data/themes/default/assembled_gui/buttons/button_next.png` (+ button)
- `data/themes/default/assembled_gui/buttons/button_settings.png` (- button)
- `data/themes/default/assembled_gui/buttons/button_menu.png` (MAP button)

### 3. Hotbar (Bottom-Center HUD)

**Module**: `ModGuiHotbar.ci.cs`

**Features**:
- 10-slot quick-access bar
- Number key bindings (1-9, 0)
- Mouse click selection
- Hover effects
- Active slot highlighting
- Item display with stack counts
- Ready for `hotbar.png` integration when available

**Controls**:
- Press 1-9, 0 keys to select slots
- Click slots with mouse
- Hover over slots to see highlight

**Asset Files**:
- `data/themes/default/assembled_gui/bars/slot_normal.png`
- `data/themes/default/assembled_gui/bars/slot_hover.png`
- `data/themes/default/assembled_gui/bars/slot_active.png`
- `hotbar.png` (when provided - will replace default rendering)

### 4. Updated Inventory

**Module**: `ModGuiInventory.ci.cs` (modified)

**Changes**:
- Uses `panel_inventory_large.png` for main background
- Individual slot rendering with new slot graphics
- Scroll buttons use new assembled GUI button style
- Equipment slots styled consistently

**Asset Files**:
- `data/themes/default/assembled_gui/inventory/panel_inventory_large.png`
- `data/themes/default/assembled_gui/inventory/panel_crafting_large.png`
- All slot files from hotbar section

### 5. Updated Escape Menu

**Module**: `ModGuiEscapeMenu.ci.cs` (modified)

**Changes**:
- Uses `panel_long_titled.png` for content area
- Tab buttons show lighter grey on hover/selection (as per design spec)
- Golden highlight for active tab
- Consistent with steampunk theme

**Asset Files**:
- `data/themes/default/assembled_gui/menus/panel_long_titled.png`
- `data/themes/default/assembled_gui/menus/panel_medium_titled.png`
- `data/themes/default/assembled_gui/menus/panel_small_titled.png`

## GuiFrameRenderer Extensions

New methods added to `GuiFrameRenderer.ci.cs`:

### DrawCapsuleBar
```csharp
GuiFrameRenderer.DrawCapsuleBar(game, x, y, width, height, 
    progress, "hp", colorRed);
```
Draws a capsule bar with progress fill. Types: "hp", "mana", "stamina", "breath"

### DrawZoomButton
```csharp
GuiFrameRenderer.DrawZoomButton(game, x, y, size, true, hover);
```
Draws a zoom button. `true` for zoom in (+), `false` for zoom out (-)

### DrawAssembledPanel
```csharp
GuiFrameRenderer.DrawAssembledPanel(game, x, y, width, height, "menu_long");
```
Draws a panel from assembled GUI. Types: "inventory_large", "crafting_large", "menu_long", "menu_medium", "menu_small"

### DrawMinimapFrame
```csharp
GuiFrameRenderer.DrawMinimapFrame(game, x, y, size);
```
Draws the circular minimap frame

### DrawIconButton
```csharp
GuiFrameRenderer.DrawIconButton(game, x, y, size, "close", hover);
```
Draws an icon button. Types: "close", "menu", "settings", "next"

## Asset Organization

```
data/themes/default/assembled_gui/
├── bars/
│   ├── capsule_hp_bar.png
│   ├── capsule_mana_bar.png
│   ├── capsule_stamina_bar.png
│   ├── capsule_breath_bar.png
│   ├── slot_normal.png
│   ├── slot_hover.png
│   └── slot_active.png
├── buttons/
│   ├── button_close.png
│   ├── button_menu.png
│   ├── button_next.png
│   └── button_settings.png
├── inventory/
│   ├── panel_inventory_large.png
│   ├── panel_crafting_large.png
│   └── panel_inventory_titled.png
├── menus/
│   ├── panel_long_titled.png
│   ├── panel_medium_titled.png
│   ├── panel_medium.png
│   └── panel_small_titled.png
└── minimap/
    ├── minimap_circular.png
    └── minimap_frame_complete.png
```

## Module Registration

In `Game.ci.cs`, modules are registered in this order:

```csharp
// New assembled GUI system
AddMod(new ModGuiCapsuleBars());      // HP/Mana/Stamina capsule bars
AddMod(new ModGuiEnhancedMinimap());  // Minimap with zoom and world map
AddMod(new ModGuiHotbar());           // Hotbar (bottom-center)
```

Old WoW-style modules are commented out to avoid conflicts.

## Future Enhancements

### Planned Features

1. **Crafting System**:
   - In-world crafting devices
   - Hand-crafting for basic items
   - Backpack upgrade system with crafting slots
   - Different tiers of crafting capabilities

2. **Capsule Upgrades**:
   - Visual progression as bars extend
   - Special effects on upgrade
   - Persistent upgrade state

3. **Hotbar Integration**:
   - Integration of `hotbar.png` when provided
   - Multiple hotbar pages (page switching)
   - Custom key bindings

4. **Inventory Enhancements**:
   - Scrollable inventory (already implemented)
   - Search/filter functionality
   - Item categories and tabs
   - Drag-and-drop between slots

5. **World Map Features**:
   - Waypoints and markers
   - Player-placed pins
   - Area names and zones
   - Fog of war (unexplored areas)

### Adding New Capsule Bar Types

To add a new capsule bar type:

1. Extract the bar graphic from source sheet
2. Save as `capsule_[type]_bar.png` in `bars/` folder
3. Add to `ModGuiCapsuleBars.cs`:

```csharp
// In DrawCapsuleBars method
currentY += scaledBarHeight + scaledSpacing;
DrawCapsuleBar(game, posX, currentY, scaledBarWidth, scaledBarHeight,
    currentValue, maxValue, "new_type_bar.png", capsuleLevel, fillColor);
```

### Creating Custom Panels

To use a custom panel design:

1. Create panel graphic matching the steampunk theme
2. Save in appropriate subfolder (bars/, inventory/, menus/, etc.)
3. Use via `GuiFrameRenderer.DrawAssembledPanel()` or direct `Draw2dBitmapFile()`

## Design Guidelines

When creating or modifying GUI elements:

1. **Color Palette**:
   - Golden/Bronze borders: RGB(184, 134, 11)
   - Dark grey backgrounds: RGB(40, 35, 30)
   - Lighter grey (hover): RGB(150, 145, 140)
   - Red (HP): RGB(255, 0, 0)
   - Blue (Mana/Oxygen): RGB(0, 100, 255)
   - Green (Stamina): RGB(0, 255, 0)
   - Cyan (Breath): RGB(0, 200, 255)

2. **Spacing**:
   - Standard spacing: 5-10 pixels
   - Border thickness: 2-3 pixels
   - Button padding: 10 pixels
   - Slot size: 48-50 pixels

3. **Typography**:
   - Title font: 16pt
   - Standard text: 11-12pt
   - Small text: 9-10pt
   - Numbers on slots: 11pt

## Troubleshooting

### Common Issues

**Issue**: GUI elements not appearing
- **Solution**: Check asset paths, ensure PNG files are in correct location

**Issue**: Capsule bars not filling correctly
- **Solution**: Verify progress value is between 0 and 1, check max values

**Issue**: Buttons not responding to clicks
- **Solution**: Check mouse position calculations, verify button bounds

**Issue**: World map not opening
- **Solution**: Check for ESC key conflicts, ensure `worldMapOpen` state is set

### Performance Optimization

- Use sprite batching where possible
- Cache texture references
- Minimize state changes
- Clip rendering to visible area only

## Code Examples

### Example: Adding a Custom Status Bar

```csharp
// In your GUI module
void DrawCustomBar(Game game, int x, int y, int width, int height)
{
    float progress = game.one * currentValue / maxValue;
    int color = Game.ColorFromArgb(255, 255, 128, 0); // Orange
    
    GuiFrameRenderer.DrawCapsuleBar(game, x, y, width, height,
        progress, "custom", color);
    
    // Add text overlay
    FontCi font = new FontCi();
    font.size = 11;
    string text = game.platform.StringFormat2("{0}/{1}", 
        game.platform.IntToString(currentValue),
        game.platform.IntToString(maxValue));
    game.Draw2dText(text, font, x + width/2 - 20, y + height/2 - 6, null, false);
}
```

### Example: Creating a Custom Panel

```csharp
void DrawCustomPanel(Game game)
{
    int panelX = game.Width() / 2 - 200;
    int panelY = game.Height() / 2 - 150;
    int panelWidth = 400;
    int panelHeight = 300;
    
    // Draw panel background
    GuiFrameRenderer.DrawAssembledPanel(game, panelX, panelY, 
        panelWidth, panelHeight, "menu_medium");
    
    // Add title
    FontCi titleFont = new FontCi();
    titleFont.size = 16;
    game.Draw2dText("Custom Panel", titleFont, 
        panelX + 20, panelY + 15, null, false);
    
    // Add close button
    GuiFrameRenderer.DrawIconButton(game, 
        panelX + panelWidth - 40, panelY + 10, 30, "close", false);
}
```

## Testing Checklist

- [ ] Capsule bars display correctly at all resolutions
- [ ] Breath bar fades in when underwater, fades out above water
- [ ] Minimap zoom buttons work (5 zoom levels)
- [ ] World map opens, pans, and closes properly
- [ ] Hotbar selects slots with number keys
- [ ] Hotbar shows hover effects on mouse over
- [ ] Inventory displays items in new panel style
- [ ] Inventory scroll buttons function correctly
- [ ] Escape menu tabs show lighter grey on hover
- [ ] All GUI elements maintain golden steampunk theme

## Credits

- GUI Design: Based on `Assembled_Gui_Pieces.PNG` specification
- Implementation: Modular GUI system with upgradeable components
- Asset Extraction: Python scripts using ImageMagick
- Theme System: UIThemeManager integration

## Version History

- **v1.0** (2025-12-14): Initial implementation
  - Capsule bars with fade animations
  - Enhanced minimap with zoom and world map
  - Hotbar with number key bindings
  - Updated inventory and escape menu
  - 27 extracted GUI assets
  - GuiFrameRenderer extensions
