# Character Preview and GUI Improvements

## Summary
This update addresses the GUI improvements requested in the issue:
1. Added a visual character preview to the Character Creator screen
2. Improved hotbar centering
3. Repositioned chat window to bottom-left with transparency
4. Added chat hide/show toggle functionality

## Changes Made

### 1. Character Creator Preview (`ManicDiggerLib/Client/MainMenu/CharacterCreator.ci.cs`)

#### Visual Character Preview
- Added a `DrawCharacterPreview()` method that displays the currently selected character appearance
- The preview is positioned to the right of the customization options
- Uses a golden ornate frame to match the game's UI style
- Displays the character skin texture in a 256x384 pixel preview area
- Shows a centered character texture (128x256) within the frame
- Includes a "Character Preview" label below the display

#### Layout Improvements
- Moved customization options to the left third of the screen
- Positioned character preview in the right two-thirds of the screen
- This provides clear visual feedback of character choices while maintaining easy access to controls

### 2. Hotbar Centering (`ManicDiggerLib/Client/Mods/GuiWoWActionBars.ci.cs`)

#### Fixed Centering Algorithm
- **Old behavior**: Used a hardcoded offset of 512 pixels, which didn't scale properly
- **New behavior**: Dynamically calculates the total width of the action bar
  - Total width = (10 slots × button size) + (11 × spacing)
  - Centers horizontally: `(screen width - total width) / 2`
- This ensures the hotbar is perfectly centered regardless of screen resolution or scaling

### 3. Chat Window Improvements (`ManicDiggerLib/Client/Mods/GuiChat.ci.cs`)

#### Bottom-Left Positioning
- Repositioned chat from top (y=90) to bottom-left of screen
- New position: `screen height - (chat lines × line spacing) - 180`
- The 180-pixel offset leaves room for the hotbar at the bottom

#### Transparency Enhancement
- Increased transparency of chat background
- Changed alpha from 80 to 60 (more transparent)
- Maintains readability while being less obtrusive

#### Hide/Show Toggle
- Added `chatHidden` boolean flag
- Press **F12** to toggle chat visibility
- Hidden state prevents drawing chat lines but still allows typing
- Useful for screenshots or when playing single-player

#### Console Functionality
- Chat already supports commands (typing with "/" prefix)
- Works identically in single-player and multiplayer modes
- Command processing is handled through `game.ClientCommand()`

## Testing Notes

### Validation Performed
- Cito compilation successful (JavaScript transpilation validates syntax)
- No compilation errors in the modified files
- Code follows existing patterns in the codebase

### Expected Behavior After Build

1. **Character Creator**:
   - When opening the character creator, users will see:
     - Customization options (Gender, Hairstyle, Beard, Outfit) on the left
     - A framed preview showing the character appearance on the right
     - Preview updates in real-time as options change

2. **Hotbar**:
   - The 10-slot action bar will be perfectly centered horizontally
   - Scales properly with different screen resolutions
   - Maintains consistent spacing between slots

3. **Chat**:
   - Appears in the bottom-left corner above the hotbar
   - Semi-transparent golden frame matches UI style
   - Press F12 to hide/show the chat window
   - Chat input always visible when typing (even if chat is hidden)
   - Commands work by typing "/" followed by the command

## Technical Details

### Character Texture Loading
The character preview uses `customization.GetTextureName()` which returns:
- Default: "mineplayer.png"
- Custom: "player_{gender}_{hairstyle}_{beard}_{outfit}.png"

This allows for future expansion with custom character textures.

### Frame Rendering
Uses the golden UI asset system:
- Frame: `local/gui/golden/frame_ornate.png`
- Background: `local/gui/golden/panel_dark.png`

### Scaling
All UI elements use `game.Scale()` or `menu.GetScale()` for proper DPI scaling across different screen sizes.

## Future Enhancements

Potential improvements for future iterations:
1. 3D character model preview with rotation
2. More character customization options (skin tone, eye color, etc.)
3. Chat window resize/drag functionality
4. Chat transparency slider in settings
5. Custom chat position saving

## Files Modified

1. `ManicDiggerLib/Client/MainMenu/CharacterCreator.ci.cs` - Character preview
2. `ManicDiggerLib/Client/Mods/GuiWoWActionBars.ci.cs` - Hotbar centering
3. `ManicDiggerLib/Client/Mods/GuiChat.ci.cs` - Chat positioning and visibility
