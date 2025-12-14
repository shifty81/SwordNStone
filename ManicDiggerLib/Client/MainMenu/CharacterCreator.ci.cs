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
		widgets[9] = backButton;
		
		title = "Character Creator";
		
		fontDefault = new FontCi();
		fontDefault.size = 16;
		
		fontTitle = new FontCi();
		fontTitle.size = 24;
		
		customization = new CharacterCustomization();
		returnToSingleplayer = false;
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
		backButton.text = "Back";
	}
	
	public override void Render(float dt)
	{
		GamePlatform p = menu.p;
		float scale = menu.GetScale();
		
		menu.DrawBackground();
		menu.DrawText(title, fontTitle, p.GetCanvasWidth() / 2, 30 * scale, TextAlign.Center, TextBaseline.Top);
		
		float centerX = p.GetCanvasWidth() / 2;
		float startY = 120 * scale;
		float rowHeight = 80 * scale;
		float buttonWidth = 50 * scale;
		float buttonHeight = 40 * scale;
		float labelWidth = 200 * scale;
		
		// Gender selection
		DrawCustomizationRow(p, scale, "Gender:", GetGenderName(), 
			centerX, startY, 
			genderLeft, genderRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Hairstyle selection
		DrawCustomizationRow(p, scale, "Hairstyle:", GetHairstyleName(), 
			centerX, startY + rowHeight, 
			hairstyleLeft, hairstyleRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Beard selection
		DrawCustomizationRow(p, scale, "Beard:", GetBeardName(), 
			centerX, startY + rowHeight * 2, 
			beardLeft, beardRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Outfit selection
		DrawCustomizationRow(p, scale, "Outfit:", GetOutfitName(), 
			centerX, startY + rowHeight * 3, 
			outfitLeft, outfitRight, 
			buttonWidth, buttonHeight, labelWidth);
		
		// Confirm button
		confirmButton.x = centerX - 128 * scale;
		confirmButton.y = p.GetCanvasHeight() - 150 * scale;
		confirmButton.sizex = 256 * scale;
		confirmButton.sizey = 64 * scale;
		
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
		else if (w == backButton)
		{
			OnBackPressed();
		}
	}
	
	void SaveCustomization()
	{
		// Save to preferences
		string data = customization.Serialize();
		menu.p.PreferencesSet("CharacterCustomization", data);
	}
	
	public void LoadCustomization()
	{
		// Load from preferences
		string data = menu.p.PreferencesGet("CharacterCustomization");
		if (data != null)
		{
			customization = CharacterCustomization.Deserialize(menu.p, data);
		}
		customization.platform = menu.p;
	}
}
