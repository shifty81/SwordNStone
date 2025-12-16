# Quick Start: UI Theme Editor

## What is the Theme Editor?

The UI Theme Editor lets you create custom visual themes for Sword&Stone's interface. Design your own buttons, frames, progress bars, and UI elements with an easy-to-use pixel art editor.

## Quick Access

### Opening the Theme Editor

**Method 1: Via Code (For Developers)**
```csharp
// In your MainMenu screen or button handler:
menu.StartThemeEditor();
```

**Method 2: Direct Navigation**
- The Theme Editor can be integrated into any menu screen
- Add a button that calls `menu.StartThemeEditor()`
- Integration point is in `MainMenu.ci.cs`

## 5-Minute Tutorial

### Create Your First Theme Asset

**Step 1: Select Asset Type**
- Click "Button" to create a button
- Canvas automatically resizes to 56x32 pixels

**Step 2: Choose Colors**
- **Primary Color**: R=20, G=100, B=220 (Blue)
- Use RGB sliders to adjust each channel

**Step 3: Paint Background**
- Select "Fill Bucket" tool
- Click canvas to fill with your color

**Step 4: Add Border**
- Select a lighter color: R=0, G=180, B=255
- Click "Add Border" button
- Creates instant 2-pixel border

**Step 5: Export**
- Click "Export Asset"
- Saves to `data/themes/custom/[theme]/buttons/`

**Done!** You've created your first custom UI element.

## Asset Types

### Buttons (56x32 pixels)
Create three states:
- **Normal**: Default appearance
- **Hover**: Mouse over (brighter)
- **Pressed**: Clicked (darker)

### Frames (96x64 pixels)
Window borders and panels:
- Border in primary color
- Dark background
- Optional decorations

### Progress Bars (192x32 pixels)
Status bars (health, mana):
- Use gradient tools
- Red for health
- Blue for mana/oxygen

### Inventory Slots (48x48 pixels)
Item container graphics:
- Border with background
- Active state highlighted

## Tools

### Drawing Tools
- **Brush**: Paint pixels (size 1-10)
- **Eraser**: Remove pixels
- **Fill Bucket**: Fill connected areas
- **Color Picker**: Sample colors

### Quick Actions
- **Clear**: Erase entire canvas
- **H Gradient**: Horizontal gradient
- **V Gradient**: Vertical gradient
- **Add Border**: Instant border

### Color Presets
- **Primary**: Main UI color
- **Secondary**: Background color
- **Accent**: Highlights

## Example Themes

### Default Theme
Bronze/orange medieval fantasy theme located in:
```
data/themes/default/
```

### Cyberpunk Theme (Example)
Blue/cyan sci-fi theme located in:
```
data/themes/custom/cyberpunk/
```

**To Use as Template:**
1. Copy `cyberpunk/` directory
2. Rename to your theme name
3. Modify assets in Theme Editor
4. Update theme.txt metadata

## Theme Structure

```
data/themes/custom/[your_theme]/
├── theme.txt              # Configuration
├── buttons/               # Button states
│   ├── button_normal.png
│   ├── button_hover.png
│   └── button_pressed.png
├── frames/                # Frames and panels
│   ├── frame_small.png
│   └── frame_ornate.png
├── bars/                  # Progress bars
│   ├── bar_full_red.png
│   └── bar_full_blue.png
└── inventory/             # Inventory slots
    ├── slot_normal.png
    └── slot_active.png
```

## Color Schemes

### Cyberpunk (Blue/Cyan)
```
Primary:   R=20,  G=100, B=220
Secondary: R=10,  G=50,  B=100
Accent:    R=0,   G=220, B=255
```

### Forest (Green/Brown)
```
Primary:   R=60,  G=140, B=60
Secondary: R=40,  G=80,  B=40
Accent:    R=150, G=200, B=100
```

### Fire (Red/Orange)
```
Primary:   R=220, G=80,  B=20
Secondary: R=140, G=40,  B=10
Accent:    R=255, G=180, B=0
```

### Minimal (Gray/White)
```
Primary:   R=200, G=200, B=200
Secondary: R=50,  G=50,  B=60
Accent:    R=255, G=255, B=255
```

## Tips

### Design Tips
✅ **Keep it simple** - Less is more for UI
✅ **Use 3-5 colors** - Consistent palette
✅ **Test at actual size** - Not just zoomed
✅ **High contrast** - Readable text
✅ **All button states** - Normal, hover, pressed

