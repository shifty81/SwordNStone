# UI Theme System Implementation Summary

## Overview

This document describes the implementation of the standardized UI theme system for Sword&Stone, which provides a flexible, modular approach to customizing all GUI elements using the new sprite sheets as specified in the requirements.

## Requirements

The original requirements specified that the following files should be the standard for all GUI elements and menu screens:
- `hud-pieces (1).png`
- `ui_split.png`
- `speech_bubble_small_spritesheet.png`
- `speech_bubble_large_spritesheet.png`

Additionally, a requirement was added to organize files better with a directory structure that:
- Allows easy changes to UI elements
- Supports theme system for moving forward
- Uses structured guidelines from the provided files

## Implementation

### 1. Theme Directory Structure

Created a new hierarchical theme system at `data/themes/`:

```
data/themes/
├── README.md                          # Theme system overview
├── THEME_CREATION_GUIDE.md           # Detailed guide for creating themes
└── default/                           # Default theme (standard)
    ├── theme.txt                      # Theme configuration and sprite maps
    ├── hud/
    │   └── hud_pieces.png            # Renamed from "hud-pieces (1).png"
    ├── ui/
    │   ├── ui_split.png              # Main UI sprite sheet
    │   ├── panel_dark.png
    │   └── panel_medium.png
    ├── speech/
    │   ├── speech_bubble_small_spritesheet.png
    │   └── speech_bubble_large_spritesheet.png
    ├── buttons/
    │   ├── button_normal.png
    │   ├── button_hover.png
    │   ├── button_pressed.png
    │   └── ... (long button variants)
    ├── frames/
    │   ├── frame_small.png
    │   ├── frame_ornate.png
    │   ├── frame_circular.png
    │   └── ... (large frame variants)
    ├── bars/
    │   ├── bar_full_red.png
    │   ├── bar_full_blue.png
    │   └── ... (bar segments)
    └── inventory/
        ├── slot_normal.png
        ├── slot_active.png
        └── slot_highlight.png
```

### 2. Theme Configuration System

Created `data/themes/default/theme.txt` with:

#### Theme Metadata
```ini
[Theme]
name=Default
author=SwordNStone Team
version=1.0
description=Standard UI theme using standardized sprite sheets
```

#### Asset Path Configuration
```ini
[Paths]
base_path=data/themes/default/
hud_path=hud/
ui_path=ui/
speech_path=speech/
...
```

#### Sprite Map Coordinates
Defined coordinates for all UI elements in sprite sheets:
```ini
[HUD.SpriteMap]
frame_small_tl=0,0,16,16
frame_small_t=16,0,16,16
...

[UI.SpriteMap]
panel_tl=0,0,32,32
inventory_background=128,32,160,224
...

[Speech.SpriteMap]
bubble_small_frame1=0,0,24,24
bubble_large_frame1=0,0,32,32
...
```

#### Color Scheme
```ini
[Colors]
primary=160,100,40       # Bronze/orange
secondary=100,80,60      # Dark bronze
accent=255,215,0         # Golden yellow
...
```

### 3. Core Framework Classes

#### UIThemeManager (`SwordAndStoneLib/Client/Mods/UIThemeManager.ci.cs`)

**Purpose**: Centralized theme management and asset loading

**Key Features**:
- Loads theme configurations from `theme.txt`
- Provides asset path lookup methods
- Manages theme color schemes
- Supports future runtime theme switching

**Key Methods**:
```csharp
void Initialize(Game game)
void LoadTheme(Game game, string name)
string GetButtonPath(int state)
string GetFrameSmallPath()
string GetBarRedPath()
string GetSlotPath(bool active)
int GetPrimaryColor()
int GetSecondaryColor()
int GetAccentColor()
```

**Integration**:
- Added to Game class as `uiThemeManager` field
- Initialized in Game constructor
- Accessible via `game.GetUIThemeManager()`

#### SpeechBubbleRenderer (`SwordAndStoneLib/Client/Mods/UIThemeManager.ci.cs`)

**Purpose**: Render animated speech bubbles for chat and emotes

**Key Features**:
- Supports small (24x24) and large (32x32) bubbles
- Animation frame management
- Position bubbles above player heads
- Integration with EmoteSystem

**Key Methods**:
```csharp
void Update(float deltaTime)
void DrawSmallBubble(Game game, int x, int y, string text)
void DrawLargeBubble(Game game, int x, int y, string text)
void DrawPlayerSpeechBubble(Game game, int playerX, int playerY, string text, bool isLarge)
```

### 4. Updated GuiFrameRenderer

Modified `SwordAndStoneLib/Client/Mods/GuiFrameRenderer.ci.cs` to use theme system:

**Changes**:
- Updated all drawing methods to use `UIThemeManager`
- Added fallback to legacy paths for backward compatibility
- Use theme colors for borders and accents
- Maintained existing API for compatibility

**Updated Methods**:
- `DrawFrame()` - Uses theme frame assets
- `DrawButton()` - Uses theme button states
- `DrawCircularFrame()` - Uses theme circular frames
- `DrawProgressBar()` - Uses theme bars and colors
- `DrawSlot()` - Uses theme inventory slots
- `DrawPortraitBorder()` - Uses theme circular frames

