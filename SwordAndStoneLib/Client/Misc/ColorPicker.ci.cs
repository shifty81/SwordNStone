// Color Picker - RGB color selection and palette management
public class ColorPicker
{
	public ColorPicker()
	{
		selectedColor = ColorFromArgb(255, 0, 0, 0); // Black
		hue = 0;
		saturation = 0;
		value = 0;
		alpha = 255;
		palette = new int[16]; // 16 color palette
		paletteSize = 0;
	}
	
	internal int selectedColor;
	internal float hue; // 0-360
	internal float saturation; // 0-1
	internal float value; // 0-1
	internal int alpha; // 0-255
	internal int[] palette;
	internal int paletteSize;
	internal GamePlatform platform;
	
	public void SetPlatform(GamePlatform p)
	{
		platform = p;
	}
	
	public int GetSelectedColor()
	{
		return selectedColor;
	}
	
	public void SetSelectedColor(int color)
	{
		selectedColor = color;
		
		// Extract ARGB components
		alpha = (color >> 24) & 0xFF;
		int r = (color >> 16) & 0xFF;
		int g = (color >> 8) & 0xFF;
		int b = color & 0xFF;
		
		// Convert RGB to HSV
		RgbToHsv(r, g, b);
	}
	
	// Set color from RGB values (0-255)
	public void SetColorRgb(int r, int g, int b)
	{
		selectedColor = ColorFromArgb(alpha, r, g, b);
		RgbToHsv(r, g, b);
	}
	
	// Set color from HSV values (h: 0-360, s: 0-1, v: 0-1)
	public void SetColorHsv(float h, float s, float v)
	{
		hue = h;
		saturation = s;
		value = v;
		
		// Convert HSV to RGB
		int r = 0;
		int g = 0;
		int b = 0;
		HsvToRgb(h, s, v, ref r, ref g, ref b);
		
		selectedColor = ColorFromArgb(alpha, r, g, b);
	}
	
	// Convert RGB to HSV
	void RgbToHsv(int r, int g, int b)
	{
		if (platform == null)
		{
			return;
		}
		
		float rf = r / 255.0f;
		float gf = g / 255.0f;
		float bf = b / 255.0f;
		
		float max = rf;
		if (gf > max) { max = gf; }
		if (bf > max) { max = bf; }
		
		float min = rf;
		if (gf < min) { min = gf; }
		if (bf < min) { min = bf; }
		
		float delta = max - min;
		
		// Value
		value = max;
		
		// Saturation
		if (max > 0)
		{
			saturation = delta / max;
		}
		else
		{
			saturation = 0;
		}
		
		// Hue
		if (delta > 0)
		{
			if (max == rf)
			{
				hue = 60.0f * ((gf - bf) / delta);
			}
			else if (max == gf)
			{
				hue = 60.0f * (2.0f + (bf - rf) / delta);
			}
			else
			{
				hue = 60.0f * (4.0f + (rf - gf) / delta);
			}
			
			if (hue < 0)
			{
				hue = hue + 360.0f;
			}
		}
		else
		{
			hue = 0;
		}
	}
	
	// Convert HSV to RGB
	void HsvToRgb(float h, float s, float v, ref int r, ref int g, ref int b)
	{
		if (platform == null)
		{
			r = 0;
			g = 0;
			b = 0;
			return;
		}
		
		if (s <= 0)
		{
			r = platform.FloatToInt(v * 255.0f);
			g = r;
			b = r;
			return;
		}
		
		float hh = h;
		if (hh >= 360.0f)
		{
			hh = 0;
		}
		hh = hh / 60.0f;
		
		int i = platform.FloatToInt(hh);
		float ff = hh - i;
		float p = v * (1.0f - s);
		float q = v * (1.0f - (s * ff));
		float t = v * (1.0f - (s * (1.0f - ff)));
		
		if (i == 0)
		{
			r = platform.FloatToInt(v * 255.0f);
			g = platform.FloatToInt(t * 255.0f);
			b = platform.FloatToInt(p * 255.0f);
		}
		else if (i == 1)
		{
			r = platform.FloatToInt(q * 255.0f);
			g = platform.FloatToInt(v * 255.0f);
			b = platform.FloatToInt(p * 255.0f);
		}
		else if (i == 2)
		{
			r = platform.FloatToInt(p * 255.0f);
			g = platform.FloatToInt(v * 255.0f);
			b = platform.FloatToInt(t * 255.0f);
		}
		else if (i == 3)
		{
			r = platform.FloatToInt(p * 255.0f);
			g = platform.FloatToInt(q * 255.0f);
			b = platform.FloatToInt(v * 255.0f);
		}
		else if (i == 4)
		{
			r = platform.FloatToInt(t * 255.0f);
			g = platform.FloatToInt(p * 255.0f);
			b = platform.FloatToInt(v * 255.0f);
		}
		else
		{
			r = platform.FloatToInt(v * 255.0f);
			g = platform.FloatToInt(p * 255.0f);
			b = platform.FloatToInt(q * 255.0f);
		}
	}
	
	// Add color to palette
	public void AddToPalette(int color)
	{
		// Check if already in palette
		for (int i = 0; i < paletteSize; i++)
		{
			if (palette[i] == color)
			{
				return;
			}
		}
		
		// Add if space available
		if (paletteSize < 16)
		{
			palette[paletteSize] = color;
			paletteSize++;
		}
	}
	
	// Remove color from palette
	public void RemoveFromPalette(int index)
	{
		if (index < 0 || index >= paletteSize)
		{
			return;
		}
		
		// Shift colors down
		for (int i = index; i < paletteSize - 1; i++)
		{
			palette[i] = palette[i + 1];
		}
		paletteSize--;
	}
	
	// Clear palette
	public void ClearPalette()
	{
		paletteSize = 0;
	}
	
	// Get palette color
	public int GetPaletteColor(int index)
	{
		if (index < 0 || index >= paletteSize)
		{
			return ColorFromArgb(255, 0, 0, 0);
		}
		return palette[index];
	}
	
	// Helper: Create ARGB color
	static int ColorFromArgb(int a, int r, int g, int b)
	{
		return (a << 24) | (r << 16) | (g << 8) | b;
	}
	
	// Extract color components
	public static void GetColorComponents(int color, ref int a, ref int r, ref int g, ref int b)
	{
		a = (color >> 24) & 0xFF;
		r = (color >> 16) & 0xFF;
		g = (color >> 8) & 0xFF;
		b = color & 0xFF;
	}
}
