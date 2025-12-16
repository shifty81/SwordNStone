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
		IntRef r = IntRef.Create(0);
		IntRef g = IntRef.Create(0);
		IntRef b = IntRef.Create(0);
		HsvToRgb(h, s, v, r, g, b);
		
		selectedColor = ColorFromArgb(alpha, r.value, g.value, b.value);
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
	void HsvToRgb(float h, float s, float v, IntRef r, IntRef g, IntRef b)
	{
		if (platform == null)
		{
			r.value = 0;
			g.value = 0;
			b.value = 0;
			return;
		}
		
		if (s <= 0)
		{
			r.value = platform.FloatToInt(v * 255.0f);
			g.value = r.value;
			b.value = r.value;
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
			r.value = platform.FloatToInt(v * 255.0f);
			g.value = platform.FloatToInt(t * 255.0f);
			b.value = platform.FloatToInt(p * 255.0f);
		}
		else if (i == 1)
		{
			r.value = platform.FloatToInt(q * 255.0f);
			g.value = platform.FloatToInt(v * 255.0f);
			b.value = platform.FloatToInt(p * 255.0f);
		}
		else if (i == 2)
		{
			r.value = platform.FloatToInt(p * 255.0f);
			g.value = platform.FloatToInt(v * 255.0f);
			b.value = platform.FloatToInt(t * 255.0f);
		}
		else if (i == 3)
		{
			r.value = platform.FloatToInt(p * 255.0f);
			g.value = platform.FloatToInt(q * 255.0f);
			b.value = platform.FloatToInt(v * 255.0f);
		}
		else if (i == 4)
		{
			r.value = platform.FloatToInt(t * 255.0f);
			g.value = platform.FloatToInt(p * 255.0f);
			b.value = platform.FloatToInt(v * 255.0f);
		}
		else
		{
			r.value = platform.FloatToInt(v * 255.0f);
			g.value = platform.FloatToInt(p * 255.0f);
			b.value = platform.FloatToInt(q * 255.0f);
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
	public static void GetColorComponents(int color, IntRef a, IntRef r, IntRef g, IntRef b)
	{
		a.value = (color >> 24) & 0xFF;
		r.value = (color >> 16) & 0xFF;
		g.value = (color >> 8) & 0xFF;
		b.value = color & 0xFF;
	}
}
