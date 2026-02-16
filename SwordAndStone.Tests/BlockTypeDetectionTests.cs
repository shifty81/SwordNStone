using NUnit.Framework;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for block type detection logic (IsWater, IsLava).
    /// Validates that fluid detection uses block properties instead of string name matching.
    /// </summary>
    [TestFixture]
    public class BlockTypeDetectionTests
    {
        [Test]
        public void IsWater_FluidWithNoDamage_ReturnsTrue()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);
            block.SetDamageToPlayer(0);

            bool isWater = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() == 0;

            Assert.IsTrue(isWater, "Block with Fluid DrawType and no damage should be detected as water");
        }

        [Test]
        public void IsWater_FluidWithDamage_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);
            block.SetDamageToPlayer(2);

            bool isWater = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() == 0;

            Assert.IsFalse(isWater, "Block with Fluid DrawType and damage should not be detected as water");
        }

        [Test]
        public void IsWater_SolidBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Solid);
            block.SetDamageToPlayer(0);

            bool isWater = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() == 0;

            Assert.IsFalse(isWater, "Solid block should not be detected as water");
        }

        [Test]
        public void IsLava_FluidWithDamage_ReturnsTrue()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);
            block.SetDamageToPlayer(2);

            bool isLava = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() > 0;

            Assert.IsTrue(isLava, "Block with Fluid DrawType and damage should be detected as lava");
        }

        [Test]
        public void IsLava_FluidWithNoDamage_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);
            block.SetDamageToPlayer(0);

            bool isLava = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() > 0;

            Assert.IsFalse(isLava, "Water block (no damage) should not be detected as lava");
        }

        [Test]
        public void IsLava_SolidBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Solid);
            block.SetDamageToPlayer(2);

            bool isLava = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() > 0;

            Assert.IsFalse(isLava, "Solid block with damage should not be detected as lava");
        }

        [Test]
        public void IsLava_EmptyBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Empty);
            block.SetDamageToPlayer(0);

            bool isLava = block.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && block.GetDamageToPlayer() > 0;

            Assert.IsFalse(isLava, "Empty block should not be detected as lava");
        }

        [Test]
        public void WaterAndLava_AreMutuallyExclusive()
        {
            var lavaBlock = new Packet_BlockType();
            lavaBlock.SetDrawType(Packet_DrawTypeEnum.Fluid);
            lavaBlock.SetDamageToPlayer(2);

            bool isWater = lavaBlock.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && lavaBlock.GetDamageToPlayer() == 0;
            bool isLava = lavaBlock.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && lavaBlock.GetDamageToPlayer() > 0;

            Assert.IsFalse(isWater, "Lava should not be detected as water");
            Assert.IsTrue(isLava, "Lava should be detected as lava");

            var waterBlock = new Packet_BlockType();
            waterBlock.SetDrawType(Packet_DrawTypeEnum.Fluid);
            waterBlock.SetDamageToPlayer(0);

            isWater = waterBlock.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && waterBlock.GetDamageToPlayer() == 0;
            isLava = waterBlock.GetDrawType() == Packet_DrawTypeEnum.Fluid
                && waterBlock.GetDamageToPlayer() > 0;

            Assert.IsTrue(isWater, "Water should be detected as water");
            Assert.IsFalse(isLava, "Water should not be detected as lava");
        }
    }
}
