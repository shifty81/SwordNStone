// Theme Editor Screen - Main UI for editing UI themes
// Allows users to create and customize themes with visual preview
public class ScreenThemeEditor : Screen
{
    public ScreenThemeEditor()
    {
        canvas = new ThemeCanvas();
        colorPicker = new ColorPicker();
        tools = new PixelArtTools(); // Reuse existing pixel art tools
        
        // Create widgets
        brushButton = CreateButtonTheme("Brush");
        eraserButton = CreateButtonTheme("Eraser");
        fillButton = CreateButtonTheme("Fill");
        pickerButton = CreateButtonTheme("Picker");
        
        brushSizeMinusButton = CreateButtonTheme("-");
        brushSizePlusButton = CreateButtonTheme("+");
        
        // Asset type selection buttons
        buttonAssetButton = CreateButtonTheme("Button");
        frameAssetButton = CreateButtonTheme("Frame");
        barAssetButton = CreateButtonTheme("Bar");
        slotAssetButton = CreateButtonTheme("Slot");
        
        // Asset state buttons (for buttons/slots)
        stateNormalButton = CreateButtonTheme("Normal");
        stateHoverButton = CreateButtonTheme("Hover");
        statePressedButton = CreateButtonTheme("Pressed");
        
        // Theme color preset buttons
        colorPrimaryButton = CreateButtonTheme("Primary");
        colorSecondaryButton = CreateButtonTheme("Secondary");
        colorAccentButton = CreateButtonTheme("Accent");
        
        // Utility buttons
        clearButton = CreateButtonTheme("Clear");
        loadDefaultButton = CreateButtonTheme("Load Default");
        saveButton = CreateButtonTheme("Save Theme");
        exportButton = CreateButtonTheme("Export Asset");
        backButton = CreateButtonTheme("Back");
        
        // Theme selection
        themeListButton = CreateButtonTheme("Theme List");
        newThemeButton = CreateButtonTheme("New Theme");
        
        // Color component sliders
        colorRMinusButton = CreateButtonTheme("-");
        colorRPlusButton = CreateButtonTheme("+");
        colorGMinusButton = CreateButtonTheme("-");
        colorGPlusButton = CreateButtonTheme("+");
        colorBMinusButton = CreateButtonTheme("-");
        colorBPlusButton = CreateButtonTheme("+");
        
        // Gradient tools
        gradientHorizButton = CreateButtonTheme("H Gradient");
        gradientVertButton = CreateButtonTheme("V Gradient");
        borderButton = CreateButtonTheme("Add Border");
        
        // Assign widgets to array
        int widgetIndex = 0;
        widgets[widgetIndex++] = brushButton;
        widgets[widgetIndex++] = eraserButton;
        widgets[widgetIndex++] = fillButton;
        widgets[widgetIndex++] = pickerButton;
        widgets[widgetIndex++] = brushSizeMinusButton;
        widgets[widgetIndex++] = brushSizePlusButton;
        widgets[widgetIndex++] = buttonAssetButton;
        widgets[widgetIndex++] = frameAssetButton;
        widgets[widgetIndex++] = barAssetButton;
        widgets[widgetIndex++] = slotAssetButton;
        widgets[widgetIndex++] = stateNormalButton;
        widgets[widgetIndex++] = stateHoverButton;
        widgets[widgetIndex++] = statePressedButton;
        widgets[widgetIndex++] = colorPrimaryButton;
        widgets[widgetIndex++] = colorSecondaryButton;
        widgets[widgetIndex++] = colorAccentButton;
        widgets[widgetIndex++] = clearButton;
        widgets[widgetIndex++] = loadDefaultButton;
        widgets[widgetIndex++] = saveButton;
        widgets[widgetIndex++] = exportButton;
        widgets[widgetIndex++] = backButton;
        widgets[widgetIndex++] = themeListButton;
        widgets[widgetIndex++] = newThemeButton;
        widgets[widgetIndex++] = colorRMinusButton;
        widgets[widgetIndex++] = colorRPlusButton;
        widgets[widgetIndex++] = colorGMinusButton;
        widgets[widgetIndex++] = colorGPlusButton;
        widgets[widgetIndex++] = colorBMinusButton;
        widgets[widgetIndex++] = colorBPlusButton;
        widgets[widgetIndex++] = gradientHorizButton;
        widgets[widgetIndex++] = gradientVertButton;
        widgets[widgetIndex++] = borderButton;
        
        title = "UI Theme Editor";
        
        fontDefault = new FontCi();
        fontDefault.size = 14;
        
        fontTitle = new FontCi();
        fontTitle.size = 24;
        
        fontSmall = new FontCi();
        fontSmall.size = 12;
        
        one = 1;  // Initialize for fixed-point arithmetic
        isDrawing = false;
        canvasTextureId = -1;
        currentAssetType = ThemeCanvas.CANVAS_TYPE_BUTTON;
        currentAssetState = ASSET_STATE_NORMAL;
        gridScale = 8; // Pixels per grid cell
        canvasOffsetX = 50;
        canvasOffsetY = 120;
        
        needsTextureUpdate = true;
        
        // Initialize with button canvas
        canvas.SetCanvasType(currentAssetType);
        
        // Current theme being edited
        currentThemeName = "custom";
        currentThemeAuthor = "Theme Editor User";
        
        showThemeList = false;
        showColorPresets = true;
    }
    
