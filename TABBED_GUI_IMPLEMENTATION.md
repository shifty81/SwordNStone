# Tabbed GUI Implementation for Settings Menu

## Overview

This document describes the implementation of a tabbed GUI interface for the Manic Digger settings menu, based on the `Settings-controls_tab-mode_1.png` design image.

## Design Reference

The implementation follows the design shown in `Settings-controls_tab-mode_1.png`, which features:
- A tab bar at the top with 8 tabs: Graphics, Mouse, Controls, Accessibility, Sound, Interface, Dev, and Back
- A dark panel background for tabs and content
- Visual feedback for the active tab (highlighted)
- A content area below the tabs showing settings and key bindings

## Key Components

### 1. Enhanced Enum (`EscapeMenuState`)

**File:** `SwordAndStoneLib/Client/Misc/GameMisc.ci.cs`

Added new states to support the tabbed interface:
- `Mouse` - Mouse settings tab
- `Controls` - Key bindings and controls configuration
- `Accessibility` - Accessibility options (placeholder)
- `Sound` - Audio settings
- `Interface` - Interface customization (placeholder)
- `Dev` - Developer options (placeholder)

### 2. Tab Navigation System

**File:** `SwordAndStoneLib/Client/Mods/GuiEscapeMenu.ci.cs`

#### Tab Buttons
- Created 8 tab buttons in `InitTabButtons()` method
- Tabs are stored separately from content widgets for proper rendering order
- Each tab has text, position, size, and color properties

