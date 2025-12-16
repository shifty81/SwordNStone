public class ScreenCharacterCreator : Screen
{
	public ScreenCharacterCreator()
	{
		genderLeft = new MenuWidget();
		genderLeft.text = "<";
		genderLeft.type = WidgetType.Button;
		
		genderRight = new MenuWidget();
		genderRight.text = ">";
		genderRight.type = WidgetType.Button;
		
		hairstyleLeft = new MenuWidget();
		hairstyleLeft.text = "<";
		hairstyleLeft.type = WidgetType.Button;
		
		hairstyleRight = new MenuWidget();
		hairstyleRight.text = ">";
		hairstyleRight.type = WidgetType.Button;
		
		beardLeft = new MenuWidget();
		beardLeft.text = "<";
		beardLeft.type = WidgetType.Button;
		
		beardRight = new MenuWidget();
		beardRight.text = ">";
		beardRight.type = WidgetType.Button;
		
		outfitLeft = new MenuWidget();
		outfitLeft.text = "<";
		outfitLeft.type = WidgetType.Button;
		
		outfitRight = new MenuWidget();
		outfitRight.text = ">";
		outfitRight.type = WidgetType.Button;
		
		confirmButton = new MenuWidget();
		confirmButton.text = "Confirm";
		confirmButton.type = WidgetType.Button;
		
		skinEditorButton = new MenuWidget();
		skinEditorButton.text = "Skin Editor";
		skinEditorButton.type = WidgetType.Button;
		
		backButton = new MenuWidget();
		backButton.text = "Back";
		backButton.type = WidgetType.Button;
		
		widgets[0] = genderLeft;
		widgets[1] = genderRight;
		widgets[2] = hairstyleLeft;
		widgets[3] = hairstyleRight;
		widgets[4] = beardLeft;
		widgets[5] = beardRight;
		widgets[6] = outfitLeft;
		widgets[7] = outfitRight;
		widgets[8] = confirmButton;
		widgets[9] = skinEditorButton;
		widgets[10] = backButton;
		
		title = "Character Creator";
		
		fontDefault = new FontCi();
		fontDefault.size = 16;
		
		fontTitle = new FontCi();
		fontTitle.size = 24;
		
		customization = new CharacterCustomization();
		returnToSingleplayer = false;
	}
	
	// Initialize platform reference when menu is set
	internal void InitializePlatform(GamePlatform p)
	{
		if (customization != null && customization.platform == null)
		{
			customization.platform = p;
		}
	}
	
	MenuWidget genderLeft;
	MenuWidget genderRight;
	MenuWidget hairstyleLeft;
	MenuWidget hairstyleRight;
	MenuWidget beardLeft;
	MenuWidget beardRight;
	MenuWidget outfitLeft;
	MenuWidget outfitRight;
	MenuWidget confirmButton;
	MenuWidget skinEditorButton;
	MenuWidget backButton;
	
	string title;
	FontCi fontDefault;
	FontCi fontTitle;
	
	internal CharacterCustomization customization;
	internal bool returnToSingleplayer;
	internal string worldPath; // Path to world being created
	
	public override void LoadTranslations()
	{
		title = "Character Creator";
		confirmButton.text = "Confirm";
		skinEditorButton.text = "Skin Editor";
		backButton.text = "Back";
	}
	
	public override void Render(float dt)
	{
		GamePlatform p = menu.p;
		InitializePlatform(p);
		float scale = menu.GetScale();
		
		menu.DrawBackground();
		menu.DrawText(title, fontTitle, p.GetCanvasWidth() / 2, 30 * scale, TextAlign.Center, TextBaseline.Top);
		
		// Calculate layout - move options to the left to make room for character preview
		float optionsX = p.GetCanvasWidth() / 3;
		float previewX = (p.GetCanvasWidth() / 3) * 2;
		float startY = 120 * scale;
		float rowHeight = 80 * scale;
		float buttonWidth = 50 * scale;
		float buttonHeight = 40 * scale;
		float labelWidth = 200 * scale;
		
		// Gender selection
		DrawCustomizationRow(p, scale, "Gender:", GetGenderName(), 
			optionsX, startY, 
			genderLeft, genderRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Hairstyle selection
		DrawCustomizationRow(p, scale, "Hairstyle:", GetHairstyleName(), 
			optionsX, startY + rowHeight, 
			hairstyleLeft, hairstyleRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Beard selection
		DrawCustomizationRow(p, scale, "Beard:", GetBeardName(), 
			optionsX, startY + rowHeight * 2, 
			beardLeft, beardRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Outfit selection
		DrawCustomizationRow(p, scale, "Outfit:", GetOutfitName(), 
			optionsX, startY + rowHeight * 3, 
			outfitLeft, outfitRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Draw character preview on the right side
		DrawCharacterPreview(p, scale, previewX, startY);
		
		// Confirm button
		confirmButton.x = p.GetCanvasWidth() / 2 - 128 * scale;
		confirmButton.y = p.GetCanvasHeight() - 150 * scale;
		confirmButton.sizex = 256 * scale;
		confirmButton.sizey = 64 * scale;
		
		// Skin Editor button
		skinEditorButton.x = p.GetCanvasWidth() - 260 * scale;
		skinEditorButton.y = p.GetCanvasHeight() - 104 * scale;
		skinEditorButton.sizex = 220 * scale;
		skinEditorButton.sizey = 64 * scale;
		
		// Back button
		backButton.x = 40 * scale;
		backButton.y = p.GetCanvasHeight() - 104 * scale;
		backButton.sizex = 200 * scale;
		backButton.sizey = 64 * scale;
		
		DrawWidgets();
	}
	
	void DrawCustomizationRow(GamePlatform p, float scale, string label, string value, 
		float centerX, float y, 
		MenuWidget leftButton, MenuWidget rightButton,
		float buttonWidth, float buttonHeight, float labelWidth)
	{
		// Draw label
		menu.DrawText(label, fontDefault, centerX - labelWidth / 2 - 20 * scale, y + buttonHeight / 2, 
			TextAlign.Right, TextBaseline.Middle);
		
		// Left button
		leftButton.x = centerX - labelWidth / 2 + 10 * scale;
		leftButton.y = y;
		leftButton.sizex = buttonWidth;
		leftButton.sizey = buttonHeight;
		
		// Value text
		menu.DrawText(value, fontDefault, centerX, y + buttonHeight / 2, 
			TextAlign.Center, TextBaseline.Middle);
		
		// Right button
		rightButton.x = centerX + labelWidth / 2 - 10 * scale - buttonWidth;
		rightButton.y = y;
		rightButton.sizex = buttonWidth;
		rightButton.sizey = buttonHeight;
	}
	
	void DrawCharacterPreview(GamePlatform p, float scale, float x, float y)
	{
		// Draw a golden frame around the character preview
		int previewWidth = p.FloatToInt(256 * scale);
		int previewHeight = p.FloatToInt(384 * scale);
		int previewX = p.FloatToInt(x - (256 * scale) / 2);
		int previewY = p.FloatToInt(y);
		
		// Draw frame border using golden frame texture
		string framePath = "data/local/gui/golden/frame_ornate.png";
		int frameTexture = menu.GetTexture(framePath);
		menu.Draw2dQuad(frameTexture, 
			previewX - p.FloatToInt(16 * scale), 
			previewY - p.FloatToInt(16 * scale),
			previewWidth + p.FloatToInt(32 * scale),
			previewHeight + p.FloatToInt(32 * scale));
		
		// Draw dark background panel
		string panelPath = "data/local/gui/golden/panel_dark.png";
		int panelTexture = menu.GetTexture(panelPath);
		menu.Draw2dQuad(panelTexture, previewX, previewY, previewWidth, previewHeight);
		
		// Draw character texture preview
		string textureName = customization.GetTextureName();
		int textureId = menu.GetTexture(textureName);
		
		// Draw the character skin centered in the preview area
		// Use a larger scale to make the character visible
		int charWidth = p.FloatToInt(128 * scale);
		int charHeight = p.FloatToInt(256 * scale);
		int charX = previewX + (previewWidth - charWidth) / 2;
		int charY = previewY + (previewHeight - charHeight) / 2;
		
		menu.Draw2dQuad(textureId, charX, charY, charWidth, charHeight);
		
		// Add a label below
		menu.DrawText("Character Preview", fontDefault, x, y + previewHeight + 20 * scale, 
			TextAlign.Center, TextBaseline.Top);
	}
	
	string GetGenderName()
	{
		if (customization.Gender == 0)
		{
			return "Male";
		}
		else
		{
			return "Female";
		}
	}
	
	string GetHairstyleName()
	{
		if (customization.Hairstyle == 0) { return "Short"; }
		if (customization.Hairstyle == 1) { return "Medium"; }
		if (customization.Hairstyle == 2) { return "Long"; }
		if (customization.Hairstyle == 3) { return "Bald"; }
		if (customization.Hairstyle == 4) { return "Ponytail"; }
		return "Short";
	}
	
	string GetBeardName()
	{
		if (customization.Beard == 0) { return "None"; }
		if (customization.Beard == 1) { return "Short"; }
		if (customization.Beard == 2) { return "Long"; }
		if (customization.Beard == 3) { return "Goatee"; }
		return "None";
	}
	
	string GetOutfitName()
	{
		if (customization.Outfit == 0) { return "Default"; }
		if (customization.Outfit == 1) { return "Armor"; }
		if (customization.Outfit == 2) { return "Robe"; }
		if (customization.Outfit == 3) { return "Casual"; }
		return "Default";
	}
	
	public override void OnBackPressed()
	{
		if (returnToSingleplayer)
		{
			menu.StartSingleplayer();
		}
		else
		{
			menu.StartMainMenu();
		}
	}
	
	public override void OnButton(MenuWidget w)
	{
		if (w == genderLeft)
		{
			customization.Gender--;
			if (customization.Gender < 0)
			{
				customization.Gender = CharacterCustomization.GetGenderCount() - 1;
			}
		}
		else if (w == genderRight)
		{
			customization.Gender++;
			if (customization.Gender >= CharacterCustomization.GetGenderCount())
			{
				customization.Gender = 0;
			}
		}
		else if (w == hairstyleLeft)
		{
			customization.Hairstyle--;
			if (customization.Hairstyle < 0)
			{
				customization.Hairstyle = CharacterCustomization.GetHairstyleCount() - 1;
			}
		}
		else if (w == hairstyleRight)
		{
			customization.Hairstyle++;
			if (customization.Hairstyle >= CharacterCustomization.GetHairstyleCount())
			{
				customization.Hairstyle = 0;
			}
		}
		else if (w == beardLeft)
		{
			customization.Beard--;
			if (customization.Beard < 0)
			{
				customization.Beard = CharacterCustomization.GetBeardCount() - 1;
			}
		}
		else if (w == beardRight)
		{
			customization.Beard++;
			if (customization.Beard >= CharacterCustomization.GetBeardCount())
			{
				customization.Beard = 0;
			}
		}
		else if (w == outfitLeft)
		{
			customization.Outfit--;
			if (customization.Outfit < 0)
			{
				customization.Outfit = CharacterCustomization.GetOutfitCount() - 1;
			}
		}
		else if (w == outfitRight)
		{
			customization.Outfit++;
			if (customization.Outfit >= CharacterCustomization.GetOutfitCount())
			{
				customization.Outfit = 0;
			}
		}
		else if (w == confirmButton)
		{
			// Save customization to preferences
			SaveCustomization();
			
			// If we have a world path, start the game
			if (worldPath != null)
			{
				menu.ConnectToSingleplayer(worldPath);
			}
			else if (returnToSingleplayer)
			{
				menu.StartSingleplayer();
			}
			else
			{
				menu.StartMainMenu();
			}
		}
		else if (w == skinEditorButton)
		{
			// Open the Pixel Art Skin Editor
			menu.StartPixelArtEditor();
		}
		else if (w == backButton)
		{
			OnBackPressed();
		}
	}
	
	void SaveCustomization()
	{
		// Save to preferences
		string data = customization.Serialize();
		Preferences prefs = menu.p.GetPreferences();
		prefs.SetString("CharacterCustomization", data);
		menu.p.SetPreferences(prefs);
	}
	
	public void LoadCustomization()
	{
		// Load from preferences
		Preferences prefs = menu.p.GetPreferences();
		string data = prefs.GetString("CharacterCustomization", null);
		if (data != null)
		{
			customization = CharacterCustomization.Deserialize(menu.p, data);
		}
		else
		{
			// Initialize with defaults if no saved data
			customization = new CharacterCustomization();
		}
		customization.platform = menu.p;
	}
}