    MenuWidget CreateButtonTheme(string text)
    {
        MenuWidget widget = new MenuWidget();
        widget.text = text;
        widget.type = WidgetType.Button;
        return widget;
    }
    
    MenuWidget brushButton;
    MenuWidget eraserButton;
    MenuWidget fillButton;
    MenuWidget pickerButton;
    MenuWidget brushSizeMinusButton;
    MenuWidget brushSizePlusButton;
    
    MenuWidget buttonAssetButton;
    MenuWidget frameAssetButton;
    MenuWidget barAssetButton;
    MenuWidget slotAssetButton;
    
    MenuWidget stateNormalButton;
    MenuWidget stateHoverButton;
    MenuWidget statePressedButton;
    
    MenuWidget colorPrimaryButton;
    MenuWidget colorSecondaryButton;
    MenuWidget colorAccentButton;
    
    MenuWidget clearButton;
    MenuWidget loadDefaultButton;
    MenuWidget saveButton;
    MenuWidget exportButton;
    MenuWidget backButton;
    
    MenuWidget themeListButton;
    MenuWidget newThemeButton;
    
    MenuWidget colorRMinusButton;
    MenuWidget colorRPlusButton;
    MenuWidget colorGMinusButton;
    MenuWidget colorGPlusButton;
    MenuWidget colorBMinusButton;
    MenuWidget colorBPlusButton;
    
    MenuWidget gradientHorizButton;
    MenuWidget gradientVertButton;
    MenuWidget borderButton;
    
    ThemeCanvas canvas;
    ColorPicker colorPicker;
    PixelArtTools tools;
    
    internal string title;
    internal FontCi fontDefault;
    internal FontCi fontTitle;
    internal FontCi fontSmall;
    
    internal int one;  // For compatibility with fixed-point arithmetic
    internal bool isDrawing;
    internal int canvasTextureId;
    internal int gridScale;
    internal int canvasOffsetX;
    internal int canvasOffsetY;
    internal bool needsTextureUpdate;
    
    // Asset states (for elements with multiple states)
    internal const int ASSET_STATE_NORMAL = 0;
    internal const int ASSET_STATE_HOVER = 1;
    internal const int ASSET_STATE_PRESSED = 2;
    
    internal int currentAssetType;
    internal int currentAssetState;
    
    // Theme metadata
    internal string currentThemeName;
    internal string currentThemeAuthor;
    
    // UI state
    internal bool showThemeList;
    internal bool showColorPresets;
    
    public override void Render(float dt)
    {
        GamePlatform p = menu.p;
        
        // Background
        menu.DrawBackground();
        
        // Title
        menu.DrawText(title, fontTitle, 20, 20, TextAlign.Left, TextBaseline.Top);
        
        // Current theme info
        string themeInfo = p.StringFormat("Theme: {0}", currentThemeName);
        themeInfo = p.StringFormat2("{0} by {1}", themeInfo, currentThemeAuthor);
        menu.DrawText(themeInfo, fontSmall, 20, 60, TextAlign.Left, TextBaseline.Top);
        
        // Canvas rendering disabled: MainMenu context lacks required texture APIs for full editor functionality
        
        // Show simple message instead
        menu.DrawText("Theme Editor - Under Construction", fontDefault, 
            p.GetCanvasWidth() / 2, p.GetCanvasHeight() / 2, 
            TextAlign.Center, TextBaseline.Middle);
        
        // Update texture if needed
        if (needsTextureUpdate)
        {
            // Canvas texture updates disabled - would require direct texture rendering API not available in MainMenu context
            needsTextureUpdate = false;
        }
        
        // Position back button
        backButton.x = 40;
        backButton.y = p.GetCanvasHeight() - 80;
        backButton.sizex = 200;
        backButton.sizey = 60;
        
        DrawWidgets();
    }
    
    // Helper methods - simplified due to API limitations
    
