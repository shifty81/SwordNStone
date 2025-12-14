# Theme Creation Guide

This guide explains how to create custom UI themes for Sword&Stone.

## Overview

The theme system allows you to customize all UI elements including:
- HUD elements (health bars, action bars, etc.)
- Menus (main menu, escape menu, inventory)
- Frames and borders
- Buttons and interactive elements
- Speech bubbles and chat elements

## Directory Structure

Each theme must follow this structure:

```
data/themes/[theme_name]/
├── theme.txt              # Theme configuration file (required)
├── hud/                   # HUD sprite sheets
├── ui/                    # General UI elements
├── speech/                # Speech bubbles and chat elements
├── menu/                  # Menu backgrounds and elements
├── inventory/             # Inventory UI elements
├── buttons/               # Button states (normal, hover, pressed)
├── frames/                # Window frames and borders
└── bars/                  # Progress bars and status bars
```

## Creating a New Theme

### Step 1: Create Theme Directory

Create a new directory under `data/themes/` with your theme name:

```
data/themes/my_theme/
```

### Step 2: Create theme.txt Configuration

The `theme.txt` file defines your theme metadata and asset mappings. Use the following template:

```ini
[Theme]
name=My Theme Name
author=Your Name
version=1.0
description=A brief description of your theme

[Paths]
base_path=data/themes/my_theme/
hud_path=hud/
ui_path=ui/
speech_path=speech/
menu_path=menu/
inventory_path=inventory/
buttons_path=buttons/
frames_path=frames/
bars_path=bars/

[SpriteSheets]
# Define your sprite sheet files
hud_pieces=hud/hud_pieces.png
ui_split=ui/ui_split.png
speech_bubble_small=speech/speech_bubble_small.png
speech_bubble_large=speech/speech_bubble_large.png

[Colors]
# Theme color scheme (R,G,B)
primary=160,100,40
secondary=100,80,60
accent=255,215,0
text_normal=255,255,255
text_highlight=255,215,0
```

### Step 3: Create Sprite Sheets

#### HUD Sprite Sheet (hud_pieces.png)

The HUD sprite sheet should contain:
- Frame corners and edges (9-slice scaling)
- Circular buttons/icons
- Decorative elements
- Status indicators

**Recommended dimensions:** 152x124 pixels or larger

**Layout example:**
```
[Frame TL] [Frame T] [Frame TR] [Button 1] [Button 2]
[Frame L ] [Frame C] [Frame R ] [Icon 1  ] [Icon 2  ]
[Frame BL] [Frame B] [Frame BR] [Decor 1 ] [Decor 2 ]
...additional rows for more elements...
```

#### UI Split Sprite Sheet (ui_split.png)

The main UI sprite sheet containing:
- Inventory panels and grids
- Player/character panels
- Action bar segments
- Scrollbars and controls
- Decorative borders

**Recommended dimensions:** 606x414 pixels or larger

This sprite sheet is divided into logical sections that can be extracted and used independently.

#### Speech Bubble Sprite Sheets

Two sprite sheets for animated speech bubbles:

**Small speech bubbles:** 48x24 pixels (2 frames)
- Frame 1: 24x24 pixels at (0,0)
- Frame 2: 24x24 pixels at (24,0)

**Large speech bubbles:** 128x32 pixels (4 frames)
- Frame 1: 32x32 pixels at (0,0)
- Frame 2: 32x32 pixels at (32,0)
- Frame 3: 32x32 pixels at (64,0)
- Frame 4: 32x32 pixels at (96,0)

### Step 4: Map Sprite Coordinates

In your `theme.txt` file, map each UI element to its sprite sheet coordinates:

```ini
[HUD.SpriteMap]
# Format: element_name=x,y,width,height
frame_small_tl=0,0,16,16
frame_small_t=16,0,16,16
button_red=96,0,16,16
```

### Step 5: Add Individual Asset Files

For elements that benefit from separate files:

**Buttons:** (buttons/ directory)
- button_normal.png
- button_hover.png
- button_pressed.png
- button_long_normal.png
- button_long_hover.png
- button_long_pressed.png

