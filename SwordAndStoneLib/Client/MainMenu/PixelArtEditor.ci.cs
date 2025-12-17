// Pixel Art Editor Screen - Main UI for editing character skins
public class ScreenPixelArtEditor : Screen
{
	public ScreenPixelArtEditor()
	{
		canvas = new PixelArtCanvas();
		colorPicker = new ColorPicker();
		tools = new PixelArtTools();
		
		// Create widgets
		brushButton = CreateButtonPixelArt("Brush");
		eraserButton = CreateButtonPixelArt("Eraser");
		fillButton = CreateButtonPixelArt("Fill");
		pickerButton = CreateButtonPixelArt("Picker");
		
		layerBaseButton = CreateButtonPixelArt("Base Layer");
		layerOverlayButton = CreateButtonPixelArt("Overlay");
		
		brushSizeMinusButton = CreateButtonPixelArt("-");
		brushSizePlusButton = CreateButtonPixelArt("+");
		
		clearButton = CreateButtonPixelArt("Clear");
		loadTemplateButton = CreateButtonPixelArt("Load Template");
		saveButton = CreateButtonPixelArt("Save Skin");
		backButton = CreateButtonPixelArt("Back");
		
		genderMaleButton = CreateButtonPixelArt("Male");
		genderFemaleButton = CreateButtonPixelArt("Female");
		
		// Color component sliders (represented as buttons for simplicity)
		colorRMinusButton = CreateButtonPixelArt("-");
		colorRPlusButton = CreateButtonPixelArt("+");
		colorGMinusButton = CreateButtonPixelArt("-");
		colorGPlusButton = CreateButtonPixelArt("+");
		colorBMinusButton = CreateButtonPixelArt("-");
		colorBPlusButton = CreateButtonPixelArt("+");
		
		// Assign widgets to array
		int widgetIndex = 0;
		widgets[widgetIndex++] = brushButton;
		widgets[widgetIndex++] = eraserButton;
		widgets[widgetIndex++] = fillButton;
		widgets[widgetIndex++] = pickerButton;
		widgets[widgetIndex++] = layerBaseButton;
		widgets[widgetIndex++] = layerOverlayButton;
		widgets[widgetIndex++] = brushSizeMinusButton;
		widgets[widgetIndex++] = brushSizePlusButton;
		widgets[widgetIndex++] = clearButton;
		widgets[widgetIndex++] = loadTemplateButton;
		widgets[widgetIndex++] = saveButton;
		widgets[widgetIndex++] = backButton;
		widgets[widgetIndex++] = genderMaleButton;
		widgets[widgetIndex++] = genderFemaleButton;
		widgets[widgetIndex++] = colorRMinusButton;
		widgets[widgetIndex++] = colorRPlusButton;
		widgets[widgetIndex++] = colorGMinusButton;
		widgets[widgetIndex++] = colorGPlusButton;
		widgets[widgetIndex++] = colorBMinusButton;
		widgets[widgetIndex++] = colorBPlusButton;
		
		title = "Pixel Art Skin Editor";
		
		fontDefault = new FontCi();
		fontDefault.size = 14;
		
		fontTitle = new FontCi();
		fontTitle.size = 24;
		
		fontSmall = new FontCi();
		fontSmall.size = 12;
		
		isDrawing = false;
		canvasTextureId = -1;
		previewTextureId = -1;
		currentGender = 0; // 0 = male, 1 = female
		gridScale = 8.0f; // Pixels per grid cell
		canvasOffsetX = 50;
		canvasOffsetY = 100;
		
		needsTextureUpdate = true;
	}
	
	MenuWidget brushButton;
	MenuWidget eraserButton;
	MenuWidget fillButton;
	MenuWidget pickerButton;
	MenuWidget layerBaseButton;
	MenuWidget layerOverlayButton;
	MenuWidget brushSizeMinusButton;
	MenuWidget brushSizePlusButton;
	MenuWidget clearButton;
	MenuWidget loadTemplateButton;
	MenuWidget saveButton;
	MenuWidget backButton;
	MenuWidget genderMaleButton;
	MenuWidget genderFemaleButton;
	MenuWidget colorRMinusButton;
	MenuWidget colorRPlusButton;
	MenuWidget colorGMinusButton;
	MenuWidget colorGPlusButton;
	MenuWidget colorBMinusButton;
	MenuWidget colorBPlusButton;
	
