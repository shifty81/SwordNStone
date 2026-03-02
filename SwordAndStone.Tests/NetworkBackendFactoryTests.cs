using System;
using NUnit.Framework;
using SwordAndStone.Server;

namespace SwordAndStone.Tests
{
	/// <summary>
	/// Tests for <see cref="NetworkBackendFactory"/>.
	/// Validates backend creation, configuration, and startup logic.
	/// </summary>
	[TestFixture]
	public class NetworkBackendFactoryTests
	{
		[Test]
		public void Create_ENet_ReturnsEnetNetServer()
		{
			NetServer server = NetworkBackendFactory.Create(NetworkBackendType.ENet);
			Assert.IsNotNull(server);
			Assert.IsInstanceOf<EnetNetServer>(server);
		}

		[Test]
		public void Create_WebSocket_ReturnsWebSocketNetServer()
		{
			NetServer server = NetworkBackendFactory.Create(NetworkBackendType.WebSocket);
			Assert.IsNotNull(server);
			Assert.IsInstanceOf<WebSocketNetServer>(server);
		}

		[Test]
		public void Create_Tcp_ReturnsTcpNetServer()
		{
			NetServer server = NetworkBackendFactory.Create(NetworkBackendType.Tcp);
			Assert.IsNotNull(server);
			Assert.IsInstanceOf<TcpNetServer>(server);
		}

		[Test]
		public void Create_InvalidType_ThrowsArgumentOutOfRangeException()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				NetworkBackendFactory.Create((NetworkBackendType)99));
		}

		[Test]
		public void DefaultConfigs_HasThreeEntries()
		{
			Assert.AreEqual(3, NetworkBackendFactory.DefaultConfigs.Length);
		}

		[Test]
		public void DefaultConfigs_FirstIsENetWithZeroOffset()
		{
			var config = NetworkBackendFactory.DefaultConfigs[0];
			Assert.AreEqual(NetworkBackendType.ENet, config.Type);
			Assert.AreEqual(0, config.PortOffset);
		}

		[Test]
		public void DefaultConfigs_SecondIsWebSocketWithZeroOffset()
		{
			var config = NetworkBackendFactory.DefaultConfigs[1];
			Assert.AreEqual(NetworkBackendType.WebSocket, config.Type);
			Assert.AreEqual(0, config.PortOffset);
		}

		[Test]
		public void DefaultConfigs_ThirdIsTcpWithOffsetTwo()
		{
			var config = NetworkBackendFactory.DefaultConfigs[2];
			Assert.AreEqual(NetworkBackendType.Tcp, config.Type);
			Assert.AreEqual(2, config.PortOffset);
		}

		[Test]
		public void CreateAll_DefaultConfigs_ReturnsThreeServers()
		{
			int count;
			NetServer[] servers = NetworkBackendFactory.CreateAll(NetworkBackendFactory.DefaultConfigs, out count);
			Assert.AreEqual(3, count);
			Assert.AreEqual(3, servers.Length);
			Assert.IsInstanceOf<EnetNetServer>(servers[0]);
			Assert.IsInstanceOf<WebSocketNetServer>(servers[1]);
			Assert.IsInstanceOf<TcpNetServer>(servers[2]);
		}

		[Test]
		public void CreateAll_SingleConfig_ReturnsSingleServer()
		{
			var configs = new[] { new NetworkBackendConfig { Type = NetworkBackendType.Tcp, PortOffset = 0 } };
			int count;
			NetServer[] servers = NetworkBackendFactory.CreateAll(configs, out count);
			Assert.AreEqual(1, count);
			Assert.AreEqual(1, servers.Length);
			Assert.IsInstanceOf<TcpNetServer>(servers[0]);
		}

		[Test]
		public void CreateAll_NullConfigs_ThrowsArgumentNullException()
		{
			int count;
			Assert.Throws<ArgumentNullException>(() =>
				NetworkBackendFactory.CreateAll(null, out count));
		}

		[Test]
		public void StartAll_NullSockets_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				NetworkBackendFactory.StartAll(null, NetworkBackendFactory.DefaultConfigs, 25565));
		}

		[Test]
		public void StartAll_NullConfigs_ThrowsArgumentNullException()
		{
			var sockets = new NetServer[0];
			Assert.Throws<ArgumentNullException>(() =>
				NetworkBackendFactory.StartAll(sockets, null, 25565));
		}
	}
}
