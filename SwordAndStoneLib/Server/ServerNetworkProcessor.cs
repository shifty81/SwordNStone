using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using ProtoBuf;
using System.Xml.Serialization;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using Jint.Delegates;
using System.Diagnostics;
using SwordAndStone.Common;
using ManicDigger;

namespace SwordAndStone.Server
{
public partial class Server
{
    private void TryReadPacket(int clientid, byte[] data)
    {
        ClientOnServer c = clients[clientid];
        //PacketClient packet = Serializer.Deserialize<PacketClient>(new MemoryStream(data));
        Packet_Client packet = new Packet_Client();
        Packet_ClientSerializer.DeserializeBuffer(data, data.Length, packet);
        if (c.queryClient)
        {
            if (!(packet.Id == Packet_ClientIdEnum.ServerQuery || packet.Id == Packet_ClientIdEnum.PlayerIdentification))
            {
                //Reject all packets other than ServerQuery or PlayerIdentification
                Console.WriteLine("Rejected packet from not authenticated client");
                SendPacket(clientid, ServerPackets.DisconnectPlayer("Either send PlayerIdentification or ServerQuery!"));
                KillPlayer(clientid);
                return;
            }
        }
        if (config.ServerMonitor && !this.serverMonitor.CheckPacket(clientid, packet))
        {
            //Console.WriteLine("Server monitor rejected packet");
            return;
        }
        int realPlayers = 0;
        switch (packet.Id)
        {
            case Packet_ClientIdEnum.PingReply:
        		clients[clientid].Ping.Receive((int)serverUptime.ElapsedMilliseconds);
                clients[clientid].LastPing = ((float)clients[clientid].Ping.RoundtripTimeTotalMilliseconds() / 1000);
                this.NotifyPing(clientid, (int)clients[clientid].Ping.RoundtripTimeTotalMilliseconds());
                break;
            case Packet_ClientIdEnum.PlayerIdentification:
                {
                    foreach (var cl in clients)
                    {
                        if (cl.Value.IsBot)
                        {
                            continue;
                        }
                        realPlayers++;
                    }
                    if (realPlayers > config.MaxClients)
                    {
                        SendPacket(clientid, ServerPackets.DisconnectPlayer(language.ServerTooManyPlayers()));
                        KillPlayer(clientid);
                        break;
                    }
                    if (config.IsPasswordProtected() && packet.Identification.ServerPassword != config.Password)
                    {
                        Console.WriteLine(string.Format("{0} fails to join (invalid server password).", packet.Identification.Username));
                        ServerEventLog(string.Format("{0} fails to join (invalid server password).", packet.Identification.Username));
                        SendPacket(clientid, ServerPackets.DisconnectPlayer(language.ServerPasswordInvalid()));
                        KillPlayer(clientid);
                        break;
                    }
                    SendServerIdentification(clientid);
                    string username = packet.Identification.Username;

                    // allowed characters in username: a-z,A-Z,0-9,-,_ length: 1-16
                    Regex allowedUsername = new Regex(@"^(\w|-){1,16}$");

                    if (string.IsNullOrEmpty(username) || !allowedUsername.IsMatch(username))
                    {
                        SendPacket(clientid, ServerPackets.DisconnectPlayer(language.ServerUsernameInvalid()));
                        ServerEventLog(string.Format("{0} can't join (invalid username: {1}).", (c.socket.RemoteEndPoint()).AddressToString(), username));
                        KillPlayer(clientid);
                        break;
                    }

                    bool isClientLocalhost = ((c.socket.RemoteEndPoint()).AddressToString() == "127.0.0.1");
                    bool verificationFailed = false;

                    if ((ComputeMd5(config.Key.Replace("-", "") + username) != packet.Identification.VerificationKey)
                        && (!isClientLocalhost))
                    {
                        //Account verification failed.
                        username = "~" + username;
                        verificationFailed = true;
                    }

                    if (!config.AllowGuests && verificationFailed)
                    {
                        SendPacket(clientid, ServerPackets.DisconnectPlayer(language.ServerNoGuests()));
                        KillPlayer(clientid);
                        break;
                    }

                    //When a duplicate user connects, append a number to name.
                    foreach (var k in clients)
                    {
                        if (k.Value.playername.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // If duplicate is a registered user, kick duplicate. It is likely that the user lost connection before.
                            if (!verificationFailed && !isClientLocalhost)
                            {
                                KillPlayer(k.Key);
                                break;
                            }

                            // Duplicates are handled as guests.
                            username = GenerateUsername(username);
                            if (!username.StartsWith("~")) { username = "~" + username; }
                            break;
                        }
                    }
                    clients[clientid].playername = username;

                    // Assign group to new client
                    //Check if client is in ServerClient.txt and assign corresponding group.
                    bool exists = false;
                    foreach (ManicDigger.Client client in serverClient.Clients)
                    {
                        if (client.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (ManicDigger.Group clientGroup in serverClient.Groups)
                            {
                                if (clientGroup.Name.Equals(client.Group))
                                {
                                    exists = true;
                                    clients[clientid].AssignGroup(clientGroup);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (!exists)
                    {
                        //Assign admin group if client connected from localhost
                        if (isClientLocalhost)
                        {
                            clients[clientid].AssignGroup(serverClient.Groups.Find(v => v.Name == "Admin"));
                        }
                        else if (clients[clientid].playername.StartsWith("~"))
                        {
                            clients[clientid].AssignGroup(this.defaultGroupGuest);
                        }
                        else
                        {
                            clients[clientid].AssignGroup(this.defaultGroupRegistered);
                        }
                    }
                    this.SetFillAreaLimit(clientid);
                    this.SendFreemoveState(clientid, clients[clientid].privileges.Contains(ServerClientMisc.Privilege.freemove));
                    c.queryClient = false;
                    clients[clientid].entity.drawName.name = username;
                    if (config.EnablePlayerPushing)
                    {
                        // Player pushing
                        clients[clientid].entity.push = new ServerEntityPush();
                        clients[clientid].entity.push.range = 1;
                    }
                    PlayerEntitySetDirty(clientid);
                }
                break;
            case Packet_ClientIdEnum.RequestBlob:
                {
                    // Set player's spawn position
                    Vector3i position = GetPlayerSpawnPositionMul32(clientid);

                    clients[clientid].PositionMul32GlX = position.x;
                    clients[clientid].PositionMul32GlY = position.y + (int)(0.5 * 32);
                    clients[clientid].PositionMul32GlZ = position.z;

                    string ip = (clients[clientid].socket.RemoteEndPoint()).AddressToString();
                    SendMessageToAll(string.Format(language.ServerPlayerJoin(), clients[clientid].ColoredPlayername(colorNormal)));
                    ServerEventLog(string.Format("{0} {1} joins.", clients[clientid].playername, ip));
                    SendMessage(clientid, colorSuccess + config.WelcomeMessage);
                    SendBlobs(clientid, packet.RequestBlob.RequestedMd5);
                    SendBlockTypes(clientid);
                    SendTranslations(clientid);
                    SendSunLevels(clientid);
                    SendLightLevels(clientid);
                    SendCraftingRecipes(clientid);

                    for (int i = 0; i < modEventHandlers.onplayerjoin.Count; i++)
                    {
                        try
                        {
                            modEventHandlers.onplayerjoin[i](clientid);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Mod exception: OnPlayerJoin");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }

                    SendPacket(clientid, ServerPackets.LevelFinalize());
                    clients[clientid].state = ClientStateOnServer.Playing;
                    NotifySeason(clientid);
                }
                break;
            case Packet_ClientIdEnum.SetBlock:
                {
                    int x = packet.SetBlock.X;
                    int y = packet.SetBlock.Y;
                    int z = packet.SetBlock.Z;
                    if (packet.SetBlock.Mode == Packet_BlockSetModeEnum.Use)	//Check if player only uses block
                    {
                        if (!CheckUsePrivileges(clientid, x, y, z))
                        {
                            break;
                        }
                        DoCommandBuild(clientid, true, packet.SetBlock);
                    }
                    else	//Player builds, deletes or uses block with tool
                    {
                        if (!CheckBuildPrivileges(clientid, x, y, z, packet.SetBlock.Mode))
                        {
                            SendSetBlock(clientid, x, y, z, d_Map.GetBlock(x, y, z)); //revert
                            break;
                        }
                        if (!DoCommandBuild(clientid, true, packet.SetBlock))
                        {
                            SendSetBlock(clientid, x, y, z, d_Map.GetBlock(x, y, z)); //revert
                        }
                        //Only log when building/destroying blocks. Prevents VandalFinder entries
                        if (packet.SetBlock.Mode != Packet_BlockSetModeEnum.UseWithTool)
                            BuildLog(string.Format("{0} {1} {2} {3} {4} {5}", x, y, z, c.playername, (c.socket.RemoteEndPoint()).AddressToString(), d_Map.GetBlock(x, y, z)));
                    }
                }
                break;
            case Packet_ClientIdEnum.FillArea:
                {
                    if (!clients[clientid].privileges.Contains(ServerClientMisc.Privilege.build))
                    {
                        SendMessage(clientid, colorError + language.ServerNoBuildPrivilege());
                        break;
                    }
                    if (clients[clientid].IsSpectator && !config.AllowSpectatorBuild)
                    {
                        SendMessage(clientid, colorError + language.ServerNoSpectatorBuild());
                        break;
                    }
                    Vector3i a = new Vector3i(packet.FillArea.X1, packet.FillArea.Y1, packet.FillArea.Z1);
                    Vector3i b = new Vector3i(packet.FillArea.X2, packet.FillArea.Y2, packet.FillArea.Z2);

                    int blockCount = (Math.Abs(a.x - b.x) + 1) * (Math.Abs(a.y - b.y) + 1) * (Math.Abs(a.z - b.z) + 1);

                    if (blockCount > clients[clientid].FillLimit)
                    {
                        SendMessage(clientid, colorError + language.ServerFillAreaTooLarge());
                        break;
                    }
                    if (!this.IsFillAreaValid(clients[clientid], a, b))
                    {
                        SendMessage(clientid, colorError + language.ServerFillAreaInvalid());
                        break;
                    }
                    this.DoFillArea(clientid, packet.FillArea, blockCount);

                    BuildLog(string.Format("{0} {1} {2} - {3} {4} {5} {6} {7} {8}", a.x, a.y, a.z, b.x, b.y, b.z,
                        c.playername, (c.socket.RemoteEndPoint()).AddressToString(),
                        d_Map.GetBlock(a.x, a.y, a.z)));
                }
                break;
            case Packet_ClientIdEnum.PositionandOrientation:
                {
                    var p = packet.PositionAndOrientation;
                    clients[clientid].PositionMul32GlX = p.X;
                    clients[clientid].PositionMul32GlY = p.Y;
                    clients[clientid].PositionMul32GlZ = p.Z;
                    clients[clientid].positionheading = p.Heading;
                    clients[clientid].positionpitch = p.Pitch;
                    clients[clientid].stance = (byte)p.Stance;
                }
                break;
            case Packet_ClientIdEnum.Message:
                {
                    packet.Message.Message = packet.Message.Message.Trim();
                    // empty message
                    if (string.IsNullOrEmpty(packet.Message.Message))
                    {
                        //Ignore empty messages
                        break;
                    }
                    // server command
                    if (packet.Message.Message.StartsWith("/"))
                    {
                        string[] ss = packet.Message.Message.Split(new[] { ' ' });
                        string command = ss[0].Replace("/", "");
                        string argument = packet.Message.Message.IndexOf(" ") < 0 ? "" : packet.Message.Message.Substring(packet.Message.Message.IndexOf(" ") + 1);
                        try
                        {
                            //Try to execute the given command
                            this.CommandInterpreter(clientid, command, argument);
                        }
                        catch (Exception ex)
                        {
                            //This will notify client of error instead of kicking him in case of an error
                            SendMessage(clientid, "Server error while executing command!", MessageType.Error);
                            SendMessage(clientid, "Details on server console!", MessageType.Error);
                            Console.WriteLine("Client {0} caused a command error.", clientid);
                            Console.WriteLine("Command: /{0}", command);
                            Console.WriteLine("Arguments: {0}", argument);
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    // client command
                    else if (packet.Message.Message.StartsWith("."))
                    {
                        //Ignore clientside commands
                        break;
                    }
                    // chat message
                    else
                    {
                        string message = packet.Message.Message;
                        for (int i = 0; i < modEventHandlers.onplayerchat.Count; i++)
                        {
                            try
                            {
                                message = modEventHandlers.onplayerchat[i](clientid, message, packet.Message.IsTeamchat != 0);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Mod exception: OnPlayerChat");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                        if (clients[clientid].privileges.Contains(ServerClientMisc.Privilege.chat))
                        {
                            if (message == null)
                            {
                                break;
                            }
                            SendMessageToAll(string.Format("{0}: {1}", clients[clientid].ColoredPlayername(colorNormal), message));
                            ChatLog(string.Format("{0}: {1}", clients[clientid].playername, message));
                        }
                        else
                        {
                            SendMessage(clientid, string.Format(language.ServerNoChatPrivilege(), colorError));
                        }
                    }
                }
                break;
            case Packet_ClientIdEnum.Craft:
                DoCommandCraft(true, packet.Craft);
                break;
            case Packet_ClientIdEnum.InventoryAction:
                DoCommandInventory(clientid, packet.InventoryAction);
                break;
            case Packet_ClientIdEnum.Health:
                {
                    var stats = GetPlayerStats(clients[clientid].playername);
                    int health = packet.Health.CurrentHealth;
                    // Server-side validation: clamp to valid range
                    if (health > stats.MaxHealth)
                    {
                        health = stats.MaxHealth;
                    }
                    stats.CurrentHealth = health;
                    if (stats.CurrentHealth < 1)
                    {
                        //death - reset health. More stuff done in Death packet handling
                        stats.CurrentHealth = stats.MaxHealth;
                    }
                    clients[clientid].IsPlayerStatsDirty = true;
                }
                break;
            case Packet_ClientIdEnum.Death:
                {
                    //Console.WriteLine("Death Packet Received. Client: {0}, Reason: {1}, Source: {2}", clientid, packet.Death.Reason, packet.Death.SourcePlayer);
                    for (int i = 0; i < modEventHandlers.onplayerdeath.Count; i++)
                    {
                        try
                        {
                            modEventHandlers.onplayerdeath[i](clientid, (DeathReason)packet.Death.Reason, packet.Death.SourcePlayer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Mod exception: OnPlayerDeath");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
                break;
            case Packet_ClientIdEnum.Oxygen:
                {
                    var stats = GetPlayerStats(clients[clientid].playername);
                    int oxygen = packet.Oxygen.CurrentOxygen;
                    // Server-side validation: clamp to valid range
                    if (oxygen < 0)
                    {
                        oxygen = 0;
                    }
                    if (oxygen > stats.MaxOxygen)
                    {
                        oxygen = stats.MaxOxygen;
                    }
                    stats.CurrentOxygen = oxygen;
                    clients[clientid].IsPlayerStatsDirty = true;
                }
                break;
            case Packet_ClientIdEnum.MonsterHit:
                HitMonsters(clientid, packet.Health.CurrentHealth);
                break;
            case Packet_ClientIdEnum.DialogClick:
                for (int i = 0; i < modEventHandlers.ondialogclick.Count; i++)
                {
                    try
                    {
                        modEventHandlers.ondialogclick[i](clientid, packet.DialogClick_.WidgetId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mod exception: OnDialogClick");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                for (int i = 0; i < modEventHandlers.ondialogclick2.Count; i++)
                {
                    try
                    {
                        DialogClickArgs args = new DialogClickArgs();
                        args.SetPlayer(clientid);
                        args.SetWidgetId(packet.DialogClick_.WidgetId);
                        args.SetTextBoxValue(packet.DialogClick_.TextBoxValue);
                        modEventHandlers.ondialogclick2[i](args);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mod exception: OnDialogClick2");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                break;
            case Packet_ClientIdEnum.Shot:
                int shootSoundIndex = pistolcycle++ % BlockTypes[packet.Shot.WeaponBlock].Sounds.ShootEnd.Length;	//Cycle all given ShootEnd sounds
                PlaySoundAtExceptPlayer((int)DeserializeFloat(packet.Shot.FromX), (int)DeserializeFloat(packet.Shot.FromZ), (int)DeserializeFloat(packet.Shot.FromY), BlockTypes[packet.Shot.WeaponBlock].Sounds.ShootEnd[shootSoundIndex] + ".ogg", clientid);
                if (BlockTypes[packet.Shot.WeaponBlock].ProjectileSpeed == 0)
                {
                    SendBullet(clientid, DeserializeFloat(packet.Shot.FromX), DeserializeFloat(packet.Shot.FromY), DeserializeFloat(packet.Shot.FromZ),
                       DeserializeFloat(packet.Shot.ToX), DeserializeFloat(packet.Shot.ToY), DeserializeFloat(packet.Shot.ToZ), 150);
                }
                else
                {
                    Vector3f from = new Vector3f(DeserializeFloat(packet.Shot.FromX), DeserializeFloat(packet.Shot.FromY), DeserializeFloat(packet.Shot.FromZ));
                    Vector3f to = new Vector3f(DeserializeFloat(packet.Shot.ToX), DeserializeFloat(packet.Shot.ToY), DeserializeFloat(packet.Shot.ToZ));
                    Vector3f v = to - from;
                    v.Normalize();
                    v *= BlockTypes[packet.Shot.WeaponBlock].ProjectileSpeed;
                    SendProjectile(clientid, DeserializeFloat(packet.Shot.FromX), DeserializeFloat(packet.Shot.FromY), DeserializeFloat(packet.Shot.FromZ),
                        v.X, v.Y, v.Z, packet.Shot.WeaponBlock, DeserializeFloat(packet.Shot.ExplodesAfter));
                    //Handle OnWeaponShot so grenade ammo is correct
                    for (int i = 0; i < modEventHandlers.onweaponshot.Count; i++)
                    {
                        try
                        {
                            modEventHandlers.onweaponshot[i](clientid, packet.Shot.WeaponBlock);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Mod exception: OnWeaponShot");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    return;
                }
                for (int i = 0; i < modEventHandlers.onweaponshot.Count; i++)
                {
                    try
                    {
                        modEventHandlers.onweaponshot[i](clientid, packet.Shot.WeaponBlock);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mod exception: OnWeaponShot");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                if (clients[clientid].LastPing < 0.3)
                {
                    if (packet.Shot.HitPlayer != -1)
                    {
                        //client-side shooting
                        for (int i = 0; i < modEventHandlers.onweaponhit.Count; i++)
                        {
                            try
                            {
                                modEventHandlers.onweaponhit[i](clientid, packet.Shot.HitPlayer, packet.Shot.WeaponBlock, packet.Shot.IsHitHead != 0);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Mod exception: OnWeaponHit");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                    return;
                }
                foreach (var k in clients)
                {
                    if (k.Key == clientid)
                    {
                        continue;
                    }
                    Line3D pick = new Line3D();
                    pick.Start = new float[] { DeserializeFloat(packet.Shot.FromX), DeserializeFloat(packet.Shot.FromY), DeserializeFloat(packet.Shot.FromZ) };
                    pick.End = new float[] { DeserializeFloat(packet.Shot.ToX), DeserializeFloat(packet.Shot.ToY), DeserializeFloat(packet.Shot.ToZ) };

                    Vector3f feetpos = new Vector3f((float)k.Value.PositionMul32GlX / 32, (float)k.Value.PositionMul32GlY / 32, (float)k.Value.PositionMul32GlZ / 32);
                    //var p = PlayerPositionSpawn;
                    Box3D bodybox = new Box3D();
                    float headsize = (k.Value.ModelHeight - k.Value.EyeHeight) * 2; //0.4f;
                    float h = k.Value.ModelHeight - headsize;
                    float r = 0.35f;

                    bodybox.AddPoint(feetpos.X - r, feetpos.Y + 0, feetpos.Z - r);
                    bodybox.AddPoint(feetpos.X - r, feetpos.Y + 0, feetpos.Z + r);
                    bodybox.AddPoint(feetpos.X + r, feetpos.Y + 0, feetpos.Z - r);
                    bodybox.AddPoint(feetpos.X + r, feetpos.Y + 0, feetpos.Z + r);

                    bodybox.AddPoint(feetpos.X - r, feetpos.Y + h, feetpos.Z - r);
                    bodybox.AddPoint(feetpos.X - r, feetpos.Y + h, feetpos.Z + r);
                    bodybox.AddPoint(feetpos.X + r, feetpos.Y + h, feetpos.Z - r);
                    bodybox.AddPoint(feetpos.X + r, feetpos.Y + h, feetpos.Z + r);

                    Box3D headbox = new Box3D();

                    headbox.AddPoint(feetpos.X - r, feetpos.Y + h, feetpos.Z - r);
                    headbox.AddPoint(feetpos.X - r, feetpos.Y + h, feetpos.Z + r);
                    headbox.AddPoint(feetpos.X + r, feetpos.Y + h, feetpos.Z - r);
                    headbox.AddPoint(feetpos.X + r, feetpos.Y + h, feetpos.Z + r);

                    headbox.AddPoint(feetpos.X - r, feetpos.Y + h + headsize, feetpos.Z - r);
                    headbox.AddPoint(feetpos.X - r, feetpos.Y + h + headsize, feetpos.Z + r);
                    headbox.AddPoint(feetpos.X + r, feetpos.Y + h + headsize, feetpos.Z - r);
                    headbox.AddPoint(feetpos.X + r, feetpos.Y + h + headsize, feetpos.Z + r);

                    if (Intersection.CheckLineBoxExact(pick, headbox) != null)
                    {
                        for (int i = 0; i < modEventHandlers.onweaponhit.Count; i++)
                        {
                            try
                            {
                                modEventHandlers.onweaponhit[i](clientid, k.Key, packet.Shot.WeaponBlock, true);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Mod exception: OnWeaponHit");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                    else if (Intersection.CheckLineBoxExact(pick, bodybox) != null)
                    {
                        for (int i = 0; i < modEventHandlers.onweaponhit.Count; i++)
                        {
                            try
                            {
                                modEventHandlers.onweaponhit[i](clientid, k.Key, packet.Shot.WeaponBlock, false);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Mod exception: OnWeaponHit");
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }
                break;
            case Packet_ClientIdEnum.SpecialKey:
                for (int i = 0; i < modEventHandlers.onspecialkey.Count; i++)
                {
                    try
                    {
                        modEventHandlers.onspecialkey[i](clientid, (SpecialKey)packet.SpecialKey_.Key_);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mod exception: OnSpecialKey");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                break;
            case Packet_ClientIdEnum.ActiveMaterialSlot:
                clients[clientid].ActiveMaterialSlot = packet.ActiveMaterialSlot.ActiveMaterialSlot;
                for (int i = 0; i < modEventHandlers.changedactivematerialslot.Count; i++)
                {
                    try
                    {
                        modEventHandlers.changedactivematerialslot[i](clientid);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mod exception: ChangedActiveMaterialSlot");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                break;
            case Packet_ClientIdEnum.Leave:
                //0: Leave - 1: Crash
                Console.WriteLine("Disconnect reason: {0}", packet.Leave.Reason);
                KillPlayer(clientid);
                break;
            case Packet_ClientIdEnum.Reload:
                break;
            case Packet_ClientIdEnum.ServerQuery:
                //Flood/DDoS-abuse protection
                if ((DateTime.UtcNow - lastQuery) < TimeSpan.FromMilliseconds(200))
                {
                    Console.WriteLine("ServerQuery rejected (too many requests)");
                    SendPacket(clientid, ServerPackets.DisconnectPlayer("Too many requests!"));
                    KillPlayer(clientid);
                    return;
                }
                Console.WriteLine("ServerQuery processed.");
                lastQuery = DateTime.UtcNow;
                //Client only wants server information. No real client.
                List<string> playernames = new List<string>();
                lock (clients)
                {
                    foreach (var k in clients)
                    {
                        if (k.Value.queryClient || k.Value.IsBot)
                        {
                            //Exclude bot players and query clients
                            continue;
                        }
                        playernames.Add(k.Value.playername);
                    }
                }
                //Create query answer
                Packet_ServerQueryAnswer answer = new Packet_ServerQueryAnswer()
                {
                    Name = config.Name,
                    MOTD = config.Motd,
                    PlayerCount = playernames.Count,
                    MaxPlayers = config.MaxClients,
                    PlayerList = string.Join(",", playernames.ToArray()),
                    Port = config.Port,
                    GameMode = gameMode,
                    Password = config.IsPasswordProtected(),
                    PublicHash = ReceivedKey,
                    ServerVersion = GameVersion.Version,
                    MapSizeX = d_Map.MapSizeX,
                    MapSizeY = d_Map.MapSizeY,
                    MapSizeZ = d_Map.MapSizeZ,
                    ServerThumbnail = GenerateServerThumbnail(),
                };
                //Send answer
                SendPacket(clientid, ServerPackets.AnswerQuery(answer));
                //Directly disconnect client after request.
                SendPacket(clientid, ServerPackets.DisconnectPlayer("Query success."));
                KillPlayer(clientid);
                break;
            case Packet_ClientIdEnum.ClientGameResolution:
                //Update client information
                clients[clientid].WindowSize = new int[] { packet.GameResolution.Width, packet.GameResolution.Height };
                //Console.WriteLine("client:{0} --> {1}x{2}", clientid, clients[clientid].WindowSize[0], clients[clientid].WindowSize[1]);
                break;
            case Packet_ClientIdEnum.EntityInteraction:
                switch (packet.EntityInteraction.InteractionType)
                {
                    case Packet_EntityInteractionTypeEnum.Use:
                        for (int i = 0; i < modEventHandlers.onentityuse.Count; i++)
                        {
                            ServerEntityId id = c.spawnedEntities[packet.EntityInteraction.EntityId - 64];
                            modEventHandlers.onentityuse[i](clientid, id);
                        }
                        break;
                    case Packet_EntityInteractionTypeEnum.Hit:
                        for (int i = 0; i < modEventHandlers.onentityhit.Count; i++)
                        {
                            ServerEntityId id = c.spawnedEntities[packet.EntityInteraction.EntityId - 64];
                            modEventHandlers.onentityhit[i](clientid, id);
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown EntityInteractionType: {0}, clientid: {1}", packet.EntityInteraction.InteractionType, clientid);
                        break;
                }
                break;
            default:
                Console.WriteLine("Invalid packet: {0}, clientid:{1}", packet.Id, clientid);
                break;
        }
    }
}
}
