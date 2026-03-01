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
    private string GenerateUsername(string name)
    {
        string defaultname = name;
        int appendNumber = 1;
        bool exists;

        do
        {
            exists = false;
            foreach (var k in clients)
            {
                if (k.Value.playername.Equals(defaultname + appendNumber))
                {
                    exists = true;
                    appendNumber++;
                    break;
                }
            }
        } while (exists);

        return defaultname + appendNumber;
    }
    public void ServerMessageToAll(string message, MessageType color)
    {
        this.SendMessageToAll(MessageTypeToString(color) + message);
        ServerEventLog(string.Format("SERVER MESSAGE: {0}.", message));
    }
    public void SendMessageToAll(string message)
    {
        Console.WriteLine("Message to all: " + message);
        foreach (var k in clients)
        {
            SendMessage(k.Key, message);
        }
    }
    private void SendSetBlock(int clientid, int x, int y, int z, int blocktype)
    {
    	if (!ClientSeenChunk(clientid, (int)(x / chunksize), (int)(y / chunksize), (int)(z / chunksize)))
        {
    		// don't send block updates for chunks a player can not see
            return;
        }
        Packet_ServerSetBlock p = new Packet_ServerSetBlock() { X = x, Y = y, Z = z, BlockType = blocktype };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.SetBlock, SetBlock = p }));
    }
    public void SendSound(int clientid, string name, int x, int y, int z)
    {
        Packet_ServerSound p = new Packet_ServerSound() { Name = name, X = x, Y = y, Z = z };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Sound, Sound = p }));
    }
    private void SendPlayerSpawnPosition(int clientid, int x, int y, int z)
    {
        Packet_ServerPlayerSpawnPosition p = new Packet_ServerPlayerSpawnPosition()
        {
            X = x,
            Y = y,
            Z = z
        };
        SendPacket(clientid, Serialize(new Packet_Server()
        {
            Id = Packet_ServerIdEnum.PlayerSpawnPosition,
            PlayerSpawnPosition = p,
        }));
    }
    
    public void SendMessage(int clientid, string message, MessageType color)
    {
        SendMessage(clientid, MessageTypeToString(color) + message);
    }

    public void SendMessage(int clientid, string message)
    {
        if (clientid == this.serverConsoleId)
        {
            this.serverConsole.Receive(message);
            return;
        }

        SendPacket(clientid, ServerPackets.Message(message));
    }

    int StatTotalPackets = 0;
    int StatTotalPacketsLength = 0;
    public long TotalSentBytes;
    public long TotalReceivedBytes;

    public void SendPacket(int clientid, Packet_Server packet)
    {
        SendPacket(clientid, Serialize(packet));
    }

    public void SendPacket(int clientid, byte[] packet)
    {
        if (clients[clientid].IsBot)
        {
            return;
        }
        StatTotalPackets++;
        StatTotalPacketsLength += packet.Length;
        TotalSentBytes += packet.Length;
        try
        {
            INetOutgoingMessage msg = new INetOutgoingMessage();
            msg.Write(packet, packet.Length);
            clients[clientid].socket.SendMessage(msg, MyNetDeliveryMethod.ReliableOrdered, 0);
        }
        catch (Exception)
        {
            Console.WriteLine("Network exception.");
            KillPlayer(clientid);
        }
    }
    void EmptyCallback(IAsyncResult result)
    {
    }
    public int drawdistance = 128;
    public const int chunksize = 32;
    internal int chunkdrawdistance { get { return drawdistance / chunksize; } }
    public byte[] CompressChunkNetwork(ushort[] chunk)
    {
        return d_NetworkCompression.Compress(Misc.UshortArrayToByteArray(chunk));
    }
    public byte[] CompressChunkNetwork(byte[, ,] chunk)
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        for (int z = 0; z <= chunk.GetUpperBound(2); z++)
        {
            for (int y = 0; y <= chunk.GetUpperBound(1); y++)
            {
                for (int x = 0; x <= chunk.GetUpperBound(0); x++)
                {
                    bw.Write((byte)chunk[x, y, z]);
                }
            }
        }
        byte[] compressedchunk = d_NetworkCompression.Compress(ms.ToArray());
        return compressedchunk;
    }

    Packet_StringList GetRequiredBlobMd5()
    {
        Packet_StringList p = new Packet_StringList();
        List<string> list = new List<string>();

        for (int i = 0; i < assets.count; i++)
        {
            list.Add(assets.items[i].md5);
        }

        p.SetItems(list.ToArray(), list.Count, list.Count);
        return p;
    }

    Packet_StringList GetRequiredBlobName()
    {
        Packet_StringList p = new Packet_StringList();
        List<string> list = new List<string>();

        for (int i = 0; i < assets.count; i++)
        {
            list.Add(assets.items[i].name);
        }

        p.SetItems(list.ToArray(), list.Count, list.Count);
        return p;
    }

    AssetLoader assetLoader;
    AssetList assets = new AssetList();

    int BlobPartLength = 1024 * 1;
    private void SendBlobs(int clientid, Packet_StringList list)
    {
        SendPacket(clientid, ServerPackets.LevelInitialize());
        LoadAssets();

        List<Asset> tosend = new List<Asset>();
        for (int i = 0; i < assets.count; i++)
        {
            Asset f = assets.items[i];
            for (int k = 0; k < list.ItemsCount; k++)
            {
                if (f.md5 == list.Items[k])
                {
                    tosend.Add(f);
                }
            }
        }

        for (int i = 0; i < tosend.Count; i++)
        {
            Asset f = tosend[i];
            SendBlobInitialize(clientid, f.md5, f.name);
            byte[] blob = f.data;
            int totalsent = 0;
            foreach (byte[] part in Parts(blob, BlobPartLength))
            {
                SendLevelProgress(clientid,
                    (int)(((float)i / tosend.Count
                                         + ((float)totalsent / blob.Length) / tosend.Count) * 100), language.ServerProgressDownloadingData());
                SendBlobPart(clientid, part);
                totalsent += part.Length;
            }
            SendBlobFinalize(clientid);
        }
        SendLevelProgress(clientid, 0, language.ServerProgressGenerating());
    }

    void LoadAssets()
    {
        FloatRef progress = new FloatRef();
        assetLoader.LoadAssetsAsync(assets, progress);
        while (progress.value < 1)
        {
            Thread.Sleep(1);
        }
    }

    public static IEnumerable<byte[]> Parts(byte[] blob, int partsize)
    {
        int i = 0;
        for (; ; )
        {
            if (i >= blob.Length) { break; }
            int curpartsize = blob.Length - i;
            if (curpartsize > partsize) { curpartsize = partsize; }
            byte[] part = new byte[curpartsize];
            for (int ii = 0; ii < curpartsize; ii++)
            {
                part[ii] = blob[i + ii];
            }
            yield return part;
            i += curpartsize;
        }
    }
    private void SendBlobInitialize(int clientid, string hash, string name)
    {
        Packet_ServerBlobInitialize p = new Packet_ServerBlobInitialize() { Name = name, Md5 = hash };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.BlobInitialize, BlobInitialize = p }));
    }
    private void SendBlobPart(int clientid, byte[] data)
    {
        Packet_ServerBlobPart p = new Packet_ServerBlobPart() { Data = data };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.BlobPart, BlobPart = p }));
    }
    private void SendBlobFinalize(int clientid)
    {
        Packet_ServerBlobFinalize p = new Packet_ServerBlobFinalize() { };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.BlobFinalize, BlobFinalize = p }));
    }
    public BlockType[] BlockTypes = new BlockType[GlobalVar.MAX_BLOCKTYPES];
    public void SendBlockTypes(int clientid)
    {
        for (int i = 0; i < BlockTypes.Length; i++)
        {
            Packet_ServerBlockType p1 = new Packet_ServerBlockType() { Id = i, Blocktype = BlockTypeConverter.GetBlockType(BlockTypes[i]) };
            SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.BlockType, BlockType = p1 }));
        }
        Packet_ServerBlockTypes p = new Packet_ServerBlockTypes() { };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.BlockTypes, BlockTypes = p }));
    }

    public void SendTranslations(int clientid)
    {
        //Read all lines from server translation and send them to the client
        TranslatedString[] strings = language.AllStrings();
        foreach (TranslatedString transString in strings)
        {
            if (transString == null)
            {
                continue;
            }
            Packet_ServerTranslatedString p = new Packet_ServerTranslatedString()
            {
                Lang = transString.language,
                Id = transString.id,
                Translation = transString.translated
            };
            SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Translation, Translation = p }));
        }
    }

    public static int SerializeFloat(float p)
    {
        return (int)(p * 32);
    }

    private void SendSunLevels(int clientid)
    {
        Packet_ServerSunLevels p = new Packet_ServerSunLevels();
        p.SetSunlevels(sunlevels, sunlevels.Length, sunlevels.Length);
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.SunLevels, SunLevels = p }));
    }
    private void SendLightLevels(int clientid)
    {
        Packet_ServerLightLevels p = new Packet_ServerLightLevels();
        int[] l = new int[lightlevels.Length];
        for (int i = 0; i < lightlevels.Length; i++)
        {
            l[i] = SerializeFloat(lightlevels[i]);
        }
        p.SetLightlevels(l, l.Length, l.Length);
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.LightLevels, LightLevels = p }));
    }
    private void SendCraftingRecipes(int clientid)
    {
        Packet_CraftingRecipe[] recipes = new Packet_CraftingRecipe[craftingrecipes.Count];
        for (int i = 0; i < craftingrecipes.Count; i++)
        {
            recipes[i] = ConvertCraftingRecipe(craftingrecipes[i]);
        }
        Packet_ServerCraftingRecipes p = new Packet_ServerCraftingRecipes();
        p.SetCraftingRecipes(recipes, recipes.Length, recipes.Length);
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.CraftingRecipes, CraftingRecipes = p }));
    }

    private Packet_CraftingRecipe ConvertCraftingRecipe(CraftingRecipe craftingRecipe)
    {
        if (craftingRecipe == null)
        {
            return null;
        }
        Packet_CraftingRecipe r = new Packet_CraftingRecipe();
        if (craftingRecipe.ingredients != null)
        {
            r.Ingredients = new Packet_Ingredient[craftingRecipe.ingredients.Length];
            for (int i = 0; i < craftingRecipe.ingredients.Length; i++)
            {
                r.Ingredients[i] = ConvertIngredient(craftingRecipe.ingredients[i]);
            }
            r.IngredientsCount = r.Ingredients.Length;
            r.IngredientsLength = r.Ingredients.Length;
        }
        if (craftingRecipe.output != null)
        {
            r.Output = new Packet_Ingredient();
            r.Output.Amount = craftingRecipe.output.Amount;
            r.Output.Type = craftingRecipe.output.Type;
        }
        return r;
    }

    private Packet_Ingredient ConvertIngredient(Ingredient ingredient)
    {
        Packet_Ingredient p = new Packet_Ingredient();
        p.Amount = ingredient.Amount;
        p.Type = ingredient.Type;
        return p;
    }

    private void SendLevelProgress(int clientid, int percentcomplete, string status)
    {
        Packet_ServerLevelProgress p = new Packet_ServerLevelProgress() { PercentComplete = percentcomplete, Status = status };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.LevelDataChunk, LevelDataChunk = p }));
    }
    public RenderHint RenderHint = RenderHint.Fast;
    private void SendServerIdentification(int clientid)
    {
        Packet_ServerIdentification p = new Packet_ServerIdentification()
        {
            MdProtocolVersion = GameVersion.Version,
            AssignedClientId = clientid,
            ServerName = config.Name,
            ServerMotd = config.Motd,
            //UsedBlobsMd5 = new List<byte[]>(new[] { terrainTextureMd5 }),
            //TerrainTextureMd5 = terrainTextureMd5,
            MapSizeX = d_Map.MapSizeX,
            MapSizeY = d_Map.MapSizeY,
            MapSizeZ = d_Map.MapSizeZ,
            DisableShadows = enableshadows ? 0 : 1,
            PlayerAreaSize = playerareasize,
            RenderHint_ = (int)RenderHint,
            RequiredBlobMd5 = GetRequiredBlobMd5(),
            RequiredBlobName = GetRequiredBlobName(),
        };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.ServerIdentification, Identification = p }));
    }

    public void SendFreemoveState(int clientid, bool isEnabled)
    {
        Packet_ServerFreemove p = new Packet_ServerFreemove()
        {
            IsEnabled = isEnabled ? 1 : 0
        };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Freemove, Freemove = p }));
    }

    byte[] terrainTextureMd5 { get { byte[] b = new byte[16]; b[0] = 1; return b; } }
    MD5 md5 = System.Security.Cryptography.MD5.Create();
    byte[] ComputeMd5(byte[] b)
    {
        return md5.ComputeHash(b);
    }
    string ComputeMd5(string input)
    {
        // step 1, calculate MD5 hash from input
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString().ToLower();
    }
    public int SIMULATION_KEYFRAME_EVERY = 4;
    public float SIMULATION_STEP_LENGTH = 1f / 64f;
    public struct BlobToSend
    {
        public string Name;
        public byte[] Data;
    }
    public Dictionary<int, ClientOnServer> clients = new Dictionary<int, ClientOnServer>();
    public Dictionary<string, bool> disabledprivileges = new Dictionary<string, bool>();
    public Dictionary<string, bool> extraPrivileges = new Dictionary<string, bool>();
    public ClientOnServer GetClient(int id)
    {
        if (id == this.serverConsoleId)
        {
            return this.serverConsoleClient;
        }
        if (!clients.ContainsKey(id))
            return null;
        return clients[id];
    }
    public ClientOnServer GetClient(string name)
    {
        if (serverConsoleClient.playername.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        {
            return serverConsoleClient;
        }
        foreach (var k in clients)
        {
            if (k.Value.playername.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return k.Value;
            }
        }
        return null;
    }

    internal bool serverClientNeedsSaving;
    public ServerClient serverClient;
    public ManicDigger.Group defaultGroupGuest;
    public ManicDigger.Group defaultGroupRegistered;
    public Vector3i defaultPlayerSpawn;

    private Vector3i SpawnToVector3i(ManicDigger.Spawn spawn)
    {
        int x = spawn.x;
        int y = spawn.y;
        int z;
        if (!MapUtil.IsValidPos(d_Map, x, y))
        {
            throw new Exception(language.ServerInvalidSpawnCoordinates());
        }

        if (spawn.z == null)
        {
            z = MapUtil.blockheight(d_Map, 0, x, y);
        }
        else
        {
            z = spawn.z.Value;
            if (!MapUtil.IsValidPos(d_Map, x, y, z))
            {
                throw new Exception(language.ServerInvalidSpawnCoordinates());
            }
        }
        return new Vector3i(x * 32, z * 32, y * 32);
    }

    public int dumpmax = 30;
    public void DropItem(ref Item item, Vector3i pos)
    {
        switch (item.ItemClass)
        {
            case ItemClass.Block:
                for (int i = 0; i < dumpmax; i++)
                {
                    if (item.BlockCount == 0) { break; }
                    //find empty position that is nearest to dump place AND has a block under.
                    Vector3i? nearpos = FindDumpPlace(pos);
                    if (nearpos == null)
                    {
                        break;
                    }
                    SetBlockAndNotify(nearpos.Value.x, nearpos.Value.y, nearpos.Value.z, item.BlockId);
                    item.BlockCount--;
                }
                if (item.BlockCount == 0)
                {
                    item = null;
                }
                break;
            default:
                Console.WriteLine("DropItem: unhandled ItemClass {0}", item.ItemClass);
                break;
        }
    }
    private Vector3i? FindDumpPlace(Vector3i pos)
    {
        List<Vector3i> l = new List<Vector3i>();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    int xx = pos.x + x - 10 / 2;
                    int yy = pos.y + y - 10 / 2;
                    int zz = pos.z + z - 10 / 2;
                    if (!MapUtil.IsValidPos(d_Map, xx, yy, zz))
                    {
                        continue;
                    }
                    if (d_Map.GetBlock(xx, yy, zz) == SpecialBlockId.Empty
                        && d_Map.GetBlock(xx, yy, zz - 1) != SpecialBlockId.Empty)
                    {
                        bool playernear = false;
                        foreach (var player in clients)
                        {
                            if (Length(Minus(PlayerBlockPosition(player.Value), new Vector3i(xx, yy, zz))) < 3)
                            {
                                playernear = true;
                            }
                        }
                        if (!playernear)
                        {
                            l.Add(new Vector3i(xx, yy, zz));
                        }
                    }
                }
            }
        }
        l.Sort((a, b) => Length(Minus(a, pos)).CompareTo(Length(Minus(b, pos))));
        if (l.Count > 0)
        {
            return l[0];
        }
        return null;
    }
    private Vector3i Minus(Vector3i a, Vector3i b)
    {
        return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    int Length(Vector3i v)
    {
        return (int)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }

    public void SetBlockType(int id, string name, BlockType block)
    {
        BlockTypes[id] = block;
        block.Name = name;
        d_Data.UseBlockType(id, BlockTypeConverter.GetBlockType(block));
    }
    public void SetBlockType(string name, BlockType block)
    {
        for (int i = 0; i < BlockTypes.Length; i++)
        {
            if (BlockTypes[i].Name == null)
            {
                SetBlockType(i, name, block);
                return;
            }
        }
    }

    int[] sunlevels;
    public void SetSunLevels(int[] sunLevels)
    {
        this.sunlevels = sunLevels;
    }
    float[] lightlevels;
    public void SetLightLevels(float[] lightLevels)
    {
        this.lightlevels = lightLevels;
    }
    public List<CraftingRecipe> craftingrecipes = new List<CraftingRecipe>();

    public bool IsSinglePlayer
    {
        get { return mainSockets[0].GetType() == typeof(DummyNetServer); }
    }

    public void SendDialog(int player, string id, Dialog dialog)
    {
        Packet_ServerDialog p = new Packet_ServerDialog()
        {
            DialogId = id,
            Dialog = ConvertDialog(dialog),
        };
        SendPacket(player, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Dialog, Dialog = p }));
    }

    private Packet_Dialog ConvertDialog(Dialog dialog)
    {
        if (dialog == null)
        {
            return null;
        }
        Packet_Dialog p = new Packet_Dialog();
        p.Height_ = dialog.Height;
        p.IsModal = dialog.IsModal ? 1 : 0;
        if (dialog.Widgets != null)
        {
            Packet_Widget[] widgets = new Packet_Widget[dialog.Widgets.Length];
            for (int i = 0; i < dialog.Widgets.Length; i++)
            {
                widgets[i] = ConvertWidget(dialog.Widgets[i]);
            }
            p.SetWidgets(widgets, widgets.Length, widgets.Length);
        }
        p.Width = dialog.Width;
        return p;
    }

    private Packet_Widget ConvertWidget(Widget widget)
    {
        if (widget == null)
        {
            return null;
        }
        Packet_Widget w = new Packet_Widget();
        w.Click = widget.Click ? 1 : 0;
        w.ClickKey = widget.ClickKey;
        w.Color = widget.Color;
        w.Font = ConvertFont(widget.Font);
        w.Height_ = widget.Height;
        w.Id = widget.Id;
        w.Image = widget.Image;
        w.Text = widget.Text;
        w.Type = (int)widget.Type;
        w.Width = widget.Width;
        w.X = widget.X;
        w.Y = widget.Y;
        return w;
    }

    private Packet_DialogFont ConvertFont(DialogFont dialogFont)
    {
        if (dialogFont == null)
        {
            return null;
        }
        Packet_DialogFont f = new Packet_DialogFont();
        f.FamilyName = dialogFont.FamilyName;
        f.FontStyle = (int)dialogFont.FontStyle;
        f.SizeFloat = SerializeFloat(dialogFont.Size);
        return f;
    }

    public bool PlayerHasPrivilege(int player, string privilege)
    {
        if (extraPrivileges.ContainsKey(privilege))
        {
            return true;
        }
        if (disabledprivileges.ContainsKey(privilege))
        {
            return false;
        }
        return GetClient(player).privileges.Contains(privilege);
    }

    public void PlaySoundAt(int posx, int posy, int posz, string sound)
    {
        PlaySoundAtExceptPlayer(posx, posy, posz, sound, null);
    }

    public void PlaySoundAtExceptPlayer(int posx, int posy, int posz, string sound, int? player)
    {
        Vector3i pos = new Vector3i(posx, posy, posz);
        foreach (var k in clients)
        {
            if (player != null && player == k.Key)
            {
                continue;
            }
            int distance = DistanceSquared(new Vector3i((int)k.Value.PositionMul32GlX / 32, (int)k.Value.PositionMul32GlZ / 32, (int)k.Value.PositionMul32GlY / 32), pos);
            if (distance < 64 * 64)
            {
                SendSound(k.Key, sound, pos.x, posy, posz);
            }
        }
    }

    public void PlaySoundAt(int posx, int posy, int posz, string sound, int range)
    {
        PlaySoundAtExceptPlayer(posx, posy, posz, sound, null, range);
    }
    public void PlaySoundAtExceptPlayer(int posx, int posy, int posz, string sound, int? player, int range)
    {
        Vector3i pos = new Vector3i(posx, posy, posz);
        foreach (var k in clients)
        {
            if (player != null && player == k.Key)
            {
                continue;
            }
            int distance = DistanceSquared(new Vector3i((int)k.Value.PositionMul32GlX / 32, (int)k.Value.PositionMul32GlZ / 32, (int)k.Value.PositionMul32GlY / 32), pos);
            if (distance < range)
            {
                SendSound(k.Key, sound, pos.x, posy, posz);
            }
        }
    }

    public void SendPacketFollow(int player, int target, bool tpp)
    {
        Packet_ServerFollow p = new Packet_ServerFollow()
        {
            Client = target == -1 ? null : clients[target].playername,
            Tpp = tpp ? 1 : 0,
        };
        SendPacket(player, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Follow, Follow = p }));
    }

    public void SendAmmo(int playerid, Dictionary<int, int> totalAmmo)
    {
        Packet_ServerAmmo p = new Packet_ServerAmmo();
        Packet_IntInt[] t = new Packet_IntInt[totalAmmo.Count];
        int i = 0;
        foreach (var k in totalAmmo)
        {
            t[i++] = new Packet_IntInt() { Key_ = k.Key, Value_ = k.Value };
        }
        p.TotalAmmoCount = totalAmmo.Count;
        p.TotalAmmoLength = totalAmmo.Count;
        p.TotalAmmo = t;
        SendPacket(playerid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Ammo, Ammo = p }));
    }

    public void SendExplosion(int player, float x, float y, float z, bool relativeposition, float range, float time)
    {
        Packet_ServerExplosion p = new Packet_ServerExplosion();
        p.XFloat = SerializeFloat(x);
        p.YFloat = SerializeFloat(y);
        p.ZFloat = SerializeFloat(z);
        p.IsRelativeToPlayerPosition = relativeposition ? 1 : 0;
        p.RangeFloat = SerializeFloat(range);
        p.TimeFloat = SerializeFloat(time);
        SendPacket(player, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Explosion, Explosion = p }));
    }

    public string GetGroupColor(int playerid)
    {
        return GetClient(playerid).clientGroup.GroupColorString();
    }

    public string GetGroupName(int playerid)
    {
        return GetClient(playerid).clientGroup.Name;
    }

    internal void InstallHttpModule(string name, ManicDigger.Func<string> description, FragLabs.HTTP.IHttpModule module)
    {
        ActiveHttpModule m = new ActiveHttpModule();
        m.name = name;
        m.description = description;
        m.module = module;
        httpModules.Add(m);
    }
    internal List<ActiveHttpModule> httpModules = new List<ActiveHttpModule>();

    public ModEventHandlers modEventHandlers = new ModEventHandlers();

    internal static bool IsTransparentForLight(BlockType b)
    {
        return b.DrawType != DrawType.Solid && b.DrawType != DrawType.ClosedDoor;
    }

    public int GetSimulationCurrentFrame()
    {
        return simulationcurrentframe;
    }

    public GameTime GetTime()
    {
        return _time;
    }

    internal void PlayerEntitySetDirty(int player)
    {
        foreach (var k in clients.Values)
        {
            k.playersDirty[player] = true;
        }
    }

    internal ServerEntity GetEntity(ServerEntityId id)
    {
        ServerChunk c = d_Map.GetChunk_(id.chunkx, id.chunky, id.chunkz);
        return c.Entities[id.id];
    }

    internal void SetEntityDirty(ServerEntityId id)
    {
        foreach (var k in clients)
        {
            for (int i = 0; i < k.Value.spawnedEntities.Length; i++)
            {
                ServerEntityId s = k.Value.spawnedEntities[i];
                if (s == null)
                {
                    continue;
                }
                if (s.chunkx == id.chunkx
                    && s.chunky == id.chunky
                    && s.chunkz == id.chunkz
                    && s.id == id.id)
                {
                    k.Value.updateEntity[i] = true;
                }
            }
        }
        ServerChunk chunk = d_Map.GetChunk(id.chunkx * Server.chunksize, id.chunky * Server.chunksize, id.chunkz * Server.chunksize);
        chunk.DirtyForSaving = true;
    }

    internal void DespawnEntity(ServerEntityId id)
    {
        ServerChunk chunk = d_Map.GetChunk(id.chunkx * Server.chunksize, id.chunky * Server.chunksize, id.chunkz * Server.chunksize);
        chunk.Entities[id.id] = null;
        if (id.id == chunk.EntitiesCount - 1)
        {
            chunk.EntitiesCount--;
        }
        chunk.DirtyForSaving = true;
    }

    internal ServerEntityId AddEntity(int x, int y, int z, ServerEntity e)
    {
        ServerChunk c = d_Map.GetChunk(x, y, z);
        if (c.Entities == null)
        {
            c.Entities = new ServerEntity[256];
        }
        if (c.Entities.Length < c.EntitiesCount + 1)
        {
            Array.Resize(ref c.Entities, c.EntitiesCount + 1);
        }
        c.Entities[c.EntitiesCount++] = e;
        c.DirtyForSaving = true;
        return new ServerEntityId(x / d_Map.chunksize, x / d_Map.chunksize, x / d_Map.chunksize, c.EntitiesCount - 1);
    }
}
}
