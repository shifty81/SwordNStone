using System;
using System.Collections.Generic;
using ManicDigger;

namespace SwordAndStone.Server
{
	public partial class Server
	{
		public bool PrivateMessage(int sourceClientId, string recipient, string message)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.pm))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			ClientOnServer targetClient = GetClient(recipient);
			ClientOnServer sourceClient = GetClient(sourceClientId);
			if (targetClient != null)
			{
				SendMessage(targetClient.Id, string.Format("PM {0}: {1}", sourceClient.ColoredPlayername(colorNormal), message));
				SendMessage(sourceClientId, string.Format("PM -> {0}: {1}", targetClient.ColoredPlayername(colorNormal), message));
				lastSender[targetClient.playername] = sourceClient.playername;
				// TODO: move message sound to client
				if (targetClient.Id != serverConsoleId)
				{
					SendSound(targetClient.Id, "message.wav", 0, 0, 0);
				}
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, recipient));
			return false;
		}

		public bool AnswerMessage(int sourceClientId, string message)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.pm))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}

			ClientOnServer sourceClient = GetClient(sourceClientId);
			if (!lastSender.ContainsKey(sourceClient.playername))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPMNoAnswer"), colorError));
				return false;
			}

			ClientOnServer targetClient = GetClient(lastSender[sourceClient.playername]);
			if (targetClient != null)
			{
				SendMessage(targetClient.Id, string.Format("PM {0}: {1}", sourceClient.ColoredPlayername(colorNormal), message));
				SendMessage(sourceClientId, string.Format("PM -> {0}: {1}", targetClient.ColoredPlayername(colorNormal), message));
				lastSender[targetClient.playername] = sourceClient.playername;
				// TODO: move message sound to client
				if (targetClient.Id != serverConsoleId)
				{
					SendSound(targetClient.Id, "message.wav", 0, 0, 0);
				}
				return true;
			}
			SendMessage(sourceClientId, string.Format(language.Get("Server_CommandPlayerNotFound"), colorError, lastSender[sourceClient.playername]));
			return false;
		}

		public bool WelcomeMessage(int sourceClientId, string welcomeMessage)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.welcome))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			config.WelcomeMessage = welcomeMessage;
			SendMessageToAll(string.Format(language.Get("Server_CommandWelcomeChanged"), colorSuccess, GetClient(sourceClientId).ColoredPlayername(colorSuccess), welcomeMessage));
			ServerEventLog(string.Format("{0} changes welcome message to {1}.", GetClient(sourceClientId).playername, welcomeMessage));
			configNeedsSaving = true;
			return true;
		}

		public bool Announcement(int sourceClientId, string message)
		{
			if (!PlayerHasPrivilege(sourceClientId, ServerClientMisc.Privilege.announcement))
			{
				SendMessage(sourceClientId, string.Format(language.Get("Server_CommandInsufficientPrivileges"), colorError));
				return false;
			}
			ServerEventLog(String.Format("{0} announced: {1}.", GetClient(sourceClientId).playername, message));
			SendMessageToAll(string.Format(language.Get("Server_CommandAnnouncementMessage"), colorError, message));
			return true;
		}
	}
}
