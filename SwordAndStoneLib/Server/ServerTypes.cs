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

public class ModEventHandlers
{
    public List<ModDelegates.WorldGenerator> getchunk = new List<ModDelegates.WorldGenerator>();
    public List<ModDelegates.BlockUse> onuse = new List<ModDelegates.BlockUse>();
    public List<ModDelegates.BlockBuild> onbuild = new List<ModDelegates.BlockBuild>();
    public List<ModDelegates.BlockDelete> ondelete = new List<ModDelegates.BlockDelete>();
    public List<ModDelegates.BlockUseWithTool> onusewithtool = new List<ModDelegates.BlockUseWithTool>();
    public List<ModDelegates.ChangedActiveMaterialSlot> changedactivematerialslot = new List<ModDelegates.ChangedActiveMaterialSlot>();
    public List<ModDelegates.BlockUpdate> blockticks = new List<ModDelegates.BlockUpdate>();
    public List<ModDelegates.PopulateChunk> populatechunk = new List<ModDelegates.PopulateChunk>();
    public List<ModDelegates.Command> oncommand = new List<ModDelegates.Command>();
    public List<ModDelegates.WeaponShot> onweaponshot = new List<ModDelegates.WeaponShot>();
    public List<ModDelegates.WeaponHit> onweaponhit = new List<ModDelegates.WeaponHit>();
    public List<ModDelegates.SpecialKey1> onspecialkey = new List<ModDelegates.SpecialKey1>();
    public List<ModDelegates.PlayerJoin> onplayerjoin = new List<ModDelegates.PlayerJoin>();
    public List<ModDelegates.PlayerLeave> onplayerleave = new List<ModDelegates.PlayerLeave>();
    public List<ModDelegates.PlayerDisconnect> onplayerdisconnect = new List<ModDelegates.PlayerDisconnect>();
    public List<ModDelegates.PlayerChat> onplayerchat = new List<ModDelegates.PlayerChat>();
    public List<ModDelegates.PlayerDeath> onplayerdeath = new List<ModDelegates.PlayerDeath>();
    public List<ModDelegates.DialogClick> ondialogclick = new List<ModDelegates.DialogClick>();
    public List<ModDelegates.DialogClick2> ondialogclick2 = new List<ModDelegates.DialogClick2>();
    public List<ModDelegates.LoadWorld> onloadworld = new List<ModDelegates.LoadWorld>();
    public List<ModDelegates.EntityUpdate> onentityupdate = new List<ModDelegates.EntityUpdate>();
    public List<ModDelegates.EntityUse> onentityuse = new List<ModDelegates.EntityUse>();
    public List<ModDelegates.EntityHit> onentityhit = new List<ModDelegates.EntityHit>();
    public List<ModDelegates.Permission> onpermission = new List<ModDelegates.Permission>();
    public List<ModDelegates.CheckBlockUse> checkonuse = new List<ModDelegates.CheckBlockUse>();
    public List<ModDelegates.CheckBlockBuild> checkonbuild = new List<ModDelegates.CheckBlockBuild>();
    public List<ModDelegates.CheckBlockDelete> checkondelete = new List<ModDelegates.CheckBlockDelete>();
}

#region GameTime
public class GameTime
{
    private int _nIngameSecondsEveryRealTimeSecond = 60;
    private int _nDaysPerYear = 4;
    private long _nLastIngameSecond = 0;

    private Stopwatch _watchIngameTime = null;
    private TimeSpan _time = TimeSpan.Zero;

    /// <summary>
    /// This changes how fast time goes on
    /// 0 freezes the time
    /// can be negative to let time run backwards
    /// </summary>
    public int SpeedOfTime
    {
        get { return _nIngameSecondsEveryRealTimeSecond; }
        set { _nIngameSecondsEveryRealTimeSecond = value; }
    }

    /// <summary>
    /// Days of a year
    /// </summary>
    public int DaysPerYear
    {
        get { return _nDaysPerYear; }
        set { _nDaysPerYear = value; }
    }

    /// <summary>
    /// ctor
    /// </summary>
    internal GameTime()
    {
        _watchIngameTime = new Stopwatch();
    }

    /// <summary>
    /// Initializes the GameTime component
    /// </summary>
    /// <param name="ticks"></param>
    internal void Init(long ticks)
    {
        _time = TimeSpan.FromTicks(ticks);
        _watchIngameTime.Start();
    }

