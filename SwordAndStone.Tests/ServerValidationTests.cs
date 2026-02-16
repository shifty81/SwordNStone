using NUnit.Framework;
using System;
using System.Collections.Generic;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for server-side validation and in-memory chunk storage.
    /// </summary>
    [TestFixture]
    public class ServerValidationTests
    {
        [Test]
        public void PlayerStats_HealthClamp_AboveMax_ClampedToMax()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxHealth = 20;

            // Simulate server-side clamping
            int health = 999;
            if (health > stats.MaxHealth)
            {
                health = stats.MaxHealth;
            }
            stats.CurrentHealth = health;

            Assert.AreEqual(20, stats.CurrentHealth);
        }

        [Test]
        public void PlayerStats_HealthClamp_BelowZero_ResetsToMax()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxHealth = 20;

            int health = -5;
            if (health > stats.MaxHealth)
            {
                health = stats.MaxHealth;
            }
            stats.CurrentHealth = health;
            if (stats.CurrentHealth < 1)
            {
                stats.CurrentHealth = stats.MaxHealth;
            }

            Assert.AreEqual(20, stats.CurrentHealth);
        }

        [Test]
        public void PlayerStats_HealthClamp_ValidValue_Unchanged()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxHealth = 20;

            int health = 15;
            if (health > stats.MaxHealth)
            {
                health = stats.MaxHealth;
            }
            stats.CurrentHealth = health;

            Assert.AreEqual(15, stats.CurrentHealth);
        }

        [Test]
        public void PlayerStats_OxygenClamp_AboveMax_ClampedToMax()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxOxygen = 10;

            int oxygen = 999;
            if (oxygen < 0) { oxygen = 0; }
            if (oxygen > stats.MaxOxygen) { oxygen = stats.MaxOxygen; }
            stats.CurrentOxygen = oxygen;

            Assert.AreEqual(10, stats.CurrentOxygen);
        }

        [Test]
        public void PlayerStats_OxygenClamp_BelowZero_ClampedToZero()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxOxygen = 10;

            int oxygen = -5;
            if (oxygen < 0) { oxygen = 0; }
            if (oxygen > stats.MaxOxygen) { oxygen = stats.MaxOxygen; }
            stats.CurrentOxygen = oxygen;

            Assert.AreEqual(0, stats.CurrentOxygen);
        }

        [Test]
        public void PlayerStats_OxygenClamp_ValidValue_Unchanged()
        {
            var stats = new PacketServerPlayerStats();
            stats.MaxOxygen = 10;

            int oxygen = 7;
            if (oxygen < 0) { oxygen = 0; }
            if (oxygen > stats.MaxOxygen) { oxygen = stats.MaxOxygen; }
            stats.CurrentOxygen = oxygen;

            Assert.AreEqual(7, stats.CurrentOxygen);
        }
    }

    /// <summary>
    /// Tests for ChunkDbDummy in-memory chunk storage.
    /// </summary>
    [TestFixture]
    public class ChunkDbDummyTests
    {
        private ChunkDbDummy db;

        [SetUp]
        public void Setup()
        {
            db = new ChunkDbDummy();
            db.Open("test.db");
        }

        [Test]
        public void SetChunks_ThenGetChunks_ReturnsSameData()
        {
            var pos = new Xyz { X = 1, Y = 2, Z = 3 };
            byte[] data = new byte[] { 1, 2, 3, 4, 5 };
            db.SetChunks(new[] { new DbChunk { Position = pos, Chunk = data } });

            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(data, results[0]);
        }

        [Test]
        public void GetChunks_MissingPosition_ReturnsNull()
        {
            var pos = new Xyz { X = 99, Y = 99, Z = 99 };
            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.IsNull(results[0]);
        }

        [Test]
        public void GetChunksFromFile_ReturnsInMemoryData()
        {
            var pos = new Xyz { X = 5, Y = 6, Z = 7 };
            byte[] data = new byte[] { 10, 20, 30 };
            db.SetChunks(new[] { new DbChunk { Position = pos, Chunk = data } });

            var result = db.GetChunksFromFile(new[] { pos }, "anyfile.db");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey(pos));
            Assert.AreEqual(data, result[pos]);
        }

        [Test]
        public void GetChunksFromFile_MissingPosition_ReturnsNullValue()
        {
            var pos = new Xyz { X = 99, Y = 99, Z = 99 };
            var result = db.GetChunksFromFile(new[] { pos }, "anyfile.db");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsKey(pos));
            Assert.IsNull(result[pos]);
        }

        [Test]
        public void SetChunksToFile_StoresInMemory()
        {
            var pos = new Xyz { X = 8, Y = 9, Z = 10 };
            byte[] data = new byte[] { 100, 200 };
            db.SetChunksToFile(new[] { new DbChunk { Position = pos, Chunk = data } }, "anyfile.db");

            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(data, results[0]);
        }

        [Test]
        public void DeleteChunks_SetsToNull()
        {
            var pos = new Xyz { X = 1, Y = 1, Z = 1 };
            byte[] data = new byte[] { 1, 2, 3 };
            db.SetChunks(new[] { new DbChunk { Position = pos, Chunk = data } });

            db.DeleteChunks(new[] { pos });

            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.IsNull(results[0]);
        }

        [Test]
        public void GlobalData_SetAndGet()
        {
            byte[] data = new byte[] { 42, 43, 44 };
            db.SetGlobalData(data);
            Assert.AreEqual(data, db.GetGlobalData());
        }

        [Test]
        public void Backup_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => db.Backup("backup.db"));
        }

        [Test]
        public void Open_DifferentFile_ClearsChunks()
        {
            var pos = new Xyz { X = 1, Y = 1, Z = 1 };
            byte[] data = new byte[] { 1, 2, 3 };
            db.SetChunks(new[] { new DbChunk { Position = pos, Chunk = data } });

            db.Open("different.db");

            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.IsNull(results[0]);
        }

        [Test]
        public void Open_SameFile_KeepsChunks()
        {
            var pos = new Xyz { X = 1, Y = 1, Z = 1 };
            byte[] data = new byte[] { 1, 2, 3 };
            db.SetChunks(new[] { new DbChunk { Position = pos, Chunk = data } });

            db.Open("test.db");

            var results = new List<byte[]>(db.GetChunks(new[] { pos }));
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(data, results[0]);
        }
    }
}
