public class ModGuiWoWUnitFrames : ClientMod
{
    public ModGuiWoWUnitFrames()
    {
        playerFrameX = 20;
        playerFrameY = 20;
        targetFrameX = 20;
        targetFrameY = 160;
        frameWidth = 256;
        frameHeight = 128;
    }

    internal int playerFrameX;
    internal int playerFrameY;
    internal int targetFrameX;
    internal int targetFrameY;
    internal int frameWidth;
    internal int frameHeight;

    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        DrawPlayerFrame(game);
        
        // Only draw target frame if player is looking at something
        if (game.SelectedBlockPositionX != -1)
        {
            DrawTargetFrame(game);
        }
    }

    void DrawPlayerFrame(Game game)
    {
        if (game.PlayerStats == null)
        {
            return;
        }

        float scale = game.Scale();
        int scaledWidth = game.platform.FloatToInt(frameWidth * scale);
        int scaledHeight = game.platform.FloatToInt(frameHeight * scale);
        
        // Draw frame background
        game.Draw2dBitmapFile("local/gui/wow/player_frame.png", playerFrameX, playerFrameY, 
            scaledWidth, scaledHeight);
        
        // Calculate bar dimensions
        int barX = playerFrameX + game.platform.FloatToInt(75 * scale);
        int barY = playerFrameY + game.platform.FloatToInt(35 * scale);
        int barWidth = game.platform.FloatToInt(160 * scale);
        int barHeight = game.platform.FloatToInt(20 * scale);
        
        // Draw health bar
        float healthProgress = game.one * game.PlayerStats.CurrentHealth / game.PlayerStats.MaxHealth;
        DrawBar(game, barX, barY, barWidth, barHeight, healthProgress, 
            Game.ColorFromArgb(255, 0, 150, 0), // Dark green
            Game.ColorFromArgb(255, 0, 255, 0)); // Bright green
        
        // Draw health text
        FontCi font = new FontCi();
        font.size = 10;
        string healthText = game.platform.StringFormat2("{0} / {1}", 
            game.platform.IntToString(game.PlayerStats.CurrentHealth),
            game.platform.IntToString(game.PlayerStats.MaxHealth));
        game.Draw2dText(healthText, font, barX + barWidth / 2 - 20, barY + 5, null, false);
        
        // Draw oxygen bar (if underwater)
        if (game.PlayerStats.CurrentOxygen < game.PlayerStats.MaxOxygen)
        {
            int oxygenBarY = barY + game.platform.FloatToInt(25 * scale);
            float oxygenProgress = game.one * game.PlayerStats.CurrentOxygen / game.PlayerStats.MaxOxygen;
            DrawBar(game, barX, oxygenBarY, barWidth, barHeight, oxygenProgress,
                Game.ColorFromArgb(255, 0, 100, 150), // Dark blue
                Game.ColorFromArgb(255, 0, 150, 255)); // Bright blue
        }
        
        // Draw portrait placeholder (circular)
        int portraitX = playerFrameX + game.platform.FloatToInt(15 * scale);
        int portraitY = playerFrameY + game.platform.FloatToInt(30 * scale);
        int portraitSize = game.platform.FloatToInt(55 * scale);
        
        // Dark background for portrait
        game.Draw2dTexture(game.WhiteTexture(), portraitX, portraitY, 
            portraitSize, portraitSize, null, 0, 
            Game.ColorFromArgb(255, 40, 40, 50), false);
        
        // Portrait border
        game.Draw2dBitmapFile("local/gui/wow/portrait_border.png", portraitX - 5, portraitY - 5,
            portraitSize + 10, portraitSize + 10);
        
        // Draw player name
        FontCi nameFont = new FontCi();
        nameFont.size = 12;
        string playerName = game.connectdata.Username;
        if (playerName == null) { playerName = "Player"; }
        game.Draw2dText(playerName, nameFont, barX, playerFrameY + game.platform.FloatToInt(20 * scale), 
            null, false);
    }

    void DrawTargetFrame(Game game)
    {
        float scale = game.Scale();
        int scaledWidth = game.platform.FloatToInt(frameWidth * scale);
        int scaledHeight = game.platform.FloatToInt(frameHeight * scale);
        
        // Draw frame background
        game.Draw2dBitmapFile("local/gui/wow/target_frame.png", targetFrameX, targetFrameY, 
            scaledWidth, scaledHeight);
        
        // Calculate bar dimensions
        int barX = targetFrameX + game.platform.FloatToInt(75 * scale);
        int barY = targetFrameY + game.platform.FloatToInt(35 * scale);
        int barWidth = game.platform.FloatToInt(160 * scale);
        int barHeight = game.platform.FloatToInt(20 * scale);
        
        // Get target block type
        int blockType = game.map.GetBlock(game.SelectedBlockPositionX, game.SelectedBlockPositionZ, game.SelectedBlockPositionY);
        
        // For now, show full health for blocks (in future could show block health)
        float targetHealth = 1.0f;
        DrawBar(game, barX, barY, barWidth, barHeight, targetHealth,
            Game.ColorFromArgb(255, 150, 0, 0), // Dark red
            Game.ColorFromArgb(255, 255, 0, 0)); // Bright red
        
        // Draw target name (block name)
        FontCi nameFont = new FontCi();
        nameFont.size = 12;
        string targetName = "Block";
        if (blockType > 0 && blockType < 512)
        {
            targetName = game.platform.StringFormat("Block {0}", game.platform.IntToString(blockType));
        }
        game.Draw2dText(targetName, nameFont, barX, targetFrameY + game.platform.FloatToInt(20 * scale), 
            null, false);
        
        // Draw portrait placeholder
        int portraitX = targetFrameX + game.platform.FloatToInt(15 * scale);
        int portraitY = targetFrameY + game.platform.FloatToInt(30 * scale);
        int portraitSize = game.platform.FloatToInt(55 * scale);
        
        // Show block texture as portrait
        game.Draw2dTexture(game.WhiteTexture(), portraitX, portraitY, 
            portraitSize, portraitSize, null, 0, 
            Game.ColorFromArgb(255, 60, 60, 70), false);
        
        if (blockType > 0)
        {
            GameDataItemsClient dataItems = new GameDataItemsClient();
            dataItems.game = game;
            game.Draw2dTexture(game.terrainTexture, portraitX + 5, portraitY + 5,
                portraitSize - 10, portraitSize - 10, 
                IntRef.Create(dataItems.TextureIdForInventory()[blockType]), 
                game.texturesPacked(), Game.ColorFromArgb(255, 255, 255, 255), false);
        }
        
        // Portrait border
        game.Draw2dBitmapFile("local/gui/wow/portrait_border.png", portraitX - 5, portraitY - 5,
            portraitSize + 10, portraitSize + 10);
    }

    void DrawBar(Game game, int x, int y, int width, int height, float progress, 
        int darkColor, int brightColor)
    {
        // Background (dark)
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, height, null, 0, 
            Game.ColorFromArgb(255, 20, 20, 20), false);
        
        // Progress bar with gradient effect
        int filledWidth = game.platform.FloatToInt(width * progress);
        if (filledWidth > 0)
        {
            // Interpolate between dark and bright color based on progress
            int r1 = (darkColor >> 16) & 0xFF;
            int g1 = (darkColor >> 8) & 0xFF;
            int b1 = darkColor & 0xFF;
            int r2 = (brightColor >> 16) & 0xFF;
            int g2 = (brightColor >> 8) & 0xFF;
            int b2 = brightColor & 0xFF;
            
            int r = game.platform.FloatToInt(r1 + (r2 - r1) * progress);
            int g = game.platform.FloatToInt(g1 + (g2 - g1) * progress);
            int b = game.platform.FloatToInt(b1 + (b2 - b1) * progress);
            
            int barColor = Game.ColorFromArgb(255, r, g, b);
            
            game.Draw2dTexture(game.WhiteTexture(), x + 2, y + 2, 
                filledWidth - 4, height - 4, null, 0, barColor, false);
        }
        
        // Border
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, 2, null, 0, 
            Game.ColorFromArgb(255, 80, 80, 80), false); // Top
        game.Draw2dTexture(game.WhiteTexture(), x, y + height - 2, width, 2, null, 0, 
            Game.ColorFromArgb(255, 80, 80, 80), false); // Bottom
        game.Draw2dTexture(game.WhiteTexture(), x, y, 2, height, null, 0, 
            Game.ColorFromArgb(255, 80, 80, 80), false); // Left
        game.Draw2dTexture(game.WhiteTexture(), x + width - 2, y, 2, height, null, 0, 
            Game.ColorFromArgb(255, 80, 80, 80), false); // Right
    }
}
