# Custom UI Themes Directory

This directory contains user-created custom themes for Sword&Stone.

## What's in This Directory?

Each subdirectory represents a different custom theme:

```
custom/
├── README.md              (This file)
├── cyberpunk/             Example sci-fi theme
├── my_theme/              Your custom themes go here
└── downloaded_theme/      Themes from community
```

## Creating Your Own Theme

### Option 1: Use the Theme Editor (Recommended)

1. Launch Sword&Stone
2. Open the **Theme Editor** from the main menu
3. Create your UI assets (buttons, frames, bars, etc.)
4. Save your theme
5. It will appear in this directory automatically

See [THEME_EDITOR_GUIDE.md](../../../THEME_EDITOR_GUIDE.md) for detailed instructions.

### Option 2: Copy and Modify Existing Theme

1. Copy an existing theme directory (e.g., `cyberpunk/`)
2. Rename it to your theme name
3. Edit the PNG files with your image editor
4. Update `theme.txt` with your metadata
5. Select your theme in-game

See [THEME_CREATION_GUIDE.md](../THEME_CREATION_GUIDE.md) for technical details.

## Installing Downloaded Themes

To install a theme you downloaded from the community:

1. **Extract** the theme package (usually a .zip file)
2. **Copy** the theme folder to this directory (`data/themes/custom/`)
3. **Verify** the theme has a `theme.txt` file
4. **Select** the theme in Sword&Stone's options menu

Example:
```
data/themes/custom/
└── awesome_theme/         <- Downloaded theme goes here
    ├── theme.txt
    ├── buttons/
    ├── frames/
    └── ...
```

## Theme Requirements

Each theme must include:

### Required File
- `theme.txt` - Theme configuration and metadata

### Required Directories (with assets)
- `buttons/` - Button states (normal, hover, pressed)
- `frames/` - Window frames and borders
- `bars/` - Progress bars (health, mana, etc.)
- `inventory/` - Inventory slot graphics

### Optional Directories
- `hud/` - HUD elements sprite sheet
- `ui/` - General UI sprite sheet
- `speech/` - Speech bubble animations
- `menu/` - Menu backgrounds

## Example Theme Structure

```
my_custom_theme/
├── theme.txt                      # Configuration
├── buttons/
│   ├── button_normal.png          # Default button
│   ├── button_hover.png           # Mouse over
│   └── button_pressed.png         # Clicked
├── frames/
│   ├── frame_small.png            # Small windows
│   └── frame_ornate.png           # Large decorated frames
├── bars/
│   ├── bar_full_red.png           # Health bar
│   └── bar_full_blue.png          # Mana bar
└── inventory/
    ├── slot_normal.png            # Item slot
    └── slot_active.png            # Selected slot
```

## Sharing Your Themes

Want to share your theme with the community?

### Steps to Share

1. **Package Your Theme**:
   ```bash
   # On Windows
   right-click theme folder → Send to → Compressed folder
   
   # On Linux/Mac
   zip -r mytheme.zip mytheme/
   ```

2. **Include Screenshots**:
   - Take in-game screenshots showing your theme
   - Name them clearly (e.g., `mytheme_preview1.png`)

3. **Write a Description**:
   Create a simple README.txt:
   ```
   Theme Name: My Awesome Theme
   Author: Your Name
   Description: A brief description of your theme's style
   Version: 1.0
   
   Installation:
   1. Extract to data/themes/custom/
   2. Select in game options
   ```

4. **Upload**:
   - Share on game forums
   - Post on community Discord
   - Create GitHub repository
   - Upload to modding sites

### Sharing Best Practices

✅ **DO:**
- Include all required assets
- Test your theme thoroughly
- Provide clear installation instructions
- Give credit to any resources used
- Use descriptive theme names

❌ **DON'T:**
- Share incomplete themes
- Use copyrighted assets without permission
- Forget to include theme.txt
- Use offensive or inappropriate content

## Theme Discovery

### Finding Community Themes

**Official Sources:**
- Sword&Stone GitHub Discussions
- Official Forums
- Community Discord

