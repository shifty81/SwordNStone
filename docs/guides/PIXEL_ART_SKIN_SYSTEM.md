# Pixel Art Skin System

## Overview

The Pixel Art Skin System is an integrated in-game editor that allows players to create custom character skins using pixel art techniques. This system provides a complete workflow from creation to in-game display.

## Features

### 🎨 Complete Drawing Tools
- **Brush Tool** - Draw individual pixels with adjustable size (1-10 pixels)
- **Eraser Tool** - Remove pixels (makes them transparent)
- **Fill Bucket** - Flood fill connected areas with a single color
- **Color Picker** - Sample colors directly from the canvas

### 🌈 Advanced Color System
- **RGB Color Picker** - Full control over Red, Green, and Blue channels (0-255)
- **Color Palette** - Save up to 16 favorite colors for quick access
- **Live Preview** - See color changes in real-time

### 📐 Layer Support
- **Base Layer** - Main character body and clothing
- **Overlay Layer** - Hair, accessories, and elements that "stick out"
- **Layer Compositing** - Automatic blending of layers for final texture

### 👤 Character Templates
- **Male Base** - Default male character with shorts
- **Female Base** - Default female character with top and shorts
- **Pre-made Examples** - 5 example skins to use as starting points
- **Blank Templates** - Clean slates with just underwear for full customization

### 🖼️ Real-time 3D Preview
- **Live Updates** - See changes instantly on the 3D character model
- **Gender Toggle** - Switch between male and female templates
- **Full Model View** - Preview complete character appearance

## Getting Started

### Accessing the Pixel Art Editor

1. Launch the game
2. Go to **Main Menu → Singleplayer**
3. Click **"Character..."** to open the Character Creator
4. Click **"Skin Editor"** button (bottom-right)

### Editor Interface Layout

```
┌─────────────────────────────────────────────────────────────┐
│                    Pixel Art Skin Editor                     │
├───────────────┬──────────────────────────┬──────────────────┤
│               │                          │                  │
│  Tools Panel  │    Drawing Canvas        │  3D Preview      │
│               │    (64x32 pixels)        │                  │
│  - Brush      │                          │  [Character      │
│  - Eraser     │    [Interactive Grid]    │   Model]         │
│  - Fill       │                          │                  │
│  - Picker     │                          │  [Male/Female]   │
│               │                          │                  │
│  Brush Size   │                          │                  │
│  [- 1 +]      │                          │                  │
│               │                          │                  │
│  Layer        │                          │                  │
│  [Base][Over] │                          │                  │
│               │                          │                  │
│  Color Picker │                          │                  │
│  R: [- 0 +]   │                          │                  │
│  G: [- 0 +]   │                          │                  │
│  B: [- 0 +]   │                          │                  │
│               │                          │                  │
│  [Clear]      │                          │                  │
│  [Load Tpl]   │                          │                  │
│               │                          │                  │
├───────────────┴──────────────────────────┴──────────────────┤
│  [Back]                                        [Save Skin]   │
└─────────────────────────────────────────────────────────────┘
```

## Using the Tools

### Brush Tool
1. Click **"Brush"** button in the tools panel
2. Adjust brush size using **-** and **+** buttons (1-10 pixels)
3. Select your color using the RGB sliders
4. Click and drag on the canvas to paint

**Tips:**
- Use size 1 for fine details
- Use size 3-5 for filling larger areas
- Hold and drag for continuous painting

### Eraser Tool
1. Click **"Eraser"** button
2. Adjust eraser size with **-** and **+** buttons
3. Click and drag to erase pixels (makes them transparent)

**Tips:**
- Use small eraser for precise corrections
- Eraser respects current layer selection

### Fill Bucket Tool
1. Click **"Fill"** button
2. Select your fill color
3. Click on any pixel to flood-fill connected pixels of the same color

**Tips:**
- Great for coloring large uniform areas
- Only fills pixels of the exact same color
- Works within the current layer