    /// <summary>
    /// Allows the time to tick
    /// </summary>
    internal void Start()
    {
        _watchIngameTime.Start();
    }

    /// <summary>
    /// Stops the time from ticking
    /// </summary>
    internal void Stop()
    {
        _watchIngameTime.Stop();
    }

    /// <summary>
    /// returns the time
    /// </summary>
    public TimeSpan Time
    {
        get { return _time; }
    }

    /// <summary>
    /// Hour of the day
    /// </summary>
    public int Hour
    {
        get { return _time.Hours; }
    }

    /// <summary>
    /// Total hours
    /// </summary>
    public double HourTotal
    {
        get { return _time.TotalHours; }
    }

    /// <summary>
    /// Day of the year
    /// </summary>
    public int Day
    {
        get { return (int)(_time.TotalDays % _nDaysPerYear); }
    }

    /// <summary>
    /// Total days
    /// </summary>
    public double DaysTotal
    {
        get { return _time.TotalDays; }
    }

    /// <summary>
    /// The current year
    /// </summary>
    public int Year
    {
        get { return (int)(_time.TotalDays / _nDaysPerYear); }
    }

    /// <summary>
    /// Gets the current season
    /// from 0 to 3
    /// </summary>
    public int Season
    {
        get { return Year % 4; }
    }

    /// <summary>
    /// Returns the amount of 15 mins that passed for this day
    /// </summary>
    /// <returns></returns>
    public int GetQuarterHourPartOfDay()
    {
        //(_tsIngameTime.Hours * 4)
        //for every hour of the current day, we got 4 x 15minutes

        //(_tsIngameTime.Minutes / 15)
        //add the 15 minutes of the current hour

        //TODO: +1 at the end beacause midnight causes daylight :/
        int nReturn = (_time.Hours * 4) + (_time.Minutes / 15) + 1;
        return nReturn;
    }

    /// <summary>
    /// Ticks the clock if neccesarry.
    /// Returns true if time changed
    /// </summary>
    /// <returns></returns>
    internal bool Tick()
    {
        bool blnTicked = false;

        //update the time of day every second
        while (_nLastIngameSecond < (_watchIngameTime.ElapsedMilliseconds / 1000))
        {
            //Update gametime
            _time += TimeSpan.FromSeconds(_nIngameSecondsEveryRealTimeSecond);

            ++_nLastIngameSecond;

            blnTicked = true;
        }

        return blnTicked;
    }

    /// <summary>
    /// change the time of the day
    /// </summary>
    internal void Set(TimeSpan time)
    {
        _time = time;
    }

    /// <summary>
    /// Adds the given time
    /// </summary>
    /// <param name="time"></param>
    internal void Add(TimeSpan time)
    {
        _time += time;
    }
}
#endregion

