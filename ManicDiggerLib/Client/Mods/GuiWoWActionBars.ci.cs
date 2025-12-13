public class ModGuiWoWActionBars : ClientMod
{
    public ModGuiWoWActionBars()
    {
        buttonStates = new int[10]; // 0=normal, 1=hover, 2=pressed
        for (int i = 0; i < 10; i++)
        {
            buttonStates[i] = 0;
        }
    }

    internal Game game;
    internal GameDataItemsClient dataItems;
    internal InventoryUtilClient inventoryUtil;
    internal IInventoryController controller;

    int[] buttonStates;

    // Layout constants
    int ActionBarStartX() { return game.Width() / 2 - 512; }
    int ActionBarStartY() { return game.Height() - 120; }
    int ButtonSize() { return game.platform.FloatToInt(64 * game.Scale()); }
    int ButtonSpacing() { return game.platform.FloatToInt(10 * game.Scale()); }

    public override void OnKeyPress(Game game_, KeyPressEventArgs args)
    {
        if (game.guistate != GuiState.Normal)
        {
            return;
        }
        
        int keyChar = args.GetKeyChar();
        // Handle number keys 1-9 and 0 for action bar slots
        if (keyChar == 49) { game.ActiveMaterial = 0; } // '1'
        if (keyChar == 50) { game.ActiveMaterial = 1; } // '2'
        if (keyChar == 51) { game.ActiveMaterial = 2; } // '3'
        if (keyChar == 52) { game.ActiveMaterial = 3; } // '4'
        if (keyChar == 53) { game.ActiveMaterial = 4; } // '5'
        if (keyChar == 54) { game.ActiveMaterial = 5; } // '6'
        if (keyChar == 55) { game.ActiveMaterial = 6; } // '7'
        if (keyChar == 56) { game.ActiveMaterial = 7; } // '8'
        if (keyChar == 57) { game.ActiveMaterial = 8; } // '9'
        if (keyChar == 48) { game.ActiveMaterial = 9; } // '0'
    }

    public override void OnMouseDown(Game game_, MouseEventArgs args)
    {
        if (game.guistate != GuiState.Normal)
        {
            return;
        }

        int mouseX = args.GetX();
        int mouseY = args.GetY();
        
        // Check if clicking on action bar buttons
        IntRef selectedSlot = GetActionBarSlot(mouseX, mouseY);
        if (selectedSlot != null)
        {
            game.ActiveMaterial = selectedSlot.value;
            buttonStates[selectedSlot.value] = 2; // pressed state
            
            // Send inventory click packet
            Packet_InventoryPosition p = new Packet_InventoryPosition();
            p.Type = Packet_InventoryPositionTypeEnum.MaterialSelector;
            p.MaterialId = game.ActiveMaterial;
            if (controller != null)
            {
                controller.InventoryClick(p);
            }
        }
    }

    public override void OnMouseUp(Game game_, MouseEventArgs args)
    {
        if (game.guistate != GuiState.Normal)
        {
            return;
        }

        // Reset all button states to normal or hover
        for (int i = 0; i < 10; i++)
        {
            if (buttonStates[i] == 2)
            {
                buttonStates[i] = 0;
            }
        }
    }

    public override void OnMouseMove(Game game_, MouseEventArgs args)
    {
        if (game.guistate != GuiState.Normal)
        {
            return;
        }

        int mouseX = args.GetX();
        int mouseY = args.GetY();
        
        // Update hover states
        for (int i = 0; i < 10; i++)
        {
            if (buttonStates[i] != 2) // Don't change if pressed
            {
                buttonStates[i] = 0; // Default to normal
            }
        }
        
        IntRef hoveredSlot = GetActionBarSlot(mouseX, mouseY);
        if (hoveredSlot != null && buttonStates[hoveredSlot.value] != 2)
        {
            buttonStates[hoveredSlot.value] = 1; // hover state
        }
    }

    IntRef GetActionBarSlot(int mouseX, int mouseY)
    {
        int startX = ActionBarStartX();
        int startY = ActionBarStartY();
        int buttonSize = ButtonSize();
        int spacing = ButtonSpacing();
        
        for (int i = 0; i < 10; i++)
        {
            int buttonX = startX + i * (buttonSize + spacing) + spacing;
            int buttonY = startY + 30;
            
            if (mouseX >= buttonX && mouseX < buttonX + buttonSize &&
                mouseY >= buttonY && mouseY < buttonY + buttonSize)
            {
                return IntRef.Create(i);
            }
        }
        return null;
    }

    public override void OnNewFrameDraw2d(Game game_, float deltaTime)
    {
        game = game_;
        
        if (dataItems == null)
        {
            dataItems = new GameDataItemsClient();
            dataItems.game = game_;
            controller = ClientInventoryController.Create(game_);
            inventoryUtil = game.d_InventoryUtil;
        }
        
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        DrawWoWActionBar();
    }

    void DrawWoWActionBar()
    {
        int startX = ActionBarStartX();
        int startY = ActionBarStartY();
        int buttonSize = ButtonSize();
        int spacing = ButtonSpacing();
        
        // Draw action bar background
        game.Draw2dBitmapFile("wow/actionbar_bg.png", startX, startY, 
            game.platform.FloatToInt(1024 * game.Scale()), 
            game.platform.FloatToInt(128 * game.Scale()));
        
        // Draw 10 action buttons
        for (int i = 0; i < 10; i++)
        {
            int buttonX = startX + i * (buttonSize + spacing) + spacing;
            int buttonY = startY + 30;
            
            // Draw button background based on state
            string buttonTexture = "wow/button_normal.png";
            if (buttonStates[i] == 1)
            {
                buttonTexture = "wow/button_hover.png";
            }
            else if (buttonStates[i] == 2)
            {
                buttonTexture = "wow/button_pressed.png";
            }
            
            game.Draw2dBitmapFile(buttonTexture, buttonX, buttonY, buttonSize, buttonSize);
            
            // Draw item in slot
            Packet_Item item = game.d_Inventory.RightHand[i];
            if (item != null)
            {
                DrawItem(buttonX, buttonY, item, buttonSize, buttonSize);
            }
            
            // Highlight active material slot
            if (i == game.ActiveMaterial)
            {
                // Draw golden highlight border
                game.Draw2dTexture(game.WhiteTexture(), buttonX - 3, buttonY - 3, 
                    buttonSize + 6, buttonSize + 6, null, 0, 
                    Game.ColorFromArgb(255, 255, 215, 0), false);
            }
            
            // Draw keybind number
            FontCi font = new FontCi();
            font.size = 12;
            string keybind = i == 9 ? "0" : game.platform.IntToString(i + 1);
            game.Draw2dText(keybind, font, buttonX + 4, buttonY + 4, null, false);
        }
    }

    void DrawItem(int screenposX, int screenposY, Packet_Item item, int drawsizeX, int drawsizeY)
    {
        if (item == null)
        {
            return;
        }
        
        if (item.ItemClass == Packet_ItemClassEnum.Block)
        {
            if (item.BlockId == 0)
            {
                return;
            }
            game.Draw2dTexture(game.terrainTexture, screenposX, screenposY,
                drawsizeX, drawsizeY, IntRef.Create(dataItems.TextureIdForInventory()[item.BlockId]), 
                game.texturesPacked(), Game.ColorFromArgb(255, 255, 255, 255), false);
            
            if (item.BlockCount > 1)
            {
                FontCi font = new FontCi();
                font.size = 10;
                game.Draw2dText(game.platform.IntToString(item.BlockCount), font, 
                    screenposX + 4, screenposY + drawsizeY - 14, null, false);
            }
        }
        else
        {
            game.Draw2dBitmapFile(dataItems.ItemGraphics(item), screenposX, screenposY,
                drawsizeX, drawsizeY);
        }
    }
}