### Color Picker Tool
1. Click **"Picker"** button
2. Click on any pixel on the canvas
3. That pixel's color becomes your selected color

**Tips:**
- Perfect for matching existing colors
- Works on both layers
- Useful for color consistency

## Working with Colors

### RGB Color Selection
- **Red (R):** Controls red channel (0-255)
- **Green (G):** Controls green channel (0-255)
- **Blue (B):** Controls blue channel (0-255)

**Color Mixing Examples:**
- Black: R=0, G=0, B=0
- White: R=255, G=255, B=255
- Red: R=255, G=0, B=0
- Skin tone: R=220, G=180, B=140
- Blue jeans: R=40, G=60, B=100

### Common Color Combinations
```
Skin Tones:
- Light: R=230, G=190, B=150
- Medium: R=200, G=160, B=120
- Dark: R=140, G=100, B=70

Hair Colors:
- Black: R=20, G=20, B=20
- Brown: R=80, G=50, B=20
- Blonde: R=200, G=180, B=100
- Red: R=140, G=60, B=30

Clothing:
- Blue shirt: R=80, G=140, B=180
- Red shirt: R=180, G=40, B=40
- Green shirt: R=60, G=140, B=60
- Gray armor: R=180, G=180, B=200
```

## Layer System

### Base Layer
- Contains the character's body and main clothing
- Default underwear is here
- Primary layer for most customization

### Overlay Layer
- For elements that "stick out" (hair, accessories)
- Allows some depth effect
- Blended on top of base layer

### Switching Layers
1. Click **"Base Layer"** for body and clothes
2. Click **"Overlay"** for hair and accessories
3. Each layer maintains separate pixel data

## Character Templates

### Loading Templates

Click **"Load Template"** to load a default template:
- Resets canvas to base player texture
- Preserves default underwear
- Good starting point for customization

### Gender Templates

Use **"Male"** and **"Female"** buttons in the preview panel:
- Switches between male and female base models
- Preview updates to show selected gender
- Useful for creating gender-specific skins

### Example Skins (in data/public/)

Pre-made examples you can reference:
- `example_male_knight.png` - Armored warrior
- `example_female_mage.png` - Magic user with robes
- `example_male_casual.png` - Modern outfit
- `example_female_knight.png` - Female warrior
- `example_male_mage.png` - Wizard with long hair

### Blank Templates
- `template_blank_male.png` - Clean male base
- `template_blank_female.png` - Clean female base

## Saving Your Work

### Save Skin
1. Click **"Save Skin"** button (bottom-right)
2. Your skin is exported as a PNG file
3. File is saved to your user data folder
4. Automatically includes both layers composited together

### File Format
- **Size:** 64x32 pixels
- **Format:** PNG with RGBA
- **Naming:** `custom_skin_[timestamp].png`
- **Location:** User data folder (game-specific)

## Best Practices

### Starting a New Skin
1. Click **"Load Template"** for a clean start
2. Choose your gender template
3. Select **Base Layer**
4. Start with the body/outfit
5. Switch to **Overlay Layer** for hair

### Creating Details
1. Use **size 1 brush** for eyes, mouth, buttons
2. Use **size 3-5 brush** for clothing areas
3. Use **Fill Bucket** for large uniform areas
4. Use **Color Picker** to match existing colors

### Adding Shading
1. Start with a base color
2. Create darker shade (reduce R, G, B by 20-40)
3. Create lighter highlight (increase R, G, B by 20-40)
4. Use brush to add shadows and highlights

### Design Tips
- **Contrast:** Use light and dark shades for depth
- **Symmetry:** Keep left and right sides balanced
- **Simplicity:** Pixel art works best with simple, clear shapes
- **Reference:** Look at example skins for inspiration
- **Preview:** Check 3D preview frequently

## Texture Layout Reference

The 64x32 pixel canvas maps to a 3D character model:

