using System;

namespace SwordAndStone.Server
{
	/// <summary>
	/// Identifies the available network transport backends.
	/// </summary>
	public enum NetworkBackendType
	{
		ENet,
		WebSocket,
		Tcp
	}

	/// <summary>
	/// Describes a network backend and its port configuration.
	/// </summary>
	public class NetworkBackendConfig
	{
		public NetworkBackendType Type { get; set; }

		/// <summary>
		/// Port offset relative to the base server port.
		/// ENet and WebSocket default to 0, TCP defaults to +2.
		/// </summary>
		public int PortOffset { get; set; }
	}

	/// <summary>
	/// Creates <see cref="NetServer"/> instances for the configured network backends.
	/// Replaces the hardcoded array-index–based backend selection in Server.cs.
	/// </summary>
	public static class NetworkBackendFactory
	{
		/// <summary>
		/// Default backend configuration: ENet (port+0), WebSocket (port+0), TCP (port+2).
		/// </summary>
		public static readonly NetworkBackendConfig[] DefaultConfigs = new[]
		{
			new NetworkBackendConfig { Type = NetworkBackendType.ENet, PortOffset = 0 },
			new NetworkBackendConfig { Type = NetworkBackendType.WebSocket, PortOffset = 0 },
			new NetworkBackendConfig { Type = NetworkBackendType.Tcp, PortOffset = 2 },
		};

		/// <summary>
		/// Creates a <see cref="NetServer"/> for the given backend type.
		/// </summary>
		public static NetServer Create(NetworkBackendType type)
		{
			switch (type)
			{
				case NetworkBackendType.ENet:
					return new EnetNetServer();
				case NetworkBackendType.WebSocket:
					return new WebSocketNetServer();
				case NetworkBackendType.Tcp:
					return new TcpNetServer();
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown network backend type");
			}
		}

		/// <summary>
		/// Creates and configures all backends from the given configuration array.
		/// Returns a populated <see cref="NetServer"/> array and sets
		/// <paramref name="count"/> to the number of entries.
		/// </summary>
		public static NetServer[] CreateAll(NetworkBackendConfig[] configs, out int count)
		{
			if (configs == null) throw new ArgumentNullException(nameof(configs));

			NetServer[] sockets = new NetServer[configs.Length];
			count = configs.Length;
			for (int i = 0; i < configs.Length; i++)
			{
				sockets[i] = Create(configs[i].Type);
			}
			return sockets;
		}

		/// <summary>
		/// Starts all backends on the given base port, applying each backend's port offset.
		/// </summary>
		public static void StartAll(NetServer[] sockets, NetworkBackendConfig[] configs, int basePort)
		{
			if (sockets == null) throw new ArgumentNullException(nameof(sockets));
			if (configs == null) throw new ArgumentNullException(nameof(configs));

			for (int i = 0; i < sockets.Length; i++)
			{
				if (sockets[i] == null)
				{
					continue;
				}
				sockets[i].SetPort(basePort + configs[i].PortOffset);
				sockets[i].Start();
			}
		}
	}
}
