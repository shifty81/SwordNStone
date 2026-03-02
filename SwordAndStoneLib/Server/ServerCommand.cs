using System;
using System.Collections.Generic;
using ManicDigger;

namespace SwordAndStone.Server
{
	public partial class Server
	{
		public void CommandInterpreter(int sourceClientId, string command, string argument)
		{
			string[] ss;
			int id;

			switch (command)
			{
				case "msg":
				case "pm":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length >= 2)
					{
						this.PrivateMessage(sourceClientId, ss[0], string.Join(" ", ss, 1, ss.Length - 1));
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "re":
					if (!string.IsNullOrEmpty(argument))
					{
						this.AnswerMessage(sourceClientId, argument);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "op":
				case "chgrp":
				case "cg":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 2)
					{
						this.ChangeGroup(sourceClientId, ss[0], ss[1]);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "op_offline":
				case "chgrp_offline":
				case "cg_offline":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 2)
					{
						this.ChangeGroupOffline(sourceClientId, ss[0], ss[1]);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "remove_client":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 1)
					{
						this.RemoveClientFromConfig(sourceClientId, ss[0]);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "login":
                // enables to change temporary group with a group's password (only if group allows it)
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 2)
					{
						this.Login(sourceClientId, ss[0], ss[1]);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "welcome":
					this.WelcomeMessage(sourceClientId, argument);
					return;
				case "announcement":
					this.Announcement(sourceClientId, argument);
					return;
				case "logging":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 1)
					{
						this.SetLogging(sourceClientId, ss[0], "");
						return;
					}
					if (ss.Length == 2)
					{
						this.SetLogging(sourceClientId, ss[0], ss[1]);
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "kick_id":
					ss = argument.Split(new[] { ' ' });
					if (!Int32.TryParse(ss[0], out id))
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					if (ss.Length >= 2)
					{
						this.Kick(sourceClientId, id, string.Join(" ", ss, 1, ss.Length - 1));
						return;
					}
					this.Kick(sourceClientId, id);
					return;
				case "kick":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length >= 2)
					{
						this.Kick(sourceClientId, ss[0], string.Join(" ", ss, 1, ss.Length - 1));
						return;
					}
					this.Kick(sourceClientId, argument);
					return;
				case "list":
					this.List(sourceClientId, argument);
					return;
				case "giveall":
					this.GiveAll(sourceClientId, argument);
					return;
				case "give":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length == 3)
					{
						int amount;
						if (!Int32.TryParse(ss[2], out amount))
						{
							SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
							return;
						}
						else
						{
							this.Give(sourceClientId, ss[0], ss[1], amount);
						}
						return;
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
					return;
				case "monsters":
					if (!argument.Equals("off") && !argument.Equals("on"))
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					this.Monsters(sourceClientId, argument);
					return;
				case "area_add":
					int areaId;
					ss = argument.Split(new[] { ' ' });

					if (ss.Length < 4 || ss.Length > 5)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}

					if (!Int32.TryParse(ss[0], out areaId))
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					string coords = ss[1];
					string[] permittedGroups = ss[2].ToString().Split(new[] { ',' });
					string[] permittedUsers = ss[3].ToString().Split(new[] { ',' });

					int? areaLevel;
					try
					{
						areaLevel = Convert.ToInt32(ss[4]);
					}
					catch (IndexOutOfRangeException)
					{
						areaLevel = null;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}

					this.AreaAdd(sourceClientId, areaId, coords, permittedGroups, permittedUsers, areaLevel);
					return;
				case "area_delete":
					if (!Int32.TryParse(argument, out areaId))
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					this.AreaDelete(sourceClientId, areaId);
					return;
				case "help":
					this.Help(sourceClientId);
					return;
				case "run":
				case "":
                // JavaScript
                // assume script expression or command coming
					var script = argument;
					RunInClientSandbox(script, sourceClientId);
					return;
				case "crash":
					KillPlayer(sourceClientId);
					return;
				case "set_spawn":
                //           0    1      2 3 4
                // argument: type target x y z
					ss = argument.Split(new[] { ' ' });

					if (ss.Length < 3 || ss.Length > 5)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}

                // Add an empty target argument, when user sets default spawn.
					if (ss[0].Equals("-d") || ss[0].Equals("-default"))
					{
						string[] ssTemp = new string[ss.Length + 1];
						ssTemp[0] = ss[0];
						ssTemp[1] = "";
						Array.Copy(ss, 1, ssTemp, 2, ss.Length - 1);
						ss = ssTemp;
					}

					int x;
					int y;
					int? z;
					try
					{
						x = Convert.ToInt32(ss[2]);
						y = Convert.ToInt32(ss[3]);
					}
					catch (IndexOutOfRangeException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}

