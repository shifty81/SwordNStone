# UI Theme System

## Overview

The Sword&Stone UI theme system provides a flexible, modular approach to customizing all user interface elements including HUD components, menus, inventory screens, and more. Themes are organized in a structured directory format that makes it easy to create, modify, and switch between different UI styles.

## Quick Start

### Using a Theme

The default theme is automatically loaded when the game starts. All UI elements will use assets from:
```
data/themes/default/
```

### Creating a New Theme

1. Copy the `data/themes/default/` directory
2. Rename it to your theme name (e.g., `data/themes/mytheme/`)
3. Edit `theme.txt` to update metadata
4. Replace PNG files with your custom designs
5. Update sprite map coordinates if needed

See [THEME_CREATION_GUIDE.md](THEME_CREATION_GUIDE.md) for detailed instructions.

## Theme Structure

Each theme follows this directory structure:

```
data/themes/[theme_name]/
├── theme.txt              # Configuration file with metadata and sprite maps
├── hud/                   # HUD elements (health bars, action bars, etc.)
│   └── hud_pieces.png     # Sprite sheet for HUD components
├── ui/                    # General UI elements
│   ├── ui_split.png       # Main UI sprite sheet
│   ├── panel_dark.png     # Background panels
│   └── panel_medium.png
├── speech/                # Speech bubbles and chat elements
│   ├── speech_bubble_small_spritesheet.png
│   └── speech_bubble_large_spritesheet.png
├── menu/                  # Menu backgrounds and elements
├── inventory/             # Inventory-specific elements
│   ├── slot_normal.png
│   ├── slot_active.png
│   └── slot_highlight.png
├── buttons/               # Button states
│   ├── button_normal.png
│   ├── button_hover.png
│   ├── button_pressed.png
│   ├── button_long_normal.png
│   ├── button_long_hover.png
│   └── button_long_pressed.png
├── frames/                # Window frames and borders
│   ├── frame_small.png
│   ├── frame_ornate.png
│   ├── frame_circular.png
│   ├── frame_large_1.png
│   ├── frame_large_2.png
│   └── frame_large_3.png
└── bars/                  # Progress bars and status bars
    ├── bar_full_red.png
    ├── bar_full_blue.png
    ├── bar_left.png
    └── bar_right.png
```

## Key Files

### theme.txt

The configuration file that defines:
- Theme metadata (name, author, version, description)
- Asset paths for each component type
- Sprite sheet coordinate maps
- Color scheme definitions

Example:
```ini
[Theme]
name=Default
author=SwordNStone Team
version=1.0
description=Standard UI theme

[Paths]
base_path=data/themes/default/
hud_path=hud/
ui_path=ui/
...

[Colors]
primary=160,100,40
accent=255,215,0
...
```

### Sprite Sheets

#### hud_pieces.png (152x124 pixels)
Contains HUD elements like:
- Frame corners and edges (9-slice scalable)
- Circular buttons and status indicators
- Decorative elements
- Action bar segments

#### ui_split.png (606x414 pixels)
Contains general UI elements:
- Inventory panels and grids
- Player/character panels
- Scrollbars and controls
- Portrait frames
- Status icons

#### speech_bubble_*.png
Animated speech bubbles for chat and emotes:
- **Small**: 48x24 pixels (2 frames)
- **Large**: 128x32 pixels (4 frames)

## Design Guidelines

### Visual Consistency

1. **Color Palette**: Use a consistent color scheme throughout your theme
2. **Style**: Maintain consistent visual style (fantasy, sci-fi, minimal, etc.)
3. **Scale**: Design elements at consistent scale for proper alignment

### Sprite Sheets

- Use PNG format with alpha channel for transparency
- Add 1-2 pixel padding between sprites to prevent bleeding
- Power-of-2 dimensions when possible (128, 256, 512)

### 9-Slice Frames

For scalable frames, design with 9 pieces:
- 4 corners (fixed size)
- 4 edges (tile/stretch in one direction)
- 1 center (scales in both directions)

### Button States

Always provide three states:
1. **Normal**: Default appearance
2. **Hover**: Visual feedback when mouse is over
3. **Pressed**: Visual feedback when clicked/active

## Default Theme

The default theme features:
- **Color Scheme**: Bronze/orange with golden accents
- **Style**: Medieval fantasy
- **Aesthetic**: Ornate frames, metallic textures, warm colors

### Sprite Sheet Sources

The default theme uses standardized sprite sheets:
- `hud_pieces.png`: Core HUD components
- `ui_split.png`: Comprehensive UI element library
- Speech bubbles: Animated chat/emote indicators

## Technical Integration

### Code Integration

The theme system is integrated through:

1. **UIThemeManager** (`SwordAndStoneLib/Client/Mods/UIThemeManager.ci.cs`)
   - Loads theme configurations
   - Provides asset path lookup
   - Manages theme colors

2. **GuiFrameRenderer** (`SwordAndStoneLib/Client/Mods/GuiFrameRenderer.ci.cs`)
   - Renders UI elements using theme assets
   - Provides standardized drawing methods

3. **SpeechBubbleRenderer** (in UIThemeManager.ci.cs)
   - Renders animated speech bubbles
   - Supports player emotes and chat

### Using Themes in Code

```csharp
// Get theme manager
UIThemeManager theme = game.GetUIThemeManager();

// Use theme assets
string buttonPath = theme.GetButtonPath(state);
game.Draw2dBitmapFile(buttonPath, x, y, width, height);

// Use theme colors
int borderColor = theme.GetPrimaryColor();

// Use standardized rendering
GuiFrameRenderer.DrawFrame(game, x, y, width, height, frameType);
GuiFrameRenderer.DrawProgressBar(game, x, y, width, height, progress, barType);
GuiFrameRenderer.DrawSlot(game, x, y, size, highlighted);
```

## Backward Compatibility

The theme system maintains backward compatibility:
- Legacy paths (`local/gui/golden/`) still work
- Fallback to default assets if theme files are missing
- Graceful degradation if theme manager is unavailable

## Future Enhancements

Planned features:
- Runtime theme switching
- Theme preview in options menu
- Community theme repository
- Theme validation tool
- Advanced sprite sheet features (texture atlases)

## Contributing

To contribute a theme:
1. Create your theme following the structure
2. Test it thoroughly in-game
3. Include screenshots
4. Document any special features
5. Submit via GitHub pull request or community forum

## Resources

- **Theme Creation Guide**: [THEME_CREATION_GUIDE.md](THEME_CREATION_GUIDE.md)
- **Character Textures**: [../public/CHARACTER_TEXTURES_GUIDE.md](../public/CHARACTER_TEXTURES_GUIDE.md)
- **Example Themes**: Check community forums for user-created themes

## License

Themes inherit the game's public domain license unless otherwise specified. Include a LICENSE.txt file in your theme directory for different terms.

---

**Questions or Issues?**

- Check the Theme Creation Guide for detailed instructions
- Visit the community forums for help
- Report bugs on GitHub: https://github.com/shifty81/SwordNStone