**Theme Tags** (search for these):
- `#SwordAndStoneTheme`
- `#SwordNStoneUI`
- `#SwordAndStoneMod`

### Rating and Feedback

When trying community themes:
- Give feedback to creators
- Report bugs or issues
- Suggest improvements
- Share your favorites

## Troubleshooting

### My Theme Doesn't Appear

**Check:**
1. Theme is in `data/themes/custom/[theme_name]/`
2. `theme.txt` exists and is properly formatted
3. Theme name in theme.txt matches directory name
4. Required asset files exist

### Assets Look Wrong

**Check:**
1. PNG files are not corrupted
2. File names match those in theme.txt
3. Transparency is set correctly
4. Image dimensions match expected sizes

### Colors Are Off

**Check:**
1. RGB values in theme.txt are 0-255
2. Color format is correct (R,G,B)
3. Monitor color calibration
4. In-game brightness settings

## Technical Information

### Supported Image Formats
- **Primary**: PNG with alpha channel
- **Alternative**: PNG without alpha (opaque)
- **Not Supported**: JPEG, GIF, BMP

### Recommended Dimensions

**Buttons:**
- Small: 56x32 pixels
- Long: 120x32 pixels

**Frames:**
- Small: 96x64 pixels
- Large: 128x96 pixels
- Ornate: 128x128 pixels

**Progress Bars:**
- Segment: 24x32 pixels
- Full: 192x32 pixels

**Inventory Slots:**
- Standard: 48x48 pixels
- Large: 64x64 pixels

### File Size Guidelines

- Individual assets: < 50 KB
- Sprite sheets: < 500 KB  
- Total theme size: < 5 MB

Keep assets reasonably sized for performance.

## Examples Included

### Cyberpunk Theme

Located in `custom/cyberpunk/`

**Style**: Futuristic sci-fi with blue/cyan accents
**Author**: Theme Editor Example
**Features**:
- Angular, tech-inspired designs
- Neon glow effects
- Dark backgrounds with bright accents

**Use as Template**: Copy and modify to create your own sci-fi themes

## Resources

### Guides and Documentation
- [Theme Editor Guide](../../../THEME_EDITOR_GUIDE.md) - How to use the in-game editor
- [Theme Creation Guide](../THEME_CREATION_GUIDE.md) - Technical reference
- [Pixel Art Guide](../../../QUICK_START_PIXEL_ART_EDITOR.md) - Drawing techniques

### Tools
- **In-Game**: Theme Editor (recommended)
- **External**: Any image editor supporting PNG with alpha
  - GIMP (Free)
  - Paint.NET (Free)
  - Photoshop
  - Aseprite (Pixel art focused)

### Learning Resources
- Default theme (`data/themes/default/`) - Study the original
- Community themes - Learn from others
- Pixel art tutorials online

## Contributing

Want to contribute to the theme system itself?

- Report bugs on GitHub
- Suggest new features
- Improve documentation
- Share your best themes as examples

## License

Custom themes follow the game's license unless otherwise specified in the theme's directory.

Always include proper attribution for:
- Base themes you modified
- Assets from other creators
- Color palettes or inspiration sources

## Support

### Getting Help

**Theme Creation Issues:**
- Check the Theme Editor Guide
- Ask in community forums
- Join the Discord

**Technical Issues:**
- Check GitHub Issues
- Contact developers
- Community support channels

### Reporting Problems

When reporting theme-related bugs:
1. Specify which theme (include theme.txt)
2. Describe the problem
3. Include screenshots if visual issue
4. List steps to reproduce
5. Mention your game version

## Version History

- **v1.0** (December 2025)
  - Custom themes directory structure created
  - Cyberpunk example theme included
  - Theme editor integration

## Credits

- **Theme System**: SwordNStone Development Team
- **Example Themes**: Community Contributors
- **Documentation**: GitHub Copilot

---

**Start Creating!**

Whether you use the in-game Theme Editor or external tools, we can't wait to see what themes you create. Share them with the community and help make Sword&Stone more beautiful and diverse!

**Happy Theming! 🎨**
