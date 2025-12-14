# Quick Start: Character Customization

## For Players

### How to Customize Your Character

1. **Launch the game** and go to the Main Menu

2. **Click "Singleplayer"**

3. **Click the "Character..." button** (bottom right corner)

4. **Use the arrow buttons (<  >) to customize**:
   - **Gender**: Male or Female
   - **Hairstyle**: Short, Medium, Long, Bald, or Ponytail
   - **Beard**: None, Short, Long, or Goatee
   - **Outfit**: Default, Armor, Robe, or Casual

5. **Click "Confirm"** to save your choices

6. **Open or create a world** - Your character will appear with your customization!

### Your character appearance will:
- ✅ Be visible in third-person view (press F5 to toggle)
- ✅ Be seen by other players in multiplayer
- ✅ Persist across game sessions (saved in preferences)
- ✅ Use enhanced animations for walking, combat, and emotes

---

## For Content Creators

### How to Create Custom Character Textures

#### Quick Method
1. Copy `data/public/mineplayer.png` as your starting point
2. Edit it in your image editor (must be 64x32 PNG)
3. Save with this naming pattern:
   ```
   player_[gender]_[hairstyle]_[beard]_[outfit].png
   ```
4. Place in `data/public/` folder
5. Test in-game!

#### Example Texture Names
- `player_0_0_0_0.png` - Base male character
- `player_1_2_0_0.png` - Female with long hair
- `player_0_1_2_3.png` - Male, medium hair, long beard, casual outfit

#### Parameter Values
```
Gender:    0=Male, 1=Female
Hairstyle: 0=Short, 1=Medium, 2=Long, 3=Bald, 4=Ponytail
Beard:     0=None, 1=Short, 2=Long, 3=Goatee
Outfit:    0=Default, 1=Armor, 2=Robe, 3=Casual
```

#### Full Guide
See [CHARACTER_TEXTURES_GUIDE.md](data/public/CHARACTER_TEXTURES_GUIDE.md) for detailed instructions including:
- Texture layout diagram
- Color palette suggestions
- Tips for creating good textures
- Testing procedures

---

## For Developers

### Key Files

**Data Structure**:
```csharp
ManicDiggerLib/Client/Misc/CharacterCustomization.ci.cs
```
Handles customization options, serialization, and texture name generation.

**UI Screen**:
```csharp
ManicDiggerLib/Client/MainMenu/CharacterCreator.ci.cs
```
The character creator menu with controls and layout.

**Application Mod**:
```csharp
ManicDiggerLib/Client/Mods/ApplyCharacterCustomization.ci.cs
```
Applies saved customization to the player entity when spawning.

### How It Works

1. **Player selects options** in CharacterCreator screen
2. **Preferences saved** as comma-separated string: `"0,1,2,0"`
3. **On spawn**, ApplyCharacterCustomization mod:
   - Loads preferences
   - Sets model to `playerenhanced.txt`
   - Sets texture to `player_[options].png` (or falls back to `mineplayer.png`)
4. **Player entity updated** with custom appearance

### Integration Points

**To add more options**:
1. Update `CharacterCustomization` class with new field
2. Add controls to `CharacterCreator` screen
3. Update serialization methods
4. Create corresponding textures

**To change model per gender**:
Modify `GetModelName()` in CharacterCustomization.ci.cs:
```csharp
public string GetModelName()
{
    if (Gender == 0)
        return "playerenhanced_male.txt";
    else
        return "playerenhanced_female.txt";
}
```

---

## Troubleshooting

### Character looks wrong
- Check that the texture file exists in `data/public/`
- Verify the filename matches the pattern exactly
- The game will use `mineplayer.png` if custom texture not found

### Customization not saving
- Make sure you clicked "Confirm" button
- Check that preferences can be written (file permissions)
- Preferences stored in system-specific location

### Model animations not working
- Ensure `playerenhanced.txt` exists in `data/public/`
- Check that the model file is not corrupted
- Falls back to `player.txt` if enhanced model missing

---

## Additional Resources

📖 **Full Documentation**: [CHARACTER_CUSTOMIZATION.md](CHARACTER_CUSTOMIZATION.md)  
🎨 **Texture Guide**: [CHARACTER_TEXTURES_GUIDE.md](data/public/CHARACTER_TEXTURES_GUIDE.md)  
📝 **Implementation Details**: [IMPLEMENTATION_SUMMARY_CHARACTER_CUSTOMIZATION.md](IMPLEMENTATION_SUMMARY_CHARACTER_CUSTOMIZATION.md)  

---

## What's Next?

Future enhancements could include:
- 3D preview while customizing
- More customization options (skin tone, eye color, accessories)
- Character presets (save favorite combinations)
- Texture pack support
- Custom texture upload
- Animation customization

---

**Enjoy customizing your character in Sword&Stone!** 🎮⚔️🪨
