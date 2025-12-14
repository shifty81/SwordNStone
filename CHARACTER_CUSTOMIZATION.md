# Character Customization System

## Overview

The character customization system allows players to customize their character's appearance before joining a world. This includes selecting gender, hairstyle, beard, and outfit options.

## Features

### 1. Character Creator UI
- **Location**: Main Menu → Singleplayer → Character...
- **Also shown**: When creating/opening a new world
- **Options**:
  - Gender (Male/Female)
  - Hairstyle (5 options)
  - Beard (4 options including none)
  - Outfit (4 options)

### 2. First-Person/Third-Person Consistency
- The character customization is applied to both first-person and third-person views
- Uses the enhanced player model (`playerenhanced.txt`) with improved animations
- Customization is saved to player preferences and persisted across sessions

### 3. Model System
- **Enhanced Model**: `data/public/playerenhanced.txt`
  - Includes advanced animations (walk, idle, chop, sword attack, bow draw, emotes)
  - Better animation system for combat and interactions
- **Texture System**: Character textures are named using the pattern:
  - Format: `player_[gender]_[hairstyle]_[beard]_[outfit].png`
  - Example: `player_0_1_2_0.png` = Male, Medium hair, Long beard, Default outfit
  - Fallback: `mineplayer.png` (default texture)

## Adding New Character Textures

To add new character appearance options:

1. **Create Character Textures**:
   - Format: 64x32 pixel PNG with RGBA channels
   - Layout matches standard Minecraft-style character texture
   - Save to: `data/public/player_[gender]_[hairstyle]_[beard]_[outfit].png`

2. **Texture Layout** (64x32 pixels):
   ```
   [Head Front] [Head Right] [Head Back] [Head Left] [Hat layers]
   [Body Front] [Body Right] [Body Back] [Body Left]
   [Leg Front]  [Leg Right]  [Leg Back]  [Leg Left]
   [Arm Front]  [Arm Right]  [Arm Back]  [Arm Left]
   ```

3. **Gender Variants**:
   - 0 = Male
   - 1 = Female
   - Can have different proportions or styling

4. **Hairstyle Options** (0-4):
   - 0 = Short
   - 1 = Medium
   - 2 = Long
   - 3 = Bald
   - 4 = Ponytail

5. **Beard Options** (0-3):
   - 0 = None
   - 1 = Short
   - 2 = Long
   - 3 = Goatee

6. **Outfit Options** (0-3):
   - 0 = Default
   - 1 = Armor
   - 2 = Robe
   - 3 = Casual

## Implementation Details

### Files Added/Modified

1. **New Files**:
   - `ManicDiggerLib/Client/Misc/CharacterCustomization.ci.cs` - Data structure for customization
   - `ManicDiggerLib/Client/MainMenu/CharacterCreator.ci.cs` - Character creator UI screen
   - `ManicDiggerLib/Client/Mods/ApplyCharacterCustomization.ci.cs` - Applies customization to player

2. **Modified Files**:
   - `ManicDiggerLib/Client/MainMenu.ci.cs` - Added StartCharacterCreator method
   - `ManicDiggerLib/Client/MainMenu/Singleplayer.ci.cs` - Added Character button
   - `ManicDiggerLib/Client/Game.ci.cs` - Registered new mod

### Data Storage

Character customization is stored in player preferences:
- **Key**: `CharacterCustomization`
- **Format**: Comma-separated values: `gender,hairstyle,beard,outfit`
- **Example**: `0,1,2,0` (Male, Medium hair, Long beard, Default outfit)

### How It Works

1. Player opens Character Creator from singleplayer menu
2. Selects appearance options using arrow buttons
3. Clicks Confirm to save preferences
4. When joining world, `ModApplyCharacterCustomization` loads preferences
5. Player entity model and texture are updated based on customization
6. Enhanced model provides better animations for all actions

## Future Enhancements

Potential additions to the system:
- Skin color/tone selection
- Eye color variations
- Accessory options (hats, glasses, etc.)
- Body type/build options
- Custom texture upload support
- Preview of character in 3D while customizing
- Save multiple character presets

## Notes

- The system currently falls back to `mineplayer.png` if custom textures don't exist
- This allows the system to work immediately without requiring all texture variants
- Content creators can add texture variants incrementally
- The enhanced model (`playerenhanced.txt`) is always used for better animations