public class BlockTypeConverter
{
    public static Packet_BlockType GetBlockType(BlockType block)
    {
        Packet_BlockType p = new Packet_BlockType();
        p.AimRadiusFloat = Server.SerializeFloat(block.AimRadius);
        p.AmmoMagazine = block.AmmoMagazine;
        p.AmmoTotal = block.AmmoTotal;
        p.BulletsPerShotFloat = Server.SerializeFloat(block.BulletsPerShot);
        p.DamageBodyFloat = Server.SerializeFloat(block.DamageBody);
        p.DamageHeadFloat = Server.SerializeFloat(block.DamageHead);
        p.DamageToPlayer = block.DamageToPlayer;
        p.DelayFloat = Server.SerializeFloat(block.Delay);
        p.DrawType = (int)block.DrawType;
        p.ExplosionRangeFloat = Server.SerializeFloat(block.ExplosionRange);
        p.ExplosionTimeFloat = Server.SerializeFloat(block.ExplosionTime);
        p.Handimage = block.handimage;
        p.IronSightsAimRadiusFloat = Server.SerializeFloat(block.IronSightsAimRadius);
        p.IronSightsEnabled = block.IronSightsEnabled;
        p.IronSightsFovFloat = Server.SerializeFloat(block.IronSightsFov);
        p.IronSightsImage = block.IronSightsImage;
        p.IronSightsMoveSpeedFloat = Server.SerializeFloat(block.IronSightsMoveSpeed);
        p.IsBuildable = block.IsBuildable;
        p.IsPistol = block.IsPistol;
        p.IsSlipperyWalk = block.IsSlipperyWalk;
        p.IsTool = block.IsTool;
        p.IsUsable = block.IsUsable;
        p.LightRadius = block.LightRadius;
        p.Name = block.Name;
        p.PistolType = (int)block.PistolType;
        p.ProjectileBounce = block.ProjectileBounce;
        p.ProjectileSpeedFloat = Server.SerializeFloat(block.ProjectileSpeed);
        p.Rail = block.Rail;
        p.RecoilFloat = Server.SerializeFloat(block.Recoil);
        p.ReloadDelayFloat = Server.SerializeFloat(block.ReloadDelay);
        p.Sounds = GetSoundSet(block.Sounds);
        p.StartInventoryAmount = block.StartInventoryAmount;
        p.Strength = block.Strength;
        p.TextureIdBack = block.TextureIdBack;
        p.TextureIdBottom = block.TextureIdBottom;
        p.TextureIdForInventory = block.TextureIdForInventory;
        p.TextureIdFront = block.TextureIdFront;
        p.TextureIdLeft = block.TextureIdLeft;
        p.TextureIdRight = block.TextureIdRight;
        p.TextureIdTop = block.TextureIdTop;
        p.WalkableType = (int)block.WalkableType;
        p.WalkSpeedFloat = Server.SerializeFloat(block.WalkSpeed);
        p.WalkSpeedWhenUsedFloat = Server.SerializeFloat(block.WalkSpeedWhenUsed);
        p.WhenPlacedGetsConvertedTo = block.WhenPlayerPlacesGetsConvertedTo;
        p.PickDistanceWhenUsedFloat = Server.SerializeFloat(block.PickDistanceWhenUsed);
        return p;
    }

    private static Packet_SoundSet GetSoundSet(SoundSet soundSet)
    {
        if (soundSet == null)
        {
            return null;
        }
        Packet_SoundSet p = new Packet_SoundSet();
        p.SetBreak1(soundSet.Break, soundSet.Break.Length, soundSet.Break.Length);
        p.SetBuild(soundSet.Build, soundSet.Build.Length, soundSet.Build.Length);
        p.SetClone(soundSet.Clone, soundSet.Clone.Length, soundSet.Clone.Length);
        p.SetReload(soundSet.Reload, soundSet.Reload.Length, soundSet.Reload.Length);
        p.SetShoot(soundSet.Shoot, soundSet.Shoot.Length, soundSet.Shoot.Length);
        p.SetShootEnd(soundSet.ShootEnd, soundSet.ShootEnd.Length, soundSet.ShootEnd.Length);
        p.SetWalk(soundSet.Walk, soundSet.Walk.Length, soundSet.Walk.Length);
        return p;
    }
}

public class MyLinq
{
    public static bool Any<T>(IEnumerable<T> l)
    {
        return l.GetEnumerator().MoveNext();
    }
    public static T First<T>(IEnumerable<T> l)
    {
        var e = l.GetEnumerator();
        e.MoveNext();
        return e.Current;
    }
    public static int Count<T>(IEnumerable<T> l)
    {
        int count = 0;
        foreach (T v in l)
        {
            count++;
        }
        return count;
    }
    public static IEnumerable<T> Take<T>(IEnumerable<T> l, int n)
    {
        int i = 0;
        foreach (var v in l)
        {
            if (i >= n)
            {
                yield break;
            }
            yield return v;
            i++;
        }
    }
    public static IEnumerable<T> Skip<T>(IEnumerable<T> l, int n)
    {
        var iterator = l.GetEnumerator();
        for (int i = 0; i < n; i++)
        {
            if (iterator.MoveNext() == false)
                yield break;
        }
        while (iterator.MoveNext())
            yield return iterator.Current;
    }
}
public interface ICurrentTime
{
    int GetSimulationCurrentFrame();
}
public class CurrentTimeDummy : ICurrentTime
{
    public int GetSimulationCurrentFrame() { return 0; }
}
public static class MapUtil
{
    public static int Index2d(int x, int y, int sizex)
    {
        return x + y * sizex;
    }

    public static int Index3d(int x, int y, int h, int sizex, int sizey)
    {
        return (h * sizey + y) * sizex + x;
    }

    public static Vector3i Pos(int index, int sizex, int sizey)
    {
        int x = index % sizex;
        int y = (index / sizex) % sizey;
        int h = index / (sizex * sizey);
        return new Vector3i(x, y, h);
    }

