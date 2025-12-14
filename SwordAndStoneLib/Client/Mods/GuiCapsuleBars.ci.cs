/// <summary>
/// Modular capsule-based HP/Mana/Stamina bar system for top-left HUD
/// Based on Assembled_Gui_Pieces.PNG design with upgradeable capsules
/// </summary>
public class ModGuiCapsuleBars : ClientMod
{
    public ModGuiCapsuleBars()
    {
        // Default positions (top-left corner)
        posX = 20;
        posY = 20;
        
        // Bar dimensions based on extracted assets
        barWidth = 195;
        barHeight = 35;
        barSpacing = 5;
        
        // Capsule upgrade levels (1-5, affects visual length)
        hpCapsuleLevel = 1;
        manaCapsuleLevel = 1;
        staminaCapsuleLevel = 1;
        
        // Breath bar state
        breathVisible = false;
        breathOpacity = 0;
        breathFadeSpeed = 2.0f;
    }
    
    internal int posX;
    internal int posY;
    internal int barWidth;
    internal int barHeight;
    internal int barSpacing;
    
    // Capsule upgrade levels (1-5)
    internal int hpCapsuleLevel;
    internal int manaCapsuleLevel;
    internal int staminaCapsuleLevel;
    
    // Breath bar underwater display
    internal bool breathVisible;
    internal float breathOpacity;
    internal float breathFadeSpeed;
    
    public override void OnNewFrameDraw2d(Game game, float deltaTime)
    {
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        DrawCapsuleBars(game, deltaTime);
    }
    
    void DrawCapsuleBars(Game game, float deltaTime)
    {
        if (game.PlayerStats == null)
        {
            return;
        }
        
        float scale = game.Scale();
        int scaledBarWidth = game.platform.FloatToInt(barWidth * scale);
        int scaledBarHeight = game.platform.FloatToInt(barHeight * scale);
        int scaledSpacing = game.platform.FloatToInt(barSpacing * scale);
        
        int currentY = posY;
        
        // Draw HP bar (red)
        DrawCapsuleBar(game, posX, currentY, scaledBarWidth, scaledBarHeight,
            game.PlayerStats.CurrentHealth, game.PlayerStats.MaxHealth,
            "capsule_hp_bar.png", hpCapsuleLevel, Game.ColorFromArgb(255, 255, 0, 0));
        currentY += scaledBarHeight + scaledSpacing;
        
        // Draw Mana bar (blue) - for future use
        DrawCapsuleBar(game, posX, currentY, scaledBarWidth, scaledBarHeight,
            100, 100,  // Placeholder values until mana system is implemented
            "capsule_mana_bar.png", manaCapsuleLevel, Game.ColorFromArgb(255, 0, 100, 255));
        currentY += scaledBarHeight + scaledSpacing;
        
        // Draw Stamina bar (green) - for future use
        DrawCapsuleBar(game, posX, currentY, scaledBarWidth, scaledBarHeight,
            100, 100,  // Placeholder values until stamina system is implemented
            "capsule_stamina_bar.png", staminaCapsuleLevel, Game.ColorFromArgb(255, 0, 255, 0));
        currentY += scaledBarHeight + scaledSpacing;
        
        // Draw Breath bar (only when underwater)
        UpdateBreathBarVisibility(game, deltaTime);
        if (breathOpacity > 0.01f)
        {
            DrawBreathBar(game, posX, currentY, scaledBarWidth, scaledBarHeight, deltaTime);
        }
    }
    