					try
					{
						z = Convert.ToInt32(ss[4]);
					}
					catch (IndexOutOfRangeException)
					{
						z = null;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					this.SetSpawnPosition(sourceClientId, ss[0], ss[1], x, y, z);
					return;
				case "set_home":
                // When no coordinates are given, set spawn to players current position.
					if (string.IsNullOrEmpty(argument))
					{
						this.SetSpawnPosition(sourceClientId,
							(int)GetClient(sourceClientId).PositionMul32GlX / 32,
							(int)GetClient(sourceClientId).PositionMul32GlZ / 32,
							(int)GetClient(sourceClientId).PositionMul32GlY / 32);
						return;
					}
                //            0 1 2
                // agrument:  x y z
					ss = argument.Split(new[] { ' ' });

					if (ss.Length < 2 || ss.Length > 3)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					try
					{
						x = Convert.ToInt32(ss[0]);
						y = Convert.ToInt32(ss[1]);
					}
					catch (IndexOutOfRangeException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}

					try
					{
						z = Convert.ToInt32(ss[2]);
					}
					catch (IndexOutOfRangeException)
					{
						z = null;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidSpawnPosition"));
						return;
					}
					this.SetSpawnPosition(sourceClientId, x, y, z);
					return;
				case "privilege_add":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length != 2)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					this.PrivilegeAdd(sourceClientId, ss[0], ss[1]);
					return;
				case "privilege_remove":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length != 2)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					this.PrivilegeRemove(sourceClientId, ss[0], ss[1]);
					return;
				case "restart":
					this.RestartServer(sourceClientId);
					break;
				case "shutdown":
					this.ShutdownServer(sourceClientId);
					break;
			//case "crashserver": for (; ; ) ;
				case "stats":
					double seconds = (DateTime.UtcNow - statsupdate).TotalSeconds;
					SendMessage(sourceClientId, "Packets/s:" + decimal.Round((decimal)(StatTotalPackets / seconds), 2, MidpointRounding.AwayFromZero));
					SendMessage(sourceClientId, "Total KBytes/s:" + decimal.Round((decimal)(StatTotalPacketsLength / seconds / 1024), 2, MidpointRounding.AwayFromZero));
					break;
				case "tp":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length != 1)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					foreach (var k in clients)
					{
						if (k.Value.playername.Equals(ss[0], StringComparison.InvariantCultureIgnoreCase))
						{
							this.TeleportToPlayer(sourceClientId, k.Key);
							return;
						}
					}
					foreach (var k in clients)
					{
						if (k.Value.playername.StartsWith(ss[0], StringComparison.InvariantCultureIgnoreCase))
						{
							this.TeleportToPlayer(sourceClientId, k.Key);
							return;
						}
					}
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandNonexistantPlayer"), colorError, ss[0]));
					break;
				case "tp_pos":
					ss = argument.Split(new[] { ' ' });
					if (ss.Length < 2 || ss.Length > 3)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}

					try
					{
						x = Convert.ToInt32(ss[0]);
						y = Convert.ToInt32(ss[1]);
					}
					catch (IndexOutOfRangeException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}

					try
					{
						z = Convert.ToInt32(ss[2]);
					}
					catch (IndexOutOfRangeException)
					{
						z = null;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					this.TeleportToPosition(sourceClientId, x, y, z);
					break;
				case "teleport_player":
					ss = argument.Split(new[] { ' ' });

					if (ss.Length < 3 || ss.Length > 4)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}

					try
					{
						x = Convert.ToInt32(ss[1]);
						y = Convert.ToInt32(ss[2]);
					}
					catch (IndexOutOfRangeException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}

					try
					{
						z = Convert.ToInt32(ss[3]);
					}
					catch (IndexOutOfRangeException)
					{
						z = null;
					}
					catch (FormatException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					catch (OverflowException)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidPosition"));
						return;
					}
					this.TeleportPlayer(sourceClientId, ss[0], x, y, z);
					break;
				case "backup_database":
					if (!GetClient(sourceClientId).privileges.Contains(ServerClientMisc.Privilege.backup_database))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						break;
					}
					if (!BackupDatabase(argument))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandBackupFailed"), colorError));
					}
					else
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandBackupCreated"), colorSuccess));
						ServerEventLog(String.Format("{0} backups database: {1}.", GetClient(sourceClientId).playername, argument));
					}
					break;
			/*
        case "load":
            if (!GetClient(sourceClientId).privileges.Contains(ServerClientMisc.Privilege.load))
            {
                SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
                break;
            }
            if (!GameStorePath.IsValidName(argument))
            {
                SendMessage(sourceClientId, string.Format("Invalid load filename: {0}", argument));
                break;
            }
            if (!LoadDatabase(argument))
            {
                SendMessage(sourceClientId, string.Format("{0}World could not be loaded. Check filename.", colorError));
            }
            else
            {
                SendMessage(sourceClientId, string.Format("{0}World loaded.", colorSuccess));
                ServerEventLog(String.Format("{0} loads world: {1}.", GetClient(sourceClientId).playername, argument));
            }
            break;
            */
				case "reset_inventory":
					this.ResetInventory(sourceClientId, argument);
					return;
				case "fill_limit":
                //           0    1      2
                // agrument: type target maxFill
					ss = argument.Split(new[] { ' ' });
					if (ss.Length < 2 || ss.Length > 3)
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
                // Add an empty target argument, when user sets default max-fill.
					if (ss[0].Equals("-d") || ss[0].Equals("-default"))
					{
						string[] ssTemp = new string[ss.Length + 1];
						ssTemp[0] = ss[0];
						ssTemp[1] = "";
						Array.Copy(ss, 1, ssTemp, 2, ss.Length - 1);
						ss = ssTemp;
					}
					int maxFill;
					if (!Int32.TryParse(ss[2], out maxFill))
					{
						SendMessage(sourceClientId, colorError + language.Get("Server_CommandInvalidArgs"));
						return;
					}
					else
					{
						this.SetFillAreaLimit(sourceClientId, ss[0], ss[1], maxFill);
					}
					return;
				case "time":
					{
						TimeCommand(sourceClientId, argument);
					}
					break;
				default:
					for (int i = 0; i < systemsCount; i++)
					{
						if (systems[i] == null)
						{
							continue;
						}
						try
						{
							if (systems[i].OnCommand(this, sourceClientId, command, argument))
							{
								return;
							}
						}
						catch
						{
							SendMessage(sourceClientId, language.Get("Server_CommandException"));
						}
					}
					for (int i = 0; i < modEventHandlers.oncommand.Count; i++)
					{
						try
						{
							if (modEventHandlers.oncommand[i](sourceClientId, command, argument))
							{
								return;
							}
						}
						catch
						{
							SendMessage(sourceClientId, language.Get("Server_CommandException"));
						}
					}
					SendMessage(sourceClientId, colorError + language.Get("Server_CommandUnknown") + command);
					return;
			}
		}

		public void Help(int sourceClientId)
		{
			SendMessage(sourceClientId, colorHelp + "Available privileges:");
			foreach (string privilege in GetClient(sourceClientId).privileges)
			{
				SendMessage(sourceClientId, string.Format("{0}{1}: {2}", colorHelp, privilege.ToString(), this.CommandHelp(privilege.ToString())));
			}
		}

		private string CommandHelp(string command)
		{
			switch (command)
			{
				case "msg":
				case "pm":
					return "/msg [username] [message]";
				case "kick":
					return "/kick [username] {reason}";
				case "kick_id":
					return "kick_id [player id] {reason}";
				case "ban":
					return "/ban [username] {reason}";
				case "ban_id":
					return "/ban_id [player id] {reason}";
				case "banip":
					return "/banip [username] {reason}";
				case "banip_id":
					return "/banip_id [player id] {reason}";
				case "ban_offline":
					return "/ban_offline [username] {reason}";
				case "unban":
					return "/unban [-p playername | -ip ipaddress]";
				case "run":
					return "/run [JavaScript (max. length 4096 char.)]";
				case "op":
					return "/op [username] [group]";
				case "chgrp":
					return "/chgrp [username] [group]";
				case "op_offline":
					return "/op_offline [username] [group]";
				case "chgrp_offline":
					return "/chgrp_offline [username] [group]";
				case "remove_client":
					return "/remove_client [username]";
				case "login":
					return "/login [group] [password]";
				case "welcome":
					return "/welcome [login motd message]";
				case "logging":
					return "/logging [-s | -b | -se | -c] {on | off}";
				case "list_clients":
					return "/list [-clients]";
				case "list_saved_clients":
					return "/list [-saved_clients]";
				case "list_groups":
					return "/list [-groups]";
				case "list_banned_users":
					return "/list [-bannedusers | -bannedips]";
				case "list_areas":
					return "/list [-areas]";
				case "give":
					return "/give [username] blockname amount";
				case "giveall":
					return "/giveall [username]";
				case "monsters":
					return "/monsters [on|off]";
				case "area_add":
					return "/area_add [ID] [x1,x2,y1,y2,z1,z2] [group1,group2,..] [user1,user2,..] {level}";
				case "area_delete":
					return "/area_delete [ID]";
				case "announcement":
					return "/announcement [message]";
				case "set_spawn":
					return "/set_spawn [-default|-group|-player] [target] [x] [y] {z}";
				case "set_home":
					return "/set_home {[x] [y] {z}}";
				case "privilege_add":
					return "/privilege_add [username] [privilege]";
				case "privilege_remove":
					return "/privilege_remove [username] [privilege]";
				case "restart":
					return "/restart";
				case "teleport_player":
					return "/teleport_player [target] [x] [y] {z}";
				case "time":
					return "/time {[set|add|speed] [value]}";
				case "tp":
					return "/tp [username]";
				case "tp_pos":
					return "/tp_pos [x] [y] {z}";
				case "backup_database":
					return "/backup_database [filename]";
				case "reset_inventory":
					return "/reset_inventory [target]";
				case "fill_limit":
					return "/fill_limit [-default|-group|-player] [limit]";
				default:
					if (commandhelps.ContainsKey(command))
					{
						return commandhelps[command];
					}
					return "No description available.";
			}
		}
		public Dictionary<string, string> commandhelps = new Dictionary<string, string>();
		public Dictionary<string, string> lastSender = new Dictionary<string, string>();
	}
}