    public static bool IsValidPos(IMapStorage2 map, int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0)
        {
            return false;
        }
        if (x >= map.GetMapSizeX() || y >= map.GetMapSizeY() || z >= map.GetMapSizeZ())
        {
            return false;
        }
        return true;
    }

    public static bool IsValidPos(IMapStorage2 map, int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }
        if (x >= map.GetMapSizeX() || y >= map.GetMapSizeY())
        {
            return false;
        }
        return true;
    }

    public static bool IsValidChunkPos(IMapStorage2 map, int cx, int cy, int cz, int chunksize)
    {
        return cx >= 0 && cy >= 0 && cz >= 0
            && cx < map.GetMapSizeX() / chunksize
            && cy < map.GetMapSizeY() / chunksize
            && cz < map.GetMapSizeZ() / chunksize;
    }

    public static int blockheight(IMapStorage2 map, int tileidempty, int x, int y)
    {
        for (int z = map.GetMapSizeZ() - 1; z >= 0; z--)
        {
            if (map.GetBlock(x, y, z) != tileidempty)
            {
                return z + 1;
            }
        }
        return map.GetMapSizeZ() / 2;
    }

    static ulong pow20minus1 = 1048576 - 1;
    public static Vector3i FromMapPos(ulong v)
    {
        uint z = (uint)(v & pow20minus1);
        v = v >> 20;
        uint y = (uint)(v & pow20minus1);
        v = v >> 20;
        uint x = (uint)(v & pow20minus1);
        return new Vector3i((int)x, (int)y, (int)z);
    }

    public static ulong ToMapPos(int x, int y, int z)
    {
        ulong v = 0;
        v = (ulong)x << 40;
        v |= (ulong)y << 20;
        v |= (ulong)(uint)z;
        return v;
    }

    public static int SearchColumn(IMapStorage2 map, int x, int y, int id, int startH)
    {
        for (int h = startH; h > 0; h--)
        {
            if (map.GetBlock(x, y, h) == (byte)id)
            {
                return h;
            }
        }
        return -1; // -1 means 'not found'
    }

    public static int SearchColumn(IMapStorage2 map, int x, int y, int id)
    {
        return SearchColumn(map, x, y, id, map.GetMapSizeZ() - 1);
    }

    public static bool IsSolidChunk(ushort[] chunk)
    {
        for (int i = 0; i <= chunk.GetUpperBound(0); i++)
        {
            if (chunk[i] != chunk[0])
            {
                return false;
            }
        }
        return true;
    }

    public static Point PlayerArea(int playerAreaSize, int centerAreaSize, Vector3i blockPosition)
    {
        Point p = PlayerCenterArea(playerAreaSize, centerAreaSize, blockPosition);
        int x = p.X + centerAreaSize / 2;
        int y = p.Y + centerAreaSize / 2;
        x -= playerAreaSize / 2;
        y -= playerAreaSize / 2;
        return new Point(x, y);
    }

    public static Point PlayerCenterArea(int playerAreaSize, int centerAreaSize, Vector3i blockPosition)
    {
        int px = blockPosition.x;
        int py = blockPosition.y;
        int gridposx = (px / centerAreaSize) * centerAreaSize;
        int gridposy = (py / centerAreaSize) * centerAreaSize;
        return new Point(gridposx, gridposy);
    }
}
public class MapManipulator
{
    public const string BinSaveExtension = ".mddbs";
}
public class Timer
{
    public double INTERVAL { get { return interval; } set { interval = value; } }
    public double MaxDeltaTime { get { return maxDeltaTime; } set { maxDeltaTime = value; } }
    double interval = 1;
    double maxDeltaTime = double.PositiveInfinity;