	string title;
	FontCi fontDefault;
	FontCi fontTitle;
	FontCi fontSmall;
	
	internal PixelArtCanvas canvas;
	internal ColorPicker colorPicker;
	internal PixelArtTools tools;
	
	internal bool isDrawing;
	internal int canvasTextureId;
	internal int previewTextureId;
	internal int currentGender;
	internal float gridScale;
	internal float canvasOffsetX;
	internal float canvasOffsetY;
	internal bool needsTextureUpdate;
	
	MenuWidget CreateButtonPixelArt(string text)
	{
		MenuWidget widget = new MenuWidget();
		widget.text = text;
		widget.type = WidgetType.Button;
		return widget;
	}
	
	public void Initialize(GamePlatform p)
	{
		canvas.Initialize(p, 64, 32);
		colorPicker.SetPlatform(p);
		LoadDefaultTemplate();
	}
	
	void LoadDefaultTemplate()
	{
		// Load default player texture as template
		string defaultTexture = "mineplayer.png";
		byte[] textureData = menu.GetFile(defaultTexture);
		if (textureData != null)
		{
			canvas.LoadFromTexture(textureData, menu.p.ByteArrayLength(textureData));
		}
	}
	
	public override void LoadTranslations()
	{
		title = "Pixel Art Skin Editor";
		brushButton.text = "Brush";
		eraserButton.text = "Eraser";
		fillButton.text = "Fill";
		pickerButton.text = "Picker";
		layerBaseButton.text = "Base Layer";
		layerOverlayButton.text = "Overlay";
		clearButton.text = "Clear";
		loadTemplateButton.text = "Load Template";
		saveButton.text = "Save Skin";
		backButton.text = "Back";
		genderMaleButton.text = "Male";
		genderFemaleButton.text = "Female";
	}
	
	public override void Render(float dt)
	{
		GamePlatform p = menu.p;
		float scale = menu.GetScale();
		
		menu.DrawBackground();
		menu.DrawText(title, fontTitle, p.GetCanvasWidth() / 2, 20 * scale, TextAlign.Center, TextBaseline.Top);
		
		// Update canvas texture if needed
		if (needsTextureUpdate || canvas.isDirty)
		{
			UpdateCanvasTexture();
			needsTextureUpdate = false;
		}
		
		// Calculate layout
		float toolsPanelX = 20 * scale;
		float toolsPanelY = 80 * scale;
		float toolsPanelWidth = 200 * scale;
		
		float canvasAreaX = toolsPanelX + toolsPanelWidth + 20 * scale;
		float canvasAreaY = toolsPanelY;
		
		float previewX = canvasAreaX + (64 * gridScale * scale) + 40 * scale;
		float previewY = canvasAreaY;
		
		// Draw tools panel
		DrawToolsPanel(p, scale, toolsPanelX, toolsPanelY, toolsPanelWidth);
		
		// Draw canvas
		DrawCanvas(p, scale, canvasAreaX, canvasAreaY);
		
		// Draw character preview
		DrawCharacterPreview(p, scale, previewX, previewY);
		
		// Draw back button
		backButton.x = 20 * scale;
		backButton.y = p.GetCanvasHeight() - 80 * scale;
		backButton.sizex = 120 * scale;
		backButton.sizey = 50 * scale;
		
		// Draw save button
		saveButton.x = p.GetCanvasWidth() - 140 * scale;
		saveButton.y = p.GetCanvasHeight() - 80 * scale;
		saveButton.sizex = 120 * scale;
		saveButton.sizey = 50 * scale;
		
		DrawWidgets();
	}
	
