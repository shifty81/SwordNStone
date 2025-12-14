using System;

namespace SwordAndStone.Mods
{
	/// <summary>
	/// This class defines basic tools (pickaxe, axe, shovel)
	/// </summary>
	public class Tools : IMod
	{
		ModManager m;

		public void PreStart(ModManager m)
		{
			m.RequireMod("Core");
		}

		public void Start(ModManager manager)
		{
			m = manager;

			// Stone Pickaxe
			m.SetBlockType("StonePickaxe", new BlockType()
			{
				Name = "Stone Pickaxe",
				IsTool = true,
				ToolType = ToolType.Pickaxe,
				Strength = 2,
				TextureIdForInventory = "StonePickaxe",
				AllTextures = "StonePickaxe",
			});

			// Stone Axe
			m.SetBlockType("StoneAxe", new BlockType()
			{
				Name = "Stone Axe",
				IsTool = true,
				ToolType = ToolType.Axe,
				Strength = 2,
				TextureIdForInventory = "StoneAxe",
				AllTextures = "StoneAxe",
			});

			// Stone Shovel
			m.SetBlockType("StoneShovel", new BlockType()
			{
				Name = "Stone Shovel",
				IsTool = true,
				ToolType = ToolType.Shovel,
				Strength = 2,
				TextureIdForInventory = "StoneShovel",
				AllTextures = "StoneShovel",
			});

			// Iron Pickaxe
			m.SetBlockType("IronPickaxe", new BlockType()
			{
				Name = "Iron Pickaxe",
				IsTool = true,
				ToolType = ToolType.Pickaxe,
				Strength = 4,
				TextureIdForInventory = "IronPickaxe",
				AllTextures = "IronPickaxe",
			});

			// Iron Axe
			m.SetBlockType("IronAxe", new BlockType()
			{
				Name = "Iron Axe",
				IsTool = true,
				ToolType = ToolType.Axe,
				Strength = 4,
				TextureIdForInventory = "IronAxe",
				AllTextures = "IronAxe",
			});

			// Iron Shovel
			m.SetBlockType("IronShovel", new BlockType()
			{
				Name = "Iron Shovel",
				IsTool = true,
				ToolType = ToolType.Shovel,
				Strength = 4,
				TextureIdForInventory = "IronShovel",
				AllTextures = "IronShovel",
			});

			// Add tools to creative inventory
			m.AddToCreativeInventory("StonePickaxe");
			m.AddToCreativeInventory("StoneAxe");
			m.AddToCreativeInventory("StoneShovel");
			m.AddToCreativeInventory("IronPickaxe");
			m.AddToCreativeInventory("IronAxe");
			m.AddToCreativeInventory("IronShovel");

			// Add basic tools to starting inventory
			m.AddToStartInventory("StonePickaxe", 1);
			m.AddToStartInventory("StoneAxe", 1);
			m.AddToStartInventory("StoneShovel", 1);
		}
	}
}