    double starttime;
    double oldtime;
    public double accumulator;
    public Timer()
    {
        Reset();
    }
    public void Reset()
    {
        starttime = gettime();
    }
    public delegate void Tick();
    public void Update(Tick tick)
    {
        double currenttime = gettime() - starttime;
        double deltaTime = currenttime - oldtime;
        accumulator += deltaTime;
        double dt = INTERVAL;
        if (MaxDeltaTime != double.PositiveInfinity && accumulator > MaxDeltaTime)
        {
            accumulator = MaxDeltaTime;
        }
        while (accumulator >= dt)
        {
            tick();
            accumulator -= dt;
        }
        oldtime = currenttime;
    }
    static double gettime()
    {
        return (double)DateTime.UtcNow.Ticks / (10 * 1000 * 1000);
    }
}
public struct Vector2i
{
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
    public override bool Equals(object obj)
    {
        if (obj is Vector2i)
        {
            Vector2i other = (Vector2i)obj;
            return this.x == other.x && this.y == other.y;
        }
        return base.Equals(obj);
    }
    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !(a.x == b.x && a.y == b.y);
    }
    public override int GetHashCode()
    {
        int hash = 23;
        unchecked
        {
            hash = hash * 37 + x;
            hash = hash * 37 + y;
        }
        return hash;
    }
    public override string ToString()
    {
        return string.Format("[{0}, {1}]", x, y);
    }
}
public struct Vector3i
{
    public int x;
    public int y;
    public int z;

    public Vector3i(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3i Add(Vector3i v)
    {
        return Add(v.x, v.y, v.z);
    }

    public Vector3i Add(int x, int y, int z)
    {
        return new Vector3i(this.x + x, this.y + y, this.z + z);
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector3i)
        {
            Vector3i other = (Vector3i)obj;
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }
        return base.Equals(obj);
    }
    public static bool operator ==(Vector3i a, Vector3i b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    public static bool operator !=(Vector3i a, Vector3i b)
    {
        return !(a.x == b.x && a.y == b.y && a.z == b.z);
    }
    public override int GetHashCode()
    {
        int hash = 23;
        unchecked
        {
            hash = hash * 37 + x;
            hash = hash * 37 + y;
            hash = hash * 37 + z;
        }
        return hash;
    }
    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }
}
public struct Vector3f
{
	public float X;
	public float Y;
	public float Z;

	public Vector3f(float x, float y, float z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public Vector3f Add(Vector3f v)
	{
		return Add(v.X, v.Y, v.Z);
	}

	public Vector3f Add(float x, float y, float z)
	{
		return new Vector3f(this.X + x, this.Y + y, this.Z + z);
	}

	public override bool Equals(object obj)
	{
		if (obj is Vector3f)
		{
			Vector3f other = (Vector3f)obj;
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
		}
		return base.Equals(obj);
	}
	public void Normalize()
	{
		float len = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
		if (len <= 0) { return; }
		X /= len;
		Y /= len;
		Z /= len;
	}
	public static bool operator ==(Vector3f a, Vector3f b)
	{
		return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
	}
	public static bool operator !=(Vector3f a, Vector3f b)
	{
		return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
	}
	public override int GetHashCode()
	{
		int hash = 23;
		unchecked
		{
			hash = hash * 37 + X.GetHashCode();
			hash = hash * 37 + Y.GetHashCode();
			hash = hash * 37 + Z.GetHashCode();
		}
		return hash;
	}
	public static Vector3f operator +(Vector3f a, Vector3f b)
	{
		return new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}
	public static Vector3f operator -(Vector3f a, Vector3f b)
	{
		return new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}
	public static Vector3f operator *(Vector3f a, float b)
	{
		return new Vector3f(a.X * b, a.Y * b, a.Z * b);
	}
	public override string ToString()
	{
		return string.Format("[{0}, {1}, {2}]", X, Y, Z);
	}
}

[ProtoContract()]
public class Ingredient
{
    [ProtoMember(1)]
    public int Type;
    [ProtoMember(2)]
    public int Amount;
}

[ProtoContract()]
public class CraftingRecipe
{
    [ProtoMember(1)]
    public Ingredient[] ingredients;
    [ProtoMember(2)]
    public Ingredient output;
}


public abstract class ServerSystem
{
    public ServerSystem()
    {
        one = 1;
    }
    internal float one;
    public virtual void Update(Server server, float dt) { }
    public virtual void OnRestart(Server server) { }
    public virtual bool OnCommand(Server server, int sourceClientId, string command, string argument) { return false; }
}

public abstract class ServerPlatform
{
    public abstract void QueueUserWorkItem(Action_ action);
    public abstract int FloatToInt(float value);
}

public class ServerPlatformNative : ServerPlatform
{
    public override void QueueUserWorkItem(Action_ action)
    {
        ThreadPool.QueueUserWorkItem((a) => { action.Run(); });
    }

    public override int FloatToInt(float value)
    {
        return (int)value;
    }
}
}