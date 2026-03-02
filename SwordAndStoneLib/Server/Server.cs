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
public class ClientException : Exception
{
    public ClientException(Exception innerException, int clientid)
        : base("Client exception", innerException)
    {
        this.clientid = clientid;
    }
    public int clientid;
}
[ProtoContract]
public class SwordAndStoneSave
{
    [ProtoMember(1, IsRequired = false)]
    public int MapSizeX;
    [ProtoMember(2, IsRequired = false)]
    public int MapSizeY;
    [ProtoMember(3, IsRequired = false)]
    public int MapSizeZ;
    [ProtoMember(4, IsRequired = false)]
    public Dictionary<string, PacketServerInventory> Inventory;
    [ProtoMember(7, IsRequired = false)]
    public int Seed;
    [ProtoMember(8, IsRequired = false)]
    public long SimulationCurrentFrame;
    [ProtoMember(9, IsRequired = false)]
    public Dictionary<string, PacketServerPlayerStats> PlayerStats;
    [ProtoMember(10, IsRequired = false)]
    public int LastMonsterId;
    [ProtoMember(11, IsRequired = false)]
    public Dictionary<string, byte[]> moddata;
    [ProtoMember(12, IsRequired = false)]
    public long TimeOfDay;
}
public partial class Server : ICurrentTime, IDropItem
{
    public Server()
    {
        serverPlatform = new ServerPlatformNative();

        server = new ServerCi();
        systems = new ServerSystem[256];
        // This ServerSystem should always be loaded first
        systems[systemsCount++] = new ServerSystemLoadFirst();

        // Regular ServerSystems
        systems[systemsCount++] = new ServerSystemLoadConfig();
        systems[systemsCount++] = new ServerSystemHeartbeat();
        systems[systemsCount++] = new ServerSystemHttpServer();
        systems[systemsCount++] = new ServerSystemUnloadUnusedChunks();
        systems[systemsCount++] = new ServerSystemNotifyMap();
        systems[systemsCount++] = new ServerSystemNotifyPing();
        systems[systemsCount++] = new ServerSystemChunksSimulation();
        systems[systemsCount++] = new ServerSystemBanList();
        systems[systemsCount++] = new ServerSystemModLoader();
        systems[systemsCount++] = new ServerSystemLoadServerClient();
        systems[systemsCount++] = new ServerSystemNotifyEntities();
        systems[systemsCount++] = new ServerSystemMonsterWalk();

        // This ServerSystem should always be loaded last
        systems[systemsCount++] = new ServerSystemLoadLast();

        //Load translations
        language = new LanguageNative();
        language.LoadTranslations();
    }
    

    internal ServerCi server;
    internal ServerSystem[] systems;
    internal int systemsCount;
    internal ServerPlatform serverPlatform;

    public GameExit exit;
    public ServerMap d_Map;
    public GameData d_Data;
    public CraftingTableTool d_CraftingTableTool;
    public IGetFileStream d_GetFile;
    public IChunkDb d_ChunkDb;
    public ICompression d_NetworkCompression;
    public NetServer[] mainSockets { get { return server.mainSockets; } set { server.mainSockets = value; } }
    public int mainSocketsCount { get { return server.mainSocketsCount; } set { server.mainSocketsCount = value; } }
    
    public bool LocalConnectionsOnly { get; set; }
    public string[] PublicDataPaths = new string[0];
    public int singleplayerport = 25570;
    public Random rnd = new Random();
    public int SpawnPositionRandomizationRange = 96;
    public bool IsMono = Type.GetType("Mono.Runtime") != null;