	void DrawToolsPanel(GamePlatform p, float scale, float x, float y, float width)
	{
		// Calculate panel height
		float panelHeight = 600 * scale;
		
		// Draw panel background using golden UI theme
		// Note: We can't use GuiFrameRenderer.DrawFrame() here because it requires a Game object
		// and we're in MainMenu context. Manual rendering is necessary.
		string framePath = "data/local/gui/golden/frame_small.png";
		int frameTexture = menu.GetTexture(framePath);
		menu.Draw2dQuad(frameTexture, 
			p.FloatToInt(x - 8 * scale), 
			p.FloatToInt(y - 8 * scale),
			p.FloatToInt(width + 16 * scale),
			p.FloatToInt(panelHeight + 16 * scale));
		
		string panelPath = "data/local/gui/golden/panel_dark.png";
		int panelTexture = menu.GetTexture(panelPath);
		menu.Draw2dQuad(panelTexture, 
			p.FloatToInt(x), 
			p.FloatToInt(y),
			p.FloatToInt(width),
			p.FloatToInt(panelHeight));
		
		float buttonHeight = 40 * scale;
		float spacing = 10 * scale;
		float currentY = y + 10 * scale;
		
		// Tools section
		menu.DrawText("Tools:", fontDefault, x + width / 2, currentY, TextAlign.Center, TextBaseline.Top);
		currentY = currentY + 25 * scale;
		
		// Tool buttons (2x2 grid)
		float halfWidth = (width - spacing) / 2;
		brushButton.x = x;
		brushButton.y = currentY;
		brushButton.sizex = halfWidth;
		brushButton.sizey = buttonHeight;
		
		eraserButton.x = x + halfWidth + spacing;
		eraserButton.y = currentY;
		eraserButton.sizex = halfWidth;
		eraserButton.sizey = buttonHeight;
		
		currentY = currentY + buttonHeight + spacing;
		
		fillButton.x = x;
		fillButton.y = currentY;
		fillButton.sizex = halfWidth;
		fillButton.sizey = buttonHeight;
		
		pickerButton.x = x + halfWidth + spacing;
		pickerButton.y = currentY;
		pickerButton.sizex = halfWidth;
		pickerButton.sizey = buttonHeight;
		
		currentY = currentY + buttonHeight + spacing * 2;
		
		// Brush size
		menu.DrawText("Brush Size:", fontDefault, x + width / 2, currentY, TextAlign.Center, TextBaseline.Top);
		currentY = currentY + 25 * scale;
		
		brushSizeMinusButton.x = x;
		brushSizeMinusButton.y = currentY;
		brushSizeMinusButton.sizex = 50 * scale;
		brushSizeMinusButton.sizey = buttonHeight;
		
		menu.DrawText(p.IntToString(tools.GetBrushSize()), fontDefault, 
			x + width / 2, currentY + buttonHeight / 2, 
			TextAlign.Center, TextBaseline.Middle);
		
		brushSizePlusButton.x = x + width - 50 * scale;
		brushSizePlusButton.y = currentY;
		brushSizePlusButton.sizex = 50 * scale;
		brushSizePlusButton.sizey = buttonHeight;
		
		currentY = currentY + buttonHeight + spacing * 2;
		
		// Layer selection
		menu.DrawText("Layer:", fontDefault, x + width / 2, currentY, TextAlign.Center, TextBaseline.Top);
		currentY = currentY + 25 * scale;
		
		layerBaseButton.x = x;
		layerBaseButton.y = currentY;
		layerBaseButton.sizex = halfWidth;
		layerBaseButton.sizey = buttonHeight;
		
		layerOverlayButton.x = x + halfWidth + spacing;
		layerOverlayButton.y = currentY;
		layerOverlayButton.sizex = halfWidth;
		layerOverlayButton.sizey = buttonHeight;
		
		currentY = currentY + buttonHeight + spacing * 2;
		
		// Color picker section
		DrawColorPickerPanel(p, scale, x, currentY, width);
		currentY = currentY + 200 * scale;
		
		// Template buttons
		clearButton.x = x;
		clearButton.y = currentY;
		clearButton.sizex = width;
		clearButton.sizey = buttonHeight;
		
		currentY = currentY + buttonHeight + spacing;
		
		loadTemplateButton.x = x;
		loadTemplateButton.y = currentY;
		loadTemplateButton.sizex = width;
		loadTemplateButton.sizey = buttonHeight;
	}
	
