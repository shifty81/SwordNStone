# World of Warcraft-Style GUI System

This document describes the WoW-style GUI implementation added to Manic Digger.

## Overview

The WoW-style GUI replaces the standard UI with a more immersive, RPG-style interface inspired by World of Warcraft. It includes:

- **Action Bar** - Bottom-center hotbar with 10 slots
- **Unit Frames** - Player and target frames with health/oxygen bars
- **Minimap** - Top-right minimap showing terrain and player position
- **Title Screen** - "Sword and Stone" animated logo

## Components

### 1. Action Bar (`GuiWoWActionBars.ci.cs`)

**Location:** Bottom center of screen

**Features:**
- 10 action slots mapped to keys 1-9 and 0
- Visual button states (normal, hover, pressed)
- Golden highlight for currently selected slot
- Item icons with stack counts
- Click to select slot
- Mousewheel to cycle through items

**Controls:**
- **Number Keys (1-0):** Select action bar slot
- **Mouse Click:** Click on button to select
- **Mouse Hover:** Buttons highlight on hover

**Customization:**
The action bar can be customized by modifying:
```csharp
int ActionBarStartX() // Horizontal position
int ActionBarStartY() // Vertical position (default: screen height - 120)
int ButtonSize()      // Size of each button (scales with screen)
int ButtonSpacing()   // Space between buttons
```

### 2. Unit Frames (`GuiWoWUnitFrames.ci.cs`)

**Location:** 
- Player Frame: Top-left corner (20, 20)
- Target Frame: Below player frame (20, 160)

**Features:**

**Player Frame:**
- Circular portrait placeholder
- Health bar with gradient (dark green → bright green)
- Oxygen bar (shows when underwater, dark blue → bright blue)
- Player name display
- Current/Max health text
- Ornate golden border

**Target Frame:**
- Shows when looking at a block
- Block icon as portrait
- Block type as name
- Full health bar (future: could show block damage)
- Same ornate border style

**Customization:**
```csharp
internal int playerFrameX = 20;   // Player frame X position
internal int playerFrameY = 20;   // Player frame Y position
internal int targetFrameX = 20;   // Target frame X position
internal int targetFrameY = 160;  // Target frame Y position
internal int frameWidth = 256;    // Frame width
internal int frameHeight = 128;   // Frame height
```

### 3. Minimap (`GuiWoWMinimap.ci.cs`)

**Location:** Top-right corner (width - 200, 20)

**Features:**
- Circular minimap with ornate border
- Real-time terrain rendering (top-down view)
- 32-block viewing radius
- Color-coded terrain:
  - Grass: Green
  - Stone: Gray
  - Dirt: Brown
  - Sand: Tan
  - Water: Blue
  - Lava: Orange-red
  - Wood: Dark brown
  - Leaves: Dark green
- Player position indicator (red dot)
- Player direction indicator (yellow line)
- Coordinate display (X, Y, Z)

**Customization:**
```csharp
internal int minimapSize = 180;  // Minimap diameter in pixels

// To change viewing range:
int viewRange = 32; // blocks in each direction (in DrawMinimap method)
```

### 4. Title Screen: "Sword and Stone" (`MainMenu/Main.ci.cs`)

**Features:**
- Stone background texture (512x256)
- Animated sword descending from above
- Smooth ease-out animation
- "SWORD AND STONE" title text in gold
- Sword sticks into stone slot

**Animation Parameters:**
```csharp
float swordAnimationSpeed = 0.5f;  // Animation speed (higher = faster)
float swordAnimationProgress;       // Current progress (0.0 to 1.0)
```

## Asset Files

All texture assets are located in `/data/local/gui/wow/`:

### Action Bar Assets
- `actionbar_bg.png` - 1024x128 semi-transparent dark background panel
- `button_normal.png` - 64x64 normal button state
- `button_hover.png` - 64x64 hover/highlight state
- `button_pressed.png` - 64x64 pressed/clicked state

### Unit Frame Assets
- `player_frame.png` - 256x128 ornate frame border for player
- `target_frame.png` - 256x128 ornate frame border for target
- `portrait_border.png` - 64x64 circular portrait border

### Minimap Assets
- `minimap_border.png` - 200x200 circular ornate border with cardinal markers
- `minimap_mask.png` - 200x200 circular transparency mask

### Title Screen Assets
- `sword.png` - 128x512 sword sprite pointing downward
- `stone_logo.png` - 512x256 stone texture with sword slot

## Integration

The WoW GUI mods are registered in `/SwordAndStoneLib/Client/Game.ci.cs`:

```csharp
// WoW-style GUI enhancements
AddMod(new ModGuiWoWActionBars());
AddMod(new ModGuiWoWUnitFrames());
AddMod(new ModGuiWoWMinimap());
```

### Disabled Original Components

To prevent overlap, the following original UI elements are disabled:

1. **Material Selector** (in `GuiInventory.ci.cs`):
   - `DrawMaterialSelector()` is commented out
   - WoW action bar replaces this functionality

