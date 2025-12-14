/// <summary>
/// Hotbar system for quick-access inventory slots (bottom-center HUD)
/// Based on golden steampunk theme from Assembled_Gui_Pieces.PNG
/// Will use hotbar.png when available
/// </summary>
public class ModGuiHotbar : ClientMod
{
    public ModGuiHotbar()
    {
        // Hotbar configuration
        slotCount = 10;
        slotSize = 50;
        slotSpacing = 4;
        
        // Initialize button states
        buttonStates = new int[10]; // 0=normal, 1=hover, 2=pressed
        for (int i = 0; i < 10; i++)
        {
            buttonStates[i] = 0;
        }
        
        // Check if custom hotbar.png exists
        useCustomHotbarImage = false;
    }
    
    internal Game game;
    internal GameDataItemsClient dataItems;
    internal InventoryUtilClient inventoryUtil;
    internal IInventoryController controller;
    
    internal int slotCount;
    internal int slotSize;
    internal int slotSpacing;
    internal int[] buttonStates;
    internal bool useCustomHotbarImage;
    
    // Positioning
    int HotbarStartX(Game game) 
    { 
        int totalWidth = slotCount * slotSize + (slotCount - 1) * slotSpacing + 20; // +20 for frame padding
        return (game.Width() - totalWidth) / 2;
    }
    
    int HotbarStartY(Game game) 
    { 
        return game.Height() - 100; 
    }
    
    public override void OnNewFrameDraw2d(Game game_, float deltaTime)
    {
        game = game_;
        
        if (game.guistate == GuiState.MapLoading)
        {
            return;
        }
        
        // Only show hotbar in normal gameplay mode
        if (game.guistate == GuiState.Normal)
        {
            DrawHotbar(game);
        }
    }
    
    void DrawHotbar(Game game)
    {
        float scale = game.Scale();
        int scaledSlotSize = game.platform.FloatToInt(slotSize * scale);
        int scaledSpacing = game.platform.FloatToInt(slotSpacing * scale);
        
        int startX = HotbarStartX(game);
        int startY = HotbarStartY(game);
        
        // Check if hotbar.png exists and use it as background
        if (CheckHotbarImageExists(game))
        {
            DrawCustomHotbar(game, startX, startY, scaledSlotSize, scaledSpacing);
        }
        else
        {
            DrawDefaultHotbar(game, startX, startY, scaledSlotSize, scaledSpacing);
        }
        
        // Draw items in slots
        DrawHotbarItems(game, startX, startY, scaledSlotSize, scaledSpacing);
        
        // Draw slot numbers
        DrawSlotNumbers(game, startX, startY, scaledSlotSize, scaledSpacing);
    }
    
    bool CheckHotbarImageExists(Game game)
    {
        // Check if hotbar.png exists in theme directory
        // Try both possible locations
        string[] possiblePaths = new string[3];
        possiblePaths[0] = "data/themes/default/assembled_gui/hotbar/hotbar.png";
        possiblePaths[1] = "hotbar.png";
        possiblePaths[2] = "data/hotbar.png";
        
        for (int i = 0; i < 3; i++)
        {
            // Basic file existence check - actual implementation depends on platform
            // For now, return false until hotbar.png is added
            // TODO: Replace with game.platform.FileExists(possiblePaths[i]) when available
        }
        
        return false; // Will return true once hotbar.png is available
    }
    
    void DrawCustomHotbar(Game game, int startX, int startY, int slotSize, int spacing)
    {
        // Use the custom hotbar.png image when available
        string hotbarPath = "hotbar.png"; // Will be moved to proper location
        int hotbarWidth = slotCount * slotSize + (slotCount - 1) * spacing + 20;
        int hotbarHeight = slotSize + 20;
        
        game.Draw2dBitmapFile(hotbarPath, startX - 10, startY - 10, hotbarWidth, hotbarHeight);
    }
    
