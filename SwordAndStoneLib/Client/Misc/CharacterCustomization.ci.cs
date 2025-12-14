public class CharacterCustomization
{
	public CharacterCustomization()
	{
		// Default values
		Gender = 0; // 0 = Male, 1 = Female
		Hairstyle = 0;
		Beard = 0;
		Outfit = 0;
	}
	
	public int Gender;
	public int Hairstyle;
	public int Beard;
	public int Outfit;
	
	internal GamePlatform platform;
	
	// Get the texture filename based on customization options
	public string GetTextureName()
	{
		// Build texture name from options
		// Format: player_[gender]_[hairstyle]_[beard]_[outfit].png
		// For now, we'll use a simplified system that maps to existing textures
		// and future texture variants
		
		// Default to mineplayer.png if no customization
		if (Gender == 0 && Hairstyle == 0 && Beard == 0 && Outfit == 0)
		{
			return "mineplayer.png";
		}
		
		// Build custom texture name
		// We'll create a naming convention for future texture variants
		if (platform != null)
		{
			return platform.StringFormat4("player_{0}_{1}_{2}_{3}.png", 
				platform.IntToString(Gender),
				platform.IntToString(Hairstyle),
				platform.IntToString(Beard),
				platform.IntToString(Outfit));
		}
		
		// Fallback
		return "mineplayer.png";
	}
	
	// Get model filename (could vary by gender)
	public string GetModelName()
	{
		// Use enhanced model with better animations
		return "playerenhanced.txt";
	}
	
	// Serialize to string for saving
	public string Serialize()
	{
		if (platform != null)
		{
			return platform.StringFormat4("{0},{1},{2},{3}", 
				platform.IntToString(Gender),
				platform.IntToString(Hairstyle),
				platform.IntToString(Beard),
				platform.IntToString(Outfit));
		}
		return "0,0,0,0";
	}
	
	// Deserialize from string
	public static CharacterCustomization Deserialize(GamePlatform p, string data)
	{
		CharacterCustomization custom = new CharacterCustomization();
		custom.platform = p;
		
		if (data == null || p.StringEmpty(data))
		{
			return custom;
		}
		
		IntRef partsCount = new IntRef();
		string[] parts = p.StringSplit(data, ",", partsCount);
		if (parts != null && partsCount.value >= 4)
		{
			custom.Gender = p.IntParse(parts[0]);
			custom.Hairstyle = p.IntParse(parts[1]);
			custom.Beard = p.IntParse(parts[2]);
			custom.Outfit = p.IntParse(parts[3]);
		}
		
		return custom;
	}
	
	// Copy customization
	public void CopyFrom(CharacterCustomization other)
	{
		this.Gender = other.Gender;
		this.Hairstyle = other.Hairstyle;
		this.Beard = other.Beard;
		this.Outfit = other.Outfit;
	}
	
	// Get number of available options for each category
	public static int GetGenderCount() { return 2; } // Male, Female
	public static int GetHairstyleCount() { return 5; } // 5 hairstyle options
	public static int GetBeardCount() { return 4; } // 4 beard options (including none)
	public static int GetOutfitCount() { return 4; } // 4 outfit options
}