	void DrawColorPickerPanel(GamePlatform p, float scale, float x, float y, float width)
	{
		menu.DrawText("Color:", fontDefault, x + width / 2, y, TextAlign.Center, TextBaseline.Top);
		y = y + 25 * scale;
		
		// Current color display
		int color = colorPicker.GetSelectedColor();
		int colorBoxSize = p.FloatToInt(40 * scale);
		
		// Draw colored rectangle (simulate with white texture tinted)
		// Note: This is a simplified approach - actual implementation would use colored quad
		menu.DrawText("█", fontDefault, x + width / 2, y + 20 * scale, TextAlign.Center, TextBaseline.Middle);
		
		y = y + 50 * scale;
		
		// RGB sliders (simplified as +/- buttons)
		int a = (color >> 24) & 0xFF;
		int r = (color >> 16) & 0xFF;
		int g = (color >> 8) & 0xFF;
		int b = color & 0xFF;
		
		// Red
		menu.DrawText("R:", fontSmall, x, y, TextAlign.Left, TextBaseline.Middle);
		colorRMinusButton.x = x + 30 * scale;
		colorRMinusButton.y = y - 15 * scale;
		colorRMinusButton.sizex = 30 * scale;
		colorRMinusButton.sizey = 30 * scale;
		
		menu.DrawText(p.IntToString(r), fontSmall, x + width / 2, y, TextAlign.Center, TextBaseline.Middle);
		
		colorRPlusButton.x = x + width - 30 * scale;
		colorRPlusButton.y = y - 15 * scale;
		colorRPlusButton.sizex = 30 * scale;
		colorRPlusButton.sizey = 30 * scale;
		
		y = y + 35 * scale;
		
		// Green
		menu.DrawText("G:", fontSmall, x, y, TextAlign.Left, TextBaseline.Middle);
		colorGMinusButton.x = x + 30 * scale;
		colorGMinusButton.y = y - 15 * scale;
		colorGMinusButton.sizex = 30 * scale;
		colorGMinusButton.sizey = 30 * scale;
		
		menu.DrawText(p.IntToString(g), fontSmall, x + width / 2, y, TextAlign.Center, TextBaseline.Middle);
		
		colorGPlusButton.x = x + width - 30 * scale;
		colorGPlusButton.y = y - 15 * scale;
		colorGPlusButton.sizex = 30 * scale;
		colorGPlusButton.sizey = 30 * scale;
		
		y = y + 35 * scale;
		
		// Blue
		menu.DrawText("B:", fontSmall, x, y, TextAlign.Left, TextBaseline.Middle);
		colorBMinusButton.x = x + 30 * scale;
		colorBMinusButton.y = y - 15 * scale;
		colorBMinusButton.sizex = 30 * scale;
		colorBMinusButton.sizey = 30 * scale;
		
		menu.DrawText(p.IntToString(b), fontSmall, x + width / 2, y, TextAlign.Center, TextBaseline.Middle);
		
		colorBPlusButton.x = x + width - 30 * scale;
		colorBPlusButton.y = y - 15 * scale;
		colorBPlusButton.sizex = 30 * scale;
		colorBPlusButton.sizey = 30 * scale;
	}
	
	void DrawCanvas(GamePlatform p, float scale, float x, float y)
	{
		// Draw canvas background
		float canvasWidth = canvas.width * gridScale * scale;
		float canvasHeight = canvas.height * gridScale * scale;
		
		// Draw frame
		string framePath = "data/local/gui/golden/frame_ornate.png";
		int frameTexture = menu.GetTexture(framePath);
		menu.Draw2dQuad(frameTexture, 
			p.FloatToInt(x - 8 * scale), 
			p.FloatToInt(y - 8 * scale),
			p.FloatToInt(canvasWidth + 16 * scale),
			p.FloatToInt(canvasHeight + 16 * scale));
		
		// Draw canvas texture
		if (canvasTextureId >= 0)
		{
			menu.Draw2dQuad(canvasTextureId, 
				p.FloatToInt(x), 
				p.FloatToInt(y),
				p.FloatToInt(canvasWidth),
				p.FloatToInt(canvasHeight));
		}
		
		// Draw grid overlay
		DrawGrid(p, scale, x, y, canvasWidth, canvasHeight);
	}
	
	void DrawGrid(GamePlatform p, float scale, float x, float y, float width, float height)
	{
		// Draw grid lines (simplified - would need line drawing in actual implementation)
		// This is a placeholder for grid visualization
	}
	
