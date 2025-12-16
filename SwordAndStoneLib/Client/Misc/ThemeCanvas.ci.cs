/// <summary>
/// Theme Canvas - Canvas for editing UI theme assets
/// Supports various sizes for different UI elements (buttons, frames, bars, etc.)
/// Similar to PixelArtCanvas but designed for UI element editing
/// </summary>
public class ThemeCanvas
{
    public ThemeCanvas()
    {
        // Default to button size (56x32)
        width = 56;
        height = 32;
        canvasType = CANVAS_TYPE_BUTTON;
        
        // Allocate pixel data arrays
        pixels = new int[MAX_WIDTH * MAX_HEIGHT];
        
        // Initialize to transparent
        for (int i = 0; i < MAX_WIDTH * MAX_HEIGHT; i++)
        {
            pixels[i] = 0; // Fully transparent
        }
    }
    
    // Canvas types for different UI elements
    internal const int CANVAS_TYPE_BUTTON = 0;        // 56x32 or 120x32
    internal const int CANVAS_TYPE_FRAME = 1;         // 96x64 or 128x96
    internal const int CANVAS_TYPE_BAR = 2;           // 24x32 (segment) or 192x32 (full)
    internal const int CANVAS_TYPE_SLOT = 3;          // 48x48
    internal const int CANVAS_TYPE_CUSTOM = 4;        // User-defined size
    
    // Maximum dimensions to support various UI elements
    internal const int MAX_WIDTH = 256;
    internal const int MAX_HEIGHT = 256;
    
    internal int width;
    internal int height;
    internal int canvasType;
    internal int[] pixels; // ARGB pixel data
    
