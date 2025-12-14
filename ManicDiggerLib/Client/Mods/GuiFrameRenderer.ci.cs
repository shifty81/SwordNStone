/// <summary>
/// Standardized frame rendering utility for all GUI components.
/// Uses the theme system to provide consistent styling across all UI elements.
/// All GUI implementations should use this helper for frames, buttons, bars, and slots.
/// </summary>
public class GuiFrameRenderer
{
    // Legacy path for backward compatibility
    internal const string GOLDEN_UI_PATH = "data/themes/default/";
    
    // Frame types
    internal const int FRAME_SMALL = 0;
    internal const int FRAME_LARGE_ORNATE = 1;
    internal const int FRAME_CIRCULAR = 2;
    
    // Button states
    internal const int BUTTON_NORMAL = 0;
    internal const int BUTTON_HOVER = 1;
    internal const int BUTTON_PRESSED = 2;
    
    // Bar types
    internal const int BAR_TYPE_RED = 0;    // Health, damage
    internal const int BAR_TYPE_BLUE = 1;   // Oxygen, mana, stamina
    
    /// <summary>
    /// Draws a standardized frame with borders using the current theme.
    /// First draws a dark panel background, then overlays the frame border.
    /// </summary>
    public static void DrawFrame(Game game, int x, int y, int width, int height, int frameType)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        
        // Draw dark background panel first
        string panelPath;
        if (theme != null)
        {
            panelPath = game.platform.StringFormat("{0}ui/panel_dark.png", theme.basePath);
        }
        else
        {
            // Fallback to legacy path
            panelPath = "data/local/gui/golden/panel_dark.png";
        }
        game.Draw2dBitmapFile(panelPath, x + 8, y + 8, width - 16, height - 16);
        
        // Then draw the frame border on top
        string framePath;
        
        if (theme != null)
        {
            if (frameType == FRAME_SMALL)
            {
                framePath = theme.GetFrameSmallPath();
            }
            else if (frameType == FRAME_LARGE_ORNATE)
            {
                framePath = theme.GetFrameOrnatePath();
            }
            else if (frameType == FRAME_CIRCULAR)
            {
                framePath = theme.GetFrameCircularPath();
            }
            else
            {
                framePath = theme.GetFrameSmallPath();
            }
        }
        else
        {
            // Fallback to legacy paths
            if (frameType == FRAME_SMALL)
            {
                framePath = "data/local/gui/golden/frame_small.png";
            }
            else if (frameType == FRAME_LARGE_ORNATE)
            {
                framePath = "data/local/gui/golden/frame_ornate.png";
            }
            else if (frameType == FRAME_CIRCULAR)
            {
                framePath = "data/local/gui/golden/frame_circular.png";
            }
            else
            {
                framePath = "data/local/gui/golden/frame_small.png";
            }
        }
        
