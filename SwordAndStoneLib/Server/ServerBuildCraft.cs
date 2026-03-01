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

    public bool CheckBuildPrivileges(int player, int x, int y, int z, int mode)
    {
        Server server = this;
        if (!server.PlayerHasPrivilege(player, ServerClientMisc.Privilege.build))
        {
            server.SendMessage(player, server.colorError + server.language.ServerNoBuildPrivilege());
            return false;
        }
        if (server.clients[player].IsSpectator && !server.config.AllowSpectatorBuild)
        {
            server.SendMessage(player, server.colorError + server.language.ServerNoSpectatorBuild());
            return false;
        }
        for (int i = 0; i < server.modEventHandlers.onpermission.Count; i++)
        {
            PermissionArgs args = new PermissionArgs();
            args.SetPlayer(player);
            args.SetX(x);
            args.SetY(y);
            args.SetZ(z);
            server.modEventHandlers.onpermission[i](args);
            if (args.GetAllowed())
            {
                return true;
            }
        }
        if (!server.config.CanUserBuild(server.clients[player], x, y, z)
            && !server.extraPrivileges.ContainsKey(ServerClientMisc.Privilege.build))
        {
            server.SendMessage(player, server.colorError + server.language.ServerNoBuildPermissionHere());
            return false;
        }
        bool retval = true;
        if (mode == Packet_BlockSetModeEnum.Create)
        {
            for (int i = 0; i < modEventHandlers.checkonbuild.Count; i++)
            {
                // All handlers must return true for operation to be permitted.
                try
                {
                    retval = (retval && modEventHandlers.checkonbuild[i](player, x, y, z));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mod exception: CheckOnBuild");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    // Do not allow interactions when check fails.
                    retval = false;
                }
            }
        }
        else if (mode == Packet_BlockSetModeEnum.Destroy)
        {
            for (int i = 0; i < modEventHandlers.checkondelete.Count; i++)
            {
                // All handlers must return true for operation to be permitted.
                try
                {
                    retval = (retval && modEventHandlers.checkondelete[i](player, x, y, z));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mod exception: CheckOnDelete");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    // Do not allow interactions when check fails.
                    retval = false;
                }
            }
        }
        return retval;
    }

    public bool CheckUsePrivileges(int player, int x, int y, int z)
    {
        Server server = this;
        if (!server.PlayerHasPrivilege(player, ServerClientMisc.Privilege.use))
        {
            SendMessage(player, colorError + server.language.ServerNoUsePrivilege());
            return false;
        }
        if (server.clients[player].IsSpectator && !server.config.AllowSpectatorUse)
        {
            SendMessage(player, colorError + server.language.ServerNoSpectatorUse());
            return false;
        }
        bool retval = true;
        for (int i = 0; i < modEventHandlers.checkonuse.Count; i++)
        {
            // All handlers must return true for operation to be permitted.
            try
            {
                retval = (retval && modEventHandlers.checkonuse[i](player, x, y, z));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mod exception: CheckOnUse");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                // Do not allow interactions when check fails.
                retval = false;
            }
        }
        return retval;
    }

    public void SendServerRedirect(int clientid, string ip_, int port_)
    {
        Packet_Server p = new Packet_Server();
        p.Id = Packet_ServerIdEnum.ServerRedirect;
        p.Redirect = new Packet_ServerRedirect()
        {
            IP = ip_,
            Port = port_,
        };
        SendPacket(clientid, p);
    }

    public static byte[] GenerateServerThumbnail()
    {
        string filename = Path.Combine(Path.Combine("data", "public"), "thumbnail.png");
        Bitmap bmp;
        if (File.Exists(filename))
        {
            try
            {
                bmp = new Bitmap(filename);
            }
            catch
            {
                //Create empty bitmap in case of failure
                bmp = new Bitmap(64, 64);
            }
        }
        else
        {
            bmp = new Bitmap(64, 64);
        }
        Bitmap bmp2 = bmp;
        if (bmp.Width != 64 || bmp.Height != 64)
        {
            //Resize the image if it does not have the proper size
            bmp2 = new Bitmap(bmp, 64, 64);
        }
        using (MemoryStream ms = new MemoryStream())
        {
            //Convert image to a byte[] for transfer
            bmp2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }

    private float DeserializeFloat(int p)
    {
        return (float)p / 32;
    }

    private void SendProjectile(int player, float fromx, float fromy, float fromz, float velocityx, float velocityy, float velocityz, int block, float explodesafter)
    {
        foreach (var k in clients)
        {
            if (k.Key == player)
            {
                continue;
            }
            Packet_Server p = new Packet_Server();
            p.Id = Packet_ServerIdEnum.Projectile;
            p.Projectile = new Packet_ServerProjectile()
            {
                FromXFloat = SerializeFloat(fromx),
                FromYFloat = SerializeFloat(fromy),
                FromZFloat = SerializeFloat(fromz),
                VelocityXFloat = SerializeFloat(velocityx),
                VelocityYFloat = SerializeFloat(velocityy),
                VelocityZFloat = SerializeFloat(velocityz),
                BlockId = block,
                ExplodesAfterFloat = SerializeFloat(explodesafter),
                SourcePlayerID = player,
            };
            SendPacket(k.Key, Serialize(p));
        }
    }

    private void SendBullet(int player, float fromx, float fromy, float fromz, float tox, float toy, float toz, float speed)
    {
        foreach (var k in clients)
        {
            if (k.Key == player)
            {
                continue;
            }
            Packet_Server p = new Packet_Server();
            p.Id = Packet_ServerIdEnum.Bullet;
            p.Bullet = new Packet_ServerBullet()
            {
                FromXFloat = SerializeFloat(fromx),
                FromYFloat = SerializeFloat(fromy),
                FromZFloat = SerializeFloat(fromz),
                ToXFloat = SerializeFloat(tox),
                ToYFloat = SerializeFloat(toy),
                ToZFloat = SerializeFloat(toz),
                SpeedFloat = SerializeFloat(speed)
            };
            SendPacket(k.Key, Serialize(p));
        }
    }
    int pistolcycle;
    public Vector3i GetPlayerSpawnPositionMul32(int clientid)
    {
        Vector3i position;
        ManicDigger.Spawn playerSpawn = null;
        // Check if there is a spawn entry for his assign group
        if (clients[clientid].clientGroup.Spawn != null)
        {
            playerSpawn = clients[clientid].clientGroup.Spawn;
        }
        // Check if there is an entry in clients with spawn member (overrides group spawn).
        foreach (ManicDigger.Client client in serverClient.Clients)
        {
            if (client.Name.Equals(clients[clientid].playername, StringComparison.InvariantCultureIgnoreCase))
            {
                if (client.Spawn != null)
                {
                    playerSpawn = client.Spawn;
                }
                break;
            }
        }

        if (playerSpawn == null)
        {
            position = new Vector3i(this.defaultPlayerSpawn.x * 32, this.defaultPlayerSpawn.z * 32, this.defaultPlayerSpawn.y * 32);
        }
        else
        {
            position = this.SpawnToVector3i(playerSpawn);
        }
        return position;
    }

    private void RunInClientSandbox(string script, int clientid)
    {
        var client = GetClient(clientid);
        if (!config.AllowScripting)
        {
            SendMessage(clientid, "Server scripts disabled.", MessageType.Error);
            return;
        }
        if (!client.privileges.Contains(ServerClientMisc.Privilege.run))
        {
            SendMessage(clientid, "Insufficient privileges to access this command.", MessageType.Error);
            return;
        }
        ServerEventLog(string.Format("{0} runs script:\n{1}", client.playername, script));
        if (client.Interpreter == null)
        {
            client.Interpreter = new JavaScriptInterpreter();
            client.Console = new ScriptConsole(this, clientid);
            client.Console.InjectConsoleCommands(client.Interpreter);
            client.Interpreter.SetVariables(new Dictionary<string, object>() { { "client", client }, { "server", this }, });
            client.Interpreter.Execute("function inspect(obj) { for( property in obj) { out(property)}}");
        }
        var interpreter = client.Interpreter;
        object result;
        SendMessage(clientid, colorNormal + script);
        if (interpreter.Execute(script, out result))
        {
            try
            {
                SendMessage(clientid, colorSuccess + " => " + result);
            }
            catch (FormatException e) // can happen
            {
                SendMessage(clientid, colorError + "Error. " + e.Message);
            }
            return;
        }
        SendMessage(clientid, colorError + "Error.");
    }

    public string colorNormal = "&f"; //white
    public string colorHelp = "&4"; //red
    public string colorOpUsername = "&2"; //green
    public string colorSuccess = "&2"; //green
    public string colorError = "&4"; //red
    public string colorImportant = "&4"; // red
    public string colorAdmin = "&e"; //yellow
    public enum MessageType { Normal, Important, Help, OpUsername, Success, Error, Admin, White, Red, Green, Yellow }
    private string MessageTypeToString(MessageType type)
    {
        switch (type)
        {
            case MessageType.Normal:
            case MessageType.White:
                return colorNormal;
            case MessageType.Important:
                return colorImportant;
            case MessageType.Help:
            case MessageType.Red:
                return colorHelp;
            case MessageType.OpUsername:
            case MessageType.Green:
                return colorOpUsername;
            case MessageType.Error:
                return colorError;
            case MessageType.Success:
                return colorSuccess;
            case MessageType.Admin:
            case MessageType.Yellow:
                return colorAdmin;
            default:
                return colorNormal;
        }
    }
    bool CompareByteArray(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) { return false; }
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) { return false; }
        }
        return true;
    }
    private void NotifyBlock(int x, int y, int z, int blocktype)
    {
        foreach (var k in clients)
        {
            SendSetBlock(k.Key, x, y, z, blocktype);
        }
    }
    bool ENABLE_FINITEINVENTORY { get { return !config.IsCreative; } }
    private bool DoCommandCraft(bool execute, Packet_ClientCraft cmd)
    {
        if (d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z) != d_Data.BlockIdCraftingTable())
        {
            return false;
        }
        if (cmd.RecipeId < 0 || cmd.RecipeId >= craftingrecipes.Count)
        {
            return false;
        }
        IntRef tableCount = new IntRef();
        Vector3IntRef[] table = d_CraftingTableTool.GetTable(cmd.X, cmd.Y, cmd.Z, tableCount);
        IntRef ontableCount = new IntRef();
        int[] ontable = d_CraftingTableTool.GetOnTable(table, tableCount.value, ontableCount);
        List<int> outputtoadd = new List<int>();
        //for (int i = 0; i < craftingrecipes.Count; i++)
        int i = cmd.RecipeId;
        {
            //try apply recipe. if success then try until fail.
            for (; ; )
            {
                //check if ingredients available
                foreach (Ingredient ingredient in craftingrecipes[i].ingredients)
                {
                    if (ontableFindAllCount(ontable, ontableCount, ingredient.Type) < ingredient.Amount)
                    {
                        goto nextrecipe;
                    }
                }
                //remove ingredients
                foreach (Ingredient ingredient in craftingrecipes[i].ingredients)
                {
                    for (int ii = 0; ii < ingredient.Amount; ii++)
                    {
                        //replace on table
                        ReplaceOne(ontable, ontableCount.value, ingredient.Type, d_Data.BlockIdEmpty());
                    }
                }
                //add output
                for (int z = 0; z < craftingrecipes[i].output.Amount; z++)
                {
                    outputtoadd.Add(craftingrecipes[i].output.Type);
                }
            }
        nextrecipe:
            ;
        }
        foreach (var v in outputtoadd)
        {
            ReplaceOne(ontable, ontableCount.value, d_Data.BlockIdEmpty(), v);
        }
        int zz = 0;
        if (execute)
        {
            for (int k = 0; k < tableCount.value; k++)
            {
                Vector3IntRef v = table[k];
                SetBlockAndNotify(v.X, v.Y, v.Z + 1, ontable[zz]);
                zz++;
            }
        }
        return true;
    }

    private int ontableFindAllCount(int[] ontable, IntRef ontableCount, int p)
    {
        int count = 0;
        for (int i = 0; i < ontableCount.value; i++)
        {
            if (ontable[i] == p)
            {
                count++;
            }
        }
        return count;
    }

    private void ReplaceOne<T>(T[] l, int lCount, T from, T to)
    {
        for (int ii = 0; ii < lCount; ii++)
        {
            if (l[ii].Equals(from))
            {
                l[ii] = to;
                break;
            }
        }
    }
    public IGameDataItems d_DataItems;
    public InventoryUtil GetInventoryUtil(Inventory inventory)
    {
        InventoryUtil util = new InventoryUtil();
        util.d_Inventory = inventory;
        util.d_Items = d_DataItems;
        return util;
    }
    private void DoCommandInventory(int player_id, Packet_ClientInventoryAction cmd)
    {
        Inventory inventory = GetPlayerInventory(clients[player_id].playername).Inventory;
        var s = new InventoryServer();
        s.d_Inventory = inventory;
        s.d_InventoryUtil = GetInventoryUtil(inventory);
        s.d_Items = d_DataItems;
        s.d_DropItem = this;

        switch (cmd.Action)
        {
            case Packet_InventoryActionTypeEnum.Click:
                s.InventoryClick(cmd.A);
                break;
            case Packet_InventoryActionTypeEnum.MoveToInventory:
                s.MoveToInventory(cmd.A);
                break;
            case Packet_InventoryActionTypeEnum.WearItem:
                s.WearItem(cmd.A, cmd.B);
                break;
            default:
                break;
        }
        clients[player_id].IsInventoryDirty = true;
        NotifyInventory(player_id);
    }

    private bool IsFillAreaValid(ClientOnServer client, Vector3i a, Vector3i b)
    {
        if (!MapUtil.IsValidPos(this.d_Map, a.x, a.y, a.z) || !MapUtil.IsValidPos(this.d_Map, b.x, b.y, b.z))
        {
            return false;
        }

        // TODO: Is there a more efficient way?
        int startx = Math.Min(a.x, b.x);
        int endx = Math.Max(a.x, b.x);
        int starty = Math.Min(a.y, b.y);
        int endy = Math.Max(a.y, b.y);
        int startz = Math.Min(a.z, b.z);
        int endz = Math.Max(a.z, b.z);
        for (int x = startx; x <= endx; x++)
        {
            for (int y = starty; y <= endy; y++)
            {
                for (int z = startz; z <= endz; z++)
                {
                    if (!config.CanUserBuild(client, x, y, z))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool DoFillArea(int player_id, Packet_ClientFillArea fill, int blockCount)
    {
        Vector3i a = new Vector3i(fill.X1, fill.Y1, fill.Z1);
        Vector3i b = new Vector3i(fill.X2, fill.Y2, fill.Z2);

        int startx = Math.Min(a.x, b.x);
        int endx = Math.Max(a.x, b.x);
        int starty = Math.Min(a.y, b.y);
        int endy = Math.Max(a.y, b.y);
        int startz = Math.Min(a.z, b.z);
        int endz = Math.Max(a.z, b.z);

        int blockType = fill.BlockType;
        blockType = d_Data.WhenPlayerPlacesGetsConvertedTo()[blockType];

        Inventory inventory = GetPlayerInventory(clients[player_id].playername).Inventory;
        var item = inventory.RightHand[fill.MaterialSlot];
        if (item == null)
        {
            return false;
        }
        //This prevents the player's inventory from getting sent to them while using fill (causes excessive bandwith usage)
        clients[player_id].usingFill = true;
        for (int x = startx; x <= endx; ++x)
        {
            for (int y = starty; y <= endy; ++y)
            {
                for (int z = startz; z <= endz; ++z)
                {
                    Packet_ClientSetBlock cmd = new Packet_ClientSetBlock();
                    cmd.X = x;
                    cmd.Y = y;
                    cmd.Z = z;
                    cmd.MaterialSlot = fill.MaterialSlot;
                    if (GetBlock(x, y, z) != 0)
                    {
                        cmd.Mode = Packet_BlockSetModeEnum.Destroy;
                        DoCommandBuild(player_id, true, cmd);
                    }
                    if (blockType != d_Data.BlockIdFillArea())
                    {
                        cmd.Mode = Packet_BlockSetModeEnum.Create;
                        DoCommandBuild(player_id, true, cmd);
                    }
                }
            }
        }
        clients[player_id].usingFill = false;
        return true;
    }
    /// <summary>
    /// Determines if a given client can see the specified chunk<br/>
    /// <b>Attention!</b> Chunk coordinates are NOT world coordinates!<br/>
    /// chunk position = (world position / chunk size)
    /// </summary>
    /// <param name="clientid">Client ID</param>
    /// <param name="vx">Chunk x coordinate</param>
    /// <param name="vy">Chunk y coordinate</param>
    /// <param name="vz">Chunk z coordinate</param>
    /// <returns>true if client can see the chunk, false otherwise</returns>
    public bool ClientSeenChunk(int clientid, int vx, int vy, int vz)
    {
        int pos = MapUtilCi.Index3d(vx, vy, vz, d_Map.MapSizeX / chunksize, d_Map.MapSizeY / chunksize);
        return clients[clientid].chunksseen[pos];
    }
    /// <summary>
    /// Sets a given chunk as seen by the client<br/>
    /// <b>Attention!</b> Chunk coordinates are NOT world coordinates!<br/>
    /// chunk position = (world position / chunk size)
    /// </summary>
    /// <param name="clientid">Client ID</param>
    /// <param name="vx">Chunk x coordinate</param>
    /// <param name="vy">Chunk y coordinate</param>
    /// <param name="vz">Chunk z coordinate</param>
    /// <param name="time"></param>
    public void ClientSeenChunkSet(int clientid, int vx, int vy, int vz, int time)
    {
        int pos = MapUtilCi.Index3d(vx, vy, vz, d_Map.MapSizeX / chunksize, d_Map.MapSizeY / chunksize);
        clients[clientid].chunksseen[pos] = true;
        clients[clientid].chunksseenTime[pos] = time;
        //Console.WriteLine("SeenChunk:   {0},{1},{2} Client: {3}", vx, vy, vz, clientid);
    }
    /// <summary>
    /// Sets a given chunk as unseen by the client<br/>
    /// <b>Attention!</b> Chunk coordinates are NOT world coordinates!<br/>
    /// chunk position = (world position / chunk size)
    /// </summary>
    /// <param name="clientid">Client ID</param>
    /// <param name="vx">Chunk x coordinate</param>
    /// <param name="vy">Chunk y coordinate</param>
    /// <param name="vz">Chunk z coordinate</param>
    public void ClientSeenChunkRemove(int clientid, int vx, int vy, int vz)
    {
        int pos = MapUtilCi.Index3d(vx, vy, vz, d_Map.MapSizeX / chunksize, d_Map.MapSizeY / chunksize);
        clients[clientid].chunksseen[pos] = false;
        clients[clientid].chunksseenTime[pos] = 0;
        //Console.WriteLine("UnseenChunk: {0},{1},{2} Client: {3}", vx, vy, vz, clientid);
    }
    private void SendFillArea(int clientid, Vector3i a, Vector3i b, int blockType, int blockCount)
    {
        // TODO: better to send a chunk?

        Vector3i v = new Vector3i((int)(a.x / chunksize), (int)(a.y / chunksize), (int)(a.z / chunksize));
        Vector3i w = new Vector3i((int)(b.x / chunksize), (int)(b.y / chunksize), (int)(b.z / chunksize));

        // TODO: Is it sufficient to regard only start- and endpoint?
        if (!ClientSeenChunk(clientid, v.x, v.y, v.z) && !ClientSeenChunk(clientid, w.x, w.y, w.z))
        {
            return;
        }

        Packet_ServerFillArea p = new Packet_ServerFillArea()
        {
            X1 = a.x,
            Y1 = a.y,
            Z1 = a.z,
            X2 = b.x,
            Y2 = b.y,
            Z2 = b.z,
            BlockType = blockType,
            BlockCount = blockCount
        };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.FillArea, FillArea = p }));
    }
    private void SetFillAreaLimit(int clientid)
    {
        ClientOnServer client = GetClient(clientid);
        if (client == null)
        {
            return;
        }

        int maxFill = 500;
        if (serverClient.DefaultFillLimit != null)
        {
            maxFill = serverClient.DefaultFillLimit.Value;
        }

        // Check if there is a fill-limit entry for his assigned group.
        if (client.clientGroup.FillLimit != null)
        {
            maxFill = client.clientGroup.FillLimit.Value;
        }

        // Check if there is an entry in clients with fill-limit member (overrides group fill-limit).
        foreach (ManicDigger.Client clientConfig in serverClient.Clients)
        {
            if (clientConfig.Name.Equals(client.playername, StringComparison.InvariantCultureIgnoreCase))
            {
                if (clientConfig.FillLimit != null)
                {
                    maxFill = clientConfig.FillLimit.Value;
                }
                break;
            }
        }
        client.FillLimit = maxFill;
        SendFillAreaLimit(clientid, maxFill);
    }



    private void SendFillAreaLimit(int clientid, int limit)
    {
        Packet_ServerFillAreaLimit p = new Packet_ServerFillAreaLimit()
        {
            Limit = limit
        };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.FillAreaLimit, FillAreaLimit = p }));
    }

    private bool DoCommandBuild(int player_id, bool execute, Packet_ClientSetBlock cmd)
    {
        Inventory inventory = GetPlayerInventory(clients[player_id].playername).Inventory;
        if (cmd.Mode == Packet_BlockSetModeEnum.Use)
        {
            for (int i = 0; i < modEventHandlers.onuse.Count; i++)
            {
                try
                {
                    modEventHandlers.onuse[i](player_id, cmd.X, cmd.Y, cmd.Z);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mod exception: OnUse");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return true;
        }
        if (cmd.Mode == Packet_BlockSetModeEnum.UseWithTool)
        {
            for (int i = 0; i < modEventHandlers.onusewithtool.Count; i++)
            {
                try
                {
                    modEventHandlers.onusewithtool[i](player_id, cmd.X, cmd.Y, cmd.Z, cmd.BlockType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mod exception: OnUseWithTool");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return true;
        }
        if (cmd.Mode == Packet_BlockSetModeEnum.Create
            && d_Data.Rail()[cmd.BlockType] != 0)
        {
            return DoCommandBuildRail(player_id, execute, cmd);
        }
        if (cmd.Mode == (int)Packet_BlockSetModeEnum.Destroy
            && d_Data.Rail()[d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z)] != 0)
        {
            return DoCommandRemoveRail(player_id, execute, cmd);
        }
        if (cmd.Mode == Packet_BlockSetModeEnum.Create)
        {
            int oldblock = d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z);
            if (!(oldblock == 0 || BlockTypes[oldblock].IsFluid()))
            {
                return false;
            }
            var item = inventory.RightHand[cmd.MaterialSlot];
            if (item == null)
            {
                return false;
            }
            switch (item.ItemClass)
            {
                case ItemClass.Block:
                    item.BlockCount--;
                    if (item.BlockCount == 0)
                    {
                        inventory.RightHand[cmd.MaterialSlot] = null;
                    }
                    if (d_Data.Rail()[item.BlockId] != 0)
                    {
                    }
                    SetBlockAndNotify(cmd.X, cmd.Y, cmd.Z, item.BlockId);
                    for (int i = 0; i < modEventHandlers.onbuild.Count; i++)
                    {
                        try
                        {
                            modEventHandlers.onbuild[i](player_id, cmd.X, cmd.Y, cmd.Z);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Mod exception: OnBuild");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    break;
                default:
                    Console.WriteLine("ProcessCommandBuild: unhandled ItemClass {0}", item.ItemClass);
                    return false;
            }
        }
        else
        {
            int blockid = d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z);
            // Validate blockid is within valid range
            if (blockid < 0 || blockid >= BlockTypes.Length)
            {
                return false;
            }
            BlockType blockType = BlockTypes[blockid];
            if (blockType == null)
            {
                return false;
            }
            
            // Determine what tool is being used
            ToolType toolUsed = ToolType.Hand;
            if (cmd.MaterialSlot >= 0 && cmd.MaterialSlot < inventory.RightHand.Length)
            {
                var heldItem = inventory.RightHand[cmd.MaterialSlot];
                if (heldItem != null && heldItem.ItemClass == ItemClass.Block)
                {
                    if (heldItem.BlockId >= 0 && heldItem.BlockId < BlockTypes.Length)
                    {
                        BlockType heldBlockType = BlockTypes[heldItem.BlockId];
                        if (heldBlockType != null && heldBlockType.IsTool)
                        {
                            toolUsed = heldBlockType.ToolType;
                        }
                    }
                }
            }
            
            // Check if tool requirement is met
            // - Blocks with PreferredTool.None can be broken with any tool (toolRequirementMet = true)
            // - Blocks with a specific PreferredTool: toolRequirementMet = true only if correct tool is used
            bool toolRequirementMet = (blockType.PreferredTool == ToolType.None) || 
                                      (blockType.PreferredTool == toolUsed);
            
            var item = new Item();
            item.ItemClass = ItemClass.Block;
            
            // Determine what item to drop
            if (blockType.RequiresTool && !toolRequirementMet)
            {
                // Block requires correct tool but wrong tool was used
                if (blockType.AlternativeDrop > 0)
                {
                    // Drop alternative item (e.g., small stones instead of stone block)
                    item.BlockId = blockType.AlternativeDrop;
                }
                else
                {
                    // Drop nothing - block is destroyed without giving anything
                    item = null;
                }
            }
            else
            {
                // Tool requirement met - give the proper block item
                item.BlockId = d_Data.WhenPlayerPlacesGetsConvertedTo()[blockid];
            }
            
            if (item != null && !config.IsCreative)
            {
                GetInventoryUtil(inventory).GrabItem(item, cmd.MaterialSlot);
            }
            SetBlockAndNotify(cmd.X, cmd.Y, cmd.Z, SpecialBlockId.Empty);
            for (int i = 0; i < modEventHandlers.ondelete.Count; i++)
            {
                try
                {
                    modEventHandlers.ondelete[i](player_id, cmd.X, cmd.Y, cmd.Z, blockid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mod exception: OnDelete");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        clients[player_id].IsInventoryDirty = true;
        NotifyInventory(player_id);
        return true;
    }

    private bool DoCommandBuildRail(int player_id, bool execute, Packet_ClientSetBlock cmd)
    {
        Inventory inventory = GetPlayerInventory(clients[player_id].playername).Inventory;
        int oldblock = d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z);
        //int blockstoput = 1;
        if (!(oldblock == SpecialBlockId.Empty || d_Data.IsRailTile(oldblock)))
        {
            return false;
        }

        //count how many rails will be created
        int oldrailcount = 0;
        if (d_Data.IsRailTile(oldblock))
        {
            oldrailcount = DirectionUtils.RailDirectionFlagsCount(
                (oldblock - d_Data.BlockIdRailstart()));
        }
        int newrailcount = DirectionUtils.RailDirectionFlagsCount(
            (cmd.BlockType - d_Data.BlockIdRailstart()));
        int blockstoput = newrailcount - oldrailcount;

        Item item = inventory.RightHand[cmd.MaterialSlot];
        if (!(item.ItemClass == ItemClass.Block && d_Data.Rail()[item.BlockId] != 0))
        {
            return false;
        }
        item.BlockCount -= blockstoput;
        if (item.BlockCount == 0)
        {
            inventory.RightHand[cmd.MaterialSlot] = null;
        }
        SetBlockAndNotify(cmd.X, cmd.Y, cmd.Z, cmd.BlockType);
        for (int i = 0; i < modEventHandlers.onbuild.Count; i++)
        {
            try
            {
                modEventHandlers.onbuild[i](player_id, cmd.X, cmd.Y, cmd.Z);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mod exception: OnBuild");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        clients[player_id].IsInventoryDirty = true;
        NotifyInventory(player_id);
        return true;
    }

    private bool DoCommandRemoveRail(int player_id, bool execute, Packet_ClientSetBlock cmd)
    {
        Inventory inventory = GetPlayerInventory(clients[player_id].playername).Inventory;
        //add to inventory
        int blockid = d_Map.GetBlock(cmd.X, cmd.Y, cmd.Z);
        int blocktype = d_Data.WhenPlayerPlacesGetsConvertedTo()[blockid];
        if ((!IsValid(blocktype))
            || blocktype == SpecialBlockId.Empty)
        {
            return false;
        }
        int blockstopick = 1;
        if (d_Data.IsRailTile(blocktype))
        {
            blockstopick = DirectionUtils.RailDirectionFlagsCount(
                (blocktype - d_Data.BlockIdRailstart()));
        }

        var item = new Item();
        item.ItemClass = ItemClass.Block;
        item.BlockId = d_Data.WhenPlayerPlacesGetsConvertedTo()[blocktype];
        item.BlockCount = blockstopick;
        if (!config.IsCreative)
        {
            GetInventoryUtil(inventory).GrabItem(item, cmd.MaterialSlot);
        }
        SetBlockAndNotify(cmd.X, cmd.Y, cmd.Z, SpecialBlockId.Empty);
        for (int i = 0; i < modEventHandlers.ondelete.Count; i++)
        {
            try
            {
                modEventHandlers.ondelete[i](player_id, cmd.X, cmd.Y, cmd.Z, blockid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mod exception: OnDelete");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        clients[player_id].IsInventoryDirty = true;
        NotifyInventory(player_id);
        return true;
    }

    private bool IsValid(int blocktype)
    {
        return BlockTypes[blocktype].Name != null;
    }

    public void SetBlockAndNotify(int x, int y, int z, int blocktype)
    {
        d_Map.SetBlockNotMakingDirty(x, y, z, blocktype);
        NotifyBlock(x, y, z, blocktype);
    }
    private int TotalAmount(Dictionary<int, int> inventory)
    {
        int sum = 0;
        foreach (var k in inventory)
        {
            sum += k.Value;
        }
        return sum;
    }
    private void RemoveEquivalent(Dictionary<int, int> inventory, int blocktype, int count)
    {
        int removed = 0;
        for (int i = 0; i < count; i++)
        {
            foreach (var k in new Dictionary<int, int>(inventory))
            {
                if (EquivalentBlock(k.Key, blocktype)
                    && k.Value > 0)
                {
                    inventory[k.Key]--;
                    removed++;
                    goto removenext;
                }
            }
        removenext:
            ;
        }
        if (removed != count)
        {
            //throw new Exception();
        }
    }
    private int GetEquivalentCount(Dictionary<int, int> inventory, int blocktype)
    {
        int count = 0;
        foreach (var k in inventory)
        {
            if (EquivalentBlock(k.Key, blocktype))
            {
                count += k.Value;
            }
        }
        return count;
    }
    private bool EquivalentBlock(int blocktypea, int blocktypeb)
    {
        if (d_Data.IsRailTile(blocktypea) && d_Data.IsRailTile(blocktypeb))
        {
            return true;
        }
        return blocktypea == blocktypeb;
    }
    public byte[] Serialize(Packet_Server p)
    {
        byte[] data = Packet_ServerSerializer.SerializeToBytes(p);
        return data;
    }
}
}
