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
        
        // Note: Full canvas rendering functionality commented out due to API limitations
        // The theme editor would require direct texture rendering capabilities
        // that are not available in the MainMenu screen context
        
        // Show simple message instead
        menu.DrawText("Theme Editor - Under Construction", fontDefault, 
            p.GetCanvasWidth() / 2, p.GetCanvasHeight() / 2, 
            TextAlign.Center, TextBaseline.Middle);
        
        // Update texture if needed
        if (needsTextureUpdate)
        {
            // UpdateCanvasTexture(p); // Disabled - requires texture API
            needsTextureUpdate = false;
        }
        
        // Position back button
        backButton.x = 40;
        backButton.y = p.GetCanvasHeight() - 80;
        backButton.sizex = 200;
        backButton.sizey = 60;
        
        DrawWidgets();
    }
    
    /* Disabled - requires direct texture rendering API not available in MainMenu context
    void DrawCanvas(GamePlatform p)
    {
        int canvasWidth = canvas.width;
        int canvasHeight = canvas.height;
        int scaledWidth = one * canvasWidth * gridScale;
        int scaledHeight = one * canvasHeight * gridScale;
        
        // Canvas background (dark gray)
        int canvasBg = Game.ColorFromArgb(255, 50, 50, 60);
        p.Draw2dTexture(p.WhiteTexture(), canvasOffsetX - 5, canvasOffsetY - 5, 
            scaledWidth + 10, scaledHeight + 10, null, 0, canvasBg, false);
        
        // Draw canvas texture
        if (canvasTextureId != -1)
        {
            p.Draw2dTexture(canvasTextureId, canvasOffsetX, canvasOffsetY, 
                scaledWidth, scaledHeight, null, 0, Game.ColorFromArgb(255, 255, 255, 255), false);
        }
        
        // Draw grid
        int gridColor = Game.ColorFromArgb(100, 200, 200, 200);
        for (int x = 0; x <= canvasWidth; x++)
        {
            int xPos = canvasOffsetX + one * x * gridScale;
            p.Draw2dTexture(p.WhiteTexture(), xPos, canvasOffsetY, 1, scaledHeight, null, 0, gridColor, false);
        }
        for (int y = 0; y <= canvasHeight; y++)
        {
            int yPos = canvasOffsetY + one * y * gridScale;
            p.Draw2dTexture(p.WhiteTexture(), canvasOffsetX, yPos, scaledWidth, 1, null, 0, gridColor, false);
        }
        
        // Canvas label
        string canvasLabel = GetAssetTypeLabel();
        p.Draw2dText(canvasLabel, fontDefault, canvasOffsetX, canvasOffsetY - 30, null, false);
    }
    
    void DrawToolsPanel(GamePlatform p)
    {
        int panelX = 20;
        int panelY = 100;
        int panelWidth = 180;
        int panelHeight = 300;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        p.Draw2dTexture(p.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        p.Draw2dText("Tools", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Position buttons (simplified layout)
        int btnY = panelY + 40;
        int btnSpacing = 35;
        
        // Draw tool buttons (will be positioned by widget system)
        // Tool selection indicators
        string currentTool = GetCurrentToolName();
        p.Draw2dText(p.StringFormat("Current: {0}", currentTool), fontSmall, panelX + 10, btnY, null, false);
        
        btnY += btnSpacing;
        p.Draw2dText(p.StringFormat("Brush Size: {0}", p.IntToString(tools.GetBrushSize())), fontSmall, panelX + 10, btnY, null, false);
    }
    
    void DrawColorPickerPanel(GamePlatform p)
    {
        int panelX = 20;
        int panelY = 420;
        int panelWidth = 180;
        int panelHeight = 200;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        p.Draw2dTexture(p.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        p.Draw2dText("Colors", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Current color preview (large square)
        int currentColor = colorPicker.GetSelectedColor();
        int colorPreviewSize = 60;
        int colorPreviewX = panelX + (panelWidth - colorPreviewSize) / 2;
        int colorPreviewY = panelY + 40;
        p.Draw2dTexture(p.WhiteTexture(), colorPreviewX, colorPreviewY, 
            colorPreviewSize, colorPreviewSize, null, 0, currentColor, false);
        
        // RGB values
        int r = colorPicker.GetRed();
        int g = colorPicker.GetGreen();
        int b = colorPicker.GetBlue();
        
        int rgbY = colorPreviewY + colorPreviewSize + 10;
        p.Draw2dText(p.StringFormat("R: {0}", p.IntToString(r)), fontSmall, panelX + 10, rgbY, null, false);
        p.Draw2dText(p.StringFormat("G: {0}", p.IntToString(g)), fontSmall, panelX + 10, rgbY + 20, null, false);
        p.Draw2dText(p.StringFormat("B: {0}", p.IntToString(b)), fontSmall, panelX + 10, rgbY + 40, null, false);
    }
    
    void DrawAssetPanel(GamePlatform p)
    {
        int panelX = p.GetCanvasWidth() - 220;
        int panelY = 100;
        int panelWidth = 200;
        int panelHeight = 300;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        p.Draw2dTexture(p.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        p.Draw2dText("Asset Type", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Asset type info
        string assetInfo = GetAssetTypeLabel();
        p.Draw2dText(assetInfo, fontSmall, panelX + 10, panelY + 40, null, false);
        
        // State info (if applicable)
        if (currentAssetType == ThemeCanvas.CANVAS_TYPE_BUTTON || currentAssetType == ThemeCanvas.CANVAS_TYPE_SLOT)
        {
            string stateLabel = GetStateLabel();
            p.Draw2dText(p.StringFormat("State: {0}", stateLabel), fontSmall, panelX + 10, panelY + 65, null, false);
        }
        
        // Canvas size info
        string widthStr = p.IntToString(canvas.width);
        string heightStr = p.IntToString(canvas.height);
        string sizeTextW = p.StringFormat("W:{0}", widthStr);
        string sizeTextH = p.StringFormat(" H:{0}", heightStr);
        p.Draw2dText(sizeTextW, fontSmall, panelX + 10, panelY + 90, null, false);
        p.Draw2dText(sizeTextH, fontSmall, panelX + 60, panelY + 90, null, false);
    }
    
    void DrawPreviewPanel(GamePlatform p)
    {
        int panelX = p.GetCanvasWidth() - 220;
        int panelY = 420;
        int panelWidth = 200;
        int panelHeight = 200;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        p.Draw2dTexture(p.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        p.Draw2dText("Preview", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Draw preview of current asset in context
        int previewX = panelX + (panelWidth - 100) / 2;
        int previewY = panelY + 60;
        
        // Show the asset as it would appear in UI
        if (canvasTextureId != -1)
        {
            p.Draw2dTexture(canvasTextureId, previewX, previewY, 100, 50, null, 0, 
                Game.ColorFromArgb(255, 255, 255, 255), false);
        }
        
        p.Draw2dText("(Actual Size)", fontSmall, panelX + 50, previewY + 60, null, false);
    }
    
    void DrawBottomButtons(GamePlatform p)
    {
        // Bottom action buttons are positioned by widget system
        // Just draw labels for context
        int bottomY = p.GetCanvasHeight() - 60;
        string helpText = "Use tools to paint, or load preset theme elements";
        p.Draw2dText(helpText, fontSmall, 20, bottomY, null, false);
    }
    
    void UpdateCanvasTexture(GamePlatform p)
    {
        // Delete old texture if exists
        if (canvasTextureId != -1)
        {
            p.GlDeleteTexture(canvasTextureId);
        }
        
        // Create bitmap from canvas
        BitmapCi bitmap = canvas.ExportToBitmap();
        
        // Create texture from bitmap
        canvasTextureId = p.LoadTextureFromBitmap(bitmap);
    }
    
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
        // First call base to handle widget clicks
        base.OnMouseDown(e);
        
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
        // First call base to handle widget hover
        base.OnMouseMove(e);
        
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
        // First call base to handle widget clicks
        base.OnMouseUp(e);
        
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
    */
}