    public string serverpathlogs = Path.Combine(GameStorePath.GetStorePath(), "Logs");
    private void BuildLog(string p)
    {
        if (!config.BuildLogging)
        {
            return;
        }
        if (!Directory.Exists(serverpathlogs))
        {
            Directory.CreateDirectory(serverpathlogs);
        }
        string filename = Path.Combine(serverpathlogs, "BuildLog.txt");
        try
        {
            File.AppendAllText(filename, string.Format("{0} {1}\n", DateTime.Now, p));
        }
        catch
        {
            Console.WriteLine(language.ServerCannotWriteLog(), filename);
        }
    }
    public void ServerEventLog(string p)
    {
        if (!config.ServerEventLogging)
        {
            return;
        }
        if (!Directory.Exists(serverpathlogs))
        {
            Directory.CreateDirectory(serverpathlogs);
        }
        string filename = Path.Combine(serverpathlogs, "ServerEventLog.txt");
        try
        {
            File.AppendAllText(filename, string.Format("{0} {1}\n", DateTime.Now, p));
        }
        catch
        {
            Console.WriteLine(language.ServerCannotWriteLog(), filename);
        }
    }
    public void ChatLog(string p)
    {
        if (!config.ChatLogging)
        {
            return;
        }
        if (!Directory.Exists(serverpathlogs))
        {
            Directory.CreateDirectory(serverpathlogs);
        }
        string filename = Path.Combine(serverpathlogs, "ChatLog.txt");
        try
        {
            File.AppendAllText(filename, string.Format("{0} {1}\n", DateTime.Now, p));
        }
        catch
        {
            Console.WriteLine(language.ServerCannotWriteLog(), filename);
        }
    }

    public bool Public;
    public bool enableshadows = true;
    public Language language = new LanguageNative();