    string GetAssetTypeLabel()
    {
        switch (currentAssetType)
        {
            case ThemeCanvas.CANVAS_TYPE_BUTTON: return "Button";
            case ThemeCanvas.CANVAS_TYPE_FRAME: return "Frame";
            case ThemeCanvas.CANVAS_TYPE_BAR: return "Progress Bar";
            case ThemeCanvas.CANVAS_TYPE_SLOT: return "Inventory Slot";
            case ThemeCanvas.CANVAS_TYPE_CUSTOM: return "Custom Asset";
            default: return "Unknown";
        }
    }
    
    string GetStateLabel()
    {
        switch (currentAssetState)
        {
            case ASSET_STATE_NORMAL: return "Normal";
            case ASSET_STATE_HOVER: return "Hover";
            case ASSET_STATE_PRESSED: return "Pressed";
            default: return "Unknown";
        }
    }
    
    string GetCurrentToolName()
    {
        PixelArtToolType tool = tools.GetCurrentTool();
        if (tool == PixelArtToolType.Brush) { return "Brush"; }
        if (tool == PixelArtToolType.Eraser) { return "Eraser"; }
        if (tool == PixelArtToolType.FillBucket) { return "Fill Bucket"; }
        if (tool == PixelArtToolType.ColorPicker) { return "Color Picker"; }
        return "Unknown";
    }
    
    public override void OnMouseDown(MouseEventArgs e)
    {
        // Base widget handling skipped - CiTo compilation doesn't support base method calls
        
        int x = e.GetX();
        int y = e.GetY();
        
        // Check if click is on canvas
        int canvasWidth = canvas.width;
        int canvasHeight = canvas.height;
        int scaledWidth = one * canvasWidth * gridScale;
        int scaledHeight = one * canvasHeight * gridScale;
        
        if (x >= canvasOffsetX && x < canvasOffsetX + scaledWidth &&
            y >= canvasOffsetY && y < canvasOffsetY + scaledHeight)
        {
            isDrawing = true;
            
            // Convert to canvas coordinates (with safety check for gridScale)
            int canvasX = 0;
            int canvasY = 0;
            if (gridScale > 0)
            {
                canvasX = (x - canvasOffsetX) / one / gridScale;
                canvasY = (y - canvasOffsetY) / one / gridScale;
            }
            
            // Apply tool
            ApplyTool(canvasX, canvasY);
        }
    }
    
    public override void OnMouseMove(MouseEventArgs e)
    {
        // Base widget hover handling skipped - CiTo compilation doesn't support base method calls
        
        if (!isDrawing)
        {
            return;
        }
        
        int x = e.GetX();
        int y = e.GetY();
        
        // Convert to canvas coordinates
        int canvasWidth = canvas.width;
        int canvasHeight = canvas.height;
        int scaledWidth = one * canvasWidth * gridScale;
        int scaledHeight = one * canvasHeight * gridScale;
        
        if (x >= canvasOffsetX && x < canvasOffsetX + scaledWidth &&
            y >= canvasOffsetY && y < canvasOffsetY + scaledHeight)
        {
            // Convert to canvas coordinates (with safety check for gridScale)
            int canvasX = 0;
            int canvasY = 0;
            if (gridScale > 0)
            {
                canvasX = (x - canvasOffsetX) / one / gridScale;
                canvasY = (y - canvasOffsetY) / one / gridScale;
            }
            
            // Apply tool
            ApplyTool(canvasX, canvasY);
        }
    }
    
    public override void OnMouseUp(MouseEventArgs e)
    {
        // Base widget click handling skipped - CiTo compilation doesn't support base method calls
        
        isDrawing = false;
    }
    
    void ApplyTool(int canvasX, int canvasY)
    {
        int color = colorPicker.GetSelectedColor();
        
        tools.ApplyToolToThemeCanvas(canvas, canvasX, canvasY, color);
        needsTextureUpdate = true;
    }
    
