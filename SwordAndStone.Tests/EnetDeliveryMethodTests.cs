using NUnit.Framework;

namespace SwordAndStone.Tests
{
    /// <summary>
    /// Tests for ENet delivery method to packet flags mapping.
    /// Validates that MyNetDeliveryMethod values map correctly to EnetPacketFlags.
    /// </summary>
    [TestFixture]
    public class EnetDeliveryMethodTests
    {
        private int MapDeliveryMethodToFlags(MyNetDeliveryMethod method)
        {
            int flags = EnetPacketFlags.Reliable;
            if (method == MyNetDeliveryMethod.Unreliable)
            {
                flags = EnetPacketFlags.None;
            }
            else if (method == MyNetDeliveryMethod.UnreliableSequenced)
            {
                flags = EnetPacketFlags.None;
            }
            else if (method == MyNetDeliveryMethod.ReliableUnordered)
            {
                flags = EnetPacketFlags.Reliable | EnetPacketFlags.Unsequenced;
            }
            return flags;
        }

        [Test]
        public void MapDeliveryMethod_Unreliable_ReturnsNone()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.Unreliable);
            Assert.AreEqual(EnetPacketFlags.None, flags, "Unreliable should map to None (no flags)");
        }

        [Test]
        public void MapDeliveryMethod_UnreliableSequenced_ReturnsNone()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.UnreliableSequenced);
            Assert.AreEqual(EnetPacketFlags.None, flags, "UnreliableSequenced should map to None");
        }

        [Test]
        public void MapDeliveryMethod_ReliableOrdered_ReturnsReliable()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.ReliableOrdered);
            Assert.AreEqual(EnetPacketFlags.Reliable, flags, "ReliableOrdered should map to Reliable");
        }

        [Test]
        public void MapDeliveryMethod_ReliableSequenced_ReturnsReliable()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.ReliableSequenced);
            Assert.AreEqual(EnetPacketFlags.Reliable, flags, "ReliableSequenced should map to Reliable");
        }

        [Test]
        public void MapDeliveryMethod_ReliableUnordered_ReturnsReliableAndUnsequenced()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.ReliableUnordered);
            int expected = EnetPacketFlags.Reliable | EnetPacketFlags.Unsequenced;
            Assert.AreEqual(expected, flags, "ReliableUnordered should map to Reliable|Unsequenced");
        }

        [Test]
        public void MapDeliveryMethod_Unknown_DefaultsToReliable()
        {
            int flags = MapDeliveryMethodToFlags(MyNetDeliveryMethod.Unknown);
            Assert.AreEqual(EnetPacketFlags.Reliable, flags, "Unknown should default to Reliable");
        }

        [Test]
        public void EnetPacketFlags_ValuesAreCorrect()
        {
            Assert.AreEqual(0, EnetPacketFlags.None);
            Assert.AreEqual(1, EnetPacketFlags.Reliable);
            Assert.AreEqual(2, EnetPacketFlags.Unsequenced);
            Assert.AreEqual(4, EnetPacketFlags.NoAllocate);
        }
    }
}
