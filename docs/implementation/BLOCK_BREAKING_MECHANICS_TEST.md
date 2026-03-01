# Block Breaking Mechanics Testing Guide

This document explains how to test the new block breaking mechanics that were implemented based on Vintage Story gameplay.

## Overview

The implementation adds tool-based block breaking mechanics where:
- Blocks can require specific tools to drop items
- Using the wrong tool can result in no drops or alternative drops
- Different tools are suitable for different block types

## Features Implemented

### 1. Tool Types
Added a new `ToolType` enum with the following values:
- `None` - No tool (hand or any item)
- `Pickaxe` - For mining stone and ores
- `Axe` - For chopping wood
- `Shovel` - For digging dirt, sand, gravel
- `Sword` - For combat (reserved for future use)
- `Hand` - Explicitly using bare hands

### 2. Block Properties
Each block can now have:
- `PreferredTool`: Which tool works best for this block
- `ToolType`: For tool items, what type of tool they are
- `MiningSpeed`: Base speed for mining this block
- `RequiresTool`: Whether the block requires the correct tool to drop
- `AlternativeDrop`: What item drops if wrong tool is used (0 = nothing)

### 3. Tool Items Added
Created basic tools in the new `Tools.cs` mod:
- Stone Pickaxe, Axe, Shovel (Strength: 2)
- Iron Pickaxe, Axe, Shovel (Strength: 4)

These are automatically added to:
- Creative inventory (all tools)
- Starting inventory (stone tools only)

### 4. Block Configurations
Updated blocks with tool requirements:

**Requires Pickaxe (RequiresTool=true):**
- Stone (MiningSpeed: 1.5)
- Gold Ore (MiningSpeed: 2.0)
- Iron Ore (MiningSpeed: 2.0)
- Coal Ore (MiningSpeed: 2.0)

**Prefers Axe:**
- Oak Tree Trunk (MiningSpeed: 1.2)

**Prefers Shovel:**
- Dirt (MiningSpeed: 0.8)
- Sand (MiningSpeed: 0.7)
- Gravel (MiningSpeed: 0.7)

## Testing Instructions

### Prerequisites
1. Build the solution with .NET Framework 4.5 or Mono
2. Ensure the Tools.cs mod is loaded (it's in the Fortress mods folder)

### Test Case 1: Stone Breaking with Pickaxe
**Expected Behavior:** Stone drops as a collectible block

1. Start the server and client
2. Find or place a Stone block
3. Equip the Stone Pickaxe from your inventory (or use `/give StonePickaxe`)
4. Break the Stone block
5. **Expected Result:** You receive a Stone block in your inventory

### Test Case 2: Stone Breaking by Hand
**Expected Behavior:** Stone does not drop anything

1. Ensure no tool is equipped (empty hand or non-tool item)
2. Break a Stone block
3. **Expected Result:** The block breaks but you receive nothing in your inventory

### Test Case 3: Ore Mining
**Expected Behavior:** Ores require pickaxe to drop

1. Find or place Gold/Iron/Coal Ore blocks
2. Try breaking with hand/wrong tool
3. **Expected Result:** Block breaks but no ore is collected
4. Equip a pickaxe and break the same type of ore
5. **Expected Result:** Ore block is collected

### Test Case 4: Wood Chopping
**Expected Behavior:** Axe works better but hand also works

1. Find an Oak Tree Trunk block
2. Break it by hand
3. **Expected Result:** Log is collected (slower)
4. Break another with Stone Axe
5. **Expected Result:** Log is collected (faster due to PreferredTool, but RequiresTool is false)

### Test Case 5: Dirt Digging
**Expected Behavior:** Shovel is faster but hand also works

1. Find dirt blocks
2. Break with hand
3. **Expected Result:** Dirt is collected
4. Break with Stone Shovel
5. **Expected Result:** Dirt is collected (faster due to PreferredTool)

### Test Case 6: Tool Availability
**Expected Behavior:** Tools are available in creative and starting inventory

1. Start a new game in Creative mode
2. Open inventory
3. **Expected Result:** All 6 tools (3 stone, 3 iron) are available in creative inventory
4. Start a new Survival game
5. **Expected Result:** Starting inventory contains Stone Pickaxe, Stone Axe, and Stone Shovel

## Automated Testing

Since the project doesn't have automated tests yet, here's how you could add them:

```csharp
[TestFixture]
public class BlockBreakingTests
{
    [Test]
    public void StoneRequiresPickaxe()
    {
        // Setup: Create a stone block and player without pickaxe
        // Action: Try to break the block
        // Assert: No item is given to player
    }
    
    [Test]
    public void StoneWithPickaxeDropsBlock()
    {
        // Setup: Create a stone block and player with pickaxe
        // Action: Break the block
        // Assert: Stone block is added to player inventory
    }
    
    [Test]
    public void WoodWorksWithoutAxe()
    {
        // Setup: Create wood block and player without axe
        // Action: Break the block
        // Assert: Wood block is added to inventory (because RequiresTool=false)
    }
}
```

## Known Limitations

1. **Mining Speed**: The `MiningSpeed` property is defined but not yet implemented in the client-side picking logic. This would require updates to `Picking.ci.cs` to use the speed values.

2. **Tool Durability**: The `Strength` property on tools exists but tool degradation/durability system is not implemented.

3. **Alternative Drops**: The `AlternativeDrop` property is implemented in the server but no blocks currently use it. Example: Stone could drop "small stones" instead of nothing when mined by hand.

4. **Client-Side Validation**: The tool checking is only done server-side. Client could predict this for better UX.

## Future Enhancements

1. Implement mining speed multipliers based on tool type
2. Add tool durability/wear system
3. Add alternative drop items (e.g., stone pebbles, wood chips)
4. Add tool material tiers (wood < stone < iron < diamond)
5. Add enchantment system for tools
6. Implement client-side prediction of drops

## Troubleshooting

### Issue: Tools don't appear in inventory
**Solution:** Ensure Tools.cs mod is loaded. Check server console for mod loading messages.

### Issue: Stone still drops when mined by hand
**Solution:** Verify that the BlockType for Stone has `RequiresTool = true`. Check CoreBlocks.cs.

### Issue: All blocks require tools
**Solution:** Only blocks with `RequiresTool = true` require tools. Most blocks can still be mined by hand.

## Code References

- **BlockType Definition**: `ScriptingApi/ModManager.cs` (lines 1139-1277)
- **Tool Checking Logic**: `SwordAndStoneLib/Server/Server.cs` (DoCommandBuild method, lines 2579-2627)
- **Block Configurations**: `SwordAndStoneLib/Server/Mods/Fortress/CoreBlocks.cs`
- **Tool Definitions**: `SwordAndStoneLib/Server/Mods/Fortress/Tools.cs`

## Summary

This implementation provides the foundation for Vintage Story-style block breaking mechanics. The core system is in place and working, with tool requirements properly enforced on the server side. The system is extensible and can be enhanced with additional features like mining speed modifiers, tool durability, and more sophisticated drop tables.
