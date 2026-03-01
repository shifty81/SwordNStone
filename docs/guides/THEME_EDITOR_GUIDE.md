# UI Theme Editor Guide

## Overview

The UI Theme Editor allows you to create custom visual themes for Sword&Stone's interface. Design unique buttons, frames, progress bars, and other UI elements using an intuitive in-game pixel art editor, then share your themes with the community or keep them local.

## Quick Start

### Accessing the Theme Editor

1. Launch Sword&Stone
2. From the Main Menu, select **"Options"** or **"Theme Editor"**
3. The Theme Editor opens with a blank canvas

### Your First Theme

**Create a Simple Button (5 minutes):**

1. **Select Asset Type**: Click "Button" to work on a button asset
2. **Choose Colors**: Use the color picker to select your primary color
   - Example: R=0, G=100, B=220 for a blue theme
3. **Draw Background**: 
   - Select "Fill Bucket" tool
   - Click canvas to fill with your color
4. **Add Border**:
   - Select a lighter color (R=0, G=180, B=255)
   - Click "Add Border" button
5. **Preview**: Check the preview panel to see your button in context
6. **Save**: Click "Export Asset" to save your button

Congratulations! You've created your first custom UI element.

## Understanding Themes

### What is a UI Theme?

A theme is a collection of visual assets that define how the game's interface looks:

- **Buttons**: Normal, hover, and pressed states
- **Frames**: Windows, panels, and containers
- **Progress Bars**: Health, mana, and other status bars
- **Slots**: Inventory and action bar slots
- **Colors**: Consistent color scheme throughout

### Theme Structure

Each theme is stored in its own directory:

```
data/themes/[theme_name]/
├── theme.txt              # Theme configuration
├── buttons/               # Button assets
├── frames/                # Frame assets
├── bars/                  # Progress bar assets
├── inventory/             # Slot assets
└── [other directories]
```

## Theme Editor Interface

### Layout

The Theme Editor is divided into several panels:

```
┌─────────────────────────────────────────────────┐
│ UI Theme Editor                                 │
├──────────┬───────────────────┬──────────────────┤
│          │                   │                  │
│  Tools   │   Canvas          │  Asset Type      │
│          │   (Drawing Area)  │                  │
│          │                   │                  │
├──────────┤                   ├──────────────────┤
│          │                   │                  │
│  Colors  │                   │  Preview         │
│          │                   │                  │
└──────────┴───────────────────┴──────────────────┘
│          Bottom Actions                         │
└─────────────────────────────────────────────────┘
```

### Tools Panel (Left Side)

**Drawing Tools:**
- **Brush**: Paint individual pixels or areas
- **Eraser**: Remove pixels (make transparent)
- **Fill Bucket**: Fill connected areas with color
- **Color Picker**: Sample colors from the canvas

**Brush Size**: Adjust from 1 (single pixel) to 10 (large area)

### Color Panel (Left Side, Below Tools)

**Color Picker:**
- **RGB Sliders**: Adjust Red, Green, Blue (0-255 each)
- **Color Preview**: Large square showing current color
- **Preset Colors**: Quick access to theme colors
  - Primary: Main UI color
  - Secondary: Supporting color
  - Accent: Highlights and important elements

**Color Tips:**
- Start with a base color scheme
- Use lighter shades for highlights
- Use darker shades for shadows
- Maintain consistent saturation

### Canvas (Center)

- **Grid Overlay**: Shows individual pixels
- **Drawing Area**: Click and drag to paint
- **Size**: Automatically adjusts based on asset type
- **Zoom**: Grid scale makes pixels easy to see

### Asset Type Panel (Right Side)

**Asset Types:**
- **Button**: 56x32 or 120x32 pixels
- **Frame**: 96x64 or larger
- **Bar**: Progress bar (192x32 full)
- **Slot**: Inventory slot (48x48)

**Asset States** (for buttons/slots):
- **Normal**: Default appearance
- **Hover**: When mouse is over
- **Pressed**: When clicked/active

### Preview Panel (Right Side, Below Asset Type)

Shows your asset as it will appear in the actual game UI:
- Actual size rendering
- In-context display
- Real-time updates as you draw

### Bottom Actions

- **Clear**: Erase entire canvas
- **Load Default**: Load a template to start from
- **Save Theme**: Save entire theme configuration
- **Export Asset**: Export current canvas as PNG
- **Back**: Return to main menu

## Creating a Theme

