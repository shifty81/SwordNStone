using NUnit.Framework;
using System;
using System.Collections.Generic;
using ManicDigger;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for inventory item handling with non-Block item types.
    /// Validates that GrabItem, ItemSizeX/Y, and ItemGraphics work for all ItemClass values.
    /// </summary>
    [TestFixture]
    public class ItemHandlingTests
    {
        private GameDataItemsBlocks gameDataItems;
        private GameData gameData;

        [SetUp]
        public void Setup()
        {
            gameData = new GameData();
            gameData.Start();
            gameDataItems = new GameDataItemsBlocks();
            gameDataItems.d_Data = gameData;
        }

        [Test]
        public void ItemSizeX_BlockItem_Returns1()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Block;
            item.BlockId = 1;
            Assert.AreEqual(1, gameDataItems.ItemSizeX(item));
        }

        [Test]
        public void ItemSizeX_WeaponItem_Returns1()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Weapon;
            Assert.AreEqual(1, gameDataItems.ItemSizeX(item));
        }

        [Test]
        public void ItemSizeY_WeaponItem_Returns1()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Weapon;
            Assert.AreEqual(1, gameDataItems.ItemSizeY(item));
        }

        [Test]
        [TestCase(ItemClass.MainArmor)]
        [TestCase(ItemClass.Boots)]
        [TestCase(ItemClass.Helmet)]
        [TestCase(ItemClass.Gauntlet)]
        [TestCase(ItemClass.Shield)]
        [TestCase(ItemClass.Other)]
        public void ItemSizeX_NonBlockItem_Returns1(ItemClass itemClass)
        {
            Item item = new Item();
            item.ItemClass = itemClass;
            Assert.AreEqual(1, gameDataItems.ItemSizeX(item));
        }

        [Test]
        [TestCase(ItemClass.MainArmor)]
        [TestCase(ItemClass.Boots)]
        [TestCase(ItemClass.Helmet)]
        [TestCase(ItemClass.Gauntlet)]
        [TestCase(ItemClass.Shield)]
        [TestCase(ItemClass.Other)]
        public void ItemSizeY_NonBlockItem_Returns1(ItemClass itemClass)
        {
            Item item = new Item();
            item.ItemClass = itemClass;
            Assert.AreEqual(1, gameDataItems.ItemSizeY(item));
        }

        [Test]
        public void ItemGraphics_BlockItem_ReturnsBlockId()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Block;
            item.BlockId = 42;
            Assert.AreEqual("42", gameDataItems.ItemGraphics(item));
        }

        [Test]
        public void ItemGraphics_WeaponWithItemId_ReturnsItemId()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Weapon;
            item.ItemId = "iron_sword";
            Assert.AreEqual("iron_sword", gameDataItems.ItemGraphics(item));
        }

        [Test]
        public void ItemGraphics_WeaponNoItemId_ReturnsClassName()
        {
            Item item = new Item();
            item.ItemClass = ItemClass.Weapon;
            item.ItemId = null;
            Assert.AreEqual("Weapon", gameDataItems.ItemGraphics(item));
        }

        [Test]
        public void GrabItem_WeaponToEmptyHand_Succeeds()
        {
            // Arrange
            Inventory inventory = new Inventory();
            inventory.RightHand = new Item[10];
            inventory.Items = new Dictionary<ProtoPoint, Item>();

            InventoryUtil util = new InventoryUtil();
            util.d_Inventory = inventory;
            util.d_Items = gameDataItems;

            Item weapon = new Item();
            weapon.ItemClass = ItemClass.Weapon;
            weapon.ItemId = "sword";

            // Act
            bool result = util.GrabItem(weapon, 0);

            // Assert
            Assert.IsTrue(result, "Should be able to grab weapon into empty hand");
            Assert.AreEqual(weapon, inventory.RightHand[0]);
        }

        [Test]
        public void GrabItem_WeaponToOccupiedHand_FindsFreeSlot()
        {
            // Arrange
            Inventory inventory = new Inventory();
            inventory.RightHand = new Item[10];
            inventory.Items = new Dictionary<ProtoPoint, Item>();

            // Occupy slot 0
            Item existingItem = new Item();
            existingItem.ItemClass = ItemClass.Block;
            existingItem.BlockId = 1;
            existingItem.BlockCount = 1;
            inventory.RightHand[0] = existingItem;

            InventoryUtil util = new InventoryUtil();
            util.d_Inventory = inventory;
            util.d_Items = gameDataItems;

            Item weapon = new Item();
            weapon.ItemClass = ItemClass.Weapon;
            weapon.ItemId = "axe";

            // Act
            bool result = util.GrabItem(weapon, 0);

            // Assert
            Assert.IsTrue(result, "Should find a free hand slot");
            Assert.AreEqual(weapon, inventory.RightHand[1],
                "Weapon should be placed in slot 1 since slot 0 is occupied");
        }

        [Test]
        public void FreeHand_ActiveSlotEmpty_ReturnsActiveMaterial()
        {
            Inventory inventory = new Inventory();
            inventory.RightHand = new Item[10];

            InventoryUtil util = new InventoryUtil();
            util.d_Inventory = inventory;

            int? result = util.FreeHand(3);
            Assert.AreEqual(3, result, "Should return active material index when slot is empty");
        }

        [Test]
        public void FreeHand_ActiveSlotOccupied_ReturnsNextFreeSlot()
        {
            Inventory inventory = new Inventory();
            inventory.RightHand = new Item[10];

            // Occupy slots 0 and 1
            inventory.RightHand[0] = new Item();
            inventory.RightHand[1] = new Item();

            InventoryUtil util = new InventoryUtil();
            util.d_Inventory = inventory;

            int? result = util.FreeHand(0);
            Assert.AreEqual(2, result, "Should return index 2 as the first free slot");
        }

        [Test]
        public void FreeHand_AllSlotsOccupied_ReturnsNull()
        {
            Inventory inventory = new Inventory();
            inventory.RightHand = new Item[10];
            for (int i = 0; i < 10; i++)
            {
                inventory.RightHand[i] = new Item();
            }

            InventoryUtil util = new InventoryUtil();
            util.d_Inventory = inventory;

            int? result = util.FreeHand(0);
            Assert.IsNull(result, "Should return null when all hand slots are full");
        }
    }
}