    Stopwatch stopwatchDt = new Stopwatch();
    public Stopwatch serverUptime = new Stopwatch();
    public void Process()
    {
        try
        {
            float dt = (float)stopwatchDt.Elapsed.TotalSeconds;
            stopwatchDt.Reset();
            stopwatchDt.Start();

            for (int i = 0; i < systemsCount; i++)
            {
                systems[i].Update(this, dt);
            }
            //Save data
            ProcessSave();
            //Do server stuff
            ProcessMain();

            //When a value of 0 or less is given, don't restart
            if (config.AutoRestartCycle > 0 && serverUptime.Elapsed.TotalHours >= config.AutoRestartCycle)
            {
                //Restart interval elapsed
                Restart();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void ProcessSave()
    {
        if ((DateTime.UtcNow - lastsave).TotalMinutes > 2)
        {
            DateTime start = DateTime.UtcNow;
            SaveGlobalData();
            Console.WriteLine(language.ServerGameSaved(), (DateTime.UtcNow - start));
            lastsave = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Tell the clients the time
    /// </summary>
    private void NotifySeason(int clientid)
    {
        if (clients[clientid].state == ClientStateOnServer.Connecting)
        {
            return;
        }

        Packet_ServerSeason p = new Packet_ServerSeason()
        {
            Hour = _time.GetQuarterHourPartOfDay(),

            //DayNightCycleSpeedup is used by the client like this:
            //day_length_in_seconds = SecondsInADay / packet.Season.DayNightCycleSpeedup;

            //Set it to 1 if we froze the time, to prevent a division by zero
            DayNightCycleSpeedup = (_time.SpeedOfTime != 0) ? _time.SpeedOfTime : 1,
            Moon = 0,
        };
        SendPacket(clientid, Serialize(new Packet_Server() { Id = Packet_ServerIdEnum.Season, Season = p }));
    }

    private GameTime _time = new GameTime();
    private int _nLastHourChangeNotify = 0;

    public void ProcessMain()
    {
        if (server.mainSockets == null)
        {
            return;
        }

        if (_time.Tick())
        {
            if (_time.GetQuarterHourPartOfDay() != _nLastHourChangeNotify)
            {
//#if DEBUG
//                SendMessageToAll("Time of day: " + _time.Time.ToString(@"hh\:mm\:ss") + " Day: " + (int)_time.Time.Days);
//#endif
                //notify clients about the time
                _nLastHourChangeNotify = _time.GetQuarterHourPartOfDay();

                foreach (var c in clients)
                {
                    NotifySeason(c.Key);
                }
            }
        }

        double currenttime = gettime() - starttime;
        double deltaTime = currenttime - oldtime;
        accumulator += deltaTime;
        double dt = SIMULATION_STEP_LENGTH;
        while (accumulator > dt)
        {
            simulationcurrentframe++;
            accumulator -= dt;
        }
        oldtime = currenttime;

        NetIncomingMessage msg;
        Stopwatch s = new Stopwatch();
        s.Start();

        //Process client packets
        for (int i = 0; i < mainSocketsCount; i++)
        {
            NetServer mainSocket = mainSockets[i];
            if (mainSocket == null)
            {
                continue;
            }
            while ((msg = mainSocket.ReadMessage()) != null)
            {
                ProcessNetMessage(msg, mainSocket, s);
            }
        }
        foreach (var k in clients)
        {
            k.Value.socket.Update();
        }

        //Send updates to player
        foreach (var k in clients)
        {
            //k.Value.notifyMapTimer.Update(delegate { NotifyMapChunks(k.Key, 1); });
            NotifyInventory(k.Key);
            NotifyPlayerStats(k.Key);
        }

        //Process Mod timers
        foreach (var k in timers)
        {
            k.Key.Update(k.Value);
        }

        //Reset data displayed in /stat
        if ((DateTime.UtcNow - statsupdate).TotalSeconds >= 2)
        {
            statsupdate = DateTime.UtcNow;
            StatTotalPackets = 0;
            StatTotalPacketsLength = 0;
        }

        //Determine how long it took all operations to finish
        lastServerTick = s.ElapsedMilliseconds;
        if (lastServerTick > 500)
        {
            //Print an error if the value gets too big - TODO: Adjust
            Console.WriteLine("Server process takes too long! Overloaded? ({0}ms)", lastServerTick);
        }
    }

    public void OnConfigLoaded()
    {
        //Initialize server map
        var map = new ServerMap();
        map.server = this;
        map.d_CurrentTime = this;
        map.chunksize = 32;
        for (int i = 0; i < BlockTypes.Length; i++)
        {
            BlockTypes[i] = new BlockType() { };
        }
        map.d_Heightmap = new InfiniteMapChunked2dServer() { chunksize = Server.chunksize, d_Map = map };
        map.Reset(config.MapSizeX, config.MapSizeY, config.MapSizeZ);
        d_Map = map;

        //Load assets (textures, sounds, etc.)
        string[] datapaths = new[] { Path.Combine(Path.Combine(Path.Combine("..", ".."), ".."), "data"), "data" };
        string[] datapathspublic = new[] { Path.Combine(datapaths[0], "public"), Path.Combine(datapaths[1], "public") };
        PublicDataPaths = datapathspublic;
        assetLoader = new AssetLoader(datapathspublic);
        LoadAssets();
        var getfile = new GetFileStream(datapaths);
        d_GetFile = getfile;

        //Initialize game components
        var data = new GameData();
        data.Start();
        d_Data = data;
        d_CraftingTableTool = new CraftingTableTool() { d_Map = map, d_Data = data };
        LocalConnectionsOnly = !Public;
        var networkcompression = new CompressionGzip();
        var diskcompression = new CompressionGzip();
        var chunkdb = new ChunkDbCompressed() { d_ChunkDb = new ChunkDbSqlite(), d_Compression = diskcompression };
        d_ChunkDb = chunkdb;
        map.d_ChunkDb = chunkdb;
        d_NetworkCompression = networkcompression;
        d_DataItems = new GameDataItemsBlocks() { d_Data = data };
        if (server.mainSockets == null)
        {
            int count;
            server.mainSockets = NetworkBackendFactory.CreateAll(NetworkBackendFactory.DefaultConfigs, out count);
            server.mainSocketsCount = count;
        }

        all_privileges.AddRange(ServerClientMisc.Privilege.All());

        //Load the savegame file
        {
            if (!Directory.Exists(GameStorePath.gamepathsaves))
            {
                Directory.CreateDirectory(GameStorePath.gamepathsaves);
            }
            Console.WriteLine(language.ServerLoadingSavegame());
            if (!File.Exists(GetSaveFilename()))
            {
                Console.WriteLine(language.ServerCreatingSavegame());
            }
            LoadGame(GetSaveFilename());
            Console.WriteLine(language.ServerLoadedSavegame() + GetSaveFilename());
        }
        if (LocalConnectionsOnly)
        {
            config.Port = singleplayerport;
        }
        Start(config.Port);

        // server monitor
        if (config.ServerMonitor)
        {
            this.serverMonitor = new ServerMonitor(this, exit);
            this.serverMonitor.Start();
        }

        // set up server console interpreter
        this.serverConsoleClient = new ClientOnServer()
        {
            Id = serverConsoleId,
            playername = "Server",
            queryClient = false
        };
        ManicDigger.Group serverGroup = new ManicDigger.Group();
        serverGroup.Name = "Server";
        serverGroup.Level = 255;
        serverGroup.GroupPrivileges = new List<string>();
        serverGroup.GroupPrivileges = all_privileges;
        serverGroup.GroupColor = ServerClientMisc.ClientColor.Red;
        serverConsoleClient.AssignGroup(serverGroup);
        serverConsole = new ServerConsole(this, exit);

        if (config.AutoRestartCycle > 0)
        {
            Console.WriteLine("AutoRestartInterval: {0}", config.AutoRestartCycle);
        }
        else
        {
            Console.WriteLine("AutoRestartInterval: DISABLED");
        }
        serverUptime.Start();
    }
    void Start(int port)
    {
        Port = port;
        NetworkBackendFactory.StartAll(mainSockets, NetworkBackendFactory.DefaultConfigs, port);
    }
    internal int Port;
    public void Stop()
    {
        Console.WriteLine("[SERVER] Doing last tick...");
        ProcessMain();
        Console.WriteLine("[SERVER] Saving data...");
        DateTime start = DateTime.UtcNow;
        SaveGlobalData();
        Console.WriteLine(language.ServerGameSaved(), (DateTime.UtcNow - start));
        Console.WriteLine("[SERVER] Stopped the server!");
    }
    public void Restart()
    {
        //Server shall exit and be restarted
        exit.SetRestart(true);
        exit.SetExit(true);
    }
    public void Exit()
    {
        //Server shall be shutdown
        exit.SetRestart(false);
        exit.SetExit(true);
    }

    public List<string> all_privileges = new List<string>();

    public List<string> ModPaths = new List<string>();
    public ModManager1 modManager;

    internal string gameMode = "Fortress";

    private ServerMonitor serverMonitor;
    private ServerConsole serverConsole;
    private int serverConsoleId = -1; // make sure that not a regular client is assigned this ID
    public int ServerConsoleId { get { return serverConsoleId; } }
    internal ClientOnServer serverConsoleClient;
    public void ReceiveServerConsole(string message)
    {
        // empty message
        if (string.IsNullOrEmpty(message))
        {
            //Ignore empty messages
            return;
        }
        // server command
        if (message.StartsWith("/"))
        {
            string[] ss = message.Split(new[] { ' ' });
            string command = ss[0].Replace("/", "");
            string argument = message.IndexOf(" ") < 0 ? "" : message.Substring(message.IndexOf(" ") + 1);
            this.CommandInterpreter(serverConsoleId, command, argument);
            return;
        }
        // client command
        if (message.StartsWith("."))
        {
            return;
        }
        // chat message
        SendMessageToAll(string.Format("{0}: {1}", serverConsoleClient.ColoredPlayername(colorNormal), message));
        ChatLog(string.Format("{0}: {1}", serverConsoleClient.playername, message));
    }

    public int Seed;
    private void LoadGame(string filename)
    {
        d_ChunkDb.Open(filename);
        byte[] globaldata = d_ChunkDb.GetGlobalData();
        if (globaldata == null)
        {
            //no savegame yet
            if (config.RandomSeed)
            {
                Seed = new Random().Next();
            }
            else
            {
                Seed = config.Seed;
            }
            MemoryStream ms = new MemoryStream();
            SaveGame(ms);
            d_ChunkDb.SetGlobalData(ms.ToArray());
            this._time.Init(TimeSpan.Parse("08:00").Ticks);
            return;
        }
        SwordAndStoneSave save = Serializer.Deserialize<SwordAndStoneSave>(new MemoryStream(globaldata));
        Seed = save.Seed;
        d_Map.Reset(d_Map.MapSizeX, d_Map.MapSizeY, d_Map.MapSizeZ);
        if (config.IsCreative) this.Inventory = Inventory = new Dictionary<string, PacketServerInventory>(StringComparer.InvariantCultureIgnoreCase);
        else this.Inventory = save.Inventory;
        this.PlayerStats = save.PlayerStats;
        this.simulationcurrentframe = (int)save.SimulationCurrentFrame;
        this._time.Init(save.TimeOfDay);
        this.LastMonsterId = save.LastMonsterId;
        this.moddata = save.moddata;
    }
    public List<ManicDigger.Action> onload = new List<ManicDigger.Action>();
    public List<ManicDigger.Action> onsave = new List<ManicDigger.Action>();
    public int LastMonsterId;
    public Dictionary<string, PacketServerInventory> Inventory = new Dictionary<string, PacketServerInventory>(StringComparer.InvariantCultureIgnoreCase);
    public Dictionary<string, PacketServerPlayerStats> PlayerStats = new Dictionary<string, PacketServerPlayerStats>(StringComparer.InvariantCultureIgnoreCase);
    public void SaveGame(Stream s)
    {
        for (int i = 0; i < onsave.Count; i++)
        {
            onsave[i]();
        }
        SwordAndStoneSave save = new SwordAndStoneSave();
        SaveAllLoadedChunks();
        if (!config.IsCreative)
        {
            save.Inventory = Inventory;
        }
        save.PlayerStats = PlayerStats;
        save.Seed = Seed;
        save.SimulationCurrentFrame = simulationcurrentframe;
        save.TimeOfDay = _time.Time.Ticks;
        save.LastMonsterId = LastMonsterId;
        save.moddata = moddata;
        Serializer.Serialize(s, save);
    }
    public Dictionary<string, byte[]> moddata = new Dictionary<string, byte[]>();
    public bool BackupDatabase(string backupFilename)
    {
        if (!GameStorePath.IsValidName(backupFilename))
        {
            Console.WriteLine(language.ServerInvalidBackupName() + backupFilename);
            return false;
        }
        if (!Directory.Exists(GameStorePath.gamepathbackup))
        {
            Directory.CreateDirectory(GameStorePath.gamepathbackup);
        }
        string finalFilename = Path.Combine(GameStorePath.gamepathbackup, backupFilename + MapManipulator.BinSaveExtension);
        d_ChunkDb.Backup(finalFilename);
        return true;
    }
    public bool LoadDatabase(string filename)
    {
        d_Map.d_ChunkDb = d_ChunkDb;
        SaveAll();
        if (filename != GetSaveFilename())
        {
            //TODO: load
        }
        var dbcompressed = (ChunkDbCompressed)d_Map.d_ChunkDb;
        var db = (ChunkDbSqlite)dbcompressed.d_ChunkDb;
        db.temporaryChunks = new Dictionary<ulong, byte[]>();
        d_Map.Clear();
        LoadGame(filename);
        foreach (var k in clients)
        {
            //SendLevelInitialize(k.Key);
            Array.Clear(k.Value.chunksseen, 0, k.Value.chunksseen.Length);
            k.Value.chunksseenTime.Clear();
        }
        return true;
    }
    private void SaveAllLoadedChunks()
    {
        List<DbChunk> tosave = new List<DbChunk>();
        for (int cx = 0; cx < d_Map.MapSizeX / chunksize; cx++)
        {
            for (int cy = 0; cy < d_Map.MapSizeY / chunksize; cy++)
            {
                for (int cz = 0; cz < d_Map.MapSizeZ / chunksize; cz++)
                {
                    ServerChunk c = d_Map.GetChunkValid(cx, cy, cz);
                    if (c == null)
                    {
                        continue;
                    }
                    if (!c.DirtyForSaving)
                    {
                        continue;
                    }
                    c.DirtyForSaving = false;
                    MemoryStream ms = new MemoryStream();
                    Serializer.Serialize(ms, c);
                    tosave.Add(new DbChunk() { Position = new Xyz() { X = cx, Y = cy, Z = cz }, Chunk = ms.ToArray() });
                    if (tosave.Count > 200)
                    {
                        d_ChunkDb.SetChunks(tosave);
                        tosave.Clear();
                    }
                }
            }
        }
        d_ChunkDb.SetChunks(tosave);
    }

    public string SaveFilenameWithoutExtension = "default";
    public string SaveFilenameOverride;
    public string GetSaveFilename()
    {
        if (SaveFilenameOverride != null)
        {
            return SaveFilenameOverride;
        }
        return Path.Combine(GameStorePath.gamepathsaves, SaveFilenameWithoutExtension + MapManipulator.BinSaveExtension);
    }

    private void SaveGlobalData()
    {
        MemoryStream ms = new MemoryStream();
        SaveGame(ms);
        d_ChunkDb.SetGlobalData(ms.ToArray());
    }
    DateTime lastsave = DateTime.UtcNow;

    public ServerConfig config;
    public ServerBanlist banlist;
    public bool configNeedsSaving;

    public void Dispose()
    {
        if (!disposed)
        {
            //d_MainSocket.Disconnect(false);
        }
        disposed = true;
    }
    bool disposed = false;
    double starttime = gettime();
    static double gettime()
    {
        return (double)DateTime.UtcNow.Ticks / (10 * 1000 * 1000);
    }
    internal int simulationcurrentframe;
    double oldtime;
    double accumulator;
    float lastServerTick;

    private int lastClientId;

    private void ProcessNetMessage(NetIncomingMessage msg, NetServer mainSocket, Stopwatch s)
    {
        if (msg.SenderConnection == null)
        {
            return;
        }
        int clientid = -1;
        foreach (var k in clients)
        {
            if (k.Value.mainSocket != mainSocket)
            {
                continue;
            }
            if (k.Value.socket.EqualsConnection(msg.SenderConnection))
            {
                clientid = k.Key;
            }
        }
        switch (msg.Type)
        {
            case NetworkMessageType.Connect:
                //new connection
                //ISocket client1 = d_MainSocket.Accept();
                NetConnection client1 = msg.SenderConnection;
                IPEndPointCi iep1 = client1.RemoteEndPoint();

                ClientOnServer c = new ClientOnServer();
                c.mainSocket = mainSocket;
                c.socket = client1;
                c.Ping.SetTimeoutValue(config.ClientConnectionTimeout);
                c.chunksseen = new bool[d_Map.MapSizeX / chunksize * d_Map.MapSizeY / chunksize * d_Map.MapSizeZ / chunksize];
                lock (clients)
                {
                    this.lastClientId = this.GenerateClientId();
                    c.Id = lastClientId;
                    clients[lastClientId] = c;
                }
                //clientid = c.Id;
                c.notifyMapTimer = new Timer()
                {
                    INTERVAL = 1.0 / SEND_CHUNKS_PER_SECOND,
                };
                c.notifyMonstersTimer = new Timer()
                {
                    INTERVAL = 1.0 / SEND_MONSTER_UDAPTES_PER_SECOND,
                };
                break;
            case NetworkMessageType.Data:
                if (clientid == -1)
                {
                    break;
                }

                // process packet
                try
                {
                    TotalReceivedBytes += msg.messageLength;
                    TryReadPacket(clientid, msg.message);
                }
                catch (Exception e)
                {
                    //client problem. disconnect client.
                    Console.WriteLine("Exception at client " + clientid + ". Disconnecting client.");
                    SendPacket(clientid, ServerPackets.DisconnectPlayer(language.ServerClientException()));
                    KillPlayer(clientid);
                    Console.WriteLine(e.ToString());
                }
                break;
            case NetworkMessageType.Disconnect:
                Console.WriteLine("Client disconnected.");
                KillPlayer(clientid);
                break;
        }
    }

    DateTime statsupdate;

    public Dictionary<Timer, Timer.Tick> timers = new Dictionary<Timer, Timer.Tick>();



    //on exit
    public void SaveAll()
    {
        for (int x = 0; x < d_Map.MapSizeX / chunksize; x++)
        {
            for (int y = 0; y < d_Map.MapSizeY / chunksize; y++)
            {
                for (int z = 0; z < d_Map.MapSizeZ / chunksize; z++)
                {
                    if (d_Map.GetChunkValid(x, y, z) != null)
                    {
                        DoSaveChunk(x, y, z, d_Map.GetChunkValid(x, y, z));
                    }
                }
            }
        }
        SaveGlobalData();
    }

    internal void DoSaveChunk(int x, int y, int z, ServerChunk c)
    {
        MemoryStream ms = new MemoryStream();
        Serializer.Serialize(ms, c);
        ChunkDb.SetChunk(d_ChunkDb, x, y, z, ms.ToArray());
    }

    int SEND_CHUNKS_PER_SECOND = 10;
    int SEND_MONSTER_UDAPTES_PER_SECOND = 3;

    public void LoadChunk(int cx, int cy, int cz)
    {
        d_Map.LoadChunk(cx, cy, cz);
    }

}
}