        game.Draw2dBitmapFile(framePath, x, y, width, height);
    }
    
    /// <summary>
    /// Draws a button in normal, hover, or pressed state using the current theme.
    /// </summary>
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
            if (state == BUTTON_HOVER)
            {
                buttonPath = "data/local/gui/golden/button_hover.png";
            }
            else if (state == BUTTON_PRESSED)
            {
                buttonPath = "data/local/gui/golden/button_pressed.png";
            }
            else
            {
                buttonPath = "data/local/gui/golden/button_normal.png";
            }
        }
        
        game.Draw2dBitmapFile(buttonPath, x, y, width, height);
    }
    
    /// <summary>
    /// Draws a circular frame (for minimap, portraits, etc) using the current theme.
    /// </summary>
    public static void DrawCircularFrame(Game game, int x, int y, int size)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        string framePath;
        
        if (theme != null)
        {
            framePath = theme.GetFrameCircularPath();
        }
        else
        {
            // Fallback to legacy path
            framePath = "data/local/gui/golden/frame_circular.png";
        }
        
        game.Draw2dBitmapFile(framePath, x, y, size, size);
    }
    
    /// <summary>
    /// Draws a progress/health bar using the theme bar pieces.
    /// </summary>
    /// <param name="game">Game instance</param>
    /// <param name="x">X position</param>
    /// <param name="y">Y position</param>
    /// <param name="width">Bar width in pixels</param>
    /// <param name="height">Bar height in pixels</param>
    /// <param name="progress">Progress value (0.0 to 1.0, clamped automatically)</param>
    /// <param name="barType">Bar color type (BAR_TYPE_RED or BAR_TYPE_BLUE)</param>
    public static void DrawProgressBar(Game game, int x, int y, int width, int height, float progress, int barType)
    {
        // Clamp progress to valid range [0.0, 1.0]
        if (progress < 0) { progress = 0; }
        if (progress > game.one) { progress = game.one; }
        
        // Draw background (dark gray)
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, height, null, 0,
            Game.ColorFromArgb(220, 50, 50, 50), false);
        
        // Draw filled portion using appropriate bar texture from theme
        int filledWidth = game.platform.FloatToInt(width * progress);
        if (filledWidth > 0)
        {
            UIThemeManager theme = game.GetUIThemeManager();
            string barPath;
            
            if (theme != null)
            {
                if (barType == BAR_TYPE_BLUE)
                {
                    barPath = theme.GetBarBluePath();
                }
                else
                {
                    barPath = theme.GetBarRedPath();
                }
            }
            else
            {
                // Fallback to legacy paths
                if (barType == BAR_TYPE_BLUE)
                {
                    barPath = "data/local/gui/golden/bar_full_blue.png";
                }
                else
                {
                    barPath = "data/local/gui/golden/bar_full_red.png";
                }
            }
            
            // Draw the bar (it will be stretched to fit)
            game.Draw2dBitmapFile(barPath, x + 2, y + 2, filledWidth - 4, height - 4);
        }
        
        // Draw border using theme color or default golden
        UIThemeManager theme2 = game.GetUIThemeManager();
        int borderColor;
        if (theme2 != null)
        {
            borderColor = theme2.GetPrimaryColor();
        }
        else
        {
            borderColor = Game.ColorFromArgb(255, 160, 100, 40);
        }
        
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, 2, null, 0, borderColor, false); // Top
        game.Draw2dTexture(game.WhiteTexture(), x, y + height - 2, width, 2, null, 0, borderColor, false); // Bottom
        game.Draw2dTexture(game.WhiteTexture(), x, y, 2, height, null, 0, borderColor, false); // Left
        game.Draw2dTexture(game.WhiteTexture(), x + width - 2, y, 2, height, null, 0, borderColor, false); // Right
    }
    
    /// <summary>
    /// Draws an inventory/action bar slot using the current theme.
    /// </summary>
    public static void DrawSlot(Game game, int x, int y, int size, bool highlighted)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        string slotPath;
        
        if (theme != null)
        {
            slotPath = theme.GetSlotPath(highlighted);
        }
        else
        {
            // Fallback to legacy paths
            if (highlighted)
            {
                slotPath = "data/local/gui/golden/slot_active.png";
            }
            else
            {
                slotPath = "data/local/gui/golden/slot_normal.png";
            }
        }
        
        game.Draw2dBitmapFile(slotPath, x, y, size, size);
    }
    
    /// <summary>
    /// Draws a portrait border (circular frame) using the current theme.
    /// </summary>
    public static void DrawPortraitBorder(Game game, int x, int y, int size)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        string borderPath;
        
        if (theme != null)
        {
            // Portrait borders use circular frames
            borderPath = theme.GetFrameCircularPath();
        }
        else
        {
            // Fallback to legacy path
            borderPath = "data/local/gui/golden/portrait_border.png";
        }
        
        game.Draw2dBitmapFile(borderPath, x, y, size, size);
    }
}