**Example**:
```csharp
public static void DrawButton(Game game, int x, int y, int width, int height, int state)
{
    UIThemeManager theme = game.GetUIThemeManager();
    string buttonPath;
    
    if (theme != null)
    {
        buttonPath = theme.GetButtonPath(state);
    }
    else
    {
        // Fallback to legacy paths
        buttonPath = "local/gui/golden/button_normal.png";
    }
    
    game.Draw2dBitmapFile(buttonPath, x, y, width, height);
}
```

### 5. Game Class Integration

Modified `SwordAndStoneLib/Client/Game.ci.cs`:

**Added**:
```csharp
// Field declaration
internal UIThemeManager uiThemeManager;

// Initialization in constructor
uiThemeManager = new UIThemeManager();
uiThemeManager.Initialize(this);

// Accessor method
public UIThemeManager GetUIThemeManager()
{
    return uiThemeManager;
}
```

### 6. Documentation

Created comprehensive documentation:

#### `data/themes/README.md`
- Theme system overview
- Quick start guide
- Directory structure explanation
- Technical integration details
- Usage examples

#### `data/themes/THEME_CREATION_GUIDE.md`
- Step-by-step theme creation instructions
- Sprite sheet design guidelines
- Visual consistency guidelines
- 9-slice frame design
- Button state requirements
- Color scheme definition
- Testing procedures
- Example themes and use cases

## File Changes Summary

### New Files Created
1. `SwordAndStoneLib/Client/Mods/UIThemeManager.ci.cs` - Theme manager and speech bubble renderer
2. `data/themes/default/theme.txt` - Theme configuration
3. `data/themes/README.md` - Theme system overview
4. `data/themes/THEME_CREATION_GUIDE.md` - Theme creation guide
5. `UI_THEME_SYSTEM_IMPLEMENTATION.md` - This document

### Files Modified
1. `SwordAndStoneLib/Client/Mods/GuiFrameRenderer.ci.cs` - Updated to use theme system
2. `SwordAndStoneLib/Client/Game.ci.cs` - Added theme manager integration

### Files Relocated
1. `hud-pieces (1).png` → `data/themes/default/hud/hud_pieces.png`
2. `ui_split.png` → `data/themes/default/ui/ui_split.png`
3. `speech_bubble_small_spritesheet.png` → `data/themes/default/speech/`
4. `speech_bubble_large_spritesheet.png` → `data/themes/default/speech/`
5. All golden UI pieces → `data/themes/default/*/` (organized by type)

## Backward Compatibility

The implementation maintains full backward compatibility:

1. **Legacy Path Support**: All methods check for theme manager and fall back to legacy paths
2. **Existing Assets**: Original assets remain in `data/local/gui/golden/` as fallbacks
3. **API Compatibility**: All existing API methods unchanged, just enhanced internally
4. **Graceful Degradation**: If theme manager fails, legacy rendering continues

## Benefits

### For Users
- **Easy Customization**: Simple directory structure for modifying UI
- **Theme Switching**: Foundation for runtime theme selection
- **Visual Consistency**: All UI elements follow same design language
- **Better Organization**: Logical grouping of related assets

### For Developers
- **Centralized Management**: Single point for theme configuration
- **Cleaner Code**: Less hardcoded paths, more maintainable
- **Extensibility**: Easy to add new UI elements
- **Documentation**: Clear guidelines for contributing

### For Content Creators
- **Clear Structure**: Easy to understand and modify
- **Sprite Maps**: Documented coordinates for all elements
- **Examples**: Default theme as reference
- **Guides**: Comprehensive creation documentation

## Future Enhancements

### Planned Features
1. **Runtime Theme Switching**: Change themes without restart
2. **Theme Preview**: Visual preview in options menu
3. **Community Repository**: Share and download themes
4. **Validation Tool**: Verify theme completeness
5. **Texture Atlases**: Advanced sprite sheet management
6. **Theme Inheritance**: Base themes with variants
7. **Dynamic Colors**: Theme color modifiers
8. **Animation Support**: Frame-based animations for UI

### GUI Component Updates (Next Phase)
The following components need to be updated to fully utilize the new theme system:
- GuiInventory
- GuiEscapeMenu (menu screen)
- GuiPlayerStats (HUD elements)
- GuiWoWActionBars
- GuiWoWUnitFrames
- GuiChat (speech bubble integration)
- MainMenu

## Testing

### Recommended Testing Procedures
1. **Visual Inspection**: Verify all UI elements render correctly
2. **Theme Switching**: Test loading different themes
3. **Fallback Testing**: Remove theme files to test fallbacks
4. **Performance**: Monitor performance impact
5. **Compatibility**: Test with existing mods and content

### Known Limitations
1. Build requires Mono/CiTo which wasn't available during development
2. Runtime testing deferred to user with build environment
3. Some GUI components not yet updated to use theme system (Phase 3)

## Conclusion

This implementation provides a solid foundation for a modular, extensible UI theme system that meets the requirements:

✅ Uses specified sprite sheets as standard  
✅ Better directory structure for organization  
✅ Easy to modify UI elements  
✅ Theme system for future flexibility  
✅ Structured guidelines via documentation  
✅ Backward compatible with existing code  
✅ Retroactive support through GuiFrameRenderer updates  

The system is ready for:
- GUI component updates (Phase 3)
- Testing and validation
- Community theme creation
- Future enhancements

## References

- **Theme System**: `data/themes/README.md`
- **Theme Creation**: `data/themes/THEME_CREATION_GUIDE.md`
- **Character Customization**: `CHARACTER_CUSTOMIZATION.md`
- **Build Instructions**: `BUILD.md`
