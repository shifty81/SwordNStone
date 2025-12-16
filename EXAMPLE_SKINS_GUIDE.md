# Example Skins Guide

This guide describes the example character skins included with the Pixel Art Skin System.

## Overview

The example skins demonstrate the capabilities of the pixel art customization system and provide starting points for creating your own custom character skins.

## Example Skin Files

All example skins are located in the `data/public/` directory and follow the standard 64x32 pixel Minecraft-style character texture format.

### Pre-made Character Skins

1. **example_male_knight.png**
   - **Gender:** Male
   - **Hair:** Short, dark brown
   - **Outfit:** Knight armor (silver/gray with dark accents)
   - **Style:** Medieval warrior
   - **Features:** Blue eyes, armored chest plate

2. **example_female_mage.png**
   - **Gender:** Female
   - **Hair:** Long, auburn/red
   - **Outfit:** Mage robe (purple/blue with light trim)
   - **Style:** Magic caster
   - **Features:** Green eyes, flowing robes

3. **example_male_casual.png**
   - **Gender:** Male
   - **Hair:** Short, black
   - **Outfit:** Casual t-shirt and jeans (blue shirt, dark pants)
   - **Style:** Modern/contemporary
   - **Features:** Dark eyes, relaxed appearance

4. **example_female_knight.png**
   - **Gender:** Female
   - **Hair:** Ponytail, blonde
   - **Outfit:** Knight armor (silver/gray)
   - **Style:** Female warrior
   - **Features:** Brown eyes, protective armor

5. **example_male_mage.png**
   - **Gender:** Male
   - **Hair:** Long, silver/white
   - **Outfit:** Mage robe (purple/blue)
   - **Style:** Wizard/sorcerer
   - **Features:** Purple eyes, mystical appearance

### Blank Templates

These templates provide a starting point with just the base body and underwear:

1. **template_blank_male.png**
   - Male base model with default shorts
   - Skin tone: Light tan
   - Ready for customization in the pixel art editor

2. **template_blank_female.png**
   - Female base model with default top and shorts
   - Skin tone: Light tan
   - Ready for customization in the pixel art editor

## How to Use Example Skins

### In the Pixel Art Editor

1. Open the Pixel Art Skin Editor from the Character Creator screen
2. Click **"Load Template"** button
3. The system will load one of the templates or example skins
4. Customize it using the drawing tools
5. Save your modified skin

### Loading Example Skins in Game

To use an example skin in your game:

1. Copy the desired example skin file
2. Rename it following the naming convention: `player_[gender]_[hairstyle]_[beard]_[outfit].png`
3. Or use it as a custom skin by loading it in the pixel art editor

Example naming:
```
player_0_1_0_1.png  = Male, Medium hair, No beard, Armor outfit
player_1_2_0_2.png  = Female, Long hair, No beard, Robe outfit
```

## Texture Layout (64x32 pixels)

The skins follow the standard Minecraft character texture layout:

```
┌────────────────────────────────────────────────────────────────┐
│  Head Top  │ Head Sides │ Head Back │ (unused)                 │  Row 0-7
├────────────────────────────────────────────────────────────────┤
│ Head Right │ Head Front │ Head Left │ Head Back │ (Hat Layer)  │  Row 8-15
├────────────────────────────────────────────────────────────────┤
│ Leg Right  │ Body Front │ Arm Right │ Arm Left │ (unused)     │  Row 16-19
├────────────────────────────────────────────────────────────────┤
│ Leg Front  │ Body Front │ Leg Front │ Arm Front│ Arm Front    │  Row 20-31
└────────────────────────────────────────────────────────────────┘
```

### Key Regions:

- **Head:** Pixels (8,8) to (15,15) - front face
- **Body:** Pixels (20,20) to (27,31) - torso
- **Right Arm:** Pixels (44,20) to (47,31)
- **Left Arm:** Pixels (36,20) to (39,31)
- **Right Leg:** Pixels (4,20) to (7,31)
- **Left Leg:** Pixels (20,20) to (23,31)

## Creating Your Own Skins

### Using the Pixel Art Editor:

1. **Start with a Template:**
   - Load a blank template or example skin
   - This ensures proper base underwear

2. **Choose Your Tools:**
   - **Brush:** Draw individual pixels
   - **Eraser:** Remove pixels (makes transparent)
   - **Fill Bucket:** Fill connected areas with color
   - **Color Picker:** Sample colors from the canvas

3. **Work in Layers:**
   - **Base Layer:** Main character body and clothing
   - **Overlay Layer:** Hair, accessories that "stick out"

4. **Customize Colors:**
   - Use RGB sliders to create custom colors
   - Save favorite colors to the palette

5. **Preview in Real-time:**
   - See your changes on the 3D character model
   - Switch between male/female templates

6. **Save Your Creation:**
   - Click "Save Skin" to export as PNG
   - File is saved to your user data folder

### Tips for Good Skins:

- **Contrast:** Use different shades for depth
- **Symmetry:** Keep left and right sides balanced
- **Details:** Small touches make big differences
- **Underwear:** Always include base underwear layer
- **Testing:** Preview your skin in-game to see how it looks

## Skin Design Ideas

### Character Archetypes:
- **Warriors:** Armor, helmets, shields
- **Mages:** Robes, staffs, mystical symbols
- **Rogues:** Dark clothes, hoods, daggers
- **Peasants:** Simple tunics, work clothes
- **Royalty:** Crowns, fancy robes, jewelry

### Color Schemes:
- **Monochrome:** Single color with various shades
- **Complementary:** Opposite colors (blue/orange)
- **Analogous:** Adjacent colors (blue/green/teal)
- **Accent:** Neutral base with one bright color

### Hair Styles:
- Short and spiky
- Long and flowing
- Ponytails and braids
- Bald or shaved
- Colored (fantasy colors allowed!)

## Technical Notes

- **Format:** PNG with RGBA channels
- **Size:** 64x32 pixels (fixed)
- **Transparency:** Supported (alpha channel)
- **Color Depth:** 32-bit RGBA
- **File Size:** Typically 300-500 bytes

## Troubleshooting

**Skin looks wrong in-game:**
- Verify it's exactly 64x32 pixels
- Check that it has RGBA color mode
- Ensure underwear is present on base layer

**Colors look different:**
- Game lighting affects appearance
- Test in different environments
- Use higher contrast for better visibility

**Can't load custom skin:**
- Check file is in correct directory
- Verify PNG format is correct
- Try loading a blank template first

## Resources

- See [CHARACTER_CUSTOMIZATION.md](CHARACTER_CUSTOMIZATION.md) for system overview
- See [data/public/CHARACTER_TEXTURES_GUIDE.md](data/public/CHARACTER_TEXTURES_GUIDE.md) for texture creation
- Reference example skins for proper layout
- Use blank templates as starting points

## Community Sharing

Players are encouraged to:
- Create and share custom skins
- Post screenshots of their creations
- Contribute skin packs to the community
- Modify existing example skins

All example skins are provided as public domain content and can be freely used, modified, and shared.
