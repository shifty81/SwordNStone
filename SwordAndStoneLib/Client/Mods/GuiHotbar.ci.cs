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
    int HotbarStartX(Game game_) 
    { 
        int totalWidth = slotCount * slotSize + (slotCount - 1) * slotSpacing + 20; // +20 for frame padding
        return (game_.Width() - totalWidth) / 2;
    }
    
    int HotbarStartY(Game game_) 
    { 
        return game_.Height() - 100; 
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
    
    void DrawHotbar(Game game_)
    {
        float scale = game_.Scale();
        int scaledSlotSize = game_.platform.FloatToInt(slotSize * scale);
        int scaledSpacing = game_.platform.FloatToInt(slotSpacing * scale);
        
        int startX = HotbarStartX(game_);
        int startY = HotbarStartY(game_);
        
        // Check if hotbar.png exists and use it as background
        if (CheckHotbarImageExists(game_))
        {
            DrawCustomHotbar(game_, startX, startY, scaledSlotSize, scaledSpacing);
        }
        else
        {
            DrawDefaultHotbar(game_, startX, startY, scaledSlotSize, scaledSpacing);
        }
        
        // Draw items in slots
        DrawHotbarItems(game_, startX, startY, scaledSlotSize, scaledSpacing);
        
        // Draw slot numbers
        DrawSlotNumbers(game_, startX, startY, scaledSlotSize, scaledSpacing);
    }
    
    bool CheckHotbarImageExists(Game game_)
    {
        // Hotbar template is now integrated and available
        // Using custom hotbar.png with bronze steampunk borders
        useCustomHotbarImage = true;
        return true;
        
        // Note: If hotbar.png is removed or unavailable, set this to false
        // to fall back to DrawDefaultHotbar() using assembled GUI pieces
    }
    
    void DrawCustomHotbar(Game game_, int startX, int startY, int scaledSlotSize, int scaledSpacing)
    {
        // Use the custom hotbar template image (598x66 pixels)
        string hotbarPath = "data/hotbar.png";
        
        // Template dimensions: 598x66 pixels
        // Contains 10 slots (56x56 each with 4px borders) + 2px spacing + rivets
        // Template design: [rivet][slot][gap][slot]...[slot][rivet]
        
        // Constants derived from template
        // int templateSlotWithBorder = 56;  // 48px inner + 4px border on each side (unused)
        // int templateSlotSpacing = 2;       // Gap between slots (unused)
        int templateRivetSize = 10;        // Decorative rivet diameter
        int templateBorderOffset = 8;      // Offset for positioning (border + rivet space)
        int templateHeightOffset = 16;     // Additional height for rivets and borders
        
        // Calculate scaled dimensions for rendering
        int hotbarWidth = slotCount * (scaledSlotSize + templateBorderOffset) + (templateRivetSize * 2);
        int hotbarHeight = scaledSlotSize + templateHeightOffset;
        
        // Draw the hotbar background image centered under the slots
        game_.Draw2dBitmapFile(hotbarPath, 
            startX - templateBorderOffset, 
            startY - templateBorderOffset, 
            hotbarWidth, 
            hotbarHeight);
    }
    
    void DrawDefaultHotbar(Game game_, int startX, int startY, int scaledSlotSize, int scaledSpacing)
    {
        // Draw golden frame background
        int totalWidth = slotCount * scaledSlotSize + (slotCount - 1) * scaledSpacing + 20;
        int totalHeight = scaledSlotSize + 20;
        
        // Background panel
        int bgColor = Game.ColorFromArgb(200, 40, 35, 30);
        game_.Draw2dTexture(game_.WhiteTexture(), startX - 10, startY - 10, 
            totalWidth, totalHeight, null, 0, bgColor, false);
        
        // Golden border (simulating the steampunk theme)
        int borderColor = Game.ColorFromArgb(255, 184, 134, 11); // Dark golden
        int borderWidth = 3;
        
        // Top border
        game_.Draw2dTexture(game_.WhiteTexture(), startX - 10, startY - 10, 
            totalWidth, borderWidth, null, 0, borderColor, false);
        // Bottom border
        game_.Draw2dTexture(game_.WhiteTexture(), startX - 10, startY + scaledSlotSize + 10 - borderWidth, 
            totalWidth, borderWidth, null, 0, borderColor, false);
        // Left border
        game_.Draw2dTexture(game_.WhiteTexture(), startX - 10, startY - 10, 
            borderWidth, totalHeight, null, 0, borderColor, false);
        // Right border
        game_.Draw2dTexture(game_.WhiteTexture(), startX + totalWidth - 10 - borderWidth, startY - 10, 
            borderWidth, totalHeight, null, 0, borderColor, false);
        
        // Draw individual slots
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (scaledSlotSize + scaledSpacing);
            int slotY = startY;
            
            DrawHotbarSlot(game_, slotX, slotY, scaledSlotSize, i);
        }
    }
    
    void DrawHotbarSlot(Game game_, int x, int y, int size, int slotIndex)
    {
        bool isActive = game_.ActiveMaterial == slotIndex;
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
        
        game_.Draw2dBitmapFile(slotTexture, x, y, size, size);
        
        // Draw slot border highlight for active slot
        if (isActive)
        {
            int highlightColor = Game.ColorFromArgb(180, 255, 215, 0);
            int borderSize = 2;
            
            // Top
            game_.Draw2dTexture(game_.WhiteTexture(), x, y, size, borderSize, 
                null, 0, highlightColor, false);
            // Bottom
            game_.Draw2dTexture(game_.WhiteTexture(), x, y + size - borderSize, size, borderSize, 
                null, 0, highlightColor, false);
            // Left
            game_.Draw2dTexture(game_.WhiteTexture(), x, y, borderSize, size, 
                null, 0, highlightColor, false);
            // Right
            game_.Draw2dTexture(game_.WhiteTexture(), x + size - borderSize, y, borderSize, size, 
                null, 0, highlightColor, false);
        }
    }
    
    void DrawHotbarItems(Game game_, int startX, int startY, int scaledSlotSize, int scaledSpacing)
    {
        if (game_.d_Inventory == null || dataItems == null)
        {
            return;
        }
        
        // Draw items in hotbar slots (slots 0-9 in inventory)
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (scaledSlotSize + scaledSpacing);
            int slotY = startY;
            
            Packet_Item item = game_.d_Inventory.RightHand[i];
            if (item != null && item.BlockId > 0)
            {
                int itemSize = scaledSlotSize - 10; // Inset from slot border
                int itemX = slotX + 5;
                int itemY = slotY + 5;
                
                // Draw item texture from terrain atlas
                game_.Draw2dTexture(game_.terrainTexture, itemX, itemY,
                    itemSize, itemSize, IntRef.Create(dataItems.TextureIdForInventory()[item.BlockId]), 
                    game_.texturesPacked(), Game.ColorFromArgb(255, 255, 255, 255), false);
                
                // Draw item count if stacked
                if (item.BlockCount > 1)
                {
                    FontCi font = new FontCi();
                    font.size = 10;
                    string countText = game_.platform.IntToString(item.BlockCount);
                    game_.Draw2dText(countText, font, slotX + scaledSlotSize - 20, slotY + scaledSlotSize - 18, 
                        null, false);
                }
            }
        }
    }
    
    void DrawSlotNumbers(Game game_, int startX, int startY, int scaledSlotSize, int scaledSpacing)
    {
        FontCi font = new FontCi();
        font.size = 11;
        
        for (int i = 0; i < slotCount; i++)
        {
            int slotX = startX + i * (scaledSlotSize + scaledSpacing);
            int slotY = startY;
            
            // Display 1-9, 0 for slots (matching keyboard layout)
            string number = i == 9 ? "0" : game_.platform.IntToString(i + 1);
            
            // Draw number in top-left corner of slot
            game_.Draw2dText(number, font, slotX + 5, slotY + 3, null, false);
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