	void DrawCharacterPreview(GamePlatform p, float scale, float x, float y)
	{
		float previewWidth = 200 * scale;
		float previewHeight = 300 * scale;
		
		// Draw frame
		string framePath = "data/local/gui/golden/frame_ornate.png";
		int frameTexture = menu.GetTexture(framePath);
		menu.Draw2dQuad(frameTexture, 
			p.FloatToInt(x - 8 * scale), 
			p.FloatToInt(y - 8 * scale),
			p.FloatToInt(previewWidth + 16 * scale),
			p.FloatToInt(previewHeight + 16 * scale));
		
		// Draw dark background
		string panelPath = "data/local/gui/golden/panel_dark.png";
		int panelTexture = menu.GetTexture(panelPath);
		menu.Draw2dQuad(panelTexture, 
			p.FloatToInt(x), 
			p.FloatToInt(y),
			p.FloatToInt(previewWidth),
			p.FloatToInt(previewHeight));
		
		// Draw character preview texture
		if (previewTextureId >= 0)
		{
			float charWidth = 128 * scale;
			float charHeight = 256 * scale;
			float charX = x + (previewWidth - charWidth) / 2;
			float charY = y + (previewHeight - charHeight) / 2;
			
			menu.Draw2dQuad(previewTextureId, 
				p.FloatToInt(charX), 
				p.FloatToInt(charY),
				p.FloatToInt(charWidth),
				p.FloatToInt(charHeight));
		}
		
		// Gender selection buttons
		float buttonY = y + previewHeight + 20 * scale;
		float halfWidth = (previewWidth - 10 * scale) / 2;
		
		genderMaleButton.x = x;
		genderMaleButton.y = buttonY;
		genderMaleButton.sizex = halfWidth;
		genderMaleButton.sizey = 40 * scale;
		
		genderFemaleButton.x = x + halfWidth + 10 * scale;
		genderFemaleButton.y = buttonY;
		genderFemaleButton.sizex = halfWidth;
		genderFemaleButton.sizey = 40 * scale;
		
		menu.DrawText("3D Preview", fontDefault, x + previewWidth / 2, 
			buttonY + 60 * scale, TextAlign.Center, TextBaseline.Top);
	}
	
	void UpdateCanvasTexture()
	{
		// Export canvas to bitmap and create texture
		BitmapCi bitmap = canvas.ExportToBitmap(true); // Include overlay
		if (bitmap != null)
		{
			if (canvasTextureId >= 0)
			{
				// Delete old texture
				menu.p.GLDeleteTexture(canvasTextureId);
			}
			canvasTextureId = menu.p.LoadTextureFromBitmap(bitmap);
			menu.p.BitmapDelete(bitmap);
			
			// Also update preview texture
			previewTextureId = canvasTextureId;
			
			canvas.isDirty = false;
		}
	}
	
	public override void OnBackPressed()
	{
		menu.StartMainMenu();
	}
	
	public override void OnButton(MenuWidget w)
	{
		if (w == brushButton)
		{
			tools.SetCurrentTool(PixelArtToolType.Brush);
		}
		else if (w == eraserButton)
		{
			tools.SetCurrentTool(PixelArtToolType.Eraser);
		}
		else if (w == fillButton)
		{
			tools.SetCurrentTool(PixelArtToolType.FillBucket);
		}
		else if (w == pickerButton)
		{
			tools.SetCurrentTool(PixelArtToolType.ColorPicker);
		}
		else if (w == layerBaseButton)
		{
			canvas.currentLayer = 0;
		}
		else if (w == layerOverlayButton)
		{
			canvas.currentLayer = 1;
		}
		else if (w == brushSizeMinusButton)
		{
			tools.SetBrushSize(tools.GetBrushSize() - 1);
		}
		else if (w == brushSizePlusButton)
		{
			tools.SetBrushSize(tools.GetBrushSize() + 1);
		}
		else if (w == clearButton)
		{
			canvas.Clear(ColorFromArgb(0, 255, 255, 255));
			needsTextureUpdate = true;
		}
		else if (w == loadTemplateButton)
		{
			LoadDefaultTemplate();
			needsTextureUpdate = true;
		}
		else if (w == saveButton)
		{
			SaveSkin();
		}
		else if (w == backButton)
		{
			OnBackPressed();
		}
		else if (w == genderMaleButton)
		{
			currentGender = 0;
		}
		else if (w == genderFemaleButton)
		{
			currentGender = 1;
		}
		else if (w == colorRMinusButton)
		{
			AdjustColor(0, -10);
		}
		else if (w == colorRPlusButton)
		{
			AdjustColor(0, 10);
		}
		else if (w == colorGMinusButton)
		{
			AdjustColor(1, -10);
		}
		else if (w == colorGPlusButton)
		{
			AdjustColor(1, 10);
		}
		else if (w == colorBMinusButton)
		{
			AdjustColor(2, -10);
		}
		else if (w == colorBPlusButton)
		{
			AdjustColor(2, 10);
		}
	}
	