#### Tab Layout (`MakeTabLayout`)
- Positions tabs horizontally across the top of the screen
- Calculates total width and centers tabs
- Applies different colors to active vs inactive tabs
- Active tab: Gold text (#FFD700), brighter background
- Inactive tab: Gray text (#B4B4B4), darker background

#### Tab Switching
- Click on tabs to switch between sections
- Keyboard support: ESC key saves and returns to main menu
- Mouse hover highlights tabs

### 3. Visual Design

#### Tab Bar
- Position: 35 pixels from top
- Height: 35 pixels
- Background: Semi-transparent dark gray (RGB: 70, 70, 70, Alpha: 180)
- Border: 2-pixel borders around each tab

#### Content Panel
- Position: Below tab bar with 5-pixel spacing
- Background: Semi-transparent darker gray (RGB: 50, 50, 50, Alpha: 200)
- Margins: 30 pixels on left and right
- Fills remaining vertical space

#### Colors
- **Active Tab Background**: RGB(100, 100, 100) at full opacity
- **Inactive Tab Background**: RGB(60, 60, 60) at 200 alpha
- **Active Tab Text**: Gold (#FFD700)
- **Inactive Tab Text**: Light gray (#B4B4B4)
- **Tab Hover**: White (#FFFFFF)
- **Active Tab Border**: RGB(180, 180, 180)
- **Inactive Tab Border**: RGB(80, 80, 80)

### 4. Tab Content

#### Graphics Tab
- Same options as before (smooth shadows, view distance, etc.)
- Now displayed in tabbed layout
- Options aligned to the left with proper spacing

#### Mouse Tab
```
Content:
- Checkbox: "Mouse click modifiers locked to Sneak/Sprint keys"
- (Additional mouse settings can be added here)
```

#### Controls Tab (Main Feature)
The Controls tab displays key bindings in an organized list:

**Movement Controls:**
- Walk forward: W
- Walk backward: S
- Walk left: A
- Walk right: D
- Sneak: LShift
- Sprint: LControl
- Jump: BackSpace
- Sit down: G

**Actions:**
- Open Backpack & Crafting Inventory: E
- Open Character Inventory: C
- Drop one item: Q
- (More actions follow...)

**Bottom Buttons:**
- "Reset Controls" - Resets all key bindings to defaults
- "Open Macro Editor" - Opens macro editor (placeholder for future feature)

#### Sound Tab
- Audio enabled/disabled toggle
- (Space for volume sliders and audio options)

#### Accessibility, Interface, Dev Tabs
- Placeholder tabs for future features
- Framework in place to add content easily

### 5. Code Structure

#### Helper Methods

**`IsTabbed(EscapeMenuState state)`**
- Checks if the current state uses the tabbed interface
- Returns true for Graphics, Mouse, Controls, Accessibility, Sound, Interface, and Dev
- Centralizes logic to reduce code duplication

**`IsActiveTab(int tabIndex, EscapeMenuState state)`**
- Determines if a specific tab index is the currently active tab
- Used for highlighting and visual feedback
- Maps tab indices to states:
  - 0: Graphics
  - 1: Mouse
  - 2: Controls
  - 3: Accessibility
  - 4: Sound
  - 5: Interface
  - 6: Dev
  - 7: Back

**`DrawBorder(int x, int y, int width, int height, int thickness, int color)`**
- Utility method to draw borders around UI elements
- Draws top, bottom, left, and right borders
- Used for tab borders and content area borders

#### Tab Content Methods

Each tab has two methods:
1. **`TabNameSet()`** - Initializes widgets for the tab
2. **`TabNameHandleClick(Button b)`** - Handles button clicks within the tab

Example for Mouse tab:
```csharp
void MouseSet()
{
    mouseCheckboxSneakSprint = new Button();
    mouseCheckboxSneakSprint.Text = "Mouse click modifiers locked to Sneak/Sprint keys";
    WidgetsClear();
    AddWidget(mouseCheckboxSneakSprint);
}

void MouseHandleClick(Button b)
{
    // Handle mouse tab button clicks
}
```

### 6. Rendering Flow

1. **`OnNewFrameDraw2d()`** is called every frame
2. Check if in tabbed state using `IsTabbed(escapemenustate)`
3. If tabbed:
   a. Draw tab bar background panel
   b. Draw content panel background
   c. Draw each tab button with appropriate colors
   d. Draw tab borders
   e. Draw tab text
4. Draw content widgets for current tab
5. Handle mouse hover and click events

### 7. Integration Points

#### State Management
- Integrated with existing `SetEscapeMenuState()` method
- Calls `InitTabButtons()` when entering a tabbed state
- Uses `MakeTabLayout()` instead of `MakeSimpleOptions()` for tabbed states

#### Input Handling
- Updated `EscapeMenuMouse1()` to check tabs before content widgets
- Updated `OnKeyDown()` to handle ESC key for all tabbed states
- Key binding selection works in both Keys and Controls tabs

#### Settings Persistence
- Settings are saved when leaving tabbed menus
- Uses existing `SaveOptions()` and `LoadOptions()` methods
- No changes to save/load logic needed

## Future Enhancements

### Potential Additions
1. **Scrollable Content Area**
   - For tabs with many options
   - Scrollbar on the right side

2. **Subsections in Controls Tab**
   - Collapsible sections for "Movement", "Combat", "Interface", etc.
   - Better organization for 20+ key bindings

3. **Visual Key Binding**
   - Show key icons instead of text names
   - Highlight conflicting key bindings

4. **Macro Editor Implementation**
   - Full macro recording and editing system
   - Save macros to file

5. **Accessibility Options**
   - Font size adjustment
   - Color blind modes
   - High contrast mode
   - Text-to-speech integration

6. **Interface Customization**
   - UI scale slider
   - HUD element positioning
   - Theme selection

7. **Developer Options**
   - Debug rendering toggles
   - Performance metrics
   - Console access
   - Mod loading interface

## Technical Notes

### Performance
- Tab rendering is lightweight (simple texture draws)
- No significant performance impact
- Buttons are only created when entering a tab (lazy initialization)

### Compatibility
- Works with existing save system
- No breaking changes to existing code
- Old-style menu (Main, Options) still available
- Backward compatible with existing settings files

### Testing
- Tested with Cito compiler - compiles successfully
- No compilation errors or warnings related to new code
- Security scan passed (0 vulnerabilities)

## File Changes Summary

### Modified Files
1. **`SwordAndStoneLib/Client/Misc/GameMisc.ci.cs`**
   - Added new EscapeMenuState enum values

2. **`SwordAndStoneLib/Client/Mods/GuiEscapeMenu.ci.cs`**
   - Added tab button array and initialization
   - Added `InitTabButtons()` method
   - Added `MakeTabLayout()` method
   - Added `IsTabbed()` and `IsActiveTab()` helper methods
   - Added `DrawBorder()` utility method
   - Added 6 new tab content methods (Mouse, Controls, Accessibility, Sound, Interface, Dev)
   - Updated `SetEscapeMenuState()` to handle new states
   - Updated `OnNewFrameDraw2d()` to render tabs
   - Updated `EscapeMenuMouse1()` to handle tab clicks
   - Updated `OnKeyDown()` to handle ESC in tabbed states
   - Updated `HandleButtonClick()` to dispatch to new tab handlers

### Lines of Code
- **Added:** ~280 lines
- **Modified:** ~40 lines
- **Deleted:** ~25 lines (duplicated code replaced with helpers)
- **Net Change:** ~295 lines

## Usage

### For Users
1. Press ESC to open the menu
2. Click "Options" from the main menu
3. Click on any tab to view settings
4. Click "Back" tab or press ESC to save and return

### For Developers
To add a new tab:
1. Add enum value to `EscapeMenuState` in `GameMisc.ci.cs`
2. Add tab button in `InitTabButtons()` in `GuiEscapeMenu.ci.cs`
3. Create `TabNameSet()` and `TabNameHandleClick()` methods
4. Add case in `SetEscapeMenuState()` to call your Set method
5. Update `IsActiveTab()` to include your tab index
6. Add handler call in `HandleButtonClick()`

Example:
```csharp
// In InitTabButtons()
tabNewFeature = new Button();
tabNewFeature.Text = "New Feature";
tabButtons[tabButtonsCount++] = tabNewFeature;

// Add methods
void NewFeatureSet()
{
    WidgetsClear();
    // Add your widgets
}

void NewFeatureHandleClick(Button b)
{
    // Handle clicks
}
```

## Conclusion

This implementation provides a solid foundation for a modern, tabbed settings interface that matches the design shown in the reference image. The code is well-structured, maintainable, and easy to extend with new tabs and features.
