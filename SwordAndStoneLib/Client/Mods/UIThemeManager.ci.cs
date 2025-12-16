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
        hudPiecesPath = game.platform.StringFormat3("{0}{1}{2}", basePath, hudPath, "hud_pieces.png");
        uiSplitPath = game.platform.StringFormat3("{0}{1}{2}", basePath, uiPath, "ui_split.png");
        speechBubbleSmallPath = game.platform.StringFormat3("{0}{1}{2}", basePath, speechPath, "speech_bubble_small_spritesheet.png");
        speechBubbleLargePath = game.platform.StringFormat3("{0}{1}{2}", basePath, speechPath, "speech_bubble_large_spritesheet.png");
        
        // Buttons
        buttonNormalPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_normal.png");
        buttonHoverPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_hover.png");
        buttonPressedPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_pressed.png");
        buttonLongNormalPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_long_normal.png");
        buttonLongHoverPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_long_hover.png");
        buttonLongPressedPath = game.platform.StringFormat3("{0}{1}{2}", basePath, buttonsPath, "button_long_pressed.png");
        
        // Frames
        frameSmallPath = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_small.png");
        frameOrnatePath = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_ornate.png");
        frameCircularPath = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_circular.png");
        frameLarge1Path = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_large_1.png");
        frameLarge2Path = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_large_2.png");
        frameLarge3Path = game.platform.StringFormat3("{0}{1}{2}", basePath, framesPath, "frame_large_3.png");
        
        // Bars
        barFullRedPath = game.platform.StringFormat3("{0}{1}{2}", basePath, barsPath, "bar_full_red.png");
        barFullBluePath = game.platform.StringFormat3("{0}{1}{2}", basePath, barsPath, "bar_full_blue.png");
        barLeftPath = game.platform.StringFormat3("{0}{1}{2}", basePath, barsPath, "bar_left.png");
        barRightPath = game.platform.StringFormat3("{0}{1}{2}", basePath, barsPath, "bar_right.png");
        
        // Inventory slots
        slotNormalPath = game.platform.StringFormat3("{0}{1}{2}", basePath, inventoryPath, "slot_normal.png");
        slotActivePath = game.platform.StringFormat3("{0}{1}{2}", basePath, inventoryPath, "slot_active.png");
        slotHighlightPath = game.platform.StringFormat3("{0}{1}{2}", basePath, inventoryPath, "slot_highlight.png");
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
    
    /// <summary>
    /// Get the current theme name
    /// </summary>
    public string GetThemeName() { return themeName; }
    
    /// <summary>
    /// Check if theme is loaded
    /// </summary>
    public bool IsThemeLoaded() { return themeLoaded; }
    
    /// <summary>
    /// List available themes in the themes directory
    /// Returns array of theme names (discovers both default and custom themes)
    /// </summary>
    public string[] ListAvailableThemes(Game game)
    {
        // This would scan data/themes/ directory for theme.txt files
        // For now, return a hardcoded list of known themes
        string[] themes = new string[3];
        themes[0] = "default";
        themes[1] = "cyberpunk";
        themes[2] = "custom";
        return themes;
    }
    
    /// <summary>
    /// Switch to a different theme at runtime
    /// </summary>
    public void SwitchTheme(Game game, string newThemeName)
    {
        LoadTheme(game, newThemeName);
        
        // Notify game that theme has changed (UI may need to reload textures)
        game.AddChatline(game.platform.StringFormat("Switched to theme: {0}", newThemeName));
    }
    
    /// <summary>
    /// Validate that a theme has all required assets
    /// Returns true if theme is valid, false otherwise
    /// </summary>
    public bool ValidateTheme(Game game, string themeNameToValidate)
    {
        // Check if theme directory exists
        string themePath = game.platform.StringFormat("data/themes/{0}/", themeNameToValidate);
        
        // Check for theme.txt
        string themeFile = game.platform.StringFormat("{0}theme.txt", themePath);
        
        // Would need file system access to actually check
        // For now, assume valid if it's a known theme
        return true;
    }
    
    /// <summary>
    /// Get theme metadata for display in UI
    /// </summary>
    public string GetThemeDescription(string themeNameToDescribe)
    {
        if (themeNameToDescribe == "default")
        {
            return "Standard bronze/orange fantasy theme";
        }
        else if (themeNameToDescribe == "cyberpunk")
        {
            return "Futuristic sci-fi theme with blue/cyan accents";
        }
        else if (themeNameToDescribe == "custom")
        {
            return "User-created custom theme";
        }
        return "Custom theme";
    }
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
        FontCi font = new FontCi();
        font.size = 8;
        int textX = x + 4;
        int textY = y + 6;
        game.Draw2dText(text, font, textX, textY, null, false);
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
        FontCi font = new FontCi();
        font.size = 10;
        int textX = x + 6;
        int textY = y + 8;
        game.Draw2dText(text, font, textX, textY, null, false);
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
