using NUnit.Framework;
using System;
using ManicDigger;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for map bounds checking utilities.
    /// Validates that IsValidChunkPos and IsValidPos correctly guard against out-of-bounds access.
    /// </summary>
    [TestFixture]
    public class MapBoundsTests
    {
        private ServerMap map;

        [SetUp]
        public void Setup()
        {
            map = new ServerMap();
            map.d_Heightmap = new InfiniteMapChunked2dServer();
            map.d_Heightmap.d_Map = map;
            // Initialize a small map: 64x64x64 blocks, chunksize 16 => 4x4x4 chunks
            map.Reset(64, 64, 64);
        }

        [Test]
        public void IsValidChunkPos_ValidCoordinates_ReturnsTrue()
        {
            Assert.IsTrue(MapUtil.IsValidChunkPos(map, 0, 0, 0, 16));
            Assert.IsTrue(MapUtil.IsValidChunkPos(map, 3, 3, 3, 16));
            Assert.IsTrue(MapUtil.IsValidChunkPos(map, 1, 2, 3, 16));
        }

        [Test]
        public void IsValidChunkPos_NegativeCoordinates_ReturnsFalse()
        {
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, -1, 0, 0, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 0, -1, 0, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 0, 0, -1, 16));
        }

        [Test]
        public void IsValidChunkPos_OutOfBounds_ReturnsFalse()
        {
            // Map is 64x64x64, chunksize=16, so valid chunk range is 0-3
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 4, 0, 0, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 0, 4, 0, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 0, 0, 4, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 100, 100, 100, 16));
        }

        [Test]
        public void IsValidChunkPos_BoundaryValues_ReturnsCorrectly()
        {
            // Max valid chunk is 3 (= 64/16 - 1)
            Assert.IsTrue(MapUtil.IsValidChunkPos(map, 3, 3, 3, 16));
            Assert.IsFalse(MapUtil.IsValidChunkPos(map, 4, 3, 3, 16));
        }

        [Test]
        public void IsValidPos_ValidCoordinates_ReturnsTrue()
        {
            Assert.IsTrue(MapUtil.IsValidPos(map, 0, 0, 0));
            Assert.IsTrue(MapUtil.IsValidPos(map, 32, 32, 32));
            Assert.IsTrue(MapUtil.IsValidPos(map, 63, 63, 63));
        }

        [Test]
        public void IsValidPos_NegativeCoordinates_ReturnsFalse()
        {
            Assert.IsFalse(MapUtil.IsValidPos(map, -1, 0, 0));
            Assert.IsFalse(MapUtil.IsValidPos(map, 0, -1, 0));
            Assert.IsFalse(MapUtil.IsValidPos(map, 0, 0, -1));
        }

        [Test]
        public void IsValidPos_OutOfBounds_ReturnsFalse()
        {
            Assert.IsFalse(MapUtil.IsValidPos(map, 64, 0, 0));
            Assert.IsFalse(MapUtil.IsValidPos(map, 0, 64, 0));
            Assert.IsFalse(MapUtil.IsValidPos(map, 0, 0, 64));
        }
    }
}
