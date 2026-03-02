using System;
using System.Collections.Generic;
using ManicDigger;

namespace SwordAndStone.Server
{
	public partial class Server
	{
		public bool GiveAll(int sourceClientId, string target)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.giveall))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				string targetName = targetClient.playername;
				string sourcename = GetClient(sourceClientId).playername;
				int maxStack = ((GameDataItemsBlocks)d_DataItems).MaxStackSize;
				for (int i = 0; i < BlockTypes.Length; i++)
				{
					if (!BlockTypes[i].IsBuildable)
					{
						continue;
					}
					Inventory inventory = GetPlayerInventory(targetName).Inventory;
					InventoryUtil util = GetInventoryUtil(inventory);

					for (int yy = 0; yy < util.CellCountY; yy++)
					{
						for (int xx = 0; xx < util.CellCountX; xx++)
						{
							if (!inventory.Items.ContainsKey(new ProtoPoint(xx, yy)))
							{
								continue;
							}
							Item currentItem = inventory.Items[new ProtoPoint(xx, yy)];
							if (currentItem != null
							    && currentItem.ItemClass == ItemClass.Block
							    && currentItem.BlockId == i)
							{
								currentItem.BlockCount = maxStack;
								goto nextblock;
							}
						}
					}
					for (int yy = 0; yy < util.CellCountY; yy++)
					{
						for (int xx = 0; xx < util.CellCountX; xx++)
						{
							Item newItem = new Item();
							newItem.ItemClass = ItemClass.Block;
							newItem.BlockId = i;
							newItem.BlockCount = maxStack;

							if (util.ItemAtCell(PointRef.Create(xx, yy)) == null)
							{
								inventory.Items[new ProtoPoint(xx, yy)] = newItem;
								goto nextblock;
							}
						}
					}
					nextblock:
					targetClient.IsInventoryDirty = true;
				}
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGiveAll"), colorSuccess, targetName));
				ServerEventLog(string.Format("{0} gives all to {1}.", sourcename, targetName));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
			return false;
		}

		public bool Give(int sourceClientId, string target, string blockname, int amount)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.give))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				string targetName = targetClient.playername;
				string sourcename = GetClient(sourceClientId).playername;
				int maxStack = ((GameDataItemsBlocks)d_DataItems).MaxStackSize;
				if (amount < 0)
				{
					return false;
				}
				if (amount > maxStack)
				{
					amount = maxStack;
				}
				for (int i = 0; i < BlockTypes.Length; i++)
				{
					if (!BlockTypes[i].IsBuildable)
					{
						continue;
					}
					if (!BlockTypes[i].Name.Equals(blockname, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					Inventory inventory = GetPlayerInventory(targetName).Inventory;
					InventoryUtil util = GetInventoryUtil(inventory);

					// Try to find given block in player inventory
					for (int xx = 0; xx < util.CellCountX; xx++)
					{
						for (int yy = 0; yy < util.CellCountY; yy++)
						{
							if (!inventory.Items.ContainsKey(new ProtoPoint(xx, yy)))
							{
								continue;
							}
							Item currentItem = inventory.Items[new ProtoPoint(xx, yy)];
							if (currentItem != null
							    && currentItem.ItemClass == ItemClass.Block
							    && currentItem.BlockId == i)
							{
								if (amount == 0)
								{
									// Delete block from player inventory if amount is 0
									inventory.Items.Remove(new ProtoPoint(xx, yy));
								}
								else
								{
									// Add specified amount to player inventory
									if (currentItem.BlockCount + amount > maxStack)
									{
										currentItem.BlockCount = maxStack;
									}
									else
									{
										currentItem.BlockCount += amount;
									}
								}
								goto nextblock;
							}
						}
					}
					// Block not yet in inventory. Add to first free slot.
					for (int xx = 0; xx < util.CellCountX; xx++)
					{
						for (int yy = 0; yy < util.CellCountY; yy++)
						{
							if (util.ItemAtCell(PointRef.Create(xx, yy)) == null)
							{
								Item newItem = new Item();
								newItem.ItemClass = ItemClass.Block;
								newItem.BlockId = i;
								newItem.BlockCount = amount;

								inventory.Items[new ProtoPoint(xx, yy)] = newItem;
								goto nextblock;
							}
						}
					}
					nextblock:
					targetClient.IsInventoryDirty = true;
				}
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGiveSuccess"), colorSuccess, amount, blockname, targetName));
				ServerEventLog(string.Format("{0} gives {1} {2} to {3}.", sourcename, amount, blockname, targetName));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
			return false;
		}

		public bool ResetInventory(int sourceClientId, string target)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.reset_inventory))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				ResetPlayerInventory(targetClient);
				SendMessageToAll(string.Format(language.Get("Server_CommandResetInventorySuccess"), colorImportant, GetClient(sourceClientId).ColoredPlayername(colorImportant), targetClient.ColoredPlayername(colorImportant)));
				ServerEventLog(string.Format("{0} resets inventory of {1}.", GetClient(sourceClientId).playername, targetClient.playername));
				return true;
			}
			// Player is not online.
			if (Inventory != null && Inventory.ContainsKey(target))
			{
				Inventory.Remove(target);
				SendMessageToAll(string.Format(language.Get("Server_CommandResetInventoryOfflineSuccess"), colorImportant, GetClient(sourceClientId).ColoredPlayername(colorImportant), target));
				ServerEventLog(string.Format("{0} resets inventory of {1} (offline).", GetClient(sourceClientId).playername, target));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
			return false;
		}

		public bool Monsters(int sourceClientId, string option)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.monsters))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			config.Monsters = option.Equals("off") ? false : true;
			configNeedsSaving = true;
			if (!config.Monsters)
			{
				foreach (var k in clients)
				{
					SendPacket(k.Key, Serialize(new Packet_Server() {
						Id = Packet_ServerIdEnum.RemoveMonsters
					}));
				}
			}
			SendMessageToAll(string.Format(language.Get("Server_CommandMonstersToggle"), GetClient(sourceClientId).ColoredPlayername(colorSuccess), option));
			ServerEventLog(string.Format("{0} turns monsters {1}.", GetClient(sourceClientId).playername, option));
			return true;
		}

		public bool AreaAdd(int sourceClientId, int id, string coords, string[] permittedGroups, string[] permittedUsers, int? level)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.area_add))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			if (config.Areas.Find(v => v.Id == id) != null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandAreaAddIdInUse"), colorError));
				return false;
			}

			AreaConfig newArea = new AreaConfig() { Id = id, Coords = coords };
			if (permittedGroups != null)
			{
				for (int i = 0; i < permittedGroups.Length; i++)
				{
					newArea.PermittedGroups.Add(permittedGroups[i]);
				}
			}
			if (permittedUsers != null)
			{
				for (int i = 0; i < permittedUsers.Length; i++)
				{
					newArea.PermittedUsers.Add(permittedUsers[i]);
				}
			}
			if (level != null)
			{
				newArea.Level = level;
			}

			config.Areas.Add(newArea);
			configNeedsSaving = true;
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandAreaAddSuccess"), colorSuccess, newArea.ToString()));
			ServerEventLog(string.Format("{0} adds area: {1}.", GetClient(sourceClientId), newArea.ToString()));
			return true;
		}

		public bool AreaDelete(int sourceClientId, int id)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.area_delete))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			AreaConfig targetArea = config.Areas.Find(v => v.Id == id);
			if (targetArea == null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandAreaDeleteNonexistant"), colorError));
				return false;
			}
			config.Areas.Remove(targetArea);
			configNeedsSaving = true;
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandAreaDeleteSuccess"), colorSuccess));
			ServerEventLog(string.Format("{0} deletes area: {1}.", GetClient(sourceClientId).playername, id));
			return true;
		}

		public bool SetSpawnPosition(int sourceClientId, string targetType, string target, int x, int y, int? z)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.set_spawn))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// validate spawn coordinates
			int rZ = 0;
			if (z == null)
			{
				if (!MapUtil.IsValidPos(d_Map, x, y))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnInvalidCoordinates"), colorError));
					return false;
				}
				rZ = MapUtil.blockheight(d_Map, 0, x, y);
			}
			else
			{
				rZ = z.Value;
			}
			if (!MapUtil.IsValidPos(d_Map, x, y, rZ))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnInvalidCoordinates"), colorError));
				return false;
			}

			switch (targetType)
			{
				case "-default":
				case "-d":
					serverClient.DefaultSpawn = new ManicDigger.Spawn() {
						x = x,
						y = y,
						z = z
					};
					serverClientNeedsSaving = true;
                // Inform related players.
					bool hasEntry = false;
					foreach (var k in clients)
					{
						hasEntry = false;
						if (k.Value.clientGroup.Spawn != null)
						{
							hasEntry = true;
						}
						else
						{
							foreach (ManicDigger.Client client in serverClient.Clients)
							{
								if (client.Name.Equals(k.Value.playername, StringComparison.InvariantCultureIgnoreCase))
								{
									if (client.Spawn != null)
									{
										hasEntry = true;
									}
									break;
								}
							}
						}
						if (!hasEntry)
						{
							this.SendPlayerSpawnPosition(k.Key, x, y, rZ);
						}
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnDefaultSuccess"), colorSuccess, x, y, rZ));
					ServerEventLog(String.Format("{0} sets default spawn to {1},{2}{3}.", GetClient(sourceClientId).playername, x, y, z == null ? "" : "," + z.Value));
					return true;
				case "-group":
				case "-g":
                // Check if group even exists.
					ManicDigger.Group targetGroup = serverClient.Groups.Find(
						                                delegate(ManicDigger.Group grp)
						{
							return grp.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase);
						}
					                                );
					if (targetGroup == null)
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGroupNotFound"), colorError, target));
						return false;
					}
					targetGroup.Spawn = new ManicDigger.Spawn() {
						x = x,
						y = y,
						z = z,
					};
					serverClientNeedsSaving = true;
                // Inform related players.
					hasEntry = false;
					foreach (var k in clients)
					{
						if (k.Value.clientGroup.Name.Equals(targetGroup.Name))
						{
							// Inform only if there is no spawn set under clients.
							foreach (ManicDigger.Client client in serverClient.Clients)
							{
								if (client.Name.Equals(k.Value.playername, StringComparison.InvariantCultureIgnoreCase))
								{
									if (client.Spawn != null)
									{
										hasEntry = true;
									}
									break;
								}
							}
							if (!hasEntry)
							{
								this.SendPlayerSpawnPosition(k.Key, x, y, rZ);
							}
						}
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnGroupSuccess"), colorSuccess, targetGroup.Name, x, y, rZ));
					ServerEventLog(String.Format("{0} sets spawn of group {1} to {2},{3}{4}.", GetClient(sourceClientId).playername, targetGroup.Name, x, y, z == null ? "" : "," + z.Value));
					return true;
				case "-player":
				case "-p":
                // Get related client.
					ClientOnServer targetClient = this.GetClient(target);
					int? targetClientId = null;
					if (targetClient != null)
					{
						targetClientId = targetClient.Id;
					}
					string targetClientPlayername = targetClient == null ? target : targetClient.playername;

					ManicDigger.Client clientEntry = serverClient.Clients.Find(
						                                 delegate(ManicDigger.Client client)
						{
							return client.Name.Equals(targetClientPlayername, StringComparison.InvariantCultureIgnoreCase);
						}
					                                 );
					if (clientEntry == null)
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
						return false;
					}
                // Change or add spawn entry of client.
					clientEntry.Spawn = new ManicDigger.Spawn() {
						x = x,
						y = y,
						z = z,
					};
					serverClientNeedsSaving = true;
                // Inform player if he's online.
					if (targetClientId != null)
					{
						this.SendPlayerSpawnPosition(targetClientId.Value, x, y, rZ);
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnPlayerSuccess"), colorSuccess, targetClientPlayername, x, y, rZ));
					ServerEventLog(String.Format("{0} sets spawn of player {1} to {2},{3}{4}.", GetClient(sourceClientId).playername, targetClientPlayername, x, y, z == null ? "" : "," + z.Value));
					return true;
				default:
					SendMessage(sourceClientId, language.Get("Server_CommandInvalidType"));
					return false;
			}
		}

		public bool SetSpawnPosition(int sourceClientId, int x, int y, int? z)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.set_home))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			Console.WriteLine(x + " " + y + " " + z);

			// Validate spawn position.
			int rZ = 0;
			if (z == null)
			{
				if (!MapUtil.IsValidPos(d_Map, x, y))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnInvalidCoordinates"), colorError));
					return false;
				}
				rZ = MapUtil.blockheight(d_Map, 0, x, y);
			}
			else
			{
				rZ = z.Value;
			}
			if (!MapUtil.IsValidPos(d_Map, x, y, rZ))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandSetSpawnInvalidCoordinates"), colorError));
				return false;
			}

			// Get related client entry.
			ManicDigger.Client clientEntry = serverClient.Clients.Find(
				                                 delegate(ManicDigger.Client client)
				{
					return client.Name.Equals(GetClient(sourceClientId).playername, StringComparison.InvariantCultureIgnoreCase);
				}
			                                 );
			// TODO: When guests have "set_home" privilege, count of client entries can quickly grow.
			if (clientEntry == null)
			{
				clientEntry = new ManicDigger.Client();
				clientEntry.Name = GetClient(sourceClientId).playername;
				clientEntry.Group = GetClient(sourceClientId).clientGroup.Name;
				serverClient.Clients.Add(clientEntry);
			}
			// Change or add spawn entry of client.
			clientEntry.Spawn = new ManicDigger.Spawn() {
				x = x,
				y = y,
				z = z,
			};
			serverClientNeedsSaving = true;
			// Send player new spawn position.
			this.SendPlayerSpawnPosition(sourceClientId, x, y, rZ);
			return true;
		}

		public bool TeleportToPlayer(int sourceClientId, int clientTo)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.tp))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			ClientOnServer t = clients[clientTo];
			ServerEntityPositionAndOrientation pos = t.entity.position.Clone();
			clients[sourceClientId].positionOverride = pos;
			return true;
		}

		public bool TeleportToPosition(int sourceClientId, int x, int y, int? z)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.tp_pos))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// validate target position
			int rZ = 0;
			if (z == null)
			{
				if (!MapUtil.IsValidPos(d_Map, x, y))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTeleportInvalidCoordinates"), colorError));
					return false;
				}
				rZ = MapUtil.blockheight(d_Map, 0, x, y);
			}
			else
			{
				rZ = z.Value;
			}
			if (!MapUtil.IsValidPos(d_Map, x, y, rZ))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTeleportInvalidCoordinates"), colorError));
				return false;
			}

			ClientOnServer client = GetClient(sourceClientId);
			ServerEntityPositionAndOrientation pos = client.entity.position.Clone();
			pos.x = x;
			pos.y = rZ;
			pos.z = y;
			client.positionOverride = pos;
			SendMessage(client.Id, string.Format(language.Get("Server_CommandTeleportSuccess"), colorSuccess, x, y, rZ));
			return true;
		}

		public bool TeleportPlayer(int sourceClientId, string target, int x, int y, int? z)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.teleport_player))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// validate target position
			int rZ = 0;
			if (z == null)
			{
				if (!MapUtil.IsValidPos(d_Map, x, y))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTeleportInvalidCoordinates"), colorError));
					return false;
				}
				rZ = MapUtil.blockheight(d_Map, 0, x, y);
			}
			else
			{
				rZ = z.Value;
			}
			if (!MapUtil.IsValidPos(d_Map, x, y, rZ))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTeleportInvalidCoordinates"), colorError));
				return false;
			}
			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				ServerEntityPositionAndOrientation pos = clients[targetClient.Id].entity.position;
				pos.x = x;
				pos.y = rZ;
				pos.z = y;
				clients[targetClient.Id].positionOverride = pos;
				SendMessage(targetClient.Id, string.Format(language.Get("Server_CommandTeleportTargetMessage"), colorImportant, x, y, rZ, GetClient(sourceClientId).ColoredPlayername(colorImportant)));
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTeleportSourceMessage"), colorSuccess, targetClient.ColoredPlayername(colorSuccess), x, y, rZ));
				ServerEventLog(string.Format("{0} teleports {1} to {2} {3} {4}.", GetClient(sourceClientId).playername, targetClient.playername, x, y, rZ));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandNonexistantPlayer"), colorError, target));
			return false;
		}

		public bool SetFillAreaLimit(int sourceClientId, string targetType, string target, int maxFill)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.fill_limit))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			switch (targetType)
			{
				case "-default":
				case "-d":
					serverClient.DefaultFillLimit = maxFill;
					serverClientNeedsSaving = true;
                // Inform related players.
					bool hasEntry = false;
					foreach (var k in clients)
					{
						hasEntry = false;
						if (k.Value.clientGroup.FillLimit != null)
						{
							hasEntry = true;
						}
						else
						{
							foreach (ManicDigger.Client client in serverClient.Clients)
							{
								if (client.Name.Equals(k.Value.playername, StringComparison.InvariantCultureIgnoreCase))
								{
									if (client.FillLimit != null)
									{
										hasEntry = true;
									}
									break;
								}
							}
						}
						if (!hasEntry)
						{
							this.SetFillAreaLimit(k.Key);
						}
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandFillLimitDefaultSuccess"), colorSuccess, maxFill));
					ServerEventLog(String.Format("{0} sets default fill area limit to {1}.", GetClient(sourceClientId).playername, maxFill));
					return true;
				case "-group":
				case "-g":
                // Check if group even exists.
					ManicDigger.Group targetGroup = serverClient.Groups.Find(
						                                delegate(ManicDigger.Group grp)
						{
							return grp.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase);
						}
					                                );
					if (targetGroup == null)
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGroupNotFound"), colorError, target));
						return false;
					}
					targetGroup.FillLimit = maxFill;
					serverClientNeedsSaving = true;
                // Inform related players.
					hasEntry = false;
					foreach (var k in clients)
					{
						if (k.Value.clientGroup.Name.Equals(targetGroup.Name))
						{
							// Inform only if there is no spawn set under clients.
							foreach (ManicDigger.Client client in serverClient.Clients)
							{
								if (client.Name.Equals(k.Value.playername, StringComparison.InvariantCultureIgnoreCase))
								{
									if (client.FillLimit != null)
									{
										hasEntry = true;
									}
									break;
								}
							}
							if (!hasEntry)
							{
								this.SetFillAreaLimit(k.Key);
							}
						}
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandFillLimitGroupSuccess"), colorSuccess, targetGroup.Name, maxFill));
					ServerEventLog(String.Format("{0} sets spawn of group {1} to {2}.", GetClient(sourceClientId).playername, targetGroup.Name, maxFill));
					return true;
				case "-player":
				case "-p":
                // Get related client.
					ClientOnServer targetClient = this.GetClient(target);
					int? targetClientId = null;
					if (targetClient != null)
					{
						targetClientId = targetClient.Id;
					}
					string targetClientPlayername = targetClient == null ? target : targetClient.playername;

					ManicDigger.Client clientEntry = serverClient.Clients.Find(
						                                 delegate(ManicDigger.Client client)
						{
							return client.Name.Equals(targetClientPlayername, StringComparison.InvariantCultureIgnoreCase);
						}
					                                 );
					if (clientEntry == null)
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
						return false;
					}
                // Change or add spawn entry of client.
					clientEntry.FillLimit = maxFill;
					serverClientNeedsSaving = true;
                // Inform player if he's online.
					if (targetClientId != null)
					{
						this.SetFillAreaLimit(targetClientId.Value);
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandFillLimitPlayerSuccess"), colorSuccess, targetClientPlayername, maxFill));
					ServerEventLog(String.Format("{0} sets fill area limit of player {1} to {2}.", GetClient(sourceClientId).playername, targetClientPlayername, maxFill));
					return true;
				default:
					SendMessage(sourceClientId, language.Get("Server_CommandInvalidType"));
					return false;
			}
		}

		public bool TimeCommand(int sourceClientId, string argument)
		{
			string[] strSplit = argument.Split(' ');

			if (strSplit.Length == 2)
			{
				//We assume that all parameterized commands require a privilege
				if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.time))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
					return false;
				}

				//We expect a operation and a value
				string strValue = strSplit[1];

				switch (strSplit[0])
				{
					case "set":
						{
							TimeSpan time;

							if (!strValue.Contains(":"))
							{
								//If only a number is present, the days will be set to the given number
								//since we don't want that, a ":" is enforced
								SendMessage(sourceClientId, colorError + language.Get("Server_CommandException") + " unable to convert \"" + strValue + "\" to a time");
							}
							else
							if (TimeSpan.TryParse(strValue, out time))
							{
								_time.Set(time);
								SendMessage(sourceClientId, "The time is: " + _time.Time.ToString());
							}
							else
							{
								SendMessage(sourceClientId, colorError + language.Get("Server_CommandException") + " unable to convert \"" + strValue + "\" to a time");
							}
						}
						break;
					case "add":
						{
							TimeSpan time;

							int nMinuts = 0;
							if (int.TryParse(strValue, out nMinuts))
							{
								//only a number
								//take it as minutes
								_time.Add(TimeSpan.FromMinutes(nMinuts));
								SendMessage(sourceClientId, "The time is: " + _time.Time.ToString());
							}
							else
							if (TimeSpan.TryParse(strValue, out time))
							{
								_time.Add(time);
								SendMessage(sourceClientId, "The time is: " + _time.Time.ToString());
							}
							else
							{
								SendMessage(sourceClientId, colorError + language.Get("Server_CommandException") + " unable to convert \"" + strValue + "\" to a time");
							}
						}
						break;
					case "speed":
						{
							int nSpeed = 0;

							if (!int.TryParse(strValue, out nSpeed))
							{
								SendMessage(sourceClientId, colorError + language.Get("Server_CommandException") + " unable to convert \"" + strValue + "\" to a number");
							}
							else
							{
								_time.SpeedOfTime = nSpeed;
								SendMessage(sourceClientId, "speed of time changed");
							}
						}
						break;
				}
			}
			else
			{
				SendMessage(sourceClientId, string.Format("Current time: Year {0}, Day {1}, {2}:{3}:{4}", _time.Year, _time.Day, _time.Time.Hours, _time.Time.Minutes, _time.Time.Seconds));
			}
			return true;
		}
	}
}
