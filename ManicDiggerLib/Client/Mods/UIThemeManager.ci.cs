/// <summary>
/// UI Theme Manager - Loads and manages UI themes for consistent styling
/// Supports theme switching and provides centralized access to theme assets
/// </summary>
public class UIThemeManager
{
    public UIThemeManager()
    {
        themeName = "default";
        basePath = "data/themes/default/";
        themeLoaded = false;
        
        // Initialize path constants
        hudPath = "hud/";
        uiPath = "ui/";
        speechPath = "speech/";
        menuPath = "menu/";
        inventoryPath = "inventory/";
        buttonsPath = "buttons/";
        framesPath = "frames/";
        barsPath = "bars/";
    }
    
    internal string themeName;
    internal string basePath;
    internal bool themeLoaded;
    
    // Asset paths
    internal string hudPath;
    internal string uiPath;
    internal string speechPath;
    internal string menuPath;
    internal string inventoryPath;
    internal string buttonsPath;
    internal string framesPath;
    internal string barsPath;
    
    // Sprite sheet paths
    internal string hudPiecesPath;
    internal string uiSplitPath;
    internal string speechBubbleSmallPath;
    internal string speechBubbleLargePath;
    
    // Individual asset paths
    internal string buttonNormalPath;
    internal string buttonHoverPath;
    internal string buttonPressedPath;
    internal string buttonLongNormalPath;
    internal string buttonLongHoverPath;
    internal string buttonLongPressedPath;
    
    internal string frameSmallPath;
    internal string frameOrnatePath;
    internal string frameCircularPath;
    internal string frameLarge1Path;
    internal string frameLarge2Path;
    internal string frameLarge3Path;
    
    internal string barFullRedPath;
    internal string barFullBluePath;
    internal string barLeftPath;
    internal string barRightPath;
    
    internal string slotNormalPath;
    internal string slotActivePath;
    internal string slotHighlightPath;
    
    // Theme colors (RGB values 0-255)
    internal int primaryR;
    internal int primaryG;
    internal int primaryB;
    internal int secondaryR;
    internal int secondaryG;
    internal int secondaryB;
    internal int accentR;
    internal int accentG;
    internal int accentB;
    
    /// <summary>
    /// Initialize the theme manager with default theme
    /// </summary>
    public void Initialize(Game game)
    {
        LoadTheme(game, "default");
    }
    
    /// <summary>
    /// Load a theme by name
    /// </summary>
    public void LoadTheme(Game game, string name)
    {
        themeName = name;
        basePath = game.platform.StringFormat("data/themes/{0}/", name);
        
        // Set up asset paths
        SetupAssetPaths(game);
        
        // Load theme colors (default values)
        LoadDefaultColors();
        
        themeLoaded = true;
    }
    
    /// <summary>
    /// Set up all asset paths based on theme base path
    /// </summary>
    void SetupAssetPaths(Game game)
    {
        // Sprite sheets
        hudPiecesPath = game.platform.StringFormat("{0}{1}hud_pieces.png", basePath, hudPath);
        uiSplitPath = game.platform.StringFormat("{0}{1}ui_split.png", basePath, uiPath);
        speechBubbleSmallPath = game.platform.StringFormat("{0}{1}speech_bubble_small_spritesheet.png", basePath, speechPath);
        speechBubbleLargePath = game.platform.StringFormat("{0}{1}speech_bubble_large_spritesheet.png", basePath, speechPath);
        
        // Buttons
        buttonNormalPath = game.platform.StringFormat("{0}{1}button_normal.png", basePath, buttonsPath);
        buttonHoverPath = game.platform.StringFormat("{0}{1}button_hover.png", basePath, buttonsPath);
        buttonPressedPath = game.platform.StringFormat("{0}{1}button_pressed.png", basePath, buttonsPath);
        buttonLongNormalPath = game.platform.StringFormat("{0}{1}button_long_normal.png", basePath, buttonsPath);
        buttonLongHoverPath = game.platform.StringFormat("{0}{1}button_long_hover.png", basePath, buttonsPath);
        buttonLongPressedPath = game.platform.StringFormat("{0}{1}button_long_pressed.png", basePath, buttonsPath);
        
        // Frames
        frameSmallPath = game.platform.StringFormat("{0}{1}frame_small.png", basePath, framesPath);
        frameOrnatePath = game.platform.StringFormat("{0}{1}frame_ornate.png", basePath, framesPath);
        frameCircularPath = game.platform.StringFormat("{0}{1}frame_circular.png", basePath, framesPath);
        frameLarge1Path = game.platform.StringFormat("{0}{1}frame_large_1.png", basePath, framesPath);
        frameLarge2Path = game.platform.StringFormat("{0}{1}frame_large_2.png", basePath, framesPath);
        frameLarge3Path = game.platform.StringFormat("{0}{1}frame_large_3.png", basePath, framesPath);
        
        // Bars
        barFullRedPath = game.platform.StringFormat("{0}{1}bar_full_red.png", basePath, barsPath);
        barFullBluePath = game.platform.StringFormat("{0}{1}bar_full_blue.png", basePath, barsPath);
        barLeftPath = game.platform.StringFormat("{0}{1}bar_left.png", basePath, barsPath);
        barRightPath = game.platform.StringFormat("{0}{1}bar_right.png", basePath, barsPath);
        
        // Inventory slots
        slotNormalPath = game.platform.StringFormat("{0}{1}slot_normal.png", basePath, inventoryPath);
        slotActivePath = game.platform.StringFormat("{0}{1}slot_active.png", basePath, inventoryPath);
        slotHighlightPath = game.platform.StringFormat("{0}{1}slot_highlight.png", basePath, inventoryPath);
    }
    
