using System;

namespace SwordAndStone.Mods
{
	/// <summary>
	/// This class defines combat weapons (sword, shield, bow)
	/// </summary>
	public class Weapons : IMod
	{
		ModManager m;

		public void PreStart(ModManager m)
		{
			m.RequireMod("Core");
		}

		public void Start(ModManager manager)
		{
			m = manager;

			// Stone Sword
			m.SetBlockType("StoneSword", new BlockType()
			{
				Name = "Stone Sword",
				IsTool = true,
				ToolType = ToolType.Sword,
				Strength = 4,  // Damage value
				TextureIdForInventory = "StoneSword",
				AllTextures = "StoneSword",
			});

			// Iron Sword
			m.SetBlockType("IronSword", new BlockType()
			{
				Name = "Iron Sword",
				IsTool = true,
				ToolType = ToolType.Sword,
				Strength = 6,  // Higher damage
				TextureIdForInventory = "IronSword",
				AllTextures = "IronSword",
			});

			// Diamond Sword (if desired for future)
			m.SetBlockType("DiamondSword", new BlockType()
			{
				Name = "Diamond Sword",
				IsTool = true,
				ToolType = ToolType.Sword,
				Strength = 8,  // Highest damage
				TextureIdForInventory = "DiamondSword",
				AllTextures = "DiamondSword",
			});

			// Wooden Shield
			m.SetBlockType("WoodenShield", new BlockType()
			{
				Name = "Wooden Shield",
				IsTool = true,
				ToolType = ToolType.None,  // Shield is not a tool for mining
				Strength = 5,  // Damage reduction value (used as multiplier * 10%)
				TextureIdForInventory = "WoodenShield",
				AllTextures = "WoodenShield",
			});

			// Iron Shield
			m.SetBlockType("IronShield", new BlockType()
			{
				Name = "Iron Shield",
				IsTool = true,
				ToolType = ToolType.None,
				Strength = 7,  // Better damage reduction (7 * 10% = 70%)
				TextureIdForInventory = "IronShield",
				AllTextures = "IronShield",
			});

			// Basic Bow (for future ranged combat)
			m.SetBlockType("Bow", new BlockType()
			{
				Name = "Bow",
				IsTool = true,
				ToolType = ToolType.None,
				Strength = 3,  // Arrow damage
				TextureIdForInventory = "Bow",
				AllTextures = "Bow",
			});

			// Add weapons to creative inventory
			m.AddToCreativeInventory("StoneSword");
			m.AddToCreativeInventory("IronSword");
			m.AddToCreativeInventory("DiamondSword");
			m.AddToCreativeInventory("WoodenShield");
			m.AddToCreativeInventory("IronShield");
			m.AddToCreativeInventory("Bow");

			// Add basic weapons to starting inventory for testing
			m.AddToStartInventory("StoneSword", 1);
			m.AddToStartInventory("WoodenShield", 1);
		}
	}
}
