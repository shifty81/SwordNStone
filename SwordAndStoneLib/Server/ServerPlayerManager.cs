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
    public int GenerateClientId()
    {
        int i = 0;
        while (clients.ContainsKey(i))
        {
            i++;
        }
        return i;
    }

    private void NotifyPing(int targetClientId, int ping)
    {
        foreach (var k in clients)
        {
            SendPlayerPing(k.Key, targetClientId, ping);
        }
    }
    private void SendPlayerPing(int recipientClientId, int targetClientId, int ping)
    {
        Packet_ServerPlayerPing p = new Packet_ServerPlayerPing()
        {
            ClientId = targetClientId,
            Ping = ping
        };
        SendPacket(recipientClientId, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.PlayerPing, PlayerPing = p }));
    }

    public const string invalidplayername = "invalid";
    public void NotifyInventory(int clientid)
    {
        ClientOnServer c = clients[clientid];
        if (c.IsInventoryDirty && c.playername != invalidplayername && !c.usingFill)
        {
            Packet_ServerInventory p;
            /*
            if (config.IsCreative)
            {
                p = new PacketServerInventory()
                {
                    BlockTypeAmount = StartInventory(),
                    IsFinite = false,
                };
            }
            else
            */
            {
                p = ConvertInventory(GetPlayerInventory(c.playername));
            }
            SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.FiniteInventory, Inventory = p }));
            c.IsInventoryDirty = false;
        }
    }

    private Packet_ServerInventory ConvertInventory(PacketServerInventory inv)
    {
        if (inv == null)
        {
            return null;
        }
        Packet_ServerInventory p = new Packet_ServerInventory();
        if (inv.Inventory != null)
        {
            p.Inventory = new Packet_Inventory();
            p.Inventory.Boots = ConvertItem(inv.Inventory.Boots);
            p.Inventory.DragDropItem = ConvertItem(inv.Inventory.DragDropItem);
            p.Inventory.Gauntlet = ConvertItem(inv.Inventory.Gauntlet);
            p.Inventory.Helmet = ConvertItem(inv.Inventory.Helmet);
            p.Inventory.Items = new Packet_PositionItem[inv.Inventory.Items.Count];
            p.Inventory.ItemsCount = inv.Inventory.Items.Count;
            p.Inventory.ItemsLength = inv.Inventory.Items.Count;
            {
                int i = 0;
                foreach (var k in inv.Inventory.Items)
                {
                    Packet_PositionItem item = new Packet_PositionItem();
                    item.Key_ = SerializePoint(k.Key.X, k.Key.Y);
                    item.Value_ = ConvertItem(k.Value);
                    item.X = k.Key.X;
                    item.Y = k.Key.Y;
                    p.Inventory.Items[i++] = item;
                }
            }
            p.Inventory.MainArmor = ConvertItem(inv.Inventory.MainArmor);
            p.Inventory.RightHand = new Packet_Item[10];
            p.Inventory.RightHandCount = 10;
            p.Inventory.RightHandLength = 10;
            for (int i = 0; i < inv.Inventory.RightHand.Length; i++)
            {
                if (inv.Inventory.RightHand[i] == null)
                {
                    p.Inventory.RightHand[i] = new Packet_Item();
                }
                else
                {
                    p.Inventory.RightHand[i] = ConvertItem(inv.Inventory.RightHand[i]);
                }
            }
        }
        return p;
    }

    private string SerializePoint(int x, int y)
    {
        return x.ToString() + " " + y.ToString();
    }

    private Packet_Item ConvertItem(Item item)
    {
        if (item == null)
        {
            return null;
        }
        Packet_Item p = new Packet_Item();
        p.BlockCount = item.BlockCount;
        p.BlockId = item.BlockId;
        p.ItemClass = (int)item.ItemClass;
        p.ItemId = item.ItemId;
        return p;
    }

    public void NotifyPlayerStats(int clientid)
    {
        ClientOnServer c = clients[clientid];
        if (c.IsPlayerStatsDirty && c.playername != invalidplayername)
        {
            PacketServerPlayerStats stats = GetPlayerStats(c.playername);
            SendPacket(clientid, ServerPackets.PlayerStats(stats.CurrentHealth, stats.MaxHealth, stats.CurrentOxygen, stats.MaxOxygen));
            c.IsPlayerStatsDirty = false;
        }
    }

    private void HitMonsters(int clientid, int health)
    {
        ClientOnServer c = clients[clientid];
        int mapx = c.PositionMul32GlX / 32;
        int mapy = c.PositionMul32GlZ / 32;
        int mapz = c.PositionMul32GlY / 32;
        //3x3x3 chunks
        for (int xx = -1; xx < 2; xx++)
        {
            for (int yy = -1; yy < 2; yy++)
            {
                for (int zz = -1; zz < 2; zz++)
                {
                    int cx = (mapx / chunksize) + xx;
                    int cy = (mapy / chunksize) + yy;
                    int cz = (mapz / chunksize) + zz;
                    if (!MapUtil.IsValidChunkPos(d_Map, cx, cy, cz, chunksize))
                    {
                        continue;
                    }
                    ServerChunk chunk = d_Map.GetChunkValid(cx, cy, cz);
                    if (chunk == null || chunk.Monsters == null)
                    {
                        continue;
                    }
                    foreach (Monster m in chunk.Monsters)
                    {
                        Vector3i mpos = new Vector3i { x = m.X, y = m.Y, z = m.Z };
                        Vector3i ppos = new Vector3i
                        {
                            x = clients[clientid].PositionMul32GlX / 32,
                            y = clients[clientid].PositionMul32GlZ / 32,
                            z = clients[clientid].PositionMul32GlY / 32
                        };
                        if (DistanceSquared(mpos, ppos) < 15)
                        {
                            m.Health -= health;
                            //Console.WriteLine("HIT! -2 = " + m.Health);
                            if (m.Health <= 0)
                            {
                                chunk.Monsters.Remove(m);
                                SendSound(clientid, "death.wav", m.X, m.Y, m.Z);
                                break;
                            }
                            SendSound(clientid, "grunt2.wav", m.X, m.Y, m.Z);
                            break;
                        }
                    }
                }
            }
        }
    }
    public PacketServerInventory GetPlayerInventory(string playername)
    {
        if (Inventory == null)
        {
            Inventory = new Dictionary<string, PacketServerInventory>(StringComparer.InvariantCultureIgnoreCase);
        }
        if (!Inventory.ContainsKey(playername))
        {
            Inventory[playername] = new PacketServerInventory()
            {
                Inventory = StartInventory(),
                /*
                IsFinite = true,
                Max = FiniteInventoryMax,
                */
            };
        }
        return Inventory[playername];
    }
    public void ResetPlayerInventory(ClientOnServer client)
    {
        if (Inventory == null)
        {
            Inventory = new Dictionary<string, PacketServerInventory>(StringComparer.InvariantCultureIgnoreCase);
        }
        this.Inventory[client.playername] = new PacketServerInventory()
        {
            Inventory = StartInventory(),
        };
        client.IsInventoryDirty = true;
        NotifyInventory(client.Id);
    }

    public PacketServerPlayerStats GetPlayerStats(string playername)
    {
        if (PlayerStats == null)
        {
            PlayerStats = new Dictionary<string, PacketServerPlayerStats>(StringComparer.InvariantCultureIgnoreCase);
        }
        if (!PlayerStats.ContainsKey(playername))
        {
            PlayerStats[playername] = StartPlayerStats();
        }
        return PlayerStats[playername];
    }
    public int FiniteInventoryMax = 200;

    Inventory StartInventory()
    {
        Inventory inv = ManicDigger.Inventory.Create();
        int x = 0;
        int y = 0;
        for (int i = 0; i < d_Data.StartInventoryAmount().Length; i++)
        {
            int amount = d_Data.StartInventoryAmount()[i];
            if (config.IsCreative)
            {
                if (amount > 0 || BlockTypes[i].IsBuildable)
                {
                    inv.Items.Add(new ProtoPoint(x, y), new Item() { ItemClass = ItemClass.Block, BlockId = i, BlockCount = 0 });
                    x++;
                    if (x >= GetInventoryUtil(inv).CellCountX)
                    {
                        x = 0;
                        y++;
                    }
                }
            }
            else if (amount > 0)
            {
                inv.Items.Add(new ProtoPoint(x, y), new Item() { ItemClass = ItemClass.Block, BlockId = i, BlockCount = amount });
                x++;
                if (x >= GetInventoryUtil(inv).CellCountX)
                {
                    x = 0;
                    y++;
                }
            }
        }
        return inv;
    }
    PacketServerPlayerStats StartPlayerStats()
    {
        var p = new PacketServerPlayerStats();
        p.CurrentHealth = 20;
        p.MaxHealth = 20;
        p.CurrentOxygen = 10;
        p.MaxOxygen = 10;
        return p;
    }
    public Vector3i PlayerBlockPosition(ClientOnServer c)
    {
        return new Vector3i(c.PositionMul32GlX / 32, c.PositionMul32GlZ / 32, c.PositionMul32GlY / 32);
    }
    public int DistanceSquared(Vector3i a, Vector3i b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        int dz = a.z - b.z;
        return dx * dx + dy * dy + dz * dz;
    }
    public void KillPlayer(int clientid)
    {
        if (!clients.ContainsKey(clientid))
        {
            return;
        }
        if (clients[clientid].queryClient)
        {
            clients.Remove(clientid);
            this.serverMonitor.RemoveMonitorClient(clientid);
            return;
        }
        for (int i = 0; i < modEventHandlers.onplayerleave.Count; i++)
        {
            try
            {
                modEventHandlers.onplayerleave[i](clientid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mod exception: OnPlayerLeave");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        for (int i = 0; i < modEventHandlers.onplayerdisconnect.Count; i++)
        {
            try
            {
                modEventHandlers.onplayerdisconnect[i](clientid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mod exception: OnPlayerDisconnect");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        string coloredName = clients[clientid].ColoredPlayername(colorNormal);
        string name = clients[clientid].playername;
        clients.Remove(clientid);
        if (config.ServerMonitor)
        {
            this.serverMonitor.RemoveMonitorClient(clientid);
        }
        foreach (var k in clients)
        {
            SendPacket(k.Key, ServerPackets.EntityDespawn(clientid));
        }
        if (name != "invalid")
        {
            SendMessageToAll(string.Format(language.ServerPlayerDisconnect(), coloredName));
            ServerEventLog(string.Format("{0} disconnects.", name));
        }
    }
    public void KillAllPlayers(string message)
    {
        List<int> ids = new List<int>(clients.Keys);
        // Disconnect all clients with a message
        foreach (int client in ids)
        {
            SendPacket(client, ServerPackets.DisconnectPlayer(message));
            KillPlayer(client);
        }
    }
    internal string ReceivedKey;
    private DateTime lastQuery = DateTime.UtcNow;
}
}
