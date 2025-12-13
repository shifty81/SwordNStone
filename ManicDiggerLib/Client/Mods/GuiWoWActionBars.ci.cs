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
    // Use 48px to match slot_normal.png natural size (48x48) - avoids stretching/distortion
    int ButtonSize() { return game.platform.FloatToInt(48 * game.Scale()); }
    // Spacing reduced from 10px to 8px to maintain compact layout with smaller 48px buttons
    int ButtonSpacing() { return game.platform.FloatToInt(8 * game.Scale()); }

    public override void OnKeyPress(Game game_, KeyPressEventArgs args)
    {
        game = game_;
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
        game = game_;
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
        game = game_;
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
        game = game_;
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
        
        // Draw action bar background using dark panel
        int bgWidth = 10 * (buttonSize + spacing) + spacing * 2;
        int bgHeight = game.platform.FloatToInt(96 * game.Scale());
        game.Draw2dTexture(game.WhiteTexture(), startX - spacing, startY, 
            bgWidth, bgHeight, null, 0, 
            Game.ColorFromArgb(200, 50, 50, 50), false);
        
        // Draw 10 action buttons using standardized golden slots
        for (int i = 0; i < 10; i++)
        {
            int buttonX = startX + i * (buttonSize + spacing) + spacing;
            int buttonY = startY + 30;
            
            // Draw slot background using standardized golden UI
            bool isHighlighted = (i == game.ActiveMaterial);
            GuiFrameRenderer.DrawSlot(game, buttonX, buttonY, buttonSize, isHighlighted);
            
            // Draw item in slot (inset slightly to fit within frame)
            Packet_Item item = game.d_Inventory.RightHand[i];
            if (item != null)
            {
                int inset = game.platform.FloatToInt(6 * game.Scale());
                DrawItem(buttonX + inset, buttonY + inset, item, buttonSize - inset * 2, buttonSize - inset * 2);
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