### Color Tips
- **Borders**: Use primary color
- **Backgrounds**: Use secondary (darker)
- **Highlights**: Use accent color
- **Hover states**: 20% brighter
- **Pressed states**: 20% darker

### Performance Tips
- Standard sizes (don't make huge)
- Solid colors over complex gradients
- Optimize PNG files

## Sharing Themes

### Export Your Theme
1. Complete all required assets
2. Click "Save Theme"
3. Theme saved to `data/themes/custom/[name]/`

### Package for Sharing
```bash
# Zip your theme directory
zip -r mytheme.zip data/themes/custom/mytheme/
```

### Include in Package
- All PNG assets
- theme.txt configuration
- README with description
- Screenshots showing theme

### Where to Share
- GitHub repositories
- Game forums
- Community Discord
- Modding sites

## Troubleshooting

### Theme Not Loading
**Check:**
- `theme.txt` exists in theme directory
- All required PNG files present
- File names match theme.txt
- Directory name matches theme name

### Colors Look Wrong
**Check:**
- RGB values are 0-255
- Format is R,G,B (no spaces after commas)
- Monitor color calibration
- In-game brightness settings

### Assets Not Appearing
**Check:**
- Files are PNG format
- Correct directory structure
- File names exact match
- Files not corrupted

## Integration

### Adding Theme Editor Button

**In Any Menu Screen:**
```csharp
// Create button widget
MenuWidget themeEditorButton = CreateButton("Theme Editor");

// In OnButton handler:
if (widget == themeEditorButton)
{
    menu.StartThemeEditor();
}
```

**Suggested Locations:**
- Main Menu (as extra option)
- Options/Settings screen
- Character Creator (next to Skin Editor)

## Resources

### Documentation
- **Full Guide**: [THEME_EDITOR_GUIDE.md](THEME_EDITOR_GUIDE.md)
- **Technical Reference**: [data/themes/THEME_CREATION_GUIDE.md](data/themes/THEME_CREATION_GUIDE.md)
- **Custom Themes README**: [data/themes/custom/README.md](data/themes/custom/README.md)

### Example Content
- **Default Theme**: `data/themes/default/`
- **Cyberpunk Example**: `data/themes/custom/cyberpunk/`

### Related Tools
- **Pixel Art Editor**: Character skin customization
- **Color Picker**: Reused from pixel art system
- **Drawing Tools**: Same tools as skin editor

## Development Notes

### Code Files
```
SwordAndStoneLib/Client/
├── Misc/
│   ├── ThemeCanvas.ci.cs      # Canvas for UI assets
│   ├── ColorPicker.ci.cs      # Color selection
│   └── PixelArtTools.ci.cs    # Drawing tools
├── MainMenu/
│   └── ThemeEditor.ci.cs      # Main editor screen
├── Mods/
│   └── UIThemeManager.ci.cs   # Theme loading/switching
└── MainMenu.ci.cs             # Integration point
```

### Key Classes
- **ThemeCanvas**: Drawing canvas for UI elements
- **ScreenThemeEditor**: Main editor UI
- **UIThemeManager**: Theme management and loading
- **ColorPicker**: RGB color selection (shared)
- **PixelArtTools**: Drawing tools (shared)

### Extending the System
- Add new asset types in ThemeCanvas
- Extend UIThemeManager for new features
- Add preset templates
- Implement undo/redo
- Add animation preview

## FAQ

**Q: Can I edit the default theme?**
A: Better to copy it and create a custom version.

**Q: How do I switch themes?**
A: Use `UIThemeManager.SwitchTheme(game, "theme_name")`

**Q: Can I share themes?**
A: Yes! Package and share on community platforms.

**Q: What image format?**
A: PNG with alpha transparency.

**Q: Max theme size?**
A: Keep under 5 MB for performance.

**Q: How many themes?**
A: Unlimited. Install as many as you want.

## Next Steps

1. **Read Full Guide**: [THEME_EDITOR_GUIDE.md](THEME_EDITOR_GUIDE.md)
2. **Study Examples**: Check default and cyberpunk themes
3. **Practice**: Create a simple button first
4. **Expand**: Build complete theme
5. **Share**: Contribute to community

---

**Start Creating Beautiful Themes! 🎨**

The Theme Editor makes UI customization accessible to everyone. Whether you're a designer or just want to personalize your game, the tools are easy to use and powerful enough for professional results.

**Have Fun and Share Your Creations!**
