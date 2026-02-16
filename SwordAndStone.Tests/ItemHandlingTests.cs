using NUnit.Framework;
using System;
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
        }
    }
}