### Step 1: Plan Your Theme

**Choose a Style:**
- Fantasy (ornate, medieval)
- Sci-Fi (angular, glowing)
- Minimal (clean, simple)
- Retro (pixel art, 8-bit)

**Choose a Color Scheme:**
- **Primary**: Main UI color (borders, frames)
- **Secondary**: Background color
- **Accent**: Highlights, active states

**Example Color Schemes:**

**Cyberpunk (Blue/Cyan):**
```
Primary:   R=20,  G=100, B=220
Secondary: R=10,  G=50,  B=100
Accent:    R=0,   G=220, B=255
```

**Forest (Green/Brown):**
```
Primary:   R=60,  G=140, B=60
Secondary: R=40,  G=80,  B=40
Accent:    R=150, G=200, B=100
```

**Fire (Red/Orange):**
```
Primary:   R=220, G=80,  B=20
Secondary: R=140, G=40,  B=10
Accent:    R=255, G=180, B=0
```

### Step 2: Create Core Assets

**Essential Assets to Create:**

1. **Button (Normal)** - Start here, it's the foundation
2. **Button (Hover)** - Slightly brighter than normal
3. **Button (Pressed)** - Slightly darker or inverted
4. **Frame (Small)** - For windows and panels
5. **Progress Bar** - For health/mana displays
6. **Inventory Slot (Normal)** - For item slots
7. **Inventory Slot (Active)** - For selected slots

### Step 3: Button Creation (Detailed)

**Creating a Button (Normal State):**

1. **Select Asset Type**: Click "Button"
2. **Create Background**:
   - Load your primary color
   - Use "Fill Bucket" to fill canvas
   
3. **Add Border**:
   - Select a lighter version of primary color
   - Click "Add Border" button (creates 2-3 pixel border)
   - Or manually draw border with Brush tool
   
4. **Add Depth** (Optional):
   - Select "H Gradient" for horizontal gradient
   - This creates a subtle 3D effect
   - Lighter at top, darker at bottom
   
5. **Add Details**:
   - Use Brush tool to add highlights
   - Add corner decorations if desired
   - Keep it simple and readable
   
6. **Export**: Click "Export Asset" → Saves to buttons/button_normal.png

**Creating Hover State:**

1. Same process as Normal
2. Make it **10-20% brighter**
3. Optionally add glow effect
4. Export as button_hover.png

**Creating Pressed State:**

1. Same process as Normal
2. Make it **10-20% darker**
3. Optionally shift content down/right 1-2 pixels
4. Export as button_pressed.png

### Step 4: Frame Creation

**Frames are larger and more complex:**

1. **Select Asset Type**: Click "Frame"
2. **Create Outer Border**:
   - Use primary color
   - 4-6 pixel thick border
   - Click "Add Border" multiple times or draw manually
   
3. **Create Inner Area**:
   - Use secondary color (darker)
   - Fill Bucket on inner area
   
4. **Add Decorative Elements**:
   - Corner decorations
   - Edge patterns
   - Small details
   
5. **Add Depth**:
   - Lighter colors on top/left
   - Darker colors on bottom/right
   - Creates 3D beveled effect

### Step 5: Progress Bar Creation

**Bars show status (health, mana, etc.):**

1. **Select Asset Type**: Click "Bar"
2. **Create Bar Container**:
   - Border in primary color
   - Dark inner area
   
3. **Create Fill Gradient**:
   - Use "H Gradient" for smooth transition
   - Start color: Bright (left side)
   - End color: Dark (right side)
   
4. **Common Bar Colors**:
   - Health: Red (R=220, G=20, B=20)
   - Mana: Blue (R=20, G=100, B=220)
   - Stamina: Green (R=20, G=180, B=20)

### Step 6: Inventory Slot Creation

**Slots hold items in inventory:**

1. **Select Asset Type**: Click "Slot"
2. **Create Border**: 2-3 pixels, primary color
3. **Inner Area**: Dark background
4. **Corner Details**: Small decorative elements
5. **Active State**: Brighter border, possibly glowing

## Advanced Techniques

### Gradient Effects

**Horizontal Gradients:**
- Good for bars (left-to-right fill)
- Creates depth on buttons
- Use "H Gradient" button

**Vertical Gradients:**
- Good for backgrounds
- Creates lighting effect (top lighter, bottom darker)
- Use "V Gradient" button

### Border Styles

**Simple Border:**
- Click "Add Border" once
- 2 pixel uniform border

