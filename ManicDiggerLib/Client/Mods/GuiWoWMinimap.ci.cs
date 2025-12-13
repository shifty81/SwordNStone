public class ModGuiWoWMinimap : ClientMod
{
    public ModGuiWoWMinimap()
    {
        minimapSize = 180;
    }

    internal int minimapSize;

    int MinimapX(Game game) { return game.Width() - minimapSize - 20; }
    int MinimapY(Game game) { return 20; }

    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        DrawMinimap(game);
    }

    void DrawMinimap(Game game)
    {
        int x = MinimapX(game);
        int y = MinimapY(game);
        float scale = game.Scale();
        int scaledSize = game.platform.FloatToInt(minimapSize * scale);
        
        // Background circle
        game.Draw2dTexture(game.WhiteTexture(), x, y, scaledSize, scaledSize, null, 0,
            Game.ColorFromArgb(200, 30, 30, 40), false);
        
        // Draw simplified top-down view
        // Get player position
        int playerX = game.platform.FloatToInt(game.player.position.x);
        int playerZ = game.platform.FloatToInt(game.player.position.z);
        
        // Draw blocks around player in a grid
        int viewRange = 32; // blocks in each direction
        int pixelsPerBlock = scaledSize / (viewRange * 2);
        
        if (pixelsPerBlock > 0)
        {
            int centerX = x + scaledSize / 2;
            int centerY = y + scaledSize / 2;
            
            for (int dx = -viewRange; dx < viewRange; dx++)
            {
                for (int dz = -viewRange; dz < viewRange; dz++)
                {
                    int blockX = playerX + dx;
                    int blockZ = playerZ + dz;
                    
                    // Check if within circular minimap
                    float distance = game.platform.FloatToInt(
                        game.platform.MathSqrt((dx * dx + dz * dz) * 1.0f));
                    if (distance > viewRange)
                    {
                        continue;
                    }
                    
                    // Get highest block at this position
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
            int playerDotSize = game.platform.FloatToInt(6 * scale);
            game.Draw2dTexture(game.WhiteTexture(), 
                centerX - playerDotSize / 2, centerY - playerDotSize / 2, 
                playerDotSize, playerDotSize, null, 0, 
                Game.ColorFromArgb(255, 255, 0, 0), false);
            
            // Draw player direction indicator (small triangle)
            DrawPlayerDirection(game, centerX, centerY, scale);
        }
        
        // Draw minimap border
        game.Draw2dBitmapFile("gui/wow/minimap_border.png", x - 10, y - 10, 
            scaledSize + 20, scaledSize + 20);
        
        // Draw coordinates text
        FontCi font = new FontCi();
        font.size = 10;
        string coords = game.platform.StringFormat3("{0}, {1}, {2}",
            game.platform.IntToString(playerX),
            game.platform.IntToString(game.platform.FloatToInt(game.player.position.y)),
            game.platform.IntToString(playerZ));
        game.Draw2dText(coords, font, x + scaledSize / 2 - 30, y + scaledSize + 5, 
            null, false);
    }

    int GetHighestBlockY(Game game, int x, int z)
    {
        // Start from a reasonable height and go down
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
        // Simple color mapping for common block types
        // This is a simplified version - in a real implementation you'd want
        // to map to actual block colors from the game
        
        if (blockType == 1) // Stone
        {
            return Game.ColorFromArgb(255, 128, 128, 128);
        }
        if (blockType == 2) // Grass
        {
            return Game.ColorFromArgb(255, 0, 180, 0);
        }
        if (blockType == 3) // Dirt
        {
            return Game.ColorFromArgb(255, 139, 90, 43);
        }
        if (blockType == 7) // Sand
        {
            return Game.ColorFromArgb(255, 237, 201, 175);
        }
        if (blockType == 8) // Gravel
        {
            return Game.ColorFromArgb(255, 136, 126, 126);
        }
        if (blockType == 17) // Wood
        {
            return Game.ColorFromArgb(255, 139, 90, 0);
        }
        if (blockType == 18) // Leaves
        {
            return Game.ColorFromArgb(255, 0, 100, 0);
        }
        if (blockType == 20) // Water
        {
            return Game.ColorFromArgb(255, 0, 0, 255);
        }
        if (blockType == 21) // Lava
        {
            return Game.ColorFromArgb(255, 255, 100, 0);
        }
        
        // Default color for unknown blocks
        return Game.ColorFromArgb(255, 150, 150, 150);
    }

    void DrawPlayerDirection(Game game, int centerX, int centerY, float scale)
    {
        // Get player orientation (roty is in radians)
        float headingRadians = game.player.position.roty;
        
        // Calculate triangle points for direction indicator
        int arrowLength = game.platform.FloatToInt(12 * scale);
        int arrowWidth = game.platform.FloatToInt(8 * scale);
        
        // Front point
        float frontX = centerX + game.platform.MathSin(headingRadians) * arrowLength;
        float frontY = centerY - game.platform.MathCos(headingRadians) * arrowLength;
        
        // Draw a simple line to indicate direction
        game.Draw2dTexture(game.WhiteTexture(), 
            centerX, centerY, 
            game.platform.FloatToInt(frontX - centerX), 
            game.platform.FloatToInt(frontY - centerY), 
            null, 0, Game.ColorFromArgb(255, 255, 255, 0), false);
    }
}
