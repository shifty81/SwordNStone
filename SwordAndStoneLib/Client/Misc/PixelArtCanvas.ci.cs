// Pixel Art Canvas - holds pixel data for character skin editing
public class PixelArtCanvas
{
	public PixelArtCanvas()
	{
		width = 64;
		height = 32;
		pixels = null;
		layerPixels = null;
		currentLayer = 0;
		isDirty = true;
	}
	
	internal int width;
	internal int height;
	internal int[] pixels; // Main texture layer (ARGB format)
	internal int[] layerPixels; // Overlay layer for hair/accessories (ARGB format)
	internal int currentLayer; // 0 = base, 1 = overlay
	internal bool isDirty; // Flag to indicate texture needs update
	internal GamePlatform platform;
	
	public void Initialize(GamePlatform p, int w, int h)
	{
		platform = p;
		width = w;
		height = h;
		pixels = new int[width * height];
		layerPixels = new int[width * height];
		
		// Initialize with transparent pixels
		for (int i = 0; i < width * height; i++)
		{
			pixels[i] = ColorFromArgb(0, 255, 255, 255); // Transparent white
			layerPixels[i] = ColorFromArgb(0, 255, 255, 255);
		}
		isDirty = true;
	}
	
	public void LoadFromTexture(byte[] textureData, int dataLength)
	{
		if (textureData == null || platform == null)
		{
			return;
		}
		
		// Create bitmap from PNG data
		BitmapCi bitmap = platform.BitmapCreateFromPng(textureData, dataLength);
		if (bitmap == null)
		{
			return;
		}
		
		// Get bitmap dimensions
		int bmpWidth = platform.FloatToInt(platform.BitmapGetWidth(bitmap));
		int bmpHeight = platform.FloatToInt(platform.BitmapGetHeight(bitmap));
		
		// Adjust canvas size if needed
		if (bmpWidth != width || bmpHeight != height)
		{
			width = bmpWidth;
			height = bmpHeight;
			pixels = new int[width * height];
			layerPixels = new int[width * height];
		}
		
		// Get pixel data
		int[] bmpPixels = new int[width * height];
		platform.BitmapGetPixelsArgb(bitmap, bmpPixels);
		
		// Copy to canvas
		for (int i = 0; i < width * height; i++)
		{
			pixels[i] = bmpPixels[i];
		}
		
		platform.BitmapDelete(bitmap);
		isDirty = true;
	}
	
	public void SetPixel(int x, int y, int color)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
		{
			return;
		}
		
		int index = y * width + x;
		if (currentLayer == 0)
		{
			pixels[index] = color;
		}
		else
		{
			layerPixels[index] = color;
		}
		isDirty = true;
	}
	
	public int GetPixel(int x, int y)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
		{
			return ColorFromArgb(255, 0, 0, 0);
		}
		
		int index = y * width + x;
		if (currentLayer == 0)
		{
			return pixels[index];
		}
		else
		{
			return layerPixels[index];
		}
	}
	
	// Get composited pixel (base + overlay)
	public int GetCompositedPixel(int x, int y)
	{
		if (x < 0 || x >= width || y < 0 || y >= height)
		{
			return ColorFromArgb(255, 0, 0, 0);
		}
		
		int index = y * width + x;
		int baseColor = pixels[index];
		int overlayColor = layerPixels[index];
		
		// Simple alpha blend
		int overlayAlpha = (overlayColor >> 24) & 0xFF;
		if (overlayAlpha == 0)
		{
			return baseColor;
		}
		if (overlayAlpha == 255)
		{
			return overlayColor;
		}
		
		// Blend colors
		int baseR = (baseColor >> 16) & 0xFF;
		int baseG = (baseColor >> 8) & 0xFF;
		int baseB = baseColor & 0xFF;
		
		int overlayR = (overlayColor >> 16) & 0xFF;
		int overlayG = (overlayColor >> 8) & 0xFF;
		int overlayB = overlayColor & 0xFF;
		
		float alpha = overlayAlpha / 255.0f;
		int r = platform.FloatToInt(overlayR * alpha + baseR * (1.0f - alpha));
		int g = platform.FloatToInt(overlayG * alpha + baseG * (1.0f - alpha));
		int b = platform.FloatToInt(overlayB * alpha + baseB * (1.0f - alpha));
		
		return ColorFromArgb(255, r, g, b);
	}
	
	// Fill area with color (flood fill)
	public void FloodFill(int startX, int startY, int fillColor)
	{
		if (startX < 0 || startX >= width || startY < 0 || startY >= height)
		{
			return;
		}
		
		int targetColor = GetPixel(startX, startY);
		if (targetColor == fillColor)
		{
			return;
		}
		
		// Simple stack-based flood fill
		IntegerArrayList stackX = new IntegerArrayList();
		IntegerArrayList stackY = new IntegerArrayList();
		
		stackX.Add(startX);
		stackY.Add(startY);
		
		int maxIterations = width * height; // Prevent infinite loop
		int iterations = 0;
		
		while (stackX.Count() > 0 && iterations < maxIterations)
		{
			iterations++;
			int x = stackX.GetItem(stackX.Count() - 1);
			int y = stackY.GetItem(stackY.Count() - 1);
			stackX.RemoveAt(stackX.Count() - 1);
			stackY.RemoveAt(stackY.Count() - 1);
			
			if (x < 0 || x >= width || y < 0 || y >= height)
			{
				continue;
			}
			
			if (GetPixel(x, y) != targetColor)
			{
				continue;
			}
			
			SetPixel(x, y, fillColor);
			
			// Add neighbors
			stackX.Add(x + 1);
			stackY.Add(y);
			stackX.Add(x - 1);
			stackY.Add(y);
			stackX.Add(x);
			stackY.Add(y + 1);
			stackX.Add(x);
			stackY.Add(y - 1);
		}
		
		isDirty = true;
	}
	
	// Clear canvas
	public void Clear(int color)
	{
		for (int i = 0; i < width * height; i++)
		{
			if (currentLayer == 0)
			{
				pixels[i] = color;
			}
			else
			{
				layerPixels[i] = color;
			}
		}
		isDirty = true;
	}
	
	// Export to bitmap
	public BitmapCi ExportToBitmap(bool includeOverlay)
	{
		if (platform == null)
		{
			return null;
		}
		
		BitmapCi bitmap = platform.BitmapCreate(width, height);
		int[] exportPixels = new int[width * height];
		
		if (includeOverlay)
		{
			// Composite both layers
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					exportPixels[y * width + x] = GetCompositedPixel(x, y);
				}
			}
		}
		else
		{
			// Just base layer
			for (int i = 0; i < width * height; i++)
			{
				exportPixels[i] = currentLayer == 0 ? pixels[i] : layerPixels[i];
			}
		}
		
		platform.BitmapSetPixelsArgb(bitmap, exportPixels);
		return bitmap;
	}
	
	// Helper: Create ARGB color
	static int ColorFromArgb(int a, int r, int g, int b)
	{
		return (a << 24) | (r << 16) | (g << 8) | b;
	}
}