```
Pixel Regions (approximate):
- Head (front face): (8,8) to (15,15)
- Body (torso): (20,20) to (27,31)
- Right Arm: (44,20) to (47,31)
- Left Arm: (36,20) to (39,31)
- Right Leg: (4,20) to (7,31)
- Left Leg: (20,20) to (23,31)
```

For detailed UV mapping, see: [EXAMPLE_SKINS_GUIDE.md](EXAMPLE_SKINS_GUIDE.md)

## Keyboard Shortcuts

Currently, the editor uses button-based navigation:
- **Mouse:** Click and drag to draw
- **Buttons:** Use UI buttons for tools and colors

## Troubleshooting

### Canvas isn't updating
- Make sure you're using Brush, Eraser, or Fill tool
- Check that you're clicking within the canvas area
- Verify you've selected a visible color (not transparent)

### Colors look wrong
- Check RGB values are in 0-255 range
- Ensure you're not using pure transparent (R=G=B=255, A=0)
- Preview in 3D to see actual appearance

### Can't see my changes
- Switch between Base and Overlay layers to check both
- Zoom mentally (remember it's 64x32, very small!)
- Check the 3D preview for final result

### Fill Bucket fills too much/too little
- It only fills connected pixels of exact same color
- Use Brush for more control
- Clear and start over if needed

## Advanced Techniques

### Creating Hair
1. Switch to **Overlay Layer**
2. Choose hair color
3. Paint hair on top and sides of head region
4. Add highlights with lighter shade
5. Add shadows with darker shade

### Making Armor
1. Base Layer - paint base armor color
2. Use darker shade for edges and joints
3. Use lighter shade for metallic highlights
4. Add details like buckles and straps

### Adding Accessories
1. Use Overlay Layer for items that stick out
2. Hats, glasses, jewelry go here
3. Keep base layer for worn items (shirts, pants)

### Creating Patterns
1. Use size 1 brush for precision
2. Plan your pattern on paper first
3. Use Fill Bucket for background
4. Add pattern with contrasting color
5. Use Color Picker to maintain consistency

## Integration with Game

### Using Custom Skins In-Game
1. Create and save your skin in the editor
2. Custom skins are stored in user data folder
3. Game automatically loads custom skins when available
4. Fall back to default textures if custom skin not found

### Sharing Skins
- Export your PNG file from user data folder
- Share with other players
- Others place it in their user data folder
- Credit the original creator!

## Technical Information

### Canvas Data Structure
- **Resolution:** 64x32 pixels (fixed)
- **Color Format:** ARGB (32-bit)
- **Layers:** 2 (Base + Overlay)
- **Blend Mode:** Alpha blend for overlay

### File Export
- **Format:** PNG with transparency
- **Compression:** Standard PNG compression
- **Alpha:** Fully supported
- **Size:** ~300-500 bytes per skin

### Performance
- Real-time canvas updates
- Efficient texture creation
- Minimal memory footprint
- Fast file I/O operations

## Future Enhancements

Potential features for future versions:
- Undo/Redo functionality
- More brush shapes (circle, square)
- Copy/Paste regions
- Symmetry mode (auto-mirror left/right)
- Animation preview
- Skin library management
- Online skin sharing
- Import from image files

## Resources

### Related Documentation
- [CHARACTER_CUSTOMIZATION.md](CHARACTER_CUSTOMIZATION.md) - Character system overview
- [EXAMPLE_SKINS_GUIDE.md](EXAMPLE_SKINS_GUIDE.md) - Example skins and templates
- [data/public/CHARACTER_TEXTURES_GUIDE.md](data/public/CHARACTER_TEXTURES_GUIDE.md) - Texture creation guide

### Example Files
- See `data/public/example_*.png` for pre-made skins
- See `data/public/template_*.png` for blank templates
- See `data/public/mineplayer.png` for default skin

### Community
- Share your creations with other players
- Post screenshots of custom skins
- Contribute to the skin library
- Help improve the editor

## Credits

The Pixel Art Skin System is part of Sword&Stone, an open-source voxel building game.

## License

Released into the public domain under the Unlicense. Use freely!