    public override void OnButton(MenuWidget widget)
    {
        // Tool selection
        if (widget == brushButton) { tools.SetCurrentTool(PixelArtToolType.Brush); }
        if (widget == eraserButton) { tools.SetCurrentTool(PixelArtToolType.Eraser); }
        if (widget == fillButton) { tools.SetCurrentTool(PixelArtToolType.FillBucket); }
        if (widget == pickerButton) { tools.SetCurrentTool(PixelArtToolType.ColorPicker); }
        
        // Brush size
        if (widget == brushSizeMinusButton) { tools.DecreaseBrushSize(); }
        if (widget == brushSizePlusButton) { tools.IncreaseBrushSize(); }
        
        // Asset type selection
        if (widget == buttonAssetButton) 
        { 
            currentAssetType = ThemeCanvas.CANVAS_TYPE_BUTTON;
            canvas.SetCanvasType(currentAssetType);
            needsTextureUpdate = true;
        }
        if (widget == frameAssetButton)
        {
            currentAssetType = ThemeCanvas.CANVAS_TYPE_FRAME;
            canvas.SetCanvasType(currentAssetType);
            needsTextureUpdate = true;
        }
        if (widget == barAssetButton)
        {
            currentAssetType = ThemeCanvas.CANVAS_TYPE_BAR;
            canvas.SetCanvasType(currentAssetType);
            needsTextureUpdate = true;
        }
        if (widget == slotAssetButton)
        {
            currentAssetType = ThemeCanvas.CANVAS_TYPE_SLOT;
            canvas.SetCanvasType(currentAssetType);
            needsTextureUpdate = true;
        }
        
        // Asset state selection
        if (widget == stateNormalButton) { currentAssetState = ASSET_STATE_NORMAL; }
        if (widget == stateHoverButton) { currentAssetState = ASSET_STATE_HOVER; }
        if (widget == statePressedButton) { currentAssetState = ASSET_STATE_PRESSED; }
        
        // Color presets (load theme colors)
        if (widget == colorPrimaryButton) { LoadPrimaryColor(); }
        if (widget == colorSecondaryButton) { LoadSecondaryColor(); }
        if (widget == colorAccentButton) { LoadAccentColor(); }
        
        // Color adjustments
        if (widget == colorRMinusButton) { colorPicker.DecreaseRed(); }
        if (widget == colorRPlusButton) { colorPicker.IncreaseRed(); }
        if (widget == colorGMinusButton) { colorPicker.DecreaseGreen(); }
        if (widget == colorGPlusButton) { colorPicker.IncreaseGreen(); }
        if (widget == colorBMinusButton) { colorPicker.DecreaseBlue(); }
        if (widget == colorBPlusButton) { colorPicker.IncreaseBlue(); }
        
        // Utility actions
        if (widget == clearButton) 
        { 
            canvas.Clear();
            needsTextureUpdate = true;
        }
        
        if (widget == loadDefaultButton)
        {
            LoadDefaultAsset();
            needsTextureUpdate = true;
        }
        
        if (widget == gradientHorizButton)
        {
            ApplyGradient(true);
            needsTextureUpdate = true;
        }
        
        if (widget == gradientVertButton)
        {
            ApplyGradient(false);
            needsTextureUpdate = true;
        }
        
        if (widget == borderButton)
        {
            ApplyBorder();
            needsTextureUpdate = true;
        }
        
        if (widget == saveButton)
        {
            SaveTheme();
        }
        
        if (widget == exportButton)
        {
            ExportAsset();
        }
        
        if (widget == backButton)
        {
            menu.StartMainMenu();
        }
    }
    
    void LoadPrimaryColor()
    {
        // Load primary color from current theme (default: bronze/orange)
        colorPicker.SetColor(160, 100, 40);
    }
    
    void LoadSecondaryColor()
    {
        // Load secondary color from current theme (default: dark bronze)
        colorPicker.SetColor(100, 80, 60);
    }
    
    void LoadAccentColor()
    {
        // Load accent color from current theme (default: golden)
        colorPicker.SetColor(255, 215, 0);
    }
    
    void ApplyGradient(bool horizontal)
    {
        int startColor = colorPicker.GetSelectedColor();
        // Create a darker end color
        int endR = Game.ColorR(startColor) / 2;
        int endG = Game.ColorG(startColor) / 2;
        int endB = Game.ColorB(startColor) / 2;
        int endColor = Game.ColorFromArgb(255, endR, endG, endB);
        
        canvas.DrawGradient(startColor, endColor, horizontal);
    }
    
    void ApplyBorder()
    {
        int borderColor = colorPicker.GetSelectedColor();
        canvas.DrawBorder(borderColor, 2);
    }
    
    void LoadDefaultAsset()
    {
        // Load a default asset based on current asset type
        // For now, create a simple default design
        
        canvas.Clear();
        
        // Fill with background color
        int bgColor = Game.ColorFromArgb(255, 50, 50, 60);
        canvas.Fill(bgColor);
        
        // Add border
        int borderColor = Game.ColorFromArgb(255, 160, 100, 40);
        canvas.DrawBorder(borderColor, 3);
    }
    
    void SaveTheme()
    {
        // Save current theme configuration
        // This would write theme.txt and assets to disk
        // TODO: Implement theme saving
    }
    
    void ExportAsset()
    {
        // Export current canvas as PNG file
        // This would save to the appropriate theme directory
        // TODO: Implement asset export
    }
}
