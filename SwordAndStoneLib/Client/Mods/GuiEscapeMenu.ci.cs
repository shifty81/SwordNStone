public class ModGuiEscapeMenu : ClientMod
{
    public ModGuiEscapeMenu()
    {
        one = 1;
        fonts = new string[4];
        fonts[0] = "Nice";
        fonts[1] = "Simple";
        fonts[2] = "BlackBackground";
        fonts[3] = "Default";
        fontsLength = 4;
        fontValues = new int[4];
        fontValues[0] = 0;
        fontValues[1] = 1;
        fontValues[2] = 2;
        fontValues[3] = 3;
        widgets = new Button[1024];
        keyselectid = -1;
        fontEscapeMenu = FontCi.Create("Arial", 20, 0);
    }
    float one;
    
    // UI Constants for tabbed interface
    const int TAB_BORDER_THICKNESS = 2;
    const int TAB_PANEL_Y = 35;
    const int TAB_PANEL_HEIGHT = 35;
    const int CONTENT_PANEL_MARGIN = 30;
    const int TAB_FONT_SIZE = 14;
    const int CONTROLS_TEXT_HEIGHT = 16;
    const int STANDARD_TEXT_HEIGHT = 20;
    
    // Color constants for UI elements
    int ColorTabPanelBg() { return Game.ColorFromArgb(180, 70, 70, 70); }
    int ColorContentPanelBg() { return Game.ColorFromArgb(200, 50, 50, 50); }
    int ColorActiveTabBg() { return Game.ColorFromArgb(255, 100, 100, 100); }
    int ColorInactiveTabBg() { return Game.ColorFromArgb(200, 60, 60, 60); }
    int ColorActiveTabText() { return Game.ColorFromArgb(255, 255, 215, 0); }
    int ColorInactiveTabText() { return Game.ColorFromArgb(255, 180, 180, 180); }
    int ColorHoverText() { return Game.ColorFromArgb(255, 255, 255, 255); }
    int ColorActiveTabBorder() { return Game.ColorFromArgb(255, 180, 180, 180); }
    int ColorInactiveTabBorder() { return Game.ColorFromArgb(255, 80, 80, 80); }
    
    Button buttonMainReturnToGame;
    Button buttonMainOptions;
    Button buttonMainExit;
    FontCi fontEscapeMenu;

    int widgetsCount;
    void MainSet()
    {
        LanguageCi language = game.language;
        buttonMainReturnToGame = new Button();
        buttonMainReturnToGame.Text = language.ReturnToGame();
        buttonMainOptions = new Button();
        buttonMainOptions.Text = language.Options();
        buttonMainExit = new Button();
        buttonMainExit.Text = language.Exit();

        WidgetsClear();
        AddWidget(buttonMainReturnToGame);
        AddWidget(buttonMainOptions);
        AddWidget(buttonMainExit);
    }

    void MainHandleClick(Button b)
    {
        if (b == buttonMainReturnToGame)
        {
            game.GuiStateBackToGame();
        }
        if (b == buttonMainOptions)
        {
            SetEscapeMenuState(EscapeMenuState.Options);
        }
        if (b == buttonMainExit)
        {
            game.SendLeave(Packet_LeaveReasonEnum.Leave);
            game.ExitToMainMenu_();
        }
    }

    // Tab buttons
    Button tabGraphics;
    Button tabMouse;
    Button tabControls;
    Button tabAccessibility;
    Button tabSound;
    Button tabInterface;
    Button tabDev;
    Button tabBack;
    
    // Tab button array for the tabbed interface
    // Stores all tab buttons for rendering and interaction
    Button[] tabButtons;
    int tabButtonsCount;
    
    Button optionsGraphics;
    Button optionsKeys;
    Button optionsOther;
    Button optionsReturnToMainMenu;
    
    void InitTabButtons()
    {
        tabButtons = new Button[8];
        tabButtonsCount = 0;
        
        LanguageCi language = game.language;
        tabGraphics = new Button();
        tabGraphics.Text = language.Graphics();
        tabButtons[tabButtonsCount++] = tabGraphics;
        
        tabMouse = new Button();
        tabMouse.Text = "Mouse";
        tabButtons[tabButtonsCount++] = tabMouse;
        
        tabControls = new Button();
        tabControls.Text = "Controls";
        tabButtons[tabButtonsCount++] = tabControls;
        
        tabAccessibility = new Button();
        tabAccessibility.Text = "Accessibility";
        tabButtons[tabButtonsCount++] = tabAccessibility;
        
        tabSound = new Button();
        tabSound.Text = "Sound";
        tabButtons[tabButtonsCount++] = tabSound;
        
        tabInterface = new Button();
        tabInterface.Text = "Interface";
        tabButtons[tabButtonsCount++] = tabInterface;
        
        tabDev = new Button();
        tabDev.Text = "Dev";
        tabButtons[tabButtonsCount++] = tabDev;
        
        tabBack = new Button();
        tabBack.Text = "Back";
        tabButtons[tabButtonsCount++] = tabBack;
    }
    
    void OptionsSet()
    {
        LanguageCi language = game.language;
        optionsGraphics = new Button();
        optionsGraphics.Text = language.Graphics();
        optionsKeys = new Button();
        optionsKeys.Text = language.Keys();
        optionsOther = new Button();
        optionsOther.Text = language.Other();
        optionsReturnToMainMenu = new Button();
        optionsReturnToMainMenu.Text = language.ReturnToMainMenu();

        WidgetsClear();
        AddWidget(optionsGraphics);
        AddWidget(optionsKeys);
        AddWidget(optionsOther);
        AddWidget(optionsReturnToMainMenu);
    }

    void OptionsHandleClick(Button b)
    {
        if (b == optionsGraphics)
        {
            SetEscapeMenuState(EscapeMenuState.Graphics);
        }
        if (b == optionsKeys)
        {
            SetEscapeMenuState(EscapeMenuState.Keys);
        }
        if (b == optionsOther)
        {
            SetEscapeMenuState(EscapeMenuState.Other);
        }
        if (b == optionsReturnToMainMenu)
        {
            SaveOptions(); SetEscapeMenuState(EscapeMenuState.Main);
        }
    }
    
    void HandleTabClick(Button b)
    {
        if (b == tabGraphics)
        {
            SetEscapeMenuState(EscapeMenuState.Graphics);
        }
        else if (b == tabMouse)
        {
            SetEscapeMenuState(EscapeMenuState.Mouse);
        }
        else if (b == tabControls)
        {
            SetEscapeMenuState(EscapeMenuState.Controls);
        }
        else if (b == tabAccessibility)
        {
            SetEscapeMenuState(EscapeMenuState.Accessibility);
        }
        else if (b == tabSound)
        {
            SetEscapeMenuState(EscapeMenuState.Sound);
        }
        else if (b == tabInterface)
        {
            SetEscapeMenuState(EscapeMenuState.Interface);
        }
        else if (b == tabDev)
        {
            SetEscapeMenuState(EscapeMenuState.Dev);
        }
        else if (b == tabBack)
        {
            SaveOptions();
            SetEscapeMenuState(EscapeMenuState.Main);
        }
    }

    Button graphicsOptionSmoothShadows;
    Button graphicsOptionDarkenSides;
    Button graphicsViewDistanceOption;
    Button graphicsOptionFramerate;
    Button graphicsOptionResolution;
    Button graphicsOptionFullscreen;
    Button graphicsUseServerTexturesOption;
    Button graphicsFontOption;
    Button graphicsReturnToOptionsMenu;
    void GraphicsSet()
    {
        OptionsCi options = game.options;
        LanguageCi language = game.language;
        graphicsOptionSmoothShadows = new Button();
        graphicsOptionSmoothShadows.Text = game.platform.StringFormat(language.OptionSmoothShadows(), options.Smoothshadows ? language.On() : language.Off());
        graphicsOptionDarkenSides = new Button();
        graphicsOptionDarkenSides.Text = game.platform.StringFormat(language.Get("OptionDarkenSides"), options.EnableBlockShadow ? language.On() : language.Off());
        graphicsViewDistanceOption = new Button();
        graphicsViewDistanceOption.Text = game.platform.StringFormat(language.ViewDistanceOption(), game.platform.IntToString(game.platform.FloatToInt(game.d_Config3d.viewdistance)));
        graphicsOptionFramerate = new Button();
        graphicsOptionFramerate.Text = game.platform.StringFormat(language.OptionFramerate(), (VsyncString()));
        graphicsOptionResolution = new Button();
        graphicsOptionResolution.Text = game.platform.StringFormat(language.OptionResolution(), (ResolutionString()));
        graphicsOptionFullscreen = new Button();
        graphicsOptionFullscreen.Text = game.platform.StringFormat(language.OptionFullscreen(), options.Fullscreen ? language.On() : language.Off());
        graphicsUseServerTexturesOption = new Button();
        graphicsUseServerTexturesOption.Text = game.platform.StringFormat(language.UseServerTexturesOption(), (options.UseServerTextures ? language.On() : language.Off()));
        graphicsFontOption = new Button();
        graphicsFontOption.Text = game.platform.StringFormat(language.FontOption(), (FontString()));
        graphicsReturnToOptionsMenu = new Button();
        graphicsReturnToOptionsMenu.Text = language.ReturnToOptionsMenu();

        WidgetsClear();
        AddWidget(graphicsOptionSmoothShadows);
        AddWidget(graphicsOptionDarkenSides);
        AddWidget(graphicsViewDistanceOption);
        AddWidget(graphicsOptionFramerate);
        AddWidget(graphicsOptionResolution);
        AddWidget(graphicsOptionFullscreen);
        AddWidget(graphicsUseServerTexturesOption);
        AddWidget(graphicsFontOption);
        AddWidget(graphicsReturnToOptionsMenu);
    }
    void GraphicsHandleClick(Button b)
    {
        OptionsCi options = game.options;
        if (b == graphicsOptionSmoothShadows)
        {
            options.Smoothshadows = !options.Smoothshadows;
            game.d_TerrainChunkTesselator.EnableSmoothLight = options.Smoothshadows;
            if (options.Smoothshadows)
            {
                options.BlockShadowSave = one * 7 / 10;
                game.d_TerrainChunkTesselator.BlockShadow = options.BlockShadowSave;
            }
            else
            {
                options.BlockShadowSave = one * 6 / 10;
                game.d_TerrainChunkTesselator.BlockShadow = options.BlockShadowSave;
            }
            game.RedrawAllBlocks();
        }
        if (b == graphicsOptionDarkenSides)
        {
            options.EnableBlockShadow = !options.EnableBlockShadow;
            game.d_TerrainChunkTesselator.option_DarkenBlockSides = options.EnableBlockShadow;
            game.RedrawAllBlocks();
        }
        if (b == graphicsViewDistanceOption)
        {
            game.ToggleFog();
        }
        if (b == graphicsOptionFramerate)
        {
            game.ToggleVsync();
        }
        if (b == graphicsOptionResolution)
        {
            ToggleResolution();
        }
        if (b == graphicsOptionFullscreen)
        {
            options.Fullscreen = !options.Fullscreen;
        }
        if (b == graphicsUseServerTexturesOption)
        {
            options.UseServerTextures = !options.UseServerTextures;
        }
        if (b == graphicsFontOption)
        {
            ToggleFont();
        }
        if (b == graphicsReturnToOptionsMenu)
        {
            UseFullscreen(); UseResolution(); SetEscapeMenuState(EscapeMenuState.Options);
        }
    }

    Button otherSoundOption;
    Button otherReturnToOptionsMenu;
    Button otherAutoJumpOption;
    Button otherLanguageSetting;
    void OtherSet()
    {
        LanguageCi language = game.language;

        otherSoundOption = new Button();
        otherSoundOption.Text = game.platform.StringFormat(language.SoundOption(), (game.AudioEnabled ? language.On() : language.Off()));
        otherAutoJumpOption = new Button();
        otherAutoJumpOption.Text = game.platform.StringFormat(language.AutoJumpOption(), (game.AutoJumpEnabled ? language.On() : language.Off()));
        otherLanguageSetting = new Button();
        otherLanguageSetting.Text = game.platform.StringFormat(language.ClientLanguageOption(), language.GetUsedLanguage());
        otherReturnToOptionsMenu = new Button();
        otherReturnToOptionsMenu.Text = language.ReturnToOptionsMenu();

        WidgetsClear();
        AddWidget(otherSoundOption);
        AddWidget(otherAutoJumpOption);
        AddWidget(otherLanguageSetting);
        AddWidget(otherReturnToOptionsMenu);
    }

    void OtherHandleClick(Button b)
    {
        if (b == otherSoundOption)
        {
            game.AudioEnabled = !game.AudioEnabled;
        }
        if (b == otherAutoJumpOption)
        {
            game.AutoJumpEnabled = !game.AutoJumpEnabled;
        }
        if (b == otherLanguageSetting)
        {
            //Switch language based on available languages
            game.language.NextLanguage();
        }
        if (b == otherReturnToOptionsMenu)
        {
            SetEscapeMenuState(EscapeMenuState.Options);
        }
    }


    Button[] keyButtons;
    Button keysDefaultKeys;
    Button keysReturnToOptionsMenu;

    const int keyButtonsCount = 1024;
    void KeysSet()
    {
        LanguageCi language = game.language;

        keyButtons = new Button[keyButtonsCount];
        for (int i = 0; i < keyButtonsCount; i++)
        {
            keyButtons[i] = null;
        }

        KeyHelp[] helps = keyhelps();
        for (int i = 0; i < keyButtonsCount; i++)
        {
            if (helps[i] == null)
            {
                break;
            }
            int defaultkey = helps[i].DefaultKey;
            int key = defaultkey;
            if (game.options.Keys[defaultkey] != 0)
            {
                key = game.options.Keys[defaultkey];
            }
            keyButtons[i] = new Button();
            keyButtons[i].Text = game.platform.StringFormat2(language.KeyChange(), helps[i].Text, KeyName(key));
            AddWidget(keyButtons[i]);

        }
        keysDefaultKeys = new Button();
        keysDefaultKeys.Text = language.DefaultKeys();
        keysReturnToOptionsMenu = new Button();
        keysReturnToOptionsMenu.Text = language.ReturnToOptionsMenu();
        AddWidget(keysDefaultKeys);
        AddWidget(keysReturnToOptionsMenu);
    }

    void KeysHandleClick(Button b)
    {
        if (keyButtons != null)
        {
            for (int i = 0; i < keyButtonsCount; i++)
            {
                if (keyButtons[i] == b)
                {
                    keyselectid = i;
                }
            }
        }
        if (b == keysDefaultKeys)
        {
            game.options.Keys = new int[256];
        }
        if (b == keysReturnToOptionsMenu)
        {
            SetEscapeMenuState(EscapeMenuState.Options);
        }
    }

    // Mouse tab
    Button mouseCheckboxSneakSprint;
    void MouseSet()
    {
        LanguageCi language = game.language;
        mouseCheckboxSneakSprint = new Button();
        mouseCheckboxSneakSprint.Text = "Mouse click modifiers locked to Sneak/Sprint keys";
        
        WidgetsClear();
        AddWidget(mouseCheckboxSneakSprint);
    }
    
    void MouseHandleClick(Button b)
    {
        // Placeholder for mouse settings
    }

    // Controls tab (main one from the image)
    Button[] controlButtons;
    Button controlsResetButton;
    Button controlsMacroEditorButton;
    
    void ControlsSet()
    {
        LanguageCi language = game.language;
        
        controlButtons = new Button[keyButtonsCount];
        for (int i = 0; i < keyButtonsCount; i++)
        {
            controlButtons[i] = null;
        }
        
        KeyHelp[] helps = keyhelps();
        for (int i = 0; i < keyButtonsCount; i++)
        {
            if (helps[i] == null)
            {
                break;
            }
            int defaultkey = helps[i].DefaultKey;
            int key = defaultkey;
            if (game.options.Keys[defaultkey] != 0)
            {
                key = game.options.Keys[defaultkey];
            }
            controlButtons[i] = new Button();
            controlButtons[i].Text = game.platform.StringFormat2("{0}: {1}", helps[i].Text, KeyName(key));
            AddWidget(controlButtons[i]);
        }
        
        controlsResetButton = new Button();
        controlsResetButton.Text = "Reset Controls";
        controlsMacroEditorButton = new Button();
        controlsMacroEditorButton.Text = "Open Macro Editor";
        
        AddWidget(controlsResetButton);
        AddWidget(controlsMacroEditorButton);
    }
    
    void ControlsHandleClick(Button b)
    {
        if (controlButtons != null)
        {
            for (int i = 0; i < keyButtonsCount; i++)
            {
                if (controlButtons[i] == b)
                {
                    keyselectid = i;
                }
            }
        }
        if (b == controlsResetButton)
        {
            game.options.Keys = new int[256];
        }
        if (b == controlsMacroEditorButton)
        {
            // Placeholder for macro editor
        }
    }

    // Accessibility tab
    void AccessibilitySet()
    {
        WidgetsClear();
        // Placeholder for accessibility options
    }
    
    void AccessibilityHandleClick(Button b)
    {
        // Placeholder
    }

    // Sound tab
    Button soundVolumeOption;
    void SoundSet()
    {
        LanguageCi language = game.language;
        soundVolumeOption = new Button();
        soundVolumeOption.Text = game.platform.StringFormat(language.SoundOption(), (game.AudioEnabled ? language.On() : language.Off()));
        
        WidgetsClear();
        AddWidget(soundVolumeOption);
    }
    
    void SoundHandleClick(Button b)
    {
        if (b == soundVolumeOption)
        {
            game.AudioEnabled = !game.AudioEnabled;
        }
    }

    // Interface tab
    void InterfaceSet()
    {
        WidgetsClear();
        // Placeholder for interface options
    }
    
    void InterfaceHandleClick(Button b)
    {
        // Placeholder
    }

    // Dev tab
    void DevSet()
    {
        WidgetsClear();
        // Placeholder for developer options
    }
    
    void DevHandleClick(Button b)
    {
        // Placeholder
    }

    void HandleButtonClick(Button w)
    {
        MainHandleClick(w);
        OptionsHandleClick(w);
        GraphicsHandleClick(w);
        OtherHandleClick(w);
        KeysHandleClick(w);
        HandleTabClick(w);
        MouseHandleClick(w);
        ControlsHandleClick(w);
        AccessibilityHandleClick(w);
        SoundHandleClick(w);
        InterfaceHandleClick(w);
        DevHandleClick(w);
    }

    void AddWidget(Button b)
    {
        widgets[widgetsCount++] = b;
    }

    void WidgetsClear()
    {
        widgetsCount = 0;
    }

    internal Game game;
    EscapeMenuState escapemenustate;
    
    bool IsTabbed(EscapeMenuState state)
    {
        return state == EscapeMenuState.Graphics 
            || state == EscapeMenuState.Mouse 
            || state == EscapeMenuState.Controls 
            || state == EscapeMenuState.Accessibility 
            || state == EscapeMenuState.Sound 
            || state == EscapeMenuState.Interface 
            || state == EscapeMenuState.Dev;
    }
    
    bool IsActiveTab(int tabIndex, EscapeMenuState state)
    {
        if (tabIndex == 0 && state == EscapeMenuState.Graphics) { return true; }
        else if (tabIndex == 1 && state == EscapeMenuState.Mouse) { return true; }
        else if (tabIndex == 2 && state == EscapeMenuState.Controls) { return true; }
        else if (tabIndex == 3 && state == EscapeMenuState.Accessibility) { return true; }
        else if (tabIndex == 4 && state == EscapeMenuState.Sound) { return true; }
        else if (tabIndex == 5 && state == EscapeMenuState.Interface) { return true; }
        else if (tabIndex == 6 && state == EscapeMenuState.Dev) { return true; }
        return false;
    }
    void EscapeMenuMouse1()
    {
        // Check tabs first
        if (tabButtons != null)
        {
            for (int i = 0; i < tabButtonsCount; i++)
            {
                Button tab = tabButtons[i];
                tab.selected = RectContains(tab.x, tab.y, tab.width, tab.height, game.mouseCurrentX, game.mouseCurrentY);
                if (tab.selected && game.mouseleftclick)
                {
                    HandleTabClick(tab);
                    return;
                }
            }
        }
        
        // Then check content widgets
        for (int i = 0; i < widgetsCount; i++)
        {
            Button w = widgets[i];
            w.selected = RectContains(w.x, w.y, w.width, w.height, game.mouseCurrentX, game.mouseCurrentY);
            if (w.selected && game.mouseleftclick)
            {
                HandleButtonClick(w);
                break;
            }
        }
    }

    bool RectContains(int x, int y, int w, int h, int px, int py)
    {
        return px >= x
            && py >= y
            && px < x + w
            && py < y + h;
    }

    void SetEscapeMenuState(EscapeMenuState state)
    {
        LanguageCi language = game.language;
        escapemenustate = state;
        WidgetsClear();
        if (state == EscapeMenuState.Main)
        {
            MainSet();
            MakeSimpleOptions(fontEscapeMenu, 50);
        }
        else if (state == EscapeMenuState.Options)
        {
            OptionsSet();
            MakeSimpleOptions(fontEscapeMenu, 50);
        }
        else if (state == EscapeMenuState.Graphics)
        {
            InitTabButtons();
            GraphicsSet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Mouse)
        {
            InitTabButtons();
            MouseSet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Controls)
        {
            InitTabButtons();
            ControlsSet();
            FontCi fontKeys = FontCi.Create("Arial", 12, 0);
            MakeTabLayout(fontKeys, CONTROLS_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Accessibility)
        {
            InitTabButtons();
            AccessibilitySet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Sound)
        {
            InitTabButtons();
            SoundSet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Interface)
        {
            InitTabButtons();
            InterfaceSet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Dev)
        {
            InitTabButtons();
            DevSet();
            MakeTabLayout(fontEscapeMenu, STANDARD_TEXT_HEIGHT);
        }
        else if (state == EscapeMenuState.Other)
        {
            OtherSet();
            MakeSimpleOptions(fontEscapeMenu, 50);
        }
        else if (state == EscapeMenuState.Keys)
        {
            KeysSet();
            FontCi fontKeys = FontCi.Create("Arial", 12, 0);
            int textheight = 20;
            MakeSimpleOptions(fontKeys, textheight);
        }
    }

    void UseFullscreen()
    {
        if (game.options.Fullscreen)
        {
            if (!changedResolution)
            {
                originalResolutionWidth = game.platform.GetDisplayResolutionDefault().Width;
                originalResolutionHeight = game.platform.GetDisplayResolutionDefault().Height;
                changedResolution = true;
            }
            game.platform.SetWindowState(WindowState.Fullscreen);
            UseResolution();
        }
        else
        {
            game.platform.SetWindowState(WindowState.Normal);
            RestoreResolution();
        }
    }

    string VsyncString()
    {
        if (game.ENABLE_LAG == 0) { return "Vsync"; }
        else if (game.ENABLE_LAG == 1) { return "Unlimited"; }
        else if (game.ENABLE_LAG == 2) { return "Lag"; }
        else return null; //throw new Exception();
    }

    string ResolutionString()
    {
        IntRef resolutionsCount = new IntRef();
        DisplayResolutionCi res = game.platform.GetDisplayResolutions(resolutionsCount)[game.options.Resolution];
        return game.platform.StringFormat4("{0}x{1}, {2}, {3} Hz",
            game.platform.IntToString(res.Width),
            game.platform.IntToString(res.Height),
            game.platform.IntToString(res.BitsPerPixel),
            game.platform.IntToString(game.platform.FloatToInt(res.RefreshRate)));
    }

    void ToggleResolution()
    {
        OptionsCi options = game.options;
        options.Resolution++;

        IntRef resolutionsCount = new IntRef();
        game.platform.GetDisplayResolutions(resolutionsCount);

        if (options.Resolution >= resolutionsCount.value)
        {
            options.Resolution = 0;
        }
    }

    int originalResolutionWidth;
    int originalResolutionHeight;
    bool changedResolution;
    public void RestoreResolution()
    {
        if (changedResolution)
        {
            game.platform.ChangeResolution(originalResolutionWidth, originalResolutionHeight, 32, -1);
            changedResolution = false;
        }
    }
    public void UseResolution()
    {
        OptionsCi options = game.options;
        IntRef resolutionsCount = new IntRef();
        DisplayResolutionCi[] resolutions = game.platform.GetDisplayResolutions(resolutionsCount);

        if (resolutions == null)
        {
            return;
        }

        if (options.Resolution >= resolutionsCount.value)
        {
            options.Resolution = 0;
        }
        DisplayResolutionCi res = resolutions[options.Resolution];
        if (game.platform.GetWindowState() == WindowState.Fullscreen)
        {
            game.platform.ChangeResolution(res.Width, res.Height, res.BitsPerPixel, res.RefreshRate);
            game.platform.SetWindowState(WindowState.Normal);
            game.platform.SetWindowState(WindowState.Fullscreen);
        }
        else
        {
            //d_GlWindow.Width = res.Width;
            //d_GlWindow.Height = res.Height;
        }
    }

    string[] fonts;
    int fontsLength;
    int[] fontValues;

    string FontString()
    {
        return fonts[game.options.Font];
    }
    void ToggleFont()
    {
        OptionsCi options = game.options;
        options.Font++;
        if (options.Font >= fontsLength)
        {
            options.Font = 0;
        }
        game.Font = fontValues[options.Font];
        game.UpdateTextRendererFont();
        for (int i = 0; i < game.cachedTextTexturesMax; i++)
        {
            game.cachedTextTextures[i] = null;
        }
    }

    string KeyName(int key)
    {
        return game.platform.KeyName(key);
    }

    void MakeSimpleOptions(FontCi font, int textheight)
    {
        int starty = game.ycenter(widgetsCount * textheight);
        for (int i = 0; i < widgetsCount; i++)
        {
            string s = widgets[i].Text;
            float sizeWidth = game.TextSizeWidth(s, font);
            float sizeHeight = game.TextSizeHeight(s, font);
            int Width = game.platform.FloatToInt(sizeWidth) + 10;
            int Height = game.platform.FloatToInt(sizeHeight);
            int X = game.xcenter(sizeWidth);
            int Y = starty + textheight * i;
            widgets[i].x = X;
            widgets[i].y = Y;
            widgets[i].width = Width;
            widgets[i].height = Height;
            widgets[i].font = font;
            if (i == keyselectid)
            {
                widgets[i].fontcolor = Game.ColorFromArgb(255, 0, 255, 0);
                widgets[i].fontcolorselected = Game.ColorFromArgb(255, 0, 255, 0);
            }
        }
    }
    
    void MakeTabLayout(FontCi font, int textheight)
    {
        FontCi tabFont = FontCi.Create("Arial", TAB_FONT_SIZE, 0);
        int tabHeight = 30;
        int tabStartY = TAB_PANEL_Y + 5;
        int tabSpacing = 5;
        
        // Calculate total width needed for all tabs
        int totalTabWidth = 0;
        for (int i = 0; i < tabButtonsCount; i++)
        {
            string s = tabButtons[i].Text;
            float sizeWidth = game.TextSizeWidth(s, tabFont);
            totalTabWidth = totalTabWidth + game.platform.FloatToInt(sizeWidth) + 20 + tabSpacing;
        }
        
        // Position tabs across the top
        int tabStartX = game.xcenter(totalTabWidth);
        int currentX = tabStartX;
        
        for (int i = 0; i < tabButtonsCount; i++)
        {
            string s = tabButtons[i].Text;
            float sizeWidth = game.TextSizeWidth(s, tabFont);
            int Width = game.platform.FloatToInt(sizeWidth) + 20;
            
            tabButtons[i].x = currentX;
            tabButtons[i].y = tabStartY;
            tabButtons[i].width = Width;
            tabButtons[i].height = tabHeight;
            tabButtons[i].font = tabFont;
            
            // Highlight selected tab
            if (IsActiveTab(i, escapemenustate))
            {
                tabButtons[i].fontcolor = Game.ColorFromArgb(255, 255, 215, 0);
                tabButtons[i].fontcolorselected = Game.ColorFromArgb(255, 255, 215, 0);
            }
            else
            {
                tabButtons[i].fontcolor = Game.ColorFromArgb(255, 180, 180, 180);
                tabButtons[i].fontcolorselected = Game.ColorFromArgb(255, 255, 255, 255);
            }
            
            currentX = currentX + Width + tabSpacing;
        }
        
        // Position content widgets below tabs
        int contentStartY = TAB_PANEL_Y + TAB_PANEL_HEIGHT + 45;
        int contentLeftMargin = CONTENT_PANEL_MARGIN + 70;
        
        for (int i = 0; i < widgetsCount; i++)
        {
            string s = widgets[i].Text;
            float sizeWidth = game.TextSizeWidth(s, font);
            float sizeHeight = game.TextSizeHeight(s, font);
            int Width = game.platform.FloatToInt(sizeWidth) + 10;
            int Height = game.platform.FloatToInt(sizeHeight);
            int X = contentLeftMargin;
            int Y = contentStartY + textheight * i;
            widgets[i].x = X;
            widgets[i].y = Y;
            widgets[i].width = Width;
            widgets[i].height = Height;
            widgets[i].font = font;
            if (i == keyselectid)
            {
                widgets[i].fontcolor = Game.ColorFromArgb(255, 0, 255, 0);
                widgets[i].fontcolorselected = Game.ColorFromArgb(255, 0, 255, 0);
            }
        }
    }
    
    bool loaded;
    public override void OnNewFrameDraw2d(Game game_, float deltaTime)
    {
        game = game_;
        if (!loaded)
        {
            loaded = true;
            LoadOptions();
        }
        if (game.escapeMenuRestart)
        {
            game.escapeMenuRestart = false;
            SetEscapeMenuState(EscapeMenuState.Main);
        }
        if (game.guistate != GuiState.EscapeMenu)
        {
            return;
        }
        SetEscapeMenuState(escapemenustate);
        EscapeMenuMouse1();
        
        // Draw tabs if in a tabbed state
        if (IsTabbed(escapemenustate) && tabButtons != null)
        {
            // Draw content panel background using new assembled GUI panel
            int contentPanelY = TAB_PANEL_Y + TAB_PANEL_HEIGHT + 5;
            int contentPanelHeight = game.Height() - contentPanelY - 50;
            int contentPanelWidth = game.Width() - CONTENT_PANEL_MARGIN * 2;
            
            // Use the large panel from assembled GUI pieces
            string panelPath = "data/themes/default/assembled_gui/menus/panel_long_titled.png";
            game.Draw2dBitmapFile(panelPath, CONTENT_PANEL_MARGIN, contentPanelY, 
                contentPanelWidth, contentPanelHeight);
            
            // Draw tabs using golden buttons with new assembled GUI style
            for (int i = 0; i < tabButtonsCount; i++)
            {
                Button tab = tabButtons[i];
                bool tabIsActive = IsActiveTab(i, escapemenustate);
                
                // Determine button appearance - use lighter grey for selected (as per problem statement)
                if (tabIsActive)
                {
                    // Active tab - lighter grey with golden highlight
                    int lightGreyColor = Game.ColorFromArgb(220, 150, 145, 140);
                    game.Draw2dTexture(game.WhiteTexture(), tab.x, tab.y, tab.width, tab.height, 
                        null, 0, lightGreyColor, false);
                    
                    // Golden border for active tab
                    int borderColor = Game.ColorFromArgb(255, 184, 134, 11);
                    DrawBorder(tab.x, tab.y, tab.width, tab.height, 2, borderColor);
                }
                else if (tab.selected)
                {
                    // Hover state - medium grey
                    int hoverColor = Game.ColorFromArgb(200, 120, 115, 110);
                    game.Draw2dTexture(game.WhiteTexture(), tab.x, tab.y, tab.width, tab.height, 
                        null, 0, hoverColor, false);
                }
                else
                {
                    // Normal state - darker
                    int normalColor = Game.ColorFromArgb(180, 70, 70, 70);
                    game.Draw2dTexture(game.WhiteTexture(), tab.x, tab.y, tab.width, tab.height, 
                        null, 0, normalColor, false);
                }
                
                // Draw tab text
                game.Draw2dText(tab.Text, tab.font, tab.x + 10, tab.y + 8, 
                    IntRef.Create(tab.selected ? tab.fontcolorselected : tab.fontcolor), false);
            }
        }
        
        // Draw content widgets
        for (int i = 0; i < widgetsCount; i++)
        {
            Button w = widgets[i];
            game.Draw2dText(w.Text, w.font, w.x, w.y, IntRef.Create(w.selected ? w.fontcolorselected : w.fontcolor), false);
        }
    }
    
    void DrawBorder(int x, int y, int width, int height, int thickness, int color)
    {
        // Top
        game.Draw2dTexture(game.WhiteTexture(), x, y, width, thickness, null, 0, color, false);
        // Bottom
        game.Draw2dTexture(game.WhiteTexture(), x, y + height - thickness, width, thickness, null, 0, color, false);
        // Left
        game.Draw2dTexture(game.WhiteTexture(), x, y, thickness, height, null, 0, color, false);
        // Right
        game.Draw2dTexture(game.WhiteTexture(), x + width - thickness, y, thickness, height, null, 0, color, false);
    }
    Button[] widgets;
    KeyHelp[] keyhelps()
    {
        int n = 1024;
        KeyHelp[] helps = new KeyHelp[n];
        for (int i = 0; i < n; i++)
        {
            helps[i] = null;
        }
        LanguageCi language = game.language;
        int count = 0;
        helps[count++] = KeyHelpCreate(language.KeyMoveFoward(), GlKeys.W);
        helps[count++] = KeyHelpCreate(language.KeyMoveBack(), GlKeys.S);
        helps[count++] = KeyHelpCreate(language.KeyMoveLeft(), GlKeys.A);
        helps[count++] = KeyHelpCreate(language.KeyMoveRight(), GlKeys.D);
        helps[count++] = KeyHelpCreate(language.KeyJump(), GlKeys.Space);
        helps[count++] = KeyHelpCreate(language.KeyShowMaterialSelector(), GlKeys.B);
        helps[count++] = KeyHelpCreate(language.KeySetSpawnPosition(), GlKeys.P);
        helps[count++] = KeyHelpCreate(language.KeyRespawn(), GlKeys.O);
        helps[count++] = KeyHelpCreate(language.KeyReloadWeapon(), GlKeys.R);
        helps[count++] = KeyHelpCreate(language.KeyToggleFogDistance(), GlKeys.F);
        helps[count++] = KeyHelpCreate(game.platform.StringFormat(language.KeyMoveSpeed(), "1"), GlKeys.F1);
        helps[count++] = KeyHelpCreate(game.platform.StringFormat(language.KeyMoveSpeed(), "10"), GlKeys.F2);
        helps[count++] = KeyHelpCreate(language.KeyFreeMove(), GlKeys.F3);
        helps[count++] = KeyHelpCreate(language.KeyThirdPersonCamera(), GlKeys.F5);
        helps[count++] = KeyHelpCreate(language.KeyTextEditor(), GlKeys.F9);
        helps[count++] = KeyHelpCreate(language.KeyFullscreen(), GlKeys.F11);
        helps[count++] = KeyHelpCreate(language.KeyScreenshot(), GlKeys.F12);
        helps[count++] = KeyHelpCreate(language.KeyPlayersList(), GlKeys.Tab);
        helps[count++] = KeyHelpCreate(language.KeyChat(), GlKeys.T);
        helps[count++] = KeyHelpCreate(language.KeyTeamChat(), GlKeys.Y);
        helps[count++] = KeyHelpCreate(language.KeyCraft(), GlKeys.C);
        helps[count++] = KeyHelpCreate(language.KeyBlockInfo(), GlKeys.I);
        helps[count++] = KeyHelpCreate(language.KeyUse(), GlKeys.E);
        helps[count++] = KeyHelpCreate(language.KeyReverseMinecart(), GlKeys.Q);
        return helps;
    }

    KeyHelp KeyHelpCreate(string text, int defaultKey)
    {
        KeyHelp h = new KeyHelp();
        h.Text = text;
        h.DefaultKey = defaultKey;
        return h;
    }


    int keyselectid;
    public override void OnKeyDown(Game game_, KeyEventArgs args)
    {
        game = game_;
        int eKey = args.GetKeyCode();
        if (eKey == game.GetKey(GlKeys.Escape))
        {
            if (IsTabbed(escapemenustate) 
                || escapemenustate == EscapeMenuState.Keys
                || escapemenustate == EscapeMenuState.Other
                || escapemenustate == EscapeMenuState.Options)
            {
                SaveOptions();
                SetEscapeMenuState(EscapeMenuState.Main);
            }
            else
            {
                SetEscapeMenuState(EscapeMenuState.Main);
                game.GuiStateBackToGame();
            }
            args.SetHandled(true);
        }
        if (escapemenustate == EscapeMenuState.Keys || escapemenustate == EscapeMenuState.Controls)
        {
            if (keyselectid != -1)
            {
                game.options.Keys[keyhelps()[keyselectid].DefaultKey] = eKey;
                keyselectid = -1;
                args.SetHandled(true);
            }
        }
        if (eKey == game.GetKey(GlKeys.F11))
        {
            if (game.platform.GetWindowState() == WindowState.Fullscreen)
            {
                game.platform.SetWindowState(WindowState.Normal);
                RestoreResolution();
                SaveOptions();
            }
            else
            {
                game.platform.SetWindowState(WindowState.Fullscreen);
                UseResolution();
                SaveOptions();
            }
            args.SetHandled(true);
        }
    }
    public void LoadOptions()
    {
        OptionsCi o = LoadOptions_();
        if (o == null)
        {
            return;
        }
        game.options = o;
        OptionsCi options = o;

        game.Font = fontValues[options.Font];
        game.UpdateTextRendererFont();
        //game.d_CurrentShadows.ShadowsFull = options.Shadows;
        game.d_Config3d.viewdistance = options.DrawDistance;
        game.AudioEnabled = options.EnableSound;
        game.AutoJumpEnabled = options.EnableAutoJump;
        if (options.ClientLanguage != "")
        {
            game.language.OverrideLanguage = options.ClientLanguage;
        }
        game.d_TerrainChunkTesselator.EnableSmoothLight = options.Smoothshadows;
        game.d_TerrainChunkTesselator.BlockShadow = options.BlockShadowSave;
        game.d_TerrainChunkTesselator.option_DarkenBlockSides = options.EnableBlockShadow;
        game.ENABLE_LAG = options.Framerate;
        UseFullscreen();
        game.UseVsync();
        UseResolution();
    }

    OptionsCi LoadOptions_()
    {
        OptionsCi options = new OptionsCi();
        Preferences preferences = game.platform.GetPreferences();
                
        options.Shadows = preferences.GetBool("Shadows", true);
        options.Font = preferences.GetInt("Font", 0);
        options.DrawDistance = preferences.GetInt("DrawDistance", game.platform.IsFastSystem() ? 128 : 32);
        options.UseServerTextures = preferences.GetBool("UseServerTextures", true);
        options.EnableSound = preferences.GetBool("EnableSound", true);
        options.EnableAutoJump = preferences.GetBool("EnableAutoJump", false);
        options.ClientLanguage = preferences.GetString("ClientLanguage", "");
        options.Framerate = preferences.GetInt("Framerate", 0);
        options.Resolution = preferences.GetInt("Resolution", 0);
        options.Fullscreen = preferences.GetBool("Fullscreen", false);
        options.Smoothshadows = preferences.GetBool("Smoothshadows", true);
        options.BlockShadowSave = one * preferences.GetInt("BlockShadowSave", 70) / 100;
        options.EnableBlockShadow = preferences.GetBool("EnableBlockShadow", true);

        for (int i = 0; i < 256; i++)
        {
            string preferencesKey = StringTools.StringAppend(game.platform, "Key", game.platform.IntToString(i));
            int value = preferences.GetInt(preferencesKey, 0);
            if (value != 0)
            {
                options.Keys[i] = value;
            }
        }

        return options;
    }

    public void SaveOptions()
    {
        OptionsCi options = game.options;

        options.Font = game.Font;
        options.Shadows = true; // game.d_CurrentShadows.ShadowsFull;
        options.DrawDistance = game.platform.FloatToInt(game.d_Config3d.viewdistance);
        options.EnableSound = game.AudioEnabled;
        options.EnableAutoJump = game.AutoJumpEnabled;
        if (game.language.OverrideLanguage != null)
        {
            options.ClientLanguage = game.language.OverrideLanguage;
        }
        options.Framerate = game.ENABLE_LAG;
        options.Fullscreen = game.platform.GetWindowState() == WindowState.Fullscreen;
        options.Smoothshadows = game.d_TerrainChunkTesselator.EnableSmoothLight;
        options.EnableBlockShadow = game.d_TerrainChunkTesselator.option_DarkenBlockSides;

        SaveOptions_(options);
    }

    void SaveOptions_(OptionsCi options)
    {
        Preferences preferences = game.platform.GetPreferences();

        preferences.SetBool("Shadows", options.Shadows);
        preferences.SetInt("Font", options.Font);
        preferences.SetInt("DrawDistance", options.DrawDistance);
        preferences.SetBool("UseServerTextures", options.UseServerTextures);
        preferences.SetBool("EnableSound", options.EnableSound);
        preferences.SetBool("EnableAutoJump", options.EnableAutoJump);
        if (options.ClientLanguage != "")
        {
            preferences.SetString("ClientLanguage", options.ClientLanguage);
        }
        preferences.SetInt("Framerate", options.Framerate);
        preferences.SetInt("Resolution", options.Resolution);
        preferences.SetBool("Fullscreen", options.Fullscreen);
        preferences.SetBool("Smoothshadows", options.Smoothshadows);
        preferences.SetInt("BlockShadowSave", game.platform.FloatToInt(options.BlockShadowSave * 100));
        preferences.SetBool("EnableBlockShadow", options.EnableBlockShadow);

        for (int i = 0; i < 256; i++)
        {
            int value = options.Keys[i];string preferencesKey = StringTools.StringAppend(game.platform, "Key", game.platform.IntToString(i));
            if (value != 0)
            {
                preferences.SetInt(preferencesKey, value);
            }
            else
            {
                preferences.Remove(preferencesKey);
            }
        }

        game.platform.SetPreferences(preferences);
    }
}

public class Button
{
    public Button()
    {
        fontcolor = Game.ColorFromArgb(255, 255, 255, 255);
        fontcolorselected = Game.ColorFromArgb(255, 255, 0, 0);
        font = new FontCi();
    }
    internal int x;
    internal int y;
    internal int width;
    internal int height;
    internal string Text;
    internal bool selected;
    internal FontCi font;
    internal int fontcolor;
    internal int fontcolorselected;
}

public class KeyHelp
{
    internal string Text;
    internal int DefaultKey;
}

public class DisplayResolutionCi
{
    public int Width;
    public int Height;
    public int BitsPerPixel;
    public float RefreshRate;
}
