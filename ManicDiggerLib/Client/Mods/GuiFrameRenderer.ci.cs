/// <summary>
/// Standardized frame rendering utility for all GUI components.
/// Uses the golden UI pieces extracted from ui_big_pieces.png to provide consistent styling.
/// All GUI implementations should use this helper for frames, buttons, bars, and slots.
/// </summary>
public class GuiFrameRenderer
{
    // Base path for golden UI assets
    internal const string GOLDEN_UI_PATH = "local/gui/golden/";
    
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
    /// Draws a standardized frame with golden borders.
    /// First draws a dark panel background, then overlays the frame border.
    /// </summary>
    public static void DrawFrame(Game game, int x, int y, int width, int height, int frameType)
    {
        // Draw dark background panel first
        string panelPath = game.platform.StringFormat("{0}panel_dark.png", GOLDEN_UI_PATH);
        game.Draw2dBitmapFile(panelPath, x + 8, y + 8, width - 16, height - 16);
        
        // Then draw the frame border on top
        string framePath = GOLDEN_UI_PATH;
        
        if (frameType == FRAME_SMALL)
        {
            framePath = game.platform.StringFormat("{0}frame_small.png", GOLDEN_UI_PATH);
        }
        else if (frameType == FRAME_LARGE_ORNATE)
        {
            framePath = game.platform.StringFormat("{0}frame_ornate.png", GOLDEN_UI_PATH);
        }
        else if (frameType == FRAME_CIRCULAR)
        {
            framePath = game.platform.StringFormat("{0}frame_circular.png", GOLDEN_UI_PATH);
        }
        
        game.Draw2dBitmapFile(framePath, x, y, width, height);
    }
    
    /// <summary>
    /// Draws a button in normal, hover, or pressed state.
    /// </summary>
    public static void DrawButton(Game game, int x, int y, int width, int height, int state)
    {
        string buttonPath = game.platform.StringFormat("{0}button_normal.png", GOLDEN_UI_PATH);
        
        if (state == BUTTON_HOVER)
        {
            buttonPath = game.platform.StringFormat("{0}button_hover.png", GOLDEN_UI_PATH);
        }
        else if (state == BUTTON_PRESSED)
        {
            buttonPath = game.platform.StringFormat("{0}button_pressed.png", GOLDEN_UI_PATH);
        }
        
        game.Draw2dBitmapFile(buttonPath, x, y, width, height);
    }
    
    /// <summary>
    /// Draws a circular frame (for minimap, portraits, etc).
    /// </summary>
    public static void DrawCircularFrame(Game game, int x, int y, int size)
    {
        string framePath = game.platform.StringFormat("{0}frame_circular.png", GOLDEN_UI_PATH);
        game.Draw2dBitmapFile(framePath, x, y, size, size);
    }
    
    /// <summary>
    /// Draws a progress/health bar using the golden bar pieces.
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
        
        // Draw filled portion using appropriate bar texture
        int filledWidth = game.platform.FloatToInt(width * progress);
        if (filledWidth > 0)
        {
            string barPath = game.platform.StringFormat("{0}bar_full_red.png", GOLDEN_UI_PATH);
            if (barType == BAR_TYPE_BLUE)
            {
                barPath = game.platform.StringFormat("{0}bar_full_blue.png", GOLDEN_UI_PATH);
            }
            
            // Draw the bar (it will be stretched to fit)
            game.Draw2dBitmapFile(barPath, x + 2, y + 2, filledWidth - 4, height - 4);
        }
        
        // Draw golden border
        int borderColor = Game.ColorFromArgb(255, 160, 100, 40);
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, 2, null, 0, borderColor, false); // Top
        game.Draw2dTexture(game.WhiteTexture(), x, y + height - 2, width, 2, null, 0, borderColor, false); // Bottom
        game.Draw2dTexture(game.WhiteTexture(), x, y, 2, height, null, 0, borderColor, false); // Left
        game.Draw2dTexture(game.WhiteTexture(), x + width - 2, y, 2, height, null, 0, borderColor, false); // Right
    }
    
    /// <summary>
    /// Draws an inventory/action bar slot.
    /// </summary>
    public static void DrawSlot(Game game, int x, int y, int size, bool highlighted)
    {
        string slotPath = game.platform.StringFormat("{0}slot_normal.png", GOLDEN_UI_PATH);
        if (highlighted)
        {
            slotPath = game.platform.StringFormat("{0}slot_active.png", GOLDEN_UI_PATH);
        }
        
        game.Draw2dBitmapFile(slotPath, x, y, size, size);
    }
    
    /// <summary>
    /// Draws a portrait border (circular with golden frame).
    /// </summary>
    public static void DrawPortraitBorder(Game game, int x, int y, int size)
    {
        string borderPath = game.platform.StringFormat("{0}portrait_border.png", GOLDEN_UI_PATH);
        game.Draw2dBitmapFile(borderPath, x, y, size, size);
    }
}
