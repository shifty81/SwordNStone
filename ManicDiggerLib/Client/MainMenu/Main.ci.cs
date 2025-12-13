public class ScreenMain : Screen
{
    public ScreenMain()
    {
        singleplayer = new MenuWidget();
        multiplayer = new MenuWidget();
        exit = new MenuWidget();
        widgets[0] = singleplayer;
        widgets[1] = multiplayer;
        widgets[2] = exit;
        queryStringChecked = false;
        cursorLoaded = false;
        fontDefault = new FontCi();
        fontDefault.size = 20;
        
        // Sword animation variables
        swordAnimationProgress = 0;
        swordAnimationSpeed = 0.5f;
        swordAnimationStarted = false;
    }

    MenuWidget singleplayer;
    MenuWidget multiplayer;
    MenuWidget exit;
    internal float windowX;
    internal float windowY;
    bool queryStringChecked;
    bool cursorLoaded;
    FontCi fontDefault;
    
    // Sword and Stone animation
    float swordAnimationProgress;
    float swordAnimationSpeed;
    bool swordAnimationStarted;

    public override void Render(float dt)
    {
        windowX = menu.p.GetCanvasWidth();
        windowY = menu.p.GetCanvasHeight();

        float scale = menu.GetScale();

        if (menu.assetsLoadProgress.value != 1)
        {
            string s = menu.p.StringFormat(menu.lang.Get("MainMenu_AssetsLoadProgress"), menu.p.FloatToString(menu.p.FloatToInt(menu.assetsLoadProgress.value * 100)));
            menu.DrawText(s, fontDefault, windowX / 2, windowY / 2, TextAlign.Center, TextBaseline.Middle);
            return;
        }

        if (!cursorLoaded)
        {
            menu.p.SetWindowCursor(0, 0, 32, 32, menu.GetFile("mousecursor.png"), menu.GetFileLength("mousecursor.png"));
            cursorLoaded = true;
        }

        UseQueryStringIpAndPort(menu);

        menu.DrawBackground();
        
        // Draw "Sword and Stone" title screen
        DrawSwordAndStone(dt, scale);

        int buttonheight = 64;
        int buttonwidth = 256;
        int spacebetween = 5;
        int offsetfromborder = 50;

        singleplayer.text = menu.lang.Get("MainMenu_Singleplayer");
        singleplayer.x = windowX / 2 - (buttonwidth / 2) * scale;
        singleplayer.y = windowY - (3 * (buttonheight * scale + spacebetween)) - offsetfromborder * scale;
        singleplayer.sizex = buttonwidth * scale;
        singleplayer.sizey = buttonheight * scale;

        multiplayer.text = menu.lang.Get("MainMenu_Multiplayer");
        multiplayer.x = windowX / 2 - (buttonwidth / 2) * scale;
        multiplayer.y = windowY - (2 * (buttonheight * scale + spacebetween)) - offsetfromborder * scale;
        multiplayer.sizex = buttonwidth * scale;
        multiplayer.sizey = buttonheight * scale;

        exit.visible = menu.p.ExitAvailable();

        exit.text = menu.lang.Get("MainMenu_Quit");
        exit.x = windowX / 2 - (buttonwidth / 2) * scale;
        exit.y = windowY - (1 * (buttonheight * scale + spacebetween)) - offsetfromborder * scale;
        exit.sizex = buttonwidth * scale;
        exit.sizey = buttonheight * scale;
        DrawWidgets();
    }

    void UseQueryStringIpAndPort(MainMenu menu)
    {
        if (queryStringChecked)
        {
            return;
        }
        queryStringChecked = true;
        string ip = menu.p.QueryStringValue("ip");
        string port = menu.p.QueryStringValue("port");
        int portInt = 25565;
        if (port != null && menu.p.FloatTryParse(port, new FloatRef()))
        {
            portInt = menu.p.IntParse(port);
        }
        if (ip != null)
        {
            menu.StartLogin(null, ip, portInt);
        }
    }

    void DrawSwordAndStone(float dt, float scale)
    {
        // Start animation after a short delay
        if (!swordAnimationStarted)
        {
            swordAnimationStarted = true;
        }
        
        // Stone logo background
        float stoneWidth = 512 * scale;
        float stoneHeight = 256 * scale;
        float stoneX = windowX / 2 - stoneWidth / 2;
        float stoneY = 50 * scale;
        
        menu.Draw2dQuad(menu.GetTexture("local/gui/wow/stone_logo.png"), 
            menu.p.FloatToInt(stoneX), menu.p.FloatToInt(stoneY), 
            menu.p.FloatToInt(stoneWidth), menu.p.FloatToInt(stoneHeight));
        
        // Animate sword descending and sticking into stone
        float swordWidth = 128 * scale;
        float swordHeight = 512 * scale;
        float swordX = windowX / 2 - swordWidth / 2;
        float swordTargetY = stoneY + 20 * scale; // Final position inside stone
        
        if (swordAnimationProgress < 1.0f)
        {
            // Update animation
            swordAnimationProgress += dt * swordAnimationSpeed;
            if (swordAnimationProgress > 1.0f)
            {
                swordAnimationProgress = 1.0f;
            }
            
            // Ease-out bounce effect
            float t = swordAnimationProgress;
            float bounce = 1;
            if (t < 1)
            {
                // Simple ease-out
                bounce = t * t * (3 - 2 * t);
            }
            
            // Calculate current Y position
            float swordStartY = -swordHeight;
            float swordY = swordStartY + (swordTargetY - swordStartY) * bounce;
            
            menu.Draw2dQuad(menu.GetTexture("local/gui/wow/sword.png"), 
                menu.p.FloatToInt(swordX), menu.p.FloatToInt(swordY), 
                menu.p.FloatToInt(swordWidth), menu.p.FloatToInt(swordHeight));
        }
        else
        {
            // Animation complete - draw sword in final position
            menu.Draw2dQuad(menu.GetTexture("local/gui/wow/sword.png"), 
                menu.p.FloatToInt(swordX), menu.p.FloatToInt(swordTargetY), 
                menu.p.FloatToInt(swordWidth), menu.p.FloatToInt(swordHeight));
        }
        
        // Draw "SWORD AND STONE" title text
        FontCi titleFont = new FontCi();
        titleFont.size = 48;
        float titleY = stoneY + stoneHeight + 30 * scale;
        menu.DrawText("SWORD AND STONE", titleFont, windowX / 2, titleY, 
            TextAlign.Center, TextBaseline.Top);
    }

    public override void OnButton(MenuWidget w)
    {
        if (w == singleplayer)
        {
            menu.StartSingleplayer();
        }
        if (w == multiplayer)
        {
            menu.StartMultiplayer();
        }
        if (w == exit)
        {
            menu.Exit();
        }
    }

    public override void OnBackPressed()
    {
        menu.Exit();
    }

    public override void OnKeyDown(KeyEventArgs e)
    {
        // debug
        if (e.GetKeyCode() == GlKeys.F5)
        {
            menu.p.SinglePlayerServerDisable();
            menu.StartGame(true, menu.p.PathCombine(menu.p.PathSavegames(), "Default.mdss"), null);
        }
        if (e.GetKeyCode() == GlKeys.F6)
        {
            menu.StartGame(true, menu.p.PathCombine(menu.p.PathSavegames(), "Default.mddbs"), null);
        }
    }
}