	void AdjustColor(int component, int delta)
	{
		int color = colorPicker.GetSelectedColor();
		int a = (color >> 24) & 0xFF;
		int r = (color >> 16) & 0xFF;
		int g = (color >> 8) & 0xFF;
		int b = color & 0xFF;
		
		if (component == 0) // Red
		{
			r = r + delta;
			if (r < 0) { r = 0; }
			if (r > 255) { r = 255; }
		}
		else if (component == 1) // Green
		{
			g = g + delta;
			if (g < 0) { g = 0; }
			if (g > 255) { g = 255; }
		}
		else if (component == 2) // Blue
		{
			b = b + delta;
			if (b < 0) { b = 0; }
			if (b > 255) { b = 255; }
		}
		
		colorPicker.SetColorRgb(r, g, b);
	}
	
	void SaveSkin()
	{
		// Export canvas and save to file
		// In actual implementation, would save PNG to user data folder
		// For now, just show that it's saved
		BitmapCi bitmap = canvas.ExportToBitmap(true);
		if (bitmap != null)
		{
			// TODO: Save bitmap to file
			// string filename = "custom_skin_" + menu.p.Timestamp() + ".png";
			// SaveBitmapToFile(bitmap, filename);
			
			menu.p.BitmapDelete(bitmap);
		}
	}
	
	public override void OnMouseDown(MouseEventArgs e)
	{
		// Check if clicking on canvas
		GamePlatform p = menu.p;
		float scale = menu.GetScale();
		
		float canvasAreaX = 20 * scale + 200 * scale + 20 * scale;
		float canvasAreaY = 80 * scale;
		float canvasWidth = canvas.width * gridScale * scale;
		float canvasHeight = canvas.height * gridScale * scale;
		
		float mouseX = e.GetX();
		float mouseY = e.GetY();
		
		if (mouseX >= canvasAreaX && mouseX < canvasAreaX + canvasWidth &&
		    mouseY >= canvasAreaY && mouseY < canvasAreaY + canvasHeight)
		{
			isDrawing = true;
			ApplyToolAtMouse(mouseX, mouseY, canvasAreaX, canvasAreaY);
		}
	}
	
	public override void OnMouseUp(MouseEventArgs e)
	{
		isDrawing = false;
	}
	
	public override void OnMouseMove(MouseEventArgs e)
	{
		if (isDrawing)
		{
			GamePlatform p = menu.p;
			float scale = menu.GetScale();
			
			float canvasAreaX = 20 * scale + 200 * scale + 20 * scale;
			float canvasAreaY = 80 * scale;
			
			ApplyToolAtMouse(e.GetX(), e.GetY(), canvasAreaX, canvasAreaY);
		}
	}
	
	void ApplyToolAtMouse(float mouseX, float mouseY, float canvasX, float canvasY)
	{
		GamePlatform p = menu.p;
		float scale = menu.GetScale();
		
		// Convert mouse position to canvas pixel coordinates
		float relativeX = (mouseX - canvasX) / (gridScale * scale);
		float relativeY = (mouseY - canvasY) / (gridScale * scale);
		
		int pixelX = p.FloatToInt(relativeX);
		int pixelY = p.FloatToInt(relativeY);
		
		// Apply tool
		if (tools.GetCurrentTool() == PixelArtToolType.ColorPicker)
		{
			int pickedColor = tools.PickColor(canvas, pixelX, pixelY);
			colorPicker.SetSelectedColor(pickedColor);
		}
		else
		{
			tools.ApplyTool(canvas, pixelX, pixelY, colorPicker.GetSelectedColor());
			needsTextureUpdate = true;
		}
	}
	
	// Helper: Create ARGB color
	static int ColorFromArgb(int a, int r, int g, int b)
	{
		return (a << 24) | (r << 16) | (g << 8) | b;
	}
}