    /// <summary>
    /// Resize canvas to specified dimensions
    /// </summary>
    public void Resize(int newWidth, int newHeight)
    {
        if (newWidth <= 0 || newWidth > MAX_WIDTH || newHeight <= 0 || newHeight > MAX_HEIGHT)
        {
            return;
        }
        
        // Create new array with old data
        int[] oldPixels = new int[width * height];
        for (int i = 0; i < width * height; i++)
        {
            oldPixels[i] = pixels[i];
        }
        int oldWidth = width;
        int oldHeight = height;
        
        width = newWidth;
        height = newHeight;
        
        // Clear and preserve what fits
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int newIndex = y * width + x;
                if (x < oldWidth && y < oldHeight)
                {
                    int oldIndex = y * oldWidth + x;
                    pixels[newIndex] = oldPixels[oldIndex];
                }
                else
                {
                    pixels[newIndex] = 0; // Transparent
                }
            }
        }
    }
    
    /// <summary>
    /// Set canvas to predefined size for specific UI element type
    /// </summary>
    public void SetCanvasType(int type)
    {
        canvasType = type;
        
        switch (type)
        {
            case CANVAS_TYPE_BUTTON:
                Resize(56, 32); // Standard button
                break;
            case CANVAS_TYPE_FRAME:
                Resize(96, 64); // Small frame
                break;
            case CANVAS_TYPE_BAR:
                Resize(192, 32); // Full bar
                break;
            case CANVAS_TYPE_SLOT:
                Resize(48, 48); // Inventory slot
                break;
            // CANVAS_TYPE_CUSTOM keeps current size
        }
    }
    
    /// <summary>
    /// Set a pixel at the specified position
    /// </summary>
    public void SetPixel(int x, int y, int color)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }
        
        int index = y * width + x;
        // Extra bounds check for array safety
        if (index >= 0 && index < width * height && index < pixels.Length)
        {
            pixels[index] = color;
        }
    }
    
    /// <summary>
    /// Get a pixel at the specified position
    /// </summary>
    public int GetPixel(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return 0; // Transparent black
        }
        
        int index = y * width + x;
        // Extra bounds check for array safety
        if (index >= 0 && index < width * height && index < pixels.Length)
        {
            return pixels[index];
        }
        return 0;
    }
    
    /// <summary>
    /// Clear the entire canvas to transparent
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < width * height; i++)
        {
            pixels[i] = 0;
        }
    }
    
    /// <summary>
    /// Fill canvas with a solid color
    /// </summary>
    public void Fill(int color)
    {
        for (int i = 0; i < width * height; i++)
        {
            pixels[i] = color;
        }
    }
    
    /// <summary>
    /// Flood fill algorithm for filling connected regions
    /// </summary>
    public void FloodFill(int startX, int startY, int fillColor)
    {
        if (startX < 0 || startX >= width || startY < 0 || startY >= height)
        {
            return;
        }
        
        int targetColor = GetPixel(startX, startY);
        
        // Don't fill if target is already the fill color
        if (targetColor == fillColor)
        {
            return;
        }
        
        // Stack-based flood fill (non-recursive to avoid stack overflow)
        int[] stackX = new int[width * height];
        int[] stackY = new int[width * height];
        int stackSize = 0;
        
        // Push starting position
        stackX[stackSize] = startX;
        stackY[stackSize] = startY;
        stackSize++;
        
        while (stackSize > 0)
        {
            // Pop from stack
            stackSize--;
            int x = stackX[stackSize];
            int y = stackY[stackSize];
            
            // Check if this pixel needs filling
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                continue;
            }
            
            if (GetPixel(x, y) != targetColor)
            {
                continue;
            }
            
            // Fill this pixel
            SetPixel(x, y, fillColor);
            
            // Push adjacent pixels
            if (stackSize < width * height - 4)
            {
                // Right
                stackX[stackSize] = x + 1;
                stackY[stackSize] = y;
                stackSize++;
                
                // Left
                stackX[stackSize] = x - 1;
                stackY[stackSize] = y;
                stackSize++;
                
                // Down
                stackX[stackSize] = x;
                stackY[stackSize] = y + 1;
                stackSize++;
                
                // Up
                stackX[stackSize] = x;
                stackY[stackSize] = y - 1;
                stackSize++;
            }
        }
    }
    
    /// <summary>
    /// Load from bitmap data (for importing existing assets)
    /// </summary>
    public void LoadFromBitmap(BitmapCi bitmap)
    {
        if (bitmap == null)
        {
            return;
        }
        
        // Resize canvas to match bitmap
        Resize(bitmap.Width, bitmap.Height);
        
        // Copy pixel data
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int color = bitmap.GetPixel(x, y);
                SetPixel(x, y, color);
            }
        }
    }
    
    /// <summary>
    /// Export to bitmap for saving
    /// </summary>
    public BitmapCi ExportToBitmap()
    {
        BitmapCi bitmap = new BitmapCi();
        bitmap.Width = width;
        bitmap.Height = height;
        
        // Allocate bitmap data
        bitmap.Pixels = new int[width * height];
        
        // Copy pixel data
        for (int i = 0; i < width * height; i++)
        {
            bitmap.Pixels[i] = pixels[i];
        }
        
        return bitmap;
    }
    
    /// <summary>
    /// Create a border outline in the canvas
    /// </summary>
    public void DrawBorder(int borderColor, int thickness)
    {
        // Top and bottom borders
        for (int x = 0; x < width; x++)
        {
            for (int t = 0; t < thickness; t++)
            {
                SetPixel(x, t, borderColor);                    // Top
                SetPixel(x, height - 1 - t, borderColor);       // Bottom
            }
        }
        
        // Left and right borders
        for (int y = 0; y < height; y++)
        {
            for (int t = 0; t < thickness; t++)
            {
                SetPixel(t, y, borderColor);                    // Left
                SetPixel(width - 1 - t, y, borderColor);        // Right
            }
        }
    }
    
    /// <summary>
    /// Create a gradient fill (useful for bars and backgrounds)
    /// </summary>
    public void DrawGradient(int startColor, int endColor, bool horizontal)
    {
        float one = 1;  // For compatibility with fixed-point arithmetic
        
        int startR = Game.ColorR(startColor);
        int startG = Game.ColorG(startColor);
        int startB = Game.ColorB(startColor);
        int startA = Game.ColorA(startColor);
        
        int endR = Game.ColorR(endColor);
        int endG = Game.ColorG(endColor);
        int endB = Game.ColorB(endColor);
        int endA = Game.ColorA(endColor);
        
        if (horizontal)
        {
            // Horizontal gradient
            for (int x = 0; x < width; x++)
            {
                float t = one * x / width;
                int r = startR + one * (endR - startR) * t;
                int g = startG + one * (endG - startG) * t;
                int b = startB + one * (endB - startB) * t;
                int a = startA + one * (endA - startA) * t;
                
                int color = Game.ColorFromArgb(a, r, g, b);
                
                for (int y = 0; y < height; y++)
                {
                    SetPixel(x, y, color);
                }
            }
        }
        else
        {
            // Vertical gradient
            for (int y = 0; y < height; y++)
            {
                float t = one * y / height;
                int r = startR + one * (endR - startR) * t;
                int g = startG + one * (endG - startG) * t;
                int b = startB + one * (endB - startB) * t;
                int a = startA + one * (endA - startA) * t;
                
                int color = Game.ColorFromArgb(a, r, g, b);
                
                for (int x = 0; x < width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }
    }
}
