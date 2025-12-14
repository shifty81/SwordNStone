/// <summary>
/// Enhanced minimap with zoom controls and world map overlay
/// Based on Assembled_Gui_Pieces.PNG design with circular frame and buttons
/// </summary>
public class ModGuiEnhancedMinimap : ClientMod
{
    public ModGuiEnhancedMinimap()
    {
        minimapSize = 110;
        minimapMargin = 20;
        
        // Zoom levels (blocks in each direction from player)
        zoomLevels = new int[5];
        zoomLevels[0] = 16;   // Closest
        zoomLevels[1] = 32;   // Default
        zoomLevels[2] = 64;
        zoomLevels[3] = 128;
        zoomLevels[4] = 256;  // Farthest
        
        currentZoomLevel = 1; // Start at 32 blocks
        
        // World map state
        worldMapOpen = false;
        worldMapX = 0;
        worldMapY = 0;
        worldMapScale = 1.0f;
        worldMapDragging = false;
        worldMapDragStartX = 0;
        worldMapDragStartY = 0;
        
        // Button positions
        buttonSize = 40;
        buttonSpacing = 5;
    }
    
    internal int minimapSize;
    internal int minimapMargin;
    internal int[] zoomLevels;
    internal int currentZoomLevel;
    
    internal bool worldMapOpen;
    internal int worldMapX;
    internal int worldMapY;
    internal float worldMapScale;
    internal bool worldMapDragging;
    internal int worldMapDragStartX;
    internal int worldMapDragStartY;
    
    internal int buttonSize;
    internal int buttonSpacing;
    
    // UI positions
    int MinimapX(Game game) { return game.Width() - minimapSize - minimapMargin - 50; }
    int MinimapY(Game game) { return minimapMargin; }
    
    int ZoomInButtonX(Game game) { return MinimapX(game) + minimapSize + 10; }
    int ZoomInButtonY(Game game) { return MinimapY(game) + 5; }
    
    int ZoomOutButtonX(Game game) { return MinimapX(game) + minimapSize + 10; }
    int ZoomOutButtonY(Game game) { return MinimapY(game) + buttonSize + buttonSpacing + 5; }
    
