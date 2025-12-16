// Pixel Art Tools - brush, eraser, fill bucket, color picker
public class PixelArtTools
{
	public PixelArtTools()
	{
		currentTool = PixelArtToolType.Brush;
		brushSize = 1;
	}
	
	internal PixelArtToolType currentTool;
	internal int brushSize; // 1-10
	
	public PixelArtToolType GetCurrentTool()
	{
		return currentTool;
	}
	
	public void SetCurrentTool(PixelArtToolType tool)
	{
		currentTool = tool;
	}
	
	public int GetBrushSize()
	{
		return brushSize;
	}
	
	public void SetBrushSize(int size)
	{
		if (size < 1) { size = 1; }
		if (size > 10) { size = 10; }
		brushSize = size;
	}
	
	// Apply tool to canvas at position
	public void ApplyTool(PixelArtCanvas canvas, int x, int y, int color)
	{
		if (canvas == null)
		{
			return;
		}
		
		if (currentTool == PixelArtToolType.Brush)
		{
			ApplyBrush(canvas, x, y, color);
		}
		else if (currentTool == PixelArtToolType.Eraser)
		{
			ApplyEraser(canvas, x, y);
		}
		else if (currentTool == PixelArtToolType.FillBucket)
		{
			canvas.FloodFill(x, y, color);
		}
		// ColorPicker is handled separately (just reading, not writing)
	}
	
	void ApplyBrush(PixelArtCanvas canvas, int centerX, int centerY, int color)
	{
		// Draw a square brush
		int halfSize = brushSize / 2;
		for (int dy = -halfSize; dy <= halfSize; dy++)
		{
			for (int dx = -halfSize; dx <= halfSize; dx++)
			{
				canvas.SetPixel(centerX + dx, centerY + dy, color);
			}
		}
	}
	
	void ApplyEraser(PixelArtCanvas canvas, int centerX, int centerY)
	{
		// Erase by setting to transparent
		int transparent = ColorFromArgb(0, 255, 255, 255);
		int halfSize = brushSize / 2;
		for (int dy = -halfSize; dy <= halfSize; dy++)
		{
			for (int dx = -halfSize; dx <= halfSize; dx++)
			{
				canvas.SetPixel(centerX + dx, centerY + dy, transparent);
			}
		}
	}
	
	// Get pixel color at position (for color picker tool)
	public int PickColor(PixelArtCanvas canvas, int x, int y)
	{
		if (canvas == null)
		{
			return ColorFromArgb(255, 0, 0, 0);
		}
		return canvas.GetPixel(x, y);
	}
	
	// Helper: Create ARGB color
	static int ColorFromArgb(int a, int r, int g, int b)
	{
		return (a << 24) | (r << 16) | (g << 8) | b;
	}
}

// Tool type enumeration
public enum PixelArtToolType
{
	Brush,
	Eraser,
	FillBucket,
	ColorPicker
}