    void DrawDefaultHotbar(Game game, int startX, int startY, int slotSize, int spacing)
    {
        // Draw golden frame background
        int totalWidth = slotCount * slotSize + (slotCount - 1) * spacing + 20;
        int totalHeight = slotSize + 20;
        
        // Background panel
        int bgColor = Game.ColorFromArgb(200, 40, 35, 30);
        game.Draw2dTexture(game.WhiteTexture(), startX - 10, startY - 10, 
            totalWidth, totalHeight, null, 0, bgColor, false);
        
        // Golden border (simulating the steampunk theme)
        int borderColor = Game.ColorFromArgb(255, 184, 134, 11); // Dark golden
        int borderWidth = 3;
        
        // Top border
        game.Draw2dTexture(game.WhiteTexture(), startX - 10, startY - 10, 
            totalWidth, borderWidth, null, 0, borderColor, false);
        // Bottom border
        game.Draw2dTexture(game.WhiteTexture(), startX - 10, startY + slotSize + 10 - borderWidth, 
            totalWidth, borderWidth, null, 0, borderColor, false);
        // Left border
        game.Draw2dTexture(game.WhiteTexture(), startX - 10, startY - 10, 
            borderWidth, totalHeight, null, 0, borderColor, false);
        // Right border
        game.Draw2dTexture(game.WhiteTexture(), startX + totalWidth - 10 - borderWidth, startY - 10, 
            borderWidth, totalHeight, null, 0, borderColor, false);
        
        // Draw individual slots
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (slotSize + spacing);
            int slotY = startY;
            
            DrawHotbarSlot(game, slotX, slotY, slotSize, i);
        }
    }
    
    void DrawHotbarSlot(Game game, int x, int y, int size, int slotIndex)
    {
        bool isActive = game.ActiveMaterial == slotIndex;
        bool isHovered = buttonStates[slotIndex] == 1;
        
        // Determine slot appearance based on state
        string slotTexture;
        if (isActive)
        {
            slotTexture = "data/themes/default/assembled_gui/bars/slot_active.png";
        }
        else if (isHovered)
        {
            slotTexture = "data/themes/default/assembled_gui/bars/slot_hover.png";
        }
        else
        {
            slotTexture = "data/themes/default/assembled_gui/bars/slot_normal.png";
        }
        
        game.Draw2dBitmapFile(slotTexture, x, y, size, size);
        
        // Draw slot border highlight for active slot
        if (isActive)
        {
            int highlightColor = Game.ColorFromArgb(180, 255, 215, 0);
            int borderSize = 2;
            
            // Top
            game.Draw2dTexture(game.WhiteTexture(), x, y, size, borderSize, 
                null, 0, highlightColor, false);
            // Bottom
            game.Draw2dTexture(game.WhiteTexture(), x, y + size - borderSize, size, borderSize, 
                null, 0, highlightColor, false);
            // Left
            game.Draw2dTexture(game.WhiteTexture(), x, y, borderSize, size, 
                null, 0, highlightColor, false);
            // Right
            game.Draw2dTexture(game.WhiteTexture(), x + size - borderSize, y, borderSize, size, 
                null, 0, highlightColor, false);
        }
    }
    
    void DrawHotbarItems(Game game, int startX, int startY, int slotSize, int spacing)
    {
        if (inventoryUtil == null || game.player == null || game.player.Inventory == null)
        {
            return;
        }
        
        // Draw items in hotbar slots (slots 0-9 in inventory)
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (slotSize + spacing);
            int slotY = startY;
            
            Inventory inventory = game.player.Inventory;
            int itemId = inventory.RightHand[i * Inventory.MaxItemsInStack];
            
            if (itemId > 0)
            {
                // Draw item texture
                string itemTexture = dataItems.ItemTexture(itemId);
                if (itemTexture != null)
                {
                    int itemSize = slotSize - 10; // Inset from slot border
                    int itemX = slotX + 5;
                    int itemY = slotY + 5;
                    
                    game.Draw2dBitmapFile(itemTexture, itemX, itemY, itemSize, itemSize);
                    
                    // Draw item count if stacked
                    int count = inventory.RightHand[i * Inventory.MaxItemsInStack + 1];
                    if (count > 1)
                    {
                        FontCi font = new FontCi();
                        font.size = 10;
                        string countText = game.platform.IntToString(count);
                        game.Draw2dText(countText, font, slotX + slotSize - 20, slotY + slotSize - 18, 
                            null, false);
                    }
                }
            }
        }
    }
    
    void DrawSlotNumbers(Game game, int startX, int startY, int slotSize, int spacing)
    {
        FontCi font = new FontCi();
        font.size = 11;
        
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (slotSize + spacing);
            int slotY = startY;
            
            // Display 1-9, 0 for slots (matching keyboard layout)
            string number = i == 9 ? "0" : game.platform.IntToString(i + 1);
            
            // Draw number in top-left corner of slot
            game.Draw2dText(number, font, slotX + 5, slotY + 3, null, false);
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
        float scale = game.Scale();
        int scaledSlotSize = game.platform.FloatToInt(slotSize * scale);
        int scaledSpacing = game.platform.FloatToInt(slotSpacing * scale);
        
        // Update hover states
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = HotbarStartX(game) + i * (scaledSlotSize + scaledSpacing);
            int slotY = HotbarStartY(game);
            
            if (mouseX >= slotX && mouseX <= slotX + scaledSlotSize &&
                mouseY >= slotY && mouseY <= slotY + scaledSlotSize)
            {
                buttonStates[i] = 1; // Hover
            }
            else if (buttonStates[i] == 1)
            {
                buttonStates[i] = 0; // Normal
            }
        }
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
        float scale = game.Scale();
        int scaledSlotSize = game.platform.FloatToInt(slotSize * scale);
        int scaledSpacing = game.platform.FloatToInt(slotSpacing * scale);
        
        // Check if clicking on hotbar slots
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = HotbarStartX(game) + i * (scaledSlotSize + scaledSpacing);
            int slotY = HotbarStartY(game);
            
            if (mouseX >= slotX && mouseX <= slotX + scaledSlotSize &&
                mouseY >= slotY && mouseY <= slotY + scaledSlotSize)
            {
                game.ActiveMaterial = i;
                buttonStates[i] = 2; // Pressed
                
                // Send inventory click packet
                Packet_InventoryPosition p = new Packet_InventoryPosition();
                p.Type = Packet_InventoryPositionTypeEnum.MaterialSelector;
                p.MaterialId = i;
                if (controller != null)
                {
                    controller.InventoryClick(p);
                }
                return;
            }
        }
    }
    
    public override void OnMouseUp(Game game_, MouseEventArgs args)
    {
        game = game_;
        
        // Reset pressed states to normal or hover
        for (int i = 0; i < slotCount; i++)
        {
            if (buttonStates[i] == 2)
            {
                buttonStates[i] = 0;
            }
        }
    }
    
    public override void OnKeyPress(Game game_, KeyPressEventArgs args)
    {
        game = game_;
        
        if (game.guistate != GuiState.Normal)
        {
            return;
        }
        
        int keyChar = args.GetKeyChar();
        
        // Handle number keys 1-9 and 0 for hotbar slots
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
}
