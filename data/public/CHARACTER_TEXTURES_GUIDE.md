# Character Texture Creation Guide

## Overview

This guide explains how to create custom character textures for the character customization system.

## Texture Format

- **Size**: 64x32 pixels
- **Format**: PNG with RGBA (32-bit color with transparency)
- **Layout**: Standard Minecraft-style character skin format

## Naming Convention

Character textures use this naming pattern:
```
player_[gender]_[hairstyle]_[beard]_[outfit].png
```

### Parameters

1. **Gender** (0-1):
   - 0 = Male
   - 1 = Female

2. **Hairstyle** (0-4):
   - 0 = Short
   - 1 = Medium
   - 2 = Long
   - 3 = Bald
   - 4 = Ponytail

3. **Beard** (0-3):
   - 0 = None
   - 1 = Short
   - 2 = Long
   - 3 = Goatee

4. **Outfit** (0-3):
   - 0 = Default
   - 1 = Armor
   - 2 = Robe
   - 3 = Casual

### Example Filenames

- `player_0_0_0_0.png` - Male, Short hair, No beard, Default outfit (base male)
- `player_1_2_0_0.png` - Female, Long hair, No beard, Default outfit
- `player_0_1_2_3.png` - Male, Medium hair, Long beard, Casual outfit
- `player_1_4_0_1.png` - Female, Ponytail, No beard, Armor outfit

## Texture Layout (64x32 pixels)

```
 0   8   16  24  32  40  48  56  64
 ┌───┬───┬───┬───┬───┬───┬───┬───┐ 0
 │HF │HR │HB │HL │HAT (overlay)  │ 8
 ├───┴───┴───┴───┼───┬───┬───┬───┤
 │               │BF │BR │BB │BL │16
 │   (unused)    ├───┼───┼───┼───┤
 │               │RF │RR │RB │RL │24
 ├───┬───┬───┬───┼───┼───┼───┼───┤
 │AF │AR │AB │AL │LF │LR │LB │LL │32
 └───┴───┴───┴───┴───┴───┴───┴───┘

Legend:
H = Head (Front, Right, Back, Left)
HAT = Hat overlay (drawn over head)
B = Body/Torso (Front, Right, Back, Left)
R = Right Arm (Front, Right, Back, Left)
L = Left Leg (Front, Right, Back, Left)
A = Left Arm (Front, Right, Back, Left)
```

## Creating Textures

### Method 1: Edit Existing Texture

1. Copy `mineplayer.png` or `player_0_0_0_0.png` as your starting point
2. Open in image editor (GIMP, Photoshop, Paint.NET, etc.)
3. Modify the relevant sections:
   - **Hair**: Modify head top portion (0-32, 0-8)
   - **Beard**: Modify head front/sides (add facial hair details)
   - **Outfit**: Modify body and limb sections
4. Save as new filename following naming convention

### Method 2: Use Skin Editor

Many Minecraft skin editors work with this format:
- NovaSkin (web-based)
- Miners Need Cool Shoes (web-based)
- Skin Edit (desktop)

### Tips for Good Textures

1. **Consistency**: Keep style consistent with game aesthetics
2. **Contrast**: Ensure good visibility at various distances
3. **Seams**: Check that texture edges align properly on 3D model
4. **Testing**: Test in-game to verify appearance

## Color Palette Suggestions

### Skin Tones
- Light: #FFE0BD, #F5D5C2
- Medium: #D4A574, #C68642
- Dark: #8D5524, #6B4423

### Hair Colors
- Blonde: #F0E68C, #DAA520
- Brown: #8B4513, #654321
- Black: #1C1C1C, #000000
- Red: #DC143C, #8B0000
- Gray: #808080, #A9A9A9

### Clothing
- Default: #4169E1, #2E8B57
- Armor: #708090, #C0C0C0
- Robe: #8B008B, #4B0082
- Casual: #8B4513, #228B22

## Required Textures

To have a complete customization system, you would need:
- 2 genders × 5 hairstyles × 4 beards × 4 outfits = 160 total textures

However, you can create them incrementally:
1. Start with base textures (default everything)
2. Add popular combinations
3. Expand based on player feedback

## Fallback System

If a specific texture doesn't exist, the game will:
1. Try to load the specified texture file
2. Fall back to `mineplayer.png` if not found
3. This allows gradual content addition

## Testing Your Textures

1. Save texture file to `data/public/` directory
2. Launch game
3. Go to Singleplayer → Character...
4. Select the combination matching your texture filename
5. Verify appearance looks correct
6. Test in both first-person and third-person views

## Contributing Textures

If you create textures you'd like to share:
1. Ensure they follow the naming convention
2. Test them in-game
3. Submit via pull request to repository
4. Include preview images if possible

## License

All character textures should be compatible with the project's public domain license (Unlicense).
