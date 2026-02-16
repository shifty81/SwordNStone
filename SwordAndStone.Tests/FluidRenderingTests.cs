using NUnit.Framework;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for fluid rendering logic.
    /// Validates that isFluid detection works for all fluid block types, not just hardcoded water (ID 8).
    /// </summary>
    [TestFixture]
    public class FluidRenderingTests
    {
        [Test]
        public void IsFluid_WaterBlock_ReturnsTrue()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsTrue(isFluid, "Water block should be detected as fluid");
        }

        [Test]
        public void IsFluid_LavaBlock_ReturnsTrue()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Fluid);
            block.SetDamageToPlayer(2);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsTrue(isFluid, "Lava block should be detected as fluid");
        }

        [Test]
        public void IsFluid_SolidBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Solid);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsFalse(isFluid, "Solid block should not be detected as fluid");
        }

        [Test]
        public void IsFluid_EmptyBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Empty);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsFalse(isFluid, "Empty block should not be detected as fluid");
        }

        [Test]
        public void IsFluid_PlantBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Plant);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsFalse(isFluid, "Plant block should not be detected as fluid");
        }

        [Test]
        public void IsFluid_TorchBlock_ReturnsFalse()
        {
            var block = new Packet_BlockType();
            block.SetDrawType(Packet_DrawTypeEnum.Torch);

            bool isFluid = block.GetDrawType() == Packet_DrawTypeEnum.Fluid;

            Assert.IsFalse(isFluid, "Torch block should not be detected as fluid");
        }

        [Test]
        [Category("Integration")]
        public void FluidDetection_SupportsMultipleFluidBlockTypes()
        {
            // Simulates the isFluid array initialization from TerrainChunkTesselator
            var blocktypes = new Packet_BlockType[10];
            var isFluid = new bool[10];

            // Set up various block types
            blocktypes[0] = new Packet_BlockType();
            blocktypes[0].SetDrawType(Packet_DrawTypeEnum.Empty);

            blocktypes[1] = new Packet_BlockType();
            blocktypes[1].SetDrawType(Packet_DrawTypeEnum.Solid);

            blocktypes[2] = new Packet_BlockType();
            blocktypes[2].SetDrawType(Packet_DrawTypeEnum.Fluid);  // Water-like

            blocktypes[3] = new Packet_BlockType();
            blocktypes[3].SetDrawType(Packet_DrawTypeEnum.Fluid);  // Lava-like
            blocktypes[3].SetDamageToPlayer(2);

            blocktypes[4] = new Packet_BlockType();
            blocktypes[4].SetDrawType(Packet_DrawTypeEnum.Fluid);  // Another fluid type
            blocktypes[4].SetDamageToPlayer(0);

            // Initialize isFluid array the same way TerrainChunkTesselator does
            for (int i = 0; i < blocktypes.Length; i++)
            {
                if (blocktypes[i] != null)
                {
                    isFluid[i] = blocktypes[i].GetDrawType() == Packet_DrawTypeEnum.Fluid;
                }
            }

            Assert.IsFalse(isFluid[0], "Empty block should not be fluid");
            Assert.IsFalse(isFluid[1], "Solid block should not be fluid");
            Assert.IsTrue(isFluid[2], "Water-like block should be fluid");
            Assert.IsTrue(isFluid[3], "Lava-like block should be fluid");
            Assert.IsTrue(isFluid[4], "Additional fluid block should be fluid");
        }
    }
}
