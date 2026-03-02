using System;
using System.Collections.Generic;
using ManicDigger;

namespace SwordAndStone.Server
{
	public partial class Server
	{
		public bool ChangeGroup(int sourceClientId, string target, string newGroupName)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.chgrp))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// Get related group from config file.
			ManicDigger.Group newGroup = serverClient.Groups.Find(
				                             delegate(ManicDigger.Group grp)
				{
					return grp.Name.Equals(newGroupName, StringComparison.InvariantCultureIgnoreCase);
				}
			                             );
			if (newGroup == null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGroupNotFound"), colorError, newGroupName));
				return false;
			}

			// Forbid to assign groups with levels higher then the source's client group level.
			if (newGroup.IsSuperior(GetClient(sourceClientId).clientGroup))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetGroupSuperior"), colorError));
				return false;
			}

			// Get related client from config file
			ManicDigger.Client clientConfig = serverClient.Clients.Find(
				                                  delegate(ManicDigger.Client client)
				{
					return client.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase);
				}
			                                  );

			// Get related client.
			ClientOnServer targetClient = GetClient(target);

			if (targetClient != null)
			{
				if (targetClient.clientGroup.IsSuperior(GetClient(sourceClientId).clientGroup) || targetClient.clientGroup.EqualLevel(GetClient(sourceClientId).clientGroup))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetUserSuperior"), colorError));
					return false;
				}
				// Add or change group membership in config file.

				// Client is not yet in config file. Create a new entry.
				if (clientConfig == null)
				{
					clientConfig = new ManicDigger.Client();
					clientConfig.Name = targetClient.playername;
					clientConfig.Group = newGroup.Name;
					serverClient.Clients.Add(clientConfig);
				}
				else
				{
					clientConfig.Group = newGroup.Name;
				}
				serverClientNeedsSaving = true;
				SendMessageToAll(string.Format(language.Get("Server_CommandSetGroupTo"), colorSuccess, GetClient(sourceClientId).ColoredPlayername(colorSuccess), targetClient.ColoredPlayername(colorSuccess), newGroup.GroupColorString() + newGroup.Name + colorSuccess));
				ServerEventLog(String.Format("{0} sets group of {1} to {2}.", GetClient(sourceClientId).playername, targetClient.playername, newGroup.Name));
				targetClient.AssignGroup(newGroup);
				SendFreemoveState(targetClient.Id, targetClient.privileges.Contains(ServerClientMisc.Privilege.freemove));
				SetFillAreaLimit(targetClient.Id);
				return true;
			}

			// Target is at the moment not online.
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandOpTargetOffline"), colorError, target));
			return false;
		}

		public bool ChangeGroupOffline(int sourceClientId, string target, string newGroupName)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.chgrp_offline))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// Get related group from config file.
			ManicDigger.Group newGroup = serverClient.Groups.Find(
				                             delegate(ManicDigger.Group grp)
				{
					return grp.Name.Equals(newGroupName, StringComparison.InvariantCultureIgnoreCase);
				}
			                             );
			if (newGroup == null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGroupNotFound"), colorError, newGroupName));
				return false;
			}

			// Forbid to assign groups with levels higher then the source's client group level.
			if (newGroup.IsSuperior(GetClient(sourceClientId).clientGroup))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetGroupSuperior"), colorError));
				return false;
			}

			// Get related client from config file.
			ManicDigger.Client clientConfig = serverClient.Clients.Find(
				                                  delegate(ManicDigger.Client client)
				{
					return client.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase);
				}
			                                  );

			// Get related client.
			ClientOnServer targetClient = GetClient(target);

			if (targetClient != null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandOpTargetOnline"), colorError, target));
				return false;
			}

			// Target is at the moment not online. Create or change a entry in ServerClient.
			string oldGroupColor = "";
			if (clientConfig == null)
			{
				clientConfig = new ManicDigger.Client();
				clientConfig.Name = target;
				clientConfig.Group = newGroup.Name;
				serverClient.Clients.Add(clientConfig);
			}
			else
			{
				// Check if target's current group is superior.
				ManicDigger.Group oldGroup = serverClient.Groups.Find(
					                             delegate(ManicDigger.Group grp)
					{
						return grp.Name.Equals(clientConfig.Group);
					}
				                             );
				if (oldGroup == null)
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInvalidGroup"), colorError));
					return false;
				}
				if (oldGroup.IsSuperior(GetClient(sourceClientId).clientGroup) || oldGroup.EqualLevel(GetClient(sourceClientId).clientGroup))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetUserSuperior"), colorError));
					return false;
				}
				oldGroupColor = oldGroup.GroupColorString();
				clientConfig.Group = newGroup.Name;
			}

			serverClientNeedsSaving = true;
			SendMessageToAll(string.Format(language.Get("Server_CommandSetOfflineGroupTo"), colorSuccess, GetClient(sourceClientId).ColoredPlayername(colorSuccess), oldGroupColor + target + colorSuccess, newGroup.GroupColorString() + newGroup.Name + colorSuccess));
			ServerEventLog(String.Format("{0} sets group of {1} to {2} (offline).", GetClient(sourceClientId).playername, target, newGroup.Name));
			return true;
		}

		public bool RemoveClientFromConfig(int sourceClientId, string target)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.remove_client))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			// Get related client from config file
			ManicDigger.Client targetClient = serverClient.Clients.Find(
				                                  delegate(ManicDigger.Client client)
				{
					return client.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase);
				}
			                                  );
			// Entry exists.
			if (targetClient != null)
			{
				// Get target's group.
				ManicDigger.Group targetGroup = serverClient.Groups.Find(
					                                delegate(ManicDigger.Group grp)
					{
						return grp.Name.Equals(targetClient.Group);
					}
				                                );
				if (targetGroup == null)
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInvalidGroup"), colorError));
					return false;
				}
				// Check if target's group is superior.
				if (targetGroup.IsSuperior(GetClient(sourceClientId).clientGroup) || targetGroup.EqualLevel(GetClient(sourceClientId).clientGroup))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetUserSuperior"), colorError));
					return false;
				}
				// Remove target's entry.
				serverClient.Clients.Remove(targetClient);
				serverClientNeedsSaving = true;
				// If client is online, change his group
				if (GetClient(target) != null)
				{
					GetClient(target).AssignGroup(this.defaultGroupGuest);
					SendMessageToAll(string.Format(language.Get("Server_CommandSetGroupTo"), colorSuccess, GetClient(sourceClientId).ColoredPlayername(colorSuccess), GetClient(target).ColoredPlayername(colorSuccess), this.defaultGroupGuest.GroupColorString() + defaultGroupGuest.Name));
				}
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandRemoveSuccess"), colorSuccess, target));
				ServerEventLog(string.Format("{0} removes client {1} from config.", GetClient(sourceClientId).playername, target));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandRemoveNotFound"), colorError, target));
			return false;
		}

		public bool Login(int sourceClientId, string targetGroupString, string password)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.login))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			ManicDigger.Group targetGroup = serverClient.Groups.Find(
				                                delegate(ManicDigger.Group grp)
				{
					return grp.Name.Equals(targetGroupString, StringComparison.InvariantCultureIgnoreCase);
				}
			                                );
			if (targetGroup == null)
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandGroupNotFound"), colorError, targetGroupString));
				return false;
			}
			if (string.IsNullOrEmpty(targetGroup.Password))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandLoginNoPW"), colorError, targetGroupString));
				return false;
			}
			if (targetGroup.Password.Equals(password))
			{
				GetClient(sourceClientId).AssignGroup(targetGroup);
				SendFreemoveState(sourceClientId, GetClient(sourceClientId).privileges.Contains(ServerClientMisc.Privilege.freemove));
				SendMessageToAll(string.Format(language.Get("Server_CommandLoginSuccess"), colorSuccess, GetClient(sourceClientId).ColoredPlayername(colorSuccess), targetGroupString));
				SendMessage(sourceClientId, language.Get("Server_CommandLoginInfo"));
				ServerEventLog(string.Format("{0} logs in group {1}.", GetClient(sourceClientId).playername, targetGroupString));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandLoginInvalidPassword"), colorError));
			ServerEventLog(string.Format("{0} fails to log in (invalid password: {1}).", GetClient(sourceClientId).playername, password));
			return false;
		}

		public bool SetLogging(int sourceClientId, string type, string option)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.logging))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			switch (type)
			{
			// all logging state
				case "-s":
					SendMessage(sourceClientId, "Build: " + config.BuildLogging);
					SendMessage(sourceClientId, "Server events: " + config.ServerEventLogging);
					SendMessage(sourceClientId, "Chat: " + config.ChatLogging);
					return true;
				case "-b":
					if (option.Equals("on"))
					{
						config.BuildLogging = true;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Build logging enabled.", colorSuccess));
						ServerEventLog(string.Format("{0} enables build logging.", GetClient(sourceClientId).playername));
						return true;
					}
					if (option.Equals("off"))
					{
						config.BuildLogging = false;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Build logging disabled.", colorSuccess));
						ServerEventLog(string.Format("{0} disables build logging.", GetClient(sourceClientId).playername));
						return true;
					}
					SendMessage(sourceClientId, string.Format("{0}Build logging: {1}", colorNormal, config.BuildLogging));
					return true;
				case "-se":
					if (option.Equals("on"))
					{
						config.ServerEventLogging = true;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Server event logging enabled.", colorSuccess));
						ServerEventLog(string.Format("{0} enables server event logging.", GetClient(sourceClientId).playername));
						return true;
					}
					if (option.Equals("off"))
					{
						ServerEventLog(string.Format("{0} disables server event logging.", GetClient(sourceClientId).playername));
						config.ServerEventLogging = false;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Server event logging disabled.", colorSuccess));
						return true;
					}
					SendMessage(sourceClientId, string.Format("{0}Server event logging: {1}", colorNormal, config.ServerEventLogging));
					return true;
				case "-c":
					if (option.Equals("on"))
					{
						config.ChatLogging = true;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Chat logging enabled.", colorSuccess));
						ServerEventLog(string.Format("{0} enables chat logging.", GetClient(sourceClientId).playername));
						return true;
					}
					if (option.Equals("off"))
					{
						config.ChatLogging = false;
						configNeedsSaving = true;
						SendMessage(sourceClientId, string.Format("{0}Chat logging disabled.", colorSuccess));
						ServerEventLog(string.Format("{0} disables chat logging.", GetClient(sourceClientId).playername));
						return true;
					}
					SendMessage(sourceClientId, string.Format("{0}Chat logging: {1}", colorNormal, config.ChatLogging));
					return true;
				default:
					SendMessage(sourceClientId, string.Format("{0}Invalid type: {1}", colorError, type));
					return false;
			}
		}

		public bool Kick(int sourceClientId, string target)
		{
			return Kick(sourceClientId, target, "");
		}

		public bool Kick(int sourceClientId, string target, string reason)
		{
			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				return this.Kick(sourceClientId, targetClient.Id, reason);
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, target));
			return false;
		}

		public bool Kick(int sourceClientId, int targetClientId)
		{
			return this.Kick(sourceClientId, targetClientId, "");
		}

		public bool Kick(int sourceClientId, int targetClientId, string reason)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.kick))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			if (!reason.Equals(""))
			{
				reason = language.Get("Server_CommandKickBanReason") + reason + ".";
			}
			ClientOnServer targetClient = GetClient(targetClientId);
			if (targetClient != null)
			{
				if (targetClient.clientGroup.IsSuperior(GetClient(sourceClientId).clientGroup) || targetClient.clientGroup.EqualLevel(GetClient(sourceClientId).clientGroup))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandTargetUserSuperior"), colorError));
					return false;
				}
				string targetName = targetClient.playername;
				string sourceName = GetClient(sourceClientId).playername;
				string targetNameColored = targetClient.ColoredPlayername(colorImportant);
				string sourceNameColored = GetClient(sourceClientId).ColoredPlayername(colorImportant);
				SendMessageToAll(string.Format(language.Get("Server_CommandKickMessage"), colorImportant, targetNameColored, sourceNameColored, reason));
				ServerEventLog(string.Format("{0} kicks {1}.{2}", sourceName, targetName, reason));
				SendPacket(targetClientId, ServerPackets.DisconnectPlayer(string.Format(language.Get("Server_CommandKickNotification"), reason)));
				KillPlayer(targetClientId);
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandNonexistantID"), colorError, targetClientId));
			return false;
		}

		public bool List(int sourceClientId, string type)
		{
			switch (type)
			{
				case "-clients":
				case "-c":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_clients))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of Players:");
					foreach (var k in clients)
					{
						// Format: Key Playername IP
						SendMessage(sourceClientId, string.Format("[{0}] {1} {2}", k.Key, k.Value.ColoredPlayername(colorNormal), (k.Value.socket.RemoteEndPoint()).AddressToString()));
					}
					return true;
				case "-clients2":
				case "-c2":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_clients))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of Players:");
					foreach (var k in clients)
					{
						// Format: Key Playername:Group:Privileges IP
						SendMessage(sourceClientId, string.Format("[{0}] {1}", k.Key, k.Value.ToString()));
					}
					return true;
				case "-areas":
				case "-a":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_areas))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of Areas:");
					foreach (AreaConfig area in config.Areas)
					{
						SendMessage(sourceClientId, area.ToString());
					}
					return true;
				case "-bannedusers":
				case "-bu":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_banned_users))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of Banned Users:");
					foreach (UserEntry currentUser in banlist.BannedUsers)
					{
						//Format:	Name: Reason
						string reason = currentUser.Reason;
						if (string.IsNullOrEmpty(reason))
							reason = "";
						SendMessage(sourceClientId, string.Format("{0}:{1}", currentUser.UserName, reason));
					}
					return true;
				case "-bannedips":
				case "-bip":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_banned_users))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of Banned IPs:");
					foreach (IPEntry currentIP in banlist.BannedIPs)
					{
						//Format:	IP: Reason
						string reason = currentIP.Reason;
						if (string.IsNullOrEmpty(reason))
							reason = "";
						SendMessage(sourceClientId, string.Format("{0}:{1}", currentIP.IPAdress, reason));
					}
					return true;
				case "-groups":
				case "-g":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_groups))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of groups:");
					foreach (ManicDigger.Group currenGroup in serverClient.Groups)
					{
						SendMessage(sourceClientId, currenGroup.ToString());
					}
					return true;
				case "-saved_clients":
				case "-sc":
					if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.list_saved_clients))
					{
						SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
						return false;
					}
					SendMessage(sourceClientId, colorImportant + "List of saved clients:");
					foreach (ManicDigger.Client currenClient in serverClient.Clients)
					{

						SendMessage(sourceClientId, currenClient.ToString());
					}
					return true;
				default:
					SendMessage(sourceClientId, "Invalid parameter.");
					return false;
			}
		}

		public bool ClearInterpreter(int sourceClientId)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.run))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			GetClient(sourceClientId).Interpreter = null;
			SendMessage(sourceClientId, "Interpreter cleared.");
			return true;
		}

		public bool PrivilegeAdd(int sourceClientId, string target, string privilege)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.privilege_add))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				if (targetClient.privileges.Contains(privilege))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPrivilegeAddHasAlready"), colorError, target, privilege.ToString()));
					return false;
				}
				targetClient.privileges.Add(privilege);
				if (privilege.Equals(ServerClientMisc.Privilege.freemove))
				{
					SendFreemoveState(targetClient.Id, targetClient.privileges.Contains(ServerClientMisc.Privilege.freemove));
				}
				SendMessageToAll(string.Format(language.Get("Server_CommandPrivilegeAddSuccess"), colorSuccess, targetClient.ColoredPlayername(colorSuccess), privilege.ToString()));
				ServerEventLog(string.Format("{0} gives {1} privilege {2}.", GetClient(sourceClientId).playername, targetClient.playername, privilege.ToString()));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandNonexistantPlayer"), colorError, target));
			return false;
		}

		public bool PrivilegeRemove(int sourceClientId, string target, string privilege)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.privilege_remove))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			ClientOnServer targetClient = GetClient(target);
			if (targetClient != null)
			{
				if (!targetClient.privileges.Remove(privilege))
				{
					SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPrivilegeRemoveNoPriv"), colorError, target, privilege.ToString()));
					return false;
				}
				if (privilege.Equals(ServerClientMisc.Privilege.freemove))
				{
					SendFreemoveState(targetClient.Id, targetClient.privileges.Contains(ServerClientMisc.Privilege.freemove));
				}
				SendMessageToAll(string.Format(language.Get("Server_CommandPrivilegeRemoveSuccess"), colorImportant, targetClient.ColoredPlayername(colorImportant), privilege.ToString()));
				ServerEventLog(string.Format("{0} removes {1} privilege {2}.", GetClient(sourceClientId).playername, targetClient.playername, privilege.ToString()));
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandNonexistantPlayer"), colorError, target));
			return false;
		}

		public bool RestartServer(int sourceClientId)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.restart))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			string message = string.Format(language.Get("Server_CommandRestartSuccess"), colorImportant, GetClient(sourceClientId).ColoredPlayername(colorImportant));
			SendMessageToAll(message);
			ServerEventLog(string.Format("{0} restarts server.", GetClient(sourceClientId).playername));
			KillAllPlayers(modManager.StripColorCodes(message));
			Restart();
			return true;
		}

		public bool ShutdownServer(int sourceClientId)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.shutdown))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			string message = string.Format(language.Get("Server_CommandShutdownSuccess"), colorImportant, GetClient(sourceClientId).ColoredPlayername(colorImportant));
			SendMessageToAll(message);
			ServerEventLog(string.Format("{0} shuts down server.", GetClient(sourceClientId).playername));
			KillAllPlayers(modManager.StripColorCodes(message));
			Exit();
			return true;
		}
	}
}