    int WorldMapButtonX(Game game) { return MinimapX(game) + minimapSize + 10; }
    int WorldMapButtonY(Game game) { return MinimapY(game) + (buttonSize + buttonSpacing) * 2 + 5; }
    
    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        if (worldMapOpen)
        {
            DrawWorldMap(game);
        }
        else
        {
            DrawMinimap(game);
            DrawMinimapButtons(game);
        }
    }
    
    void DrawMinimap(Game game)
    {
        int x = MinimapX(game);
        int y = MinimapY(game);
        float scale = game.Scale();
        int scaledSize = game.platform.FloatToInt(minimapSize * scale);
        
        // Background circle (dark)
        game.Draw2dTexture(game.WhiteTexture(), x, y, scaledSize, scaledSize, null, 0,
            Game.ColorFromArgb(220, 30, 30, 40), false);
        
        // Draw map content
        DrawMapContent(game, x, y, scaledSize, zoomLevels[currentZoomLevel]);
        
        // Draw minimap frame using the extracted circular frame
        string framePath = "data/themes/default/assembled_gui/minimap/minimap_circular.png";
        game.Draw2dBitmapFile(framePath, x - 5, y - 5, scaledSize + 10, scaledSize + 10);
        
        // Draw coordinates
        FontCi font = new FontCi();
        font.size = 9;
        int playerX = game.platform.FloatToInt(game.player.position.x);
        int playerZ = game.platform.FloatToInt(game.player.position.z);
        string coords = game.platform.StringFormat2("X:{0} Z:{1}",
            game.platform.IntToString(playerX),
            game.platform.IntToString(playerZ));
        game.Draw2dText(coords, font, x + 10, y + scaledSize + 3, null, false);
    }
    
    void DrawMapContent(Game game, int x, int y, int size, int viewRange)
    {
        int playerX = game.platform.FloatToInt(game.player.position.x);
        int playerZ = game.platform.FloatToInt(game.player.position.z);
        
        int pixelsPerBlock = size / (viewRange * 2);
        
        if (pixelsPerBlock > 0)
        {
            int centerX = x + size / 2;
            int centerY = y + size / 2;
            
            for (int dx = -viewRange; dx < viewRange; dx++)
            {
                for (int dz = -viewRange; dz < viewRange; dz++)
                {
                    int blockX = playerX + dx;
                    int blockZ = playerZ + dz;
                    
                    // Check if within circular minimap
                    float distance = game.platform.MathSqrt((dx * dx + dz * dz) * 1.0f);
                    if (distance > viewRange)
                    {
                        continue;
                    }
                    
                    int highestY = GetHighestBlockY(game, blockX, blockZ);
                    if (highestY > 0)
                    {
                        int blockType = game.map.GetBlock(blockX, blockZ, highestY);
                        if (blockType != 0)
                        {
                            int color = GetBlockMinimapColor(blockType);
                            int pixelX = centerX + dx * pixelsPerBlock;
                            int pixelY = centerY + dz * pixelsPerBlock;
                            
                            game.Draw2dTexture(game.WhiteTexture(),
                                pixelX, pixelY, pixelsPerBlock, pixelsPerBlock,
                                null, 0, color, false);
                        }
                    }
                }
            }
            
            // Draw player position (red dot)
            int playerDotSize = game.platform.FloatToInt(6 * game.Scale());
            game.Draw2dTexture(game.WhiteTexture(),
                centerX - playerDotSize / 2, centerY - playerDotSize / 2,
                playerDotSize, playerDotSize, null, 0,
                Game.ColorFromArgb(255, 255, 0, 0), false);
        }
    }
    
    void DrawMinimapButtons(Game game)
    {
        float scale = game.Scale();
        int scaledButtonSize = game.platform.FloatToInt(buttonSize * scale);
        
        // Zoom In button (+)
        string zoomInPath = "data/themes/default/assembled_gui/buttons/button_next.png";
        game.Draw2dBitmapFile(zoomInPath, ZoomInButtonX(game), ZoomInButtonY(game),
            scaledButtonSize, scaledButtonSize);
        
        FontCi font = new FontCi();
        font.size = 20;
        game.Draw2dText("+", font, ZoomInButtonX(game) + 12, ZoomInButtonY(game) + 8, null, false);
        
        // Zoom Out button (-)
        string zoomOutPath = "data/themes/default/assembled_gui/buttons/button_settings.png";
        game.Draw2dBitmapFile(zoomOutPath, ZoomOutButtonX(game), ZoomOutButtonY(game),
            scaledButtonSize, scaledButtonSize);
        game.Draw2dText("-", font, ZoomOutButtonX(game) + 15, ZoomOutButtonY(game) + 5, null, false);
        
        // World Map button (magnifying glass icon)
        string worldMapPath = "data/themes/default/assembled_gui/buttons/button_menu.png";
        game.Draw2dBitmapFile(worldMapPath, WorldMapButtonX(game), WorldMapButtonY(game),
            scaledButtonSize, scaledButtonSize);
        
        FontCi smallFont = new FontCi();
        smallFont.size = 11;
        game.Draw2dText("MAP", smallFont, WorldMapButtonX(game) + 8, WorldMapButtonY(game) + 13, null, false);
    }
    
    void DrawWorldMap(Game game)
    {
        // Full screen semi-transparent overlay
        int overlayColor = Game.ColorFromArgb(200, 20, 20, 30);
        game.Draw2dTexture(game.WhiteTexture(), 0, 0, game.Width(), game.Height(),
            null, 0, overlayColor, false);
        
        // Main map panel (centered)
        int mapWidth = game.Width() - 100;
        int mapHeight = game.Height() - 100;
        int mapX = 50;
        int mapY = 50;
        
        // Draw panel background
        string panelPath = "data/themes/default/assembled_gui/menus/panel_long_titled.png";
        game.Draw2dBitmapFile(panelPath, mapX, mapY, mapWidth, mapHeight);
        
        // Draw title
        FontCi titleFont = new FontCi();
        titleFont.size = 16;
        game.Draw2dText("World Map", titleFont, mapX + 20, mapY + 15, null, false);
        
        // Draw map content area
        int contentX = mapX + 20;
        int contentY = mapY + 50;
        int contentWidth = mapWidth - 40;
        int contentHeight = mapHeight - 100;
        
        DrawWorldMapContent(game, contentX, contentY, contentWidth, contentHeight);
        
        // Draw close button
        string closePath = "data/themes/default/assembled_gui/buttons/button_close.png";
        int closeSize = game.platform.FloatToInt(35 * game.Scale());
        int closeX = mapX + mapWidth - closeSize - 10;
        int closeY = mapY + 10;
        game.Draw2dBitmapFile(closePath, closeX, closeY, closeSize, closeSize);
        
        // Draw instructions
        FontCi font = new FontCi();
        font.size = 11;
        game.Draw2dText("Click and drag to pan | Mouse wheel to zoom | ESC to close",
            font, mapX + 20, mapY + mapHeight - 30, null, false);
    }
    
    void DrawWorldMapContent(Game game, int x, int y, int width, int height)
    {
        // Draw a larger area of the map
        int playerX = game.platform.FloatToInt(game.player.position.x);
        int playerZ = game.platform.FloatToInt(game.player.position.z);
        
        int viewRange = game.platform.FloatToInt(128 * worldMapScale);
        int pixelsPerBlock = 2;
        
        int centerX = x + width / 2 + worldMapX;
        int centerY = y + height / 2 + worldMapY;
        
        for (int dx = -viewRange; dx < viewRange; dx += 2)
        {
            for (int dz = -viewRange; dz < viewRange; dz += 2)
            {
                int blockX = playerX + dx;
                int blockZ = playerZ + dz;
                
                int pixelX = centerX + dx * pixelsPerBlock;
                int pixelY = centerY + dz * pixelsPerBlock;
                
                // Clip to content area
                if (pixelX < x || pixelX > x + width || pixelY < y || pixelY > y + height)
                {
                    continue;
                }
                
                int highestY = GetHighestBlockY(game, blockX, blockZ);
                if (highestY > 0)
                {
                    int blockType = game.map.GetBlock(blockX, blockZ, highestY);
                    if (blockType != 0)
                    {
                        int color = GetBlockMinimapColor(blockType);
                        game.Draw2dTexture(game.WhiteTexture(),
                            pixelX, pixelY, pixelsPerBlock * 2, pixelsPerBlock * 2,
                            null, 0, color, false);
                    }
                }
            }
        }
        
        // Draw player position on world map
        int playerDotSize = 8;
        game.Draw2dTexture(game.WhiteTexture(),
            centerX - playerDotSize / 2, centerY - playerDotSize / 2,
            playerDotSize, playerDotSize, null, 0,
            Game.ColorFromArgb(255, 255, 0, 0), false);
    }
    
    public override void OnMouseDown(Game game, MouseEventArgs args)
    {
        int mouseX = args.GetX();
        int mouseY = args.GetY();
        
        if (worldMapOpen)
        {
            // Check close button
            int mapWidth = game.Width() - 100;
            int mapX = 50;
            int mapY = 50;
            int closeSize = game.platform.FloatToInt(35 * game.Scale());
            int closeX = mapX + mapWidth - closeSize - 10;
            int closeY = mapY + 10;
            
            if (mouseX >= closeX && mouseX <= closeX + closeSize &&
                mouseY >= closeY && mouseY <= closeY + closeSize)
            {
                worldMapOpen = false;
                return;
            }
            
            // Start dragging
            worldMapDragging = true;
            worldMapDragStartX = mouseX;
            worldMapDragStartY = mouseY;
        }
        else
        {
            float scale = game.Scale();
            int scaledButtonSize = game.platform.FloatToInt(buttonSize * scale);
            
            // Check zoom in button
            if (mouseX >= ZoomInButtonX(game) && mouseX <= ZoomInButtonX(game) + scaledButtonSize &&
                mouseY >= ZoomInButtonY(game) && mouseY <= ZoomInButtonY(game) + scaledButtonSize)
            {
                if (currentZoomLevel > 0)
                {
                    currentZoomLevel--;
                }
                return;
            }
            
            // Check zoom out button
            if (mouseX >= ZoomOutButtonX(game) && mouseX <= ZoomOutButtonX(game) + scaledButtonSize &&
                mouseY >= ZoomOutButtonY(game) && mouseY <= ZoomOutButtonY(game) + scaledButtonSize)
            {
                if (currentZoomLevel < 4)
                {
                    currentZoomLevel++;
                }
                return;
            }
            
            // Check world map button
            if (mouseX >= WorldMapButtonX(game) && mouseX <= WorldMapButtonX(game) + scaledButtonSize &&
                mouseY >= WorldMapButtonY(game) && mouseY <= WorldMapButtonY(game) + scaledButtonSize)
            {
                worldMapOpen = true;
                worldMapX = 0;
                worldMapY = 0;
                return;
            }
        }
    }
    
    public override void OnMouseUp(Game game, MouseEventArgs args)
    {
        worldMapDragging = false;
    }
    
    public override void OnMouseMove(Game game, MouseEventArgs args)
    {
        if (worldMapDragging)
        {
            int mouseX = args.GetX();
            int mouseY = args.GetY();
            
            worldMapX += mouseX - worldMapDragStartX;
            worldMapY += mouseY - worldMapDragStartY;
            
            worldMapDragStartX = mouseX;
            worldMapDragStartY = mouseY;
        }
    }
    
    public override void OnKeyDown(Game game, KeyEventArgs args)
    {
        if (worldMapOpen && args.GetKeyCode() == 256) // ESC key
        {
            worldMapOpen = false;
        }
    }
    
    int GetHighestBlockY(Game game, int x, int z)
    {
        for (int y = 127; y >= 0; y--)
        {
            int blockType = game.map.GetBlock(x, z, y);
            if (blockType != 0)
            {
                return y;
            }
        }
        return 0;
    }
    
    int GetBlockMinimapColor(int blockType)
    {
        if (blockType == 1) return Game.ColorFromArgb(255, 128, 128, 128); // Stone
        if (blockType == 2) return Game.ColorFromArgb(255, 0, 180, 0);     // Grass
        if (blockType == 3) return Game.ColorFromArgb(255, 139, 90, 43);   // Dirt
        if (blockType == 7) return Game.ColorFromArgb(255, 237, 201, 175); // Sand
        if (blockType == 8) return Game.ColorFromArgb(255, 136, 126, 126); // Gravel
        if (blockType == 17) return Game.ColorFromArgb(255, 139, 90, 0);   // Wood
        if (blockType == 18) return Game.ColorFromArgb(255, 0, 100, 0);    // Leaves
        if (blockType == 20) return Game.ColorFromArgb(255, 0, 0, 255);    // Water
        if (blockType == 21) return Game.ColorFromArgb(255, 255, 100, 0);  // Lava
        
        return Game.ColorFromArgb(255, 150, 150, 150); // Default
    }
}