**Ornate Border:**
- Multiple border layers
- Different colors per layer
- Add corner decorations

**Glowing Border:**
- Use bright accent color
- Apply 1-2 times
- Creates neon effect

### 3D Effect

**Beveled Look:**
1. Fill with base color
2. Top edge: Lighter color (1-2 pixels)
3. Bottom edge: Darker color (1-2 pixels)
4. Left edge: Medium-light
5. Right edge: Medium-dark

**Embossed Look:**
- Opposite of beveled
- Dark on top/left
- Light on bottom/right

### Transparency

**Semi-Transparent Elements:**
- Useful for overlays
- Use RGB values with lower alpha
- Creates glassmorphism effect

**Fully Transparent:**
- Use Eraser tool
- Good for irregular shapes
- Creates cutouts

## Sharing and Local Themes

### Exporting Your Theme

1. **Complete All Assets**: Ensure all required assets are created
2. **Save Theme**: Click "Save Theme" button
3. **Theme Location**: Saved to `data/themes/custom/[your_theme_name]/`

### Theme Package Contents

Your theme directory includes:
```
[theme_name]/
├── theme.txt              # Configuration file
├── buttons/
│   ├── button_normal.png
│   ├── button_hover.png
│   └── button_pressed.png
├── frames/
│   ├── frame_small.png
│   └── frame_ornate.png
├── bars/
│   ├── bar_full_red.png
│   └── bar_full_blue.png
└── inventory/
    ├── slot_normal.png
    └── slot_active.png
```

### Sharing with Community

**To Share Your Theme:**

1. **Zip Your Theme Directory**:
   - Compress `data/themes/custom/[theme_name]/`
   - Name it: `[theme_name]_theme.zip`

2. **Include Screenshots**:
   - In-game screenshots showing your theme
   - Preview of individual assets
   - Name screenshots clearly

3. **Write a README**:
   ```markdown
   # [Theme Name] Theme
   
   ## Description
   Brief description of your theme style
   
   ## Installation
   1. Extract zip to data/themes/custom/
   2. Select theme in Options menu
   
   ## Credits
   Created by [Your Name]
   ```

4. **Upload to Community**:
   - GitHub repository
   - Game forums
   - Community Discord

### Installing Downloaded Themes

**To Install a Community Theme:**

1. **Download Theme Package**: Get .zip file
2. **Extract**: Unzip to `data/themes/custom/`
3. **Verify Structure**: Ensure theme.txt exists in theme directory
4. **Select Theme**: Use Options menu or Theme Editor to switch
5. **Enjoy**: Theme loads automatically

### Local-Only Themes

**Keep Themes Private:**

- Themes in `data/themes/custom/` are local by default
- Only you can see them
- Not automatically shared
- Can be shared manually if desired

## Tips and Best Practices

### Design Tips

✅ **DO:**
- Keep designs consistent across all assets
- Use limited color palette (3-5 main colors)
- Test at actual size (not just zoomed in)
- Consider readability (text must be visible)
- Save work frequently
- Create all button states (normal, hover, pressed)

❌ **DON'T:**
- Use too many colors (looks messy)
- Make borders too thin (hard to see)
- Forget hover/pressed states
- Use very dark colors for borders (invisible)
- Overcomplicate designs
- Skip the preview panel

### Color Selection Tips

**Contrast:**
- High contrast for borders/text
- Medium contrast for depth
- Low contrast for subtle details

**Harmony:**
- Use color wheel for complementary colors
- Stick to one hue family with variations
- Accent color should pop

**Accessibility:**
- Avoid red/green only combinations
- Ensure sufficient brightness difference
- Test with color blindness simulators

### Performance Tips