**Frames:** (frames/ directory)
- frame_small.png
- frame_ornate.png
- frame_circular.png
- frame_large_1.png

**Bars:** (bars/ directory)
- bar_full_red.png (health bars)
- bar_full_blue.png (mana/oxygen bars)
- bar_left.png (end cap)
- bar_right.png (end cap)

**Inventory:** (inventory/ directory)
- slot_normal.png
- slot_active.png
- slot_highlight.png

## Design Guidelines

### Visual Consistency

1. **Color Palette:** Use a consistent color scheme throughout your theme
2. **Style:** Maintain consistent visual style (fantasy, sci-fi, minimal, etc.)
3. **Scale:** Design elements at consistent scale for proper alignment

### Sprite Sheet Optimization

1. **Power of 2:** Use dimensions that are powers of 2 when possible (128, 256, 512)
2. **Padding:** Add 1-2 pixel padding between sprites to prevent bleeding
3. **Format:** Use PNG with alpha channel for transparency

### 9-Slice Frames

For scalable frames, create 9-piece sprites:
```
[TL] [Top    ] [TR]
[L ] [Center ] [R ]
[BL] [Bottom ] [BR]
```

Corners remain fixed size, edges tile/stretch in one direction, center scales in both.

### Button States

Always provide three states for interactive elements:
1. **Normal:** Default appearance
2. **Hover:** Visual feedback when mouse is over
3. **Pressed:** Visual feedback when clicked/active

### Color Scheme

Define your color scheme in the theme.txt file:
- **Primary:** Main UI color
- **Secondary:** Supporting UI color
- **Accent:** Highlights and important elements
- **Text colors:** Normal, highlight, disabled
- **Status colors:** Health (red), mana (blue), stamina (green)

## Testing Your Theme

1. Place your theme directory in `data/themes/`
2. Modify the game configuration to use your theme
3. Launch the game and test all UI elements:
   - Main menu
   - In-game HUD
   - Inventory screen
   - Character panel
   - Chat and speech bubbles
   - Escape menu and options

## Example Themes

### Default Theme

The default theme (included with the game) provides a reference implementation:
- Bronze/orange color scheme
- Medieval fantasy style
- Ornate frames and decorative elements

Located at: `data/themes/default/`

### Creating a Minimal Theme

For a minimal/modern theme:
1. Use simple geometric shapes
2. Flat colors without gradients
3. Clean, thin borders
4. High contrast for readability

### Creating a Sci-Fi Theme

For a science fiction theme:
1. Use blue/cyan color scheme
2. Hexagonal or angular shapes
3. Glowing effects
4. Tech-inspired icons

## Advanced Features

### Dynamic Colors

Some UI elements support dynamic coloring:
- Progress bars can be tinted based on percentage
- Status effects can change color
- Text can use theme colors

### Animation Frames

Speech bubbles and some effects support animation:
- Define multiple frames in sprite sheets
- Specify frame timing in theme.txt
- Use sequential sprite coordinates

### Responsive Scaling

UI elements automatically scale with screen resolution:
- Design at 1x scale (base resolution)
- Test at multiple resolutions
- Ensure text remains readable when scaled

## Troubleshooting

### Sprites Not Appearing
- Check file paths in theme.txt
- Verify sprite coordinates are correct
- Ensure PNG files are not corrupted

### Colors Look Wrong
- Check color values (should be 0-255 per channel)
- Verify theme.txt color format (R,G,B)
- Test with different lighting conditions

### Scaling Issues
- Use power-of-2 dimensions
- Add padding between sprites
- Test at multiple resolutions

## Resources

- **Default theme:** `data/themes/default/` - Reference implementation
- **Sprite sheet templates:** Create templates based on default theme dimensions
- **Color palette tools:** Use tools like Adobe Color or Coolors for palette creation

## Contributing Themes

To share your theme with the community:
1. Package your theme directory
2. Include screenshots
3. Document any special features
4. Submit via the community forums or GitHub

## License

Themes inherit the game's license unless otherwise specified. Include a LICENSE.txt file in your theme directory if using different terms.
