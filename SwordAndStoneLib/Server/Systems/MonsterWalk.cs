using System;
using System.Collections.Generic;
using System.Text;
using ManicDigger;

namespace SwordAndStone.Server
{
	/// <summary>
	/// Updates monster movement each tick.
	/// Monsters wander randomly and change direction periodically.
	/// </summary>
	class ServerSystemMonsterWalk : ServerSystem
	{
		float elapsed;
		const float WalkSpeed = 2.0f;
		const float DirectionChangeInterval = 0.5f;

		public override void Update(Server server, float dt)
		{
			elapsed += dt;
			if (elapsed < DirectionChangeInterval)
			{
				return;
			}
			elapsed -= DirectionChangeInterval;

			foreach (var clientEntry in server.clients)
			{
				var playerPos = server.PlayerBlockPosition(clientEntry.Value);
				int chunksize = Server.chunksize;

				// Check chunks around the player
				for (int dx = -1; dx <= 1; dx++)
				{
					for (int dy = -1; dy <= 1; dy++)
					{
						for (int dz = -1; dz <= 1; dz++)
						{
							int cx = playerPos.x / chunksize + dx;
							int cy = playerPos.y / chunksize + dy;
							int cz = playerPos.z / chunksize + dz;

							if (!MapUtil.IsValidPos(server.d_Map, cx * chunksize, cy * chunksize, cz * chunksize))
							{
								continue;
							}

							ServerChunk chunk = server.d_Map.GetChunkValid(cx, cy, cz);
							if (chunk == null || chunk.Monsters == null)
							{
								continue;
							}

							for (int i = 0; i < chunk.Monsters.Count; i++)
							{
								Monster m = chunk.Monsters[i];
								UpdateMonster(server, m, dt);
							}
						}
					}
				}
			}
		}

		void UpdateMonster(Server server, Monster m, float dt)
		{
			m.WalkProgress += WalkSpeed * DirectionChangeInterval;

			if (m.WalkProgress >= 1.0f)
			{
				m.WalkProgress = 0;
				m.X += m.WalkDirection.x;
				m.Y += m.WalkDirection.y;

				// Pick a new random direction (stay on same Z level)
				int dir = server.rnd.Next(5); // 0=idle, 1-4=cardinal directions
				switch (dir)
				{
					case 0:
						m.WalkDirection = new Vector3i(0, 0, 0);
						break;
					case 1:
						m.WalkDirection = new Vector3i(1, 0, 0);
						break;
					case 2:
						m.WalkDirection = new Vector3i(-1, 0, 0);
						break;
					case 3:
						m.WalkDirection = new Vector3i(0, 1, 0);
						break;
					case 4:
						m.WalkDirection = new Vector3i(0, -1, 0);
						break;
				}

				// Validate destination is within map bounds
				int destX = m.X + m.WalkDirection.x;
				int destY = m.Y + m.WalkDirection.y;
				int destZ = m.Z + m.WalkDirection.z;
				if (!MapUtil.IsValidPos(server.d_Map, destX, destY, destZ))
				{
					m.WalkDirection = new Vector3i(0, 0, 0);
				}
			}
		}
	}
}