- Keep asset sizes standard (don't make unnecessarily large)
- Use solid colors where possible (not complex gradients)
- Reuse assets where applicable
- Optimize PNG files

### Testing Your Theme

**Before Sharing:**

1. **Visual Test**: Check all UI elements in-game
2. **Consistency Test**: Ensure style is consistent
3. **Readability Test**: Can you read text on backgrounds?
4. **State Test**: Do hover/pressed states work well?
5. **Color Test**: Do colors work in different lighting?

## Troubleshooting

### Asset Not Appearing

**Problem**: Exported asset doesn't show in game

**Solution**:
1. Check asset is saved to correct directory
2. Verify filename matches theme.txt configuration
3. Ensure PNG format (not other image formats)
4. Check file isn't corrupted

### Colors Look Wrong

**Problem**: Colors appear different in game than in editor

**Solution**:
1. Check RGB values are correct (0-255 range)
2. Verify no transparency where there shouldn't be
3. Test under different game lighting conditions
4. Monitor color calibration

### Theme Won't Load

**Problem**: Game doesn't recognize theme

**Solution**:
1. Verify theme.txt exists in theme directory
2. Check theme.txt syntax (proper format)
3. Ensure all required assets exist
4. Check directory name matches theme name in theme.txt

### Canvas Too Small/Large

**Problem**: Working area is wrong size

**Solution**:
1. Select correct asset type (Button, Frame, Bar, Slot)
2. Asset type determines canvas size automatically
3. Use "Custom" for non-standard sizes

## Examples

### Example 1: Cyberpunk Theme

**Style**: Futuristic, angular, glowing
**Colors**: Blue (#1464DC), Cyan (#00DCFF), Dark Blue (#0A3264)

**Button Design**:
1. Fill with dark blue background
2. Add 2-pixel cyan border
3. Add horizontal gradient (lighter at top)
4. Add small corner accents

**Result**: Sleek, modern sci-fi look

### Example 2: Forest Theme

**Style**: Natural, organic, earthy
**Colors**: Green (#3C8C3C), Brown (#5A3A28), Light Green (#96C864)

**Button Design**:
1. Fill with brown background
2. Add 3-pixel green border
3. Add wood grain texture (manual brush strokes)
4. Add leaf decorations in corners

**Result**: Warm, natural fantasy look

### Example 3: Minimal Theme

**Style**: Clean, simple, modern
**Colors**: Light Gray (#C8C8C8), Dark Gray (#323232), White (#FFFFFF)

**Button Design**:
1. Fill with light gray
2. Add 1-pixel dark border
3. No gradients or decorations
4. Flat, simple appearance

**Result**: Professional, unobtrusive interface

## Resources

### Built-in Templates

The Theme Editor includes templates you can start from:
- **Load Default**: Loads standard button/frame template
- **Primary Color**: Quick access to theme primary color
- **Border Tool**: Automatic border creation
- **Gradient Tools**: One-click gradient generation

### Learning Resources

- **Default Theme**: Study the default theme for reference
- **Cyberpunk Theme**: Example of custom theme included
- **Theme Creation Guide**: Advanced documentation (THEME_CREATION_GUIDE.md)
- **Pixel Art Guide**: Techniques from character skin editor (QUICK_START_PIXEL_ART_EDITOR.md)

### Color Palette Tools

External tools for creating color schemes:
- Adobe Color
- Coolors.co  
- Paletton
- Color Hunt

### Community

- GitHub Repository: Share and discover themes
- Game Forums: Discuss designs and get feedback
- Discord: Real-time help and inspiration

## Keyboard Shortcuts

_(Planned for future update)_

- **B**: Brush tool
- **E**: Eraser tool
- **F**: Fill bucket
- **P**: Color picker
- **[** / **]**: Decrease/Increase brush size
- **Ctrl+Z**: Undo (planned)
- **Ctrl+S**: Save theme
- **Ctrl+E**: Export asset

## FAQ

**Q: Can I edit the default theme?**
A: It's better to create a copy. The default theme may be overwritten on updates.

**Q: How do I switch between themes?**
A: Use the Options menu or Theme Editor to select a different theme.

**Q: Can I use copyrighted images?**
A: No. Only use original artwork or properly licensed assets.

**Q: What if my theme looks bad?**
A: Everyone starts somewhere! Keep practicing, study examples, and iterate.

**Q: Can I sell my themes?**
A: Check the game's license. Community themes are typically free.

**Q: How many themes can I have installed?**
A: No limit. Install as many as you want.

**Q: Can I partially customize themes?**
A: Yes. You can override specific assets while keeping others from another theme.

## Version History

- **v1.0** (December 2025)
  - Initial theme editor release
  - Basic drawing tools
  - Asset type support
  - Export functionality
  
## Credits

- **UI Framework**: Standardized UI System
- **Pixel Art Tools**: Based on Character Skin Editor
- **Default Theme**: SwordNStone Team
- **Editor Design**: GitHub Copilot

---

**Ready to Create?**

Launch the Theme Editor and start designing your perfect UI! Share your creations with the community and help make Sword&Stone more beautiful for everyone.

**Happy Theming! 🎨**