2. **Health/Oxygen Bars** (in `GuiPlayerStats.ci.cs`):
   - `DrawPlayerHealth()` and `DrawPlayerOxygen()` are commented out
   - WoW unit frames replace these

## Customizing Colors

### Action Bar Button Colors
Edit `GuiWoWActionBars.ci.cs`:
```csharp
// Golden highlight for active slot (line ~189)
Game.ColorFromArgb(255, 255, 215, 0)  // R, G, B = gold color

// Change to blue:
Game.ColorFromArgb(255, 0, 100, 255)
```

### Health Bar Colors
Edit `GuiWoWUnitFrames.ci.cs`:
```csharp
// Player health bar (line ~74)
DrawBar(game, barX, barY, barWidth, barHeight, healthProgress, 
    Game.ColorFromArgb(255, 0, 150, 0),   // Dark green
    Game.ColorFromArgb(255, 0, 255, 0));  // Bright green
```

### Minimap Block Colors
Edit `GuiWoWMinimap.ci.cs` in the `GetBlockMinimapColor()` method:
```csharp
if (blockType == 2) // Grass
{
    return Game.ColorFromArgb(255, 0, 180, 0);  // Change RGB values
}
```

## Creating Custom Textures

To replace the default textures with your own:

1. Create PNG files with the same dimensions
2. Use transparency (RGBA) for borders and circular elements
3. Replace files in `/data/local/gui/wow/`
4. Maintain naming conventions

### Recommended Tools
- **GIMP** - Free image editor
- **Photoshop** - Professional image editor
- **Paint.NET** - Windows-friendly free editor
- **Aseprite** - Pixel art editor

### Design Guidelines
- Use consistent color palette (warm golds/browns for fantasy theme)
- Keep borders readable at different resolutions
- Use semi-transparency for backgrounds (alpha 180-200)
- Maintain visual hierarchy (important elements more prominent)

## Extending the System

### Adding More Action Bars

To add a second action bar above the first:

1. Edit `GuiWoWActionBars.ci.cs`
2. Add second set of button states
3. Duplicate draw loop with different Y position
4. Map to different keys (Shift+1-0, etc.)

### Adding Buffs/Debuffs Display

Create `GuiWoWBuffs.ci.cs`:
```csharp
public class ModGuiWoWBuffs : ClientMod
{
    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        // Draw buff icons below player frame
        DrawBuffs(game);
    }
}
```

### Adding Cast Bar

For showing action progress (mining, crafting):
```csharp
public class ModGuiWoWCastBar : ClientMod
{
    internal float castProgress = 0;
    
    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (castProgress > 0)
        {
            DrawCastBar(game, castProgress);
        }
    }
}
```

## Performance Considerations

- **Minimap rendering** can be CPU-intensive. Reduce `viewRange` for better performance
- **Texture loading** is cached by the game engine
- **Animation** uses delta time for consistent speed across framerates

### Optimization Tips

1. **Minimap:**
   - Reduce viewing radius: `int viewRange = 16;`
   - Update less frequently: Add frame counter, update every N frames
   - Use coarser pixel grid: `int pixelsPerBlock = scaledSize / (viewRange);`

2. **Action Bar:**
   - Button state checks are lightweight
   - Consider disabling hover effects on touch devices

3. **Unit Frames:**
   - Bar rendering is efficient (single texture draws)
   - Portrait rendering minimal (one texture per frame)

## Troubleshooting

### GUI Elements Not Showing
- Check that assets are in `/data/local/gui/wow/`
- Verify mods are registered in `Game.ci.cs`
- Check for console errors

### Action Bar Not Responding to Keys
- Ensure `OnKeyPress` is not blocked by other mods
- Check GuiState (doesn't work in inventory screen)
- Verify key mappings match game settings

### Minimap Shows Black/Empty
- Check player position is valid
- Verify world is loaded (`GuiState.MapLoading` check)
- Increase `viewRange` if terrain is sparse

### Animation Stuttering
- Reduce `swordAnimationSpeed`
- Check frame rate (dt parameter)
- Verify texture is loading correctly

## Future Enhancements

Potential improvements for future versions:

1. **Quest Tracker** - Right side of screen showing active quests
2. **Bag Bar** - Quick access to inventory bags
3. **Social Frame** - Friends list and party/guild info
4. **Talent Tree** - Character progression screen
5. **Achievement Notifications** - Pop-up toasts for accomplishments
6. **Actionable Minimap** - Click to set waypoints
7. **Portrait Animations** - Animated character models in frames
8. **Damage Numbers** - Floating combat text
9. **Combo Points** - Resource tracking for abilities
10. **Pet Frame** - If pets/companions are added

## Credits

- **Original Manic Digger** - Base game architecture
- **WoW GUI Inspiration** - Blizzard Entertainment
- **Implementation** - GitHub Copilot with community feedback

## License

This GUI system follows the same license as Manic Digger. See main LICENSE file.

---

**Version:** 1.0  
**Last Updated:** December 13, 2025  
**Compatible with:** Manic Digger (current development branch)
