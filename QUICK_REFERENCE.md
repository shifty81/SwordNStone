# WoW GUI Quick Reference

## At a Glance

### Controls
| Key | Action |
|-----|--------|
| **1-9, 0** | Select action bar slot |
| **Click** | Select action bar button |
| **Mouse Hover** | Highlight buttons |

### GUI Layout

```
┌────────────────────────────────────────────────────────────┐
│ [Player Frame]                          [Minimap]          │
│  Portrait │ Health: ████████░░                             │
│           │ Name: Player                                   │
│                                                             │
│ [Target Frame]                                             │
│  Portrait │ Health: ██████████                             │
│           │ Name: Block                                    │
│                                                             │
│                      ... GAME VIEW ...                     │
│                                                             │
│                                                             │
│                                                             │
│          [═══════ Action Bar ═══════]                      │
│          [1][2][3][4][5][6][7][8][9][0]                   │
└────────────────────────────────────────────────────────────┘
```

## Component Locations

| Component | Position | Size |
|-----------|----------|------|
| **Player Frame** | Top-left (20, 20) | 256x128 |
| **Target Frame** | Below player (20, 160) | 256x128 |
| **Minimap** | Top-right corner | 180x180 |
| **Action Bar** | Bottom center | 1024x128 |

## Color Codes

### Health Bars
- **Player:** Green gradient (0, 150, 0) → (0, 255, 0)
- **Target:** Red gradient (150, 0, 0) → (255, 0, 0)
- **Oxygen:** Blue gradient (0, 100, 150) → (0, 150, 255)

### Minimap Terrain
- **Grass:** Green (0, 180, 0)
- **Stone:** Gray (128, 128, 128)
- **Dirt:** Brown (139, 90, 43)
- **Water:** Blue (0, 0, 255)
- **Lava:** Orange-red (255, 100, 0)
- **Wood:** Dark brown (139, 90, 0)
- **Leaves:** Dark green (0, 100, 0)
- **Sand:** Tan (237, 201, 175)

### Action Bar
- **Active Slot:** Gold (255, 215, 0)
- **Normal Button:** Dark gray (40, 40, 40)
- **Hover Button:** Blue-gray (60, 60, 80)
- **Pressed Button:** Darker gray (30, 30, 30)

## Quick Customization

### Change Action Bar Position
In `GuiWoWActionBars.ci.cs`:
```csharp
int ActionBarStartY() { return game.Height() - 120; } // Change 120 to move up/down
```

### Change Minimap Size
In `GuiWoWMinimap.ci.cs`:
```csharp
internal int minimapSize = 180; // Change to 150, 200, etc.
```

### Change Player Frame Position
In `GuiWoWUnitFrames.ci.cs`:
```csharp
internal int playerFrameX = 20; // Horizontal position
internal int playerFrameY = 20; // Vertical position
```

## Texture Asset Sizes

| Asset | Dimensions | Purpose |
|-------|------------|---------|
| `actionbar_bg.png` | 1024x128 | Action bar background panel |
| `button_normal.png` | 64x64 | Normal button state |
| `button_hover.png` | 64x64 | Hover highlight state |
| `button_pressed.png` | 64x64 | Pressed/clicked state |
| `player_frame.png` | 256x128 | Player unit frame border |
| `target_frame.png` | 256x128 | Target unit frame border |
| `portrait_border.png` | 64x64 | Circular portrait border |
| `minimap_border.png` | 200x200 | Minimap ornate border |
| `minimap_mask.png` | 200x200 | Circular transparency mask |
| `sword.png` | 128x512 | Title screen sword sprite |
| `stone_logo.png` | 512x256 | Title screen stone background |

## Common Issues & Fixes

| Issue | Solution |
|-------|----------|
| Action bar not visible | Check assets in `/data/local/gui/wow/` |
| Keys not working | Verify GuiState (doesn't work in inventory) |
| Minimap empty | Player position invalid or world not loaded |
| Overlapping UI | Old UI disabled in `GuiInventory.ci.cs` |
| Sword not animating | Check texture path `wow/sword.png` |

## Performance Tips

1. **Reduce minimap range:** Change `viewRange = 32` to `16` in `GuiWoWMinimap.ci.cs`
2. **Disable hover effects:** Comment out hover state in `GuiWoWActionBars.ci.cs`
3. **Update less frequently:** Add frame counter to limit updates

## File Structure

```
ManicDiggerLib/Client/Mods/
├── GuiWoWActionBars.ci.cs    ← Action bar code
├── GuiWoWUnitFrames.ci.cs    ← Player/target frames
└── GuiWoWMinimap.ci.cs        ← Minimap rendering

data/local/gui/wow/
├── actionbar_bg.png
├── button_normal.png
├── button_hover.png
├── button_pressed.png
├── player_frame.png
├── target_frame.png
├── portrait_border.png
├── minimap_border.png
├── minimap_mask.png
├── sword.png
└── stone_logo.png
```

## Need More Help?

See **WOW_GUI_README.md** for detailed documentation including:
- Complete feature descriptions
- Customization tutorials
- Extending the system
- Creating custom textures
- Troubleshooting guide

---

**Quick Start:** All features work out-of-the-box. Just build and run the game!