    /// <summary>
    /// Load default color scheme (bronze/orange fantasy theme)
    /// </summary>
    void LoadDefaultColors()
    {
        // Primary color (bronze/orange)
        primaryR = 160;
        primaryG = 100;
        primaryB = 40;
        
        // Secondary color (dark bronze)
        secondaryR = 100;
        secondaryG = 80;
        secondaryB = 60;
        
        // Accent color (golden yellow)
        accentR = 255;
        accentG = 215;
        accentB = 0;
    }
    
    /// <summary>
    /// Get primary color as ARGB
    /// </summary>
    public int GetPrimaryColor()
    {
        return Game.ColorFromArgb(255, primaryR, primaryG, primaryB);
    }
    
    /// <summary>
    /// Get secondary color as ARGB
    /// </summary>
    public int GetSecondaryColor()
    {
        return Game.ColorFromArgb(255, secondaryR, secondaryG, secondaryB);
    }
    
    /// <summary>
    /// Get accent color as ARGB
    /// </summary>
    public int GetAccentColor()
    {
        return Game.ColorFromArgb(255, accentR, accentG, accentB);
    }
    
    // Sprite sheet getters
    public string GetHudPiecesPath() { return hudPiecesPath; }
    public string GetUiSplitPath() { return uiSplitPath; }
    public string GetSpeechBubbleSmallPath() { return speechBubbleSmallPath; }
    public string GetSpeechBubbleLargePath() { return speechBubbleLargePath; }
    
    // Button getters
    public string GetButtonPath(int state)
    {
        if (state == 1) { return buttonHoverPath; }
        if (state == 2) { return buttonPressedPath; }
        return buttonNormalPath;
    }
    
    public string GetButtonLongPath(int state)
    {
        if (state == 1) { return buttonLongHoverPath; }
        if (state == 2) { return buttonLongPressedPath; }
        return buttonLongNormalPath;
    }
    
    // Frame getters
    public string GetFrameSmallPath() { return frameSmallPath; }
    public string GetFrameOrnatePath() { return frameOrnatePath; }
    public string GetFrameCircularPath() { return frameCircularPath; }
    public string GetFrameLarge1Path() { return frameLarge1Path; }
    public string GetFrameLarge2Path() { return frameLarge2Path; }
    public string GetFrameLarge3Path() { return frameLarge3Path; }
    
    // Bar getters
    public string GetBarRedPath() { return barFullRedPath; }
    public string GetBarBluePath() { return barFullBluePath; }
    public string GetBarLeftPath() { return barLeftPath; }
    public string GetBarRightPath() { return barRightPath; }
    
    // Inventory slot getters
    public string GetSlotPath(bool active)
    {
        if (active) { return slotActivePath; }
        return slotNormalPath;
    }
    
    public string GetSlotHighlightPath() { return slotHighlightPath; }
}

/// <summary>
/// Speech bubble renderer using theme sprite sheets
/// Supports both small and large animated speech bubbles
/// </summary>
public class SpeechBubbleRenderer
{
    public SpeechBubbleRenderer()
    {
        animationFrame = 0;
        animationTime = 0;
        frameDuration = 0.15f; // 150ms per frame
    }
    
    internal int animationFrame;
    internal float animationTime;
    internal float frameDuration;
    
    /// <summary>
    /// Update animation frame based on time
    /// </summary>
    public void Update(float deltaTime)
    {
        animationTime += deltaTime;
        if (animationTime >= frameDuration)
        {
            animationTime = 0;
            animationFrame++;
        }
    }
    
    /// <summary>
    /// Draw a small speech bubble with text
    /// </summary>
    public void DrawSmallBubble(Game game, int x, int y, string text)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        if (theme == null) { return; }
        
        // Get current animation frame (2 frames total)
        int frame = animationFrame % 2;
        
        // Draw speech bubble sprite (24x24)
        // For now, draw the entire sprite sheet and use texture coordinates later
        // This is a simplified version - proper implementation would use texture atlases
        game.Draw2dBitmapFile(theme.GetSpeechBubbleSmallPath(), x, y, 24, 24);
        
        // Draw text inside bubble
        int textX = x + 4;
        int textY = y + 6;
        game.Draw2dText(text, game.fontDefault, textX, textY, null);
    }
    
    /// <summary>
    /// Draw a large speech bubble with text
    /// </summary>
    public void DrawLargeBubble(Game game, int x, int y, string text)
    {
        UIThemeManager theme = game.GetUIThemeManager();
        if (theme == null) { return; }
        
        // Get current animation frame (4 frames total)
        int frame = animationFrame % 4;
        
        // Draw speech bubble sprite (32x32)
        game.Draw2dBitmapFile(theme.GetSpeechBubbleLargePath(), x, y, 32, 32);
        
        // Draw text inside bubble
        int textX = x + 6;
        int textY = y + 8;
        game.Draw2dText(text, game.fontDefault, textX, textY, null);
    }
    
    /// <summary>
    /// Draw speech bubble above player for emotes
    /// </summary>
    public void DrawPlayerSpeechBubble(Game game, int playerX, int playerY, string text, bool isLarge)
    {
        // Position bubble above player head
        int bubbleX = playerX - 12;
        int bubbleY = playerY - 40;
        
        if (isLarge)
        {
            bubbleX = playerX - 16;
            bubbleY = playerY - 50;
            DrawLargeBubble(game, bubbleX, bubbleY, text);
        }
        else
        {
            DrawSmallBubble(game, bubbleX, bubbleY, text);
        }
    }
}