    void DrawCapsuleBar(Game game, int x, int y, int width, int height,
        int currentValue, int maxValue, string barTexture, int capsuleLevel, int fillColor)
    {
        if (maxValue <= 0)
        {
            return;
        }
        
        // Calculate fill percentage
        float progress = game.one * currentValue / maxValue;
        if (progress > game.one) { progress = game.one; }
        if (progress < 0) { progress = 0; }
        
        // Adjust width based on capsule level (upgrades extend the bar)
        int levelWidth = width + (capsuleLevel - 1) * game.platform.FloatToInt(20 * game.Scale());
        
        // Draw the capsule frame
        string framePath = game.platform.StringFormat("data/themes/default/assembled_gui/bars/{0}", barTexture);
        game.Draw2dBitmapFile(framePath, x, y, levelWidth, height);
        
        // Draw the fill (inset from border)
        int fillInset = game.platform.FloatToInt(8 * game.Scale());
        int fillX = x + fillInset;
        int fillY = y + fillInset;
        int fillWidth = game.platform.FloatToInt((levelWidth - fillInset * 2) * progress);
        int fillHeight = height - fillInset * 2;
        
        if (fillWidth > 0)
        {
            game.Draw2dTexture(game.WhiteTexture(), fillX, fillY, fillWidth, fillHeight,
                null, 0, fillColor, false);
        }
        
        // Draw text overlay (current / max)
        FontCi font = new FontCi();
        font.size = 11;
        string text = game.platform.StringFormat2("{0} / {1}",
            game.platform.IntToString(currentValue),
            game.platform.IntToString(maxValue));
        
        int textX = x + levelWidth / 2 - game.platform.FloatToInt(30 * game.Scale());
        int textY = y + height / 2 - game.platform.FloatToInt(6 * game.Scale());
        game.Draw2dText(text, font, textX, textY, null, false);
    }
    
    void UpdateBreathBarVisibility(Game game, float deltaTime)
    {
        if (game.PlayerStats == null)
        {
            return;
        }
        
        // Show breath bar when underwater (oxygen < max)
        bool shouldShow = game.PlayerStats.CurrentOxygen < game.PlayerStats.MaxOxygen;
        
        if (shouldShow)
        {
            // Fade in
            breathOpacity += breathFadeSpeed * deltaTime;
            if (breathOpacity > game.one)
            {
                breathOpacity = game.one;
            }
            breathVisible = true;
        }
        else
        {
            // Fade out
            breathOpacity -= breathFadeSpeed * deltaTime;
            if (breathOpacity < 0)
            {
                breathOpacity = 0;
                breathVisible = false;
            }
        }
    }
    
    void DrawBreathBar(Game game, int x, int y, int width, int height, float deltaTime)
    {
        if (game.PlayerStats == null || breathOpacity <= 0)
        {
            return;
        }
        
        float progress = game.one * game.PlayerStats.CurrentOxygen / game.PlayerStats.MaxOxygen;
        if (progress > game.one) { progress = game.one; }
        if (progress < 0) { progress = 0; }
        
        // Calculate alpha based on fade
        int alpha = game.platform.FloatToInt(255 * breathOpacity);
        
        // Draw breath capsule frame with fade
        string framePath = "data/themes/default/assembled_gui/bars/capsule_breath_bar.png";
        game.Draw2dBitmapFile(framePath, x, y, width, height);
        
        // Draw the fill (cyan/light blue for breath)
        int fillInset = game.platform.FloatToInt(8 * game.Scale());
        int fillX = x + fillInset;
        int fillY = y + fillInset;
        int fillWidth = game.platform.FloatToInt((width - fillInset * 2) * progress);
        int fillHeight = height - fillInset * 2;
        
        if (fillWidth > 0)
        {
            int fillColor = Game.ColorFromArgb(alpha, 0, 200, 255);
            game.Draw2dTexture(game.WhiteTexture(), fillX, fillY, fillWidth, fillHeight,
                null, 0, fillColor, false);
        }
        
        // Draw text with fade
        FontCi font = new FontCi();
        font.size = 11;
        string text = game.platform.StringFormat2("{0} / {1}",
            game.platform.IntToString(game.PlayerStats.CurrentOxygen),
            game.platform.IntToString(game.PlayerStats.MaxOxygen));
        
        int textX = x + width / 2 - game.platform.FloatToInt(30 * game.Scale());
        int textY = y + height / 2 - game.platform.FloatToInt(6 * game.Scale());
        game.Draw2dText(text, font, textX, textY, null, false);
    }
    
    /// <summary>
    /// Upgrade capsule level (called when player upgrades their stats)
    /// </summary>
    public void UpgradeCapsule(int statType, int newLevel)
    {
        if (statType == 0) // HP
        {
            hpCapsuleLevel = newLevel;
        }
        else if (statType == 1) // Mana
        {
            manaCapsuleLevel = newLevel;
        }
        else if (statType == 2) // Stamina
        {
            staminaCapsuleLevel = newLevel;
        }
    }
}
