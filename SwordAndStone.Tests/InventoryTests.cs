using NUnit.Framework;
using System;
using ManicDigger;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for inventory stack size limits and stacking behavior.
    /// Validates that the stack size limit is properly enforced on the server side.
    /// </summary>
    [TestFixture]
    public class InventoryStackTests
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
        public void Stack_SameBlockType_ReturnsCombinedCount()
        {
            // Arrange
            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 10;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 5;

            // Act
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNotNull(result, "Stacking same block types should succeed");
            Assert.AreEqual(15, result.BlockCount, "Combined count should be 15");
            Assert.AreEqual(1, result.BlockId, "Block ID should be preserved");
        }

        [Test]
        public void Stack_ExceedsMaxStackSize_ReturnsNull()
        {
            // Arrange
            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 50;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 20;

            // Act - total would be 70, exceeding default max of 64
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNull(result, "Stacking beyond max stack size (64) should return null");
        }

        [Test]
        public void Stack_ExactlyAtMaxStackSize_Succeeds()
        {
            // Arrange
            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 32;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 32;

            // Act - total is exactly 64 (default max)
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNotNull(result, "Stacking to exactly max stack size should succeed");
            Assert.AreEqual(64, result.BlockCount, "Combined count should be 64");
        }

        [Test]
        public void Stack_DifferentBlockTypes_ReturnsNull()
        {
            // Arrange
            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 10;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 2;
            itemB.BlockCount = 5;

            // Act
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNull(result, "Stacking different block types should return null");
        }

        [Test]
        public void Stack_CustomMaxStackSize_IsRespected()
        {
            // Arrange
            gameDataItems.MaxStackSize = 32;

            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 20;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 20;

            // Act - total is 40, exceeding custom max of 32
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNull(result, "Stacking beyond custom max stack size (32) should return null");
        }

        [Test]
        public void Stack_CustomMaxStackSize_AllowsBelowLimit()
        {
            // Arrange
            gameDataItems.MaxStackSize = 128;

            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 60;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 60;

            // Act - total is 120, within custom max of 128
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNotNull(result, "Stacking within custom max of 128 should succeed");
            Assert.AreEqual(120, result.BlockCount, "Combined count should be 120");
        }

        [Test]
        public void Stack_SingleItem_ReturnsCombined()
        {
            // Arrange
            Item itemA = new Item();
            itemA.ItemClass = ItemClass.Block;
            itemA.BlockId = 1;
            itemA.BlockCount = 1;

            Item itemB = new Item();
            itemB.ItemClass = ItemClass.Block;
            itemB.BlockId = 1;
            itemB.BlockCount = 1;

            // Act
            Item result = gameDataItems.Stack(itemA, itemB);

            // Assert
            Assert.IsNotNull(result, "Stacking single items should succeed");
            Assert.AreEqual(2, result.BlockCount, "Combined count should be 2");
        }

        [Test]
        public void Stack_DefaultMaxStackSize_Is64()
        {
            // Assert
            Assert.AreEqual(64, gameDataItems.MaxStackSize,
                "Default max stack size should be 64");
        }
    }
}
