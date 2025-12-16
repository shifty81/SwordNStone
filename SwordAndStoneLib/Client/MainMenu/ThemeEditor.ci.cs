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
        brushButton = CreateButton("Brush");
        eraserButton = CreateButton("Eraser");
        fillButton = CreateButton("Fill");
        pickerButton = CreateButton("Picker");
        
        brushSizeMinusButton = CreateButton("-");
        brushSizePlusButton = CreateButton("+");
        
        // Asset type selection buttons
        buttonAssetButton = CreateButton("Button");
        frameAssetButton = CreateButton("Frame");
        barAssetButton = CreateButton("Bar");
        slotAssetButton = CreateButton("Slot");
        
        // Asset state buttons (for buttons/slots)
        stateNormalButton = CreateButton("Normal");
        stateHoverButton = CreateButton("Hover");
        statePressedButton = CreateButton("Pressed");
        
        // Theme color preset buttons
        colorPrimaryButton = CreateButton("Primary");
        colorSecondaryButton = CreateButton("Secondary");
        colorAccentButton = CreateButton("Accent");
        
        // Utility buttons
        clearButton = CreateButton("Clear");
        loadDefaultButton = CreateButton("Load Default");
        saveButton = CreateButton("Save Theme");
        exportButton = CreateButton("Export Asset");
        backButton = CreateButton("Back");
        
        // Theme selection
        themeListButton = CreateButton("Theme List");
        newThemeButton = CreateButton("New Theme");
        
        // Color component sliders
        colorRMinusButton = CreateButton("-");
        colorRPlusButton = CreateButton("+");
        colorGMinusButton = CreateButton("-");
        colorGPlusButton = CreateButton("+");
        colorBMinusButton = CreateButton("-");
        colorBPlusButton = CreateButton("+");
        
        // Gradient tools
        gradientHorizButton = CreateButton("H Gradient");
        gradientVertButton = CreateButton("V Gradient");
        borderButton = CreateButton("Add Border");
        
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
        gridScale = 8.0f; // Pixels per grid cell
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
    
    internal float one;  // For compatibility with fixed-point arithmetic
    internal bool isDrawing;
    internal int canvasTextureId;
    internal float gridScale;
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
    
    public override void Render(Game game, float deltaTime)
    {
        // Background
        int bgColor = Game.ColorFromArgb(255, 30, 30, 40);
        game.Draw2dTexture(game.WhiteTexture(), 0, 0, game.Width(), game.Height(), null, 0, bgColor, false);
        
        // Title
        int titleColor = Game.ColorFromArgb(255, 255, 215, 0);
        game.Draw2dText(title, fontTitle, 20, 20, null, false);
        
        // Current theme info
        string themeInfo = game.platform.StringFormat("Theme: {0} by {1}", currentThemeName, currentThemeAuthor);
        game.Draw2dText(themeInfo, fontSmall, 20, 60, null, false);
        
        // Draw canvas with grid
        DrawCanvas(game);
        
        // Draw tools panel (left side)
        DrawToolsPanel(game);
        
        // Draw color picker panel (left side, below tools)
        DrawColorPickerPanel(game);
        
        // Draw asset selection panel (right side)
        DrawAssetPanel(game);
        
        // Draw preview panel (right side, below asset selection)
        DrawPreviewPanel(game);
        
        // Draw bottom buttons
        DrawBottomButtons(game);
        
        // Update texture if needed
        if (needsTextureUpdate)
        {
            UpdateCanvasTexture(game);
            needsTextureUpdate = false;
        }
    }
    
    void DrawCanvas(Game game)
    {
        int canvasWidth = canvas.width;
        int canvasHeight = canvas.height;
        int scaledWidth = one * canvasWidth * gridScale;
        int scaledHeight = one * canvasHeight * gridScale;
        
        // Canvas background (dark gray)
        int canvasBg = Game.ColorFromArgb(255, 50, 50, 60);
        game.Draw2dTexture(game.WhiteTexture(), canvasOffsetX - 5, canvasOffsetY - 5, 
            scaledWidth + 10, scaledHeight + 10, null, 0, canvasBg, false);
        
        // Draw canvas texture
        if (canvasTextureId != -1)
        {
            game.Draw2dTexture(canvasTextureId, canvasOffsetX, canvasOffsetY, 
                scaledWidth, scaledHeight, null, 0, Game.ColorFromArgb(255, 255, 255, 255), false);
        }
        
        // Draw grid
        int gridColor = Game.ColorFromArgb(100, 200, 200, 200);
        for (int x = 0; x <= canvasWidth; x++)
        {
            int xPos = canvasOffsetX + one * x * gridScale;
            game.Draw2dTexture(game.WhiteTexture(), xPos, canvasOffsetY, 1, scaledHeight, null, 0, gridColor, false);
        }
        for (int y = 0; y <= canvasHeight; y++)
        {
            int yPos = canvasOffsetY + one * y * gridScale;
            game.Draw2dTexture(game.WhiteTexture(), canvasOffsetX, yPos, scaledWidth, 1, null, 0, gridColor, false);
        }
        
        // Canvas label
        string canvasLabel = GetAssetTypeLabel();
        game.Draw2dText(canvasLabel, fontDefault, canvasOffsetX, canvasOffsetY - 30, null, false);
    }
    
    void DrawToolsPanel(Game game)
    {
        int panelX = 20;
        int panelY = 100;
        int panelWidth = 180;
        int panelHeight = 300;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        game.Draw2dTexture(game.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        game.Draw2dText("Tools", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Position buttons (simplified layout)
        int btnY = panelY + 40;
        int btnSpacing = 35;
        
        // Draw tool buttons (will be positioned by widget system)
        // Tool selection indicators
        string currentTool = GetCurrentToolName();
        game.Draw2dText(game.platform.StringFormat("Current: {0}", currentTool), fontSmall, panelX + 10, btnY, null, false);
        
        btnY += btnSpacing;
        game.Draw2dText(game.platform.StringFormat("Brush Size: {0}", tools.GetBrushSize()), fontSmall, panelX + 10, btnY, null, false);
    }
    
    void DrawColorPickerPanel(Game game)
    {
        int panelX = 20;
        int panelY = 420;
        int panelWidth = 180;
        int panelHeight = 200;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        game.Draw2dTexture(game.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        game.Draw2dText("Colors", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Current color preview (large square)
        int currentColor = colorPicker.GetCurrentColor();
        int colorPreviewSize = 60;
        int colorPreviewX = panelX + (panelWidth - colorPreviewSize) / 2;
        int colorPreviewY = panelY + 40;
        game.Draw2dTexture(game.WhiteTexture(), colorPreviewX, colorPreviewY, 
            colorPreviewSize, colorPreviewSize, null, 0, currentColor, false);
        
        // RGB values
        int r = colorPicker.GetRed();
        int g = colorPicker.GetGreen();
        int b = colorPicker.GetBlue();
        
        int rgbY = colorPreviewY + colorPreviewSize + 10;
        game.Draw2dText(game.platform.StringFormat("R: {0}", r), fontSmall, panelX + 10, rgbY, null, false);
        game.Draw2dText(game.platform.StringFormat("G: {0}", g), fontSmall, panelX + 10, rgbY + 20, null, false);
        game.Draw2dText(game.platform.StringFormat("B: {0}", b), fontSmall, panelX + 10, rgbY + 40, null, false);
    }
    
    void DrawAssetPanel(Game game)
    {
        int panelX = game.Width() - 220;
        int panelY = 100;
        int panelWidth = 200;
        int panelHeight = 300;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        game.Draw2dTexture(game.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        game.Draw2dText("Asset Type", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Asset type info
        string assetInfo = GetAssetTypeLabel();
        game.Draw2dText(assetInfo, fontSmall, panelX + 10, panelY + 40, null, false);
        
        // State info (if applicable)
        if (currentAssetType == ThemeCanvas.CANVAS_TYPE_BUTTON || currentAssetType == ThemeCanvas.CANVAS_TYPE_SLOT)
        {
            string stateLabel = GetStateLabel();
            game.Draw2dText(game.platform.StringFormat("State: {0}", stateLabel), fontSmall, panelX + 10, panelY + 65, null, false);
        }
        
        // Canvas size info
        game.Draw2dText(game.platform.StringFormat("Size: {0}x{1}", canvas.width, canvas.height), 
            fontSmall, panelX + 10, panelY + 90, null, false);
    }
    
    void DrawPreviewPanel(Game game)
    {
        int panelX = game.Width() - 220;
        int panelY = 420;
        int panelWidth = 200;
        int panelHeight = 200;
        
        // Panel background
        int panelBg = Game.ColorFromArgb(200, 40, 40, 50);
        game.Draw2dTexture(game.WhiteTexture(), panelX, panelY, panelWidth, panelHeight, null, 0, panelBg, false);
        
        // Title
        game.Draw2dText("Preview", fontDefault, panelX + 10, panelY + 10, null, false);
        
        // Draw preview of current asset in context
        int previewX = panelX + (panelWidth - 100) / 2;
        int previewY = panelY + 60;
        
        // Show the asset as it would appear in UI
        if (canvasTextureId != -1)
        {
            game.Draw2dTexture(canvasTextureId, previewX, previewY, 100, 50, null, 0, 
                Game.ColorFromArgb(255, 255, 255, 255), false);
        }
        
        game.Draw2dText("(Actual Size)", fontSmall, panelX + 50, previewY + 60, null, false);
    }
    
    void DrawBottomButtons(Game game)
    {
        // Bottom action buttons are positioned by widget system
        // Just draw labels for context
        int bottomY = game.Height() - 60;
        string helpText = "Use tools to paint, or load preset theme elements";
        game.Draw2dText(helpText, fontSmall, 20, bottomY, null, false);
    }
    
    void UpdateCanvasTexture(Game game)
    {
        // Delete old texture if exists
        if (canvasTextureId != -1)
        {
            game.platform.GlDeleteTexture(canvasTextureId);
        }
        
        // Create bitmap from canvas
        BitmapCi bitmap = canvas.ExportToBitmap();
        
        // Create texture from bitmap
        canvasTextureId = game.LoadTextureFromBitmap(bitmap);
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
        int tool = tools.GetCurrentTool();
        switch (tool)
        {
            case 0: return "Brush";
            case 1: return "Eraser";
            case 2: return "Fill Bucket";
            case 3: return "Color Picker";
            default: return "Unknown";
        }
    }
    
    public override void OnMouseDown(Game game, int x, int y, MouseButton button)
    {
        if (button != MouseButton.Left)
        {
            return;
        }
        
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
            ApplyTool(game, canvasX, canvasY);
        }
    }
    
    public override void OnMouseMove(Game game, int x, int y)
    {
        if (!isDrawing)
        {
            return;
        }
        
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
            ApplyTool(game, canvasX, canvasY);
        }
    }
    
    public override void OnMouseUp(Game game, int x, int y, MouseButton button)
    {
        if (button != MouseButton.Left)
        {
            return;
        }
        
        isDrawing = false;
    }
    
    void ApplyTool(Game game, int canvasX, int canvasY)
    {
        int tool = tools.GetCurrentTool();
        int color = colorPicker.GetCurrentColor();
        
        tools.ApplyToolToThemeCanvas(canvas, canvasX, canvasY, color);
        needsTextureUpdate = true;
    }
    
    public override void OnButtonClick(Game game, MenuWidget widget)
    {
        // Tool selection
        if (widget == brushButton) { tools.SetCurrentTool(0); }
        if (widget == eraserButton) { tools.SetCurrentTool(1); }
        if (widget == fillButton) { tools.SetCurrentTool(2); }
        if (widget == pickerButton) { tools.SetCurrentTool(3); }
        
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
        if (widget == colorPrimaryButton) { LoadPrimaryColor(game); }
        if (widget == colorSecondaryButton) { LoadSecondaryColor(game); }
        if (widget == colorAccentButton) { LoadAccentColor(game); }
        
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
            LoadDefaultAsset(game);
            needsTextureUpdate = true;
        }
        
        if (widget == gradientHorizButton)
        {
            ApplyGradient(game, true);
            needsTextureUpdate = true;
        }
        
        if (widget == gradientVertButton)
        {
            ApplyGradient(game, false);
            needsTextureUpdate = true;
        }
        
        if (widget == borderButton)
        {
            ApplyBorder(game);
            needsTextureUpdate = true;
        }
        
        if (widget == saveButton)
        {
            SaveTheme(game);
        }
        
        if (widget == exportButton)
        {
            ExportAsset(game);
        }
        
        if (widget == backButton)
        {
            game.SetScreen(new ScreenMainMenu());
        }
    }
    
    void LoadPrimaryColor(Game game)
    {
        // Load primary color from current theme (default: bronze/orange)
        colorPicker.SetColor(160, 100, 40);
    }
    
    void LoadSecondaryColor(Game game)
    {
        // Load secondary color from current theme (default: dark bronze)
        colorPicker.SetColor(100, 80, 60);
    }
    
    void LoadAccentColor(Game game)
    {
        // Load accent color from current theme (default: golden)
        colorPicker.SetColor(255, 215, 0);
    }
    
    void ApplyGradient(Game game, bool horizontal)
    {
        int startColor = colorPicker.GetCurrentColor();
        // Create a darker end color
        int endR = Game.ColorR(startColor) / 2;
        int endG = Game.ColorG(startColor) / 2;
        int endB = Game.ColorB(startColor) / 2;
        int endColor = Game.ColorFromArgb(255, endR, endG, endB);
        
        canvas.DrawGradient(startColor, endColor, horizontal);
    }
    
    void ApplyBorder(Game game)
    {
        int borderColor = colorPicker.GetCurrentColor();
        canvas.DrawBorder(borderColor, 2);
    }
    
    void LoadDefaultAsset(Game game)
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
    
    void SaveTheme(Game game)
    {
        // Save current theme configuration
        // This would write theme.txt and assets to disk
        // For now, just show a message
        game.ShowChatMessage("Theme saved! (Feature in progress)");
    }
    
    void ExportAsset(Game game)
    {
        // Export current canvas as PNG file
        // This would save to the appropriate theme directory
        game.ShowChatMessage("Asset exported! (Feature in progress)");
    }
}
