using NUnit.Framework;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
	/// <summary>
	/// Tests for server command infrastructure.
	/// Validates that <see cref="NetworkBackendType"/> enum and
	/// <see cref="NetworkBackendConfig"/> behave correctly, and that
	/// command help text is consistent.
	/// </summary>
	[TestFixture]
	public class ServerCommandTests
	{
		[Test]
		public void NetworkBackendType_HasThreeValues()
		{
			var values = System.Enum.GetValues(typeof(NetworkBackendType));
			Assert.AreEqual(3, values.Length, "Should have ENet, WebSocket, Tcp");
		}

		[Test]
		public void NetworkBackendConfig_DefaultPortOffsetIsZero()
		{
			var config = new NetworkBackendConfig { Type = NetworkBackendType.ENet };
			Assert.AreEqual(0, config.PortOffset, "Default port offset should be 0");
		}

		[Test]
		public void NetworkBackendConfig_PortOffsetCanBeSet()
		{
			var config = new NetworkBackendConfig { Type = NetworkBackendType.Tcp, PortOffset = 5 };
			Assert.AreEqual(5, config.PortOffset);
			Assert.AreEqual(NetworkBackendType.Tcp, config.Type);
		}
	}
}
