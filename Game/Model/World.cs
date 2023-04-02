using Cache;
using Cache.FS;
using Exceptions;
using Game.FS;
using Game.FS.Def;
using Game.Model.Attr;
using Game.Model.Collision;
using Game.Model.Entity;
using Game.Model.Instance;
using Game.Model.Priv;
using Game.Model.Region;
using Game.Model.Timer;
using Game.Service;
using Game.Service.Xtea;
using Game.Sync.Block;
using Util;

namespace Game.Model;

class World
{
	public const int REJECT_LOGIN_REBOOT_THRESHOLD = 50;
	
	public GameContext GameContext { get; private set; }
	public DevContext DevContext { get; private set; }
	public RLStore? FileStore;
	public DefinitionSet Definitions { get; private set; } = new DefinitionSet();
	private HuffmanCodec? _huffman;
	public HuffmanCodec Huffman {
		get {
			if (FileStore == null)
				throw new RuntimeException("Attempt to initialize HuffmanCodec with null FileStore");
			if (_huffman == null)
			{
				RLIndex? binary = FileStore.GetIndex(RLIndexType.BINARY);
				RLArchive? archive = binary!.FindArchiveByName("huffman");
				RLFSFile file = archive!.GetFiles(FileStore.Storage.LoadArchive(archive)!).Files[0];
				_huffman = new HuffmanCodec(file.Contents!);
			}
			return _huffman;
		}
	}
	public List<IService> Services { get; private set; } = new List<IService>();
	// public CoroutineDispatcher? CoroutineDispatcher;
	public QueueTaskSet Queues { get; private set; } = new WorldQueueTaskSet();
	public PawnList<Player> Players { get; private set; }
	public PawnList<Npc> Npcs { get; private set; }
	public ChunkSet Chunks { get; private set; }
	public CollisionManager Collision { get; private set; }
	public InstancedMapAllocator InstanceAllocator { get; private set; } = new InstancedMapAllocator();
	public PluginRepository Plugins { get; private set; }
	public PrivilegeSet Privileges { get; private set; } = new PrivilegeSet();
	public XTeaKeyService? XTeaKeyService;
	public UpdateBlockSet PlayerUpdateBlocks { get; private set; } = new UpdateBlockSet();
	public UpdateBlockSet NpcUpdateBlocks { get; private set; } = new UpdateBlockSet();
	public Random Random { get; private set; } = new Random();
	public int CurrentCycle = 0;
	public bool MultiThreadPathfinding = false;
	public TimerMap Timers { get; private set; } = new TimerMap();
	public AttributeMap Attributes { get; private set; } = new AttributeMap();
	public int RebootTimer = -1;
	
	private List<GroundItem> _groundItemQueues = new List<GroundItem>();
	private List<GroundItem> _groundItems = new List<GroundItem>();
	
	public World(GameContext gameContext, DevContext devContext)
	{
		GameContext = gameContext;
		DevContext = devContext;
		Players = new PawnList<Player>(new Player[gameContext.PlayerLimit]);
		Npcs = new PawnList<Npc>(new Npc[Int16.MaxValue]);
		Chunks = new ChunkSet(this);
		Collision = new CollisionManager(Chunks);
		Plugins = new PluginRepository(this);
	}
	
	// TODO: check this
	public void Init()
	{
		GameService? service = GetService<GameService>(typeof(GameService));
		if (service != null)
		{
			CoroutineDispatcher = service.Dispatcher;
		}
	}
	
	public void PostLoad()
	{
		Plugins.ExecuteWorldInit(this);
	}
	
	// TODO: fill this out
	public void Cycle()
	{
		if (CurrentCycle++ >= Int32.MaxValue - 1)
		{
			CurrentCycle = 0;
		}
		
		// TODO: I can't find out further due to the timers not being fileld out
	}
	
	public void SendRebootTimer(int cycles = RebootTimer)
	{
		foreach (Player p in Players)
		{
			p.Write(new UpdateRebootTimerMessage(cycles));
		}
	}
	
	public bool Register(Player p)
	{
		if (players.AddIfNotExist<Player>(p)) {
			p.LastIndex = p.Index;
			return true;
		}
		return false;
	}
	
	// TODO: nullable: Get(p.Tile)!
	public void Unregistered(Player p)
	{
		Players.Remove(p);
		Chunks.Get(p.Tile).RemoveEntity(this, p, p.Tile);
	}
	
	public bool Spawn(Npc npc)
	{
		bool added = Npcs.AddIfNotExist<Npc>(npc);
		if (added) {
			SetNpcDefaults(npc);
			Plugins.ExecuteNpcSpawn(npc);
		}
		return added;
	}
	
	// TODO: nullable: Get(p.Tile)!
	public void Remove(Npc npc)
	{
		Npcs.Remove(npc);
		Chunks.Get(npc.Tile).RemoveEntity(this, npc, npc.Tile);
	}
	
	public void Spawn(GameObject obj)
	{
		Tile tile = obj.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		
		GameObject? oldObj = chunk.GetEntities<GameObject>(tile, EntityType.STATIC_OBJECT, EntityType.DYNAMIC_OBJECT).FirstOrDefault((it) => it.Type == obj.Type);
		if (oldObj != null) {
			chunk.RemoveEntity(this, oldObj, tile);
		}
		chunk.AddEntity(this, obj, tile);
	}
	
	public void Remove(GameObject obj)
	{
		Tile tile = obj.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		chunk.RemoveEntity(this, obj, tile);
	}
	
	public void Spawn(GroundItem item)
	{
		Tile tile = item.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		ItemDef def = Definitions.Get(typeof(ItemDef), item.Item);
		if (def.IsStackable) {
			GroundItem oldItem = chunk.GetEntities<GroundItem>(tile, EntityType.GROUND_ITEM).FirstOrDefault((it) => it.Item == item.Item && it.OwnerUID == item.OwnerUID);
			if (oldItem != null) {
				int oldAmount = oldItem.Amount;
				int newAmount = Math.Min(int.MaxValue, item.Amount + oldItem.Amount);
				oldItem.amount = newAmount;
				chunk.UpdateGroundItems(this, item, oldAmount, newAmount);
			}
		}
		GroundItems.Add(item);
		chunk.AddEntity(this, item, tile);
	}
	
	public void Remove(GroundItem item)
	{
		Tile tile = item.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		
		GroundItems.Remove(item);
		chunk.RemoveEntity(this, item, tile);
		
		if (item.RespawnCycles > 0) {
			item.CurrentCycle = 0;
			GroundItemQueue.Add(item);
		}
	}
	
	public void Spawn(Projectile projectile)
	{
		Tile tile = projectile.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		
		chunk.AddEntity(this, projectile, tile);
	}
	
	public void Spawn(AreaSound sound)
	{
		Tile tile = sound.Tile;
		Chunk chunk = Chunks.GetOrCreate(tile);
		
		chunk.AddEntity(this, sound, tile);
	}
	
	public void Spawn(TileGraphic graphic)
	{
		Tile tile = graphic.Tile;
		Chunk chunk = Chunks.GetorCreate(tile);
		
		chunk.AddEntity(this, graphic, tile);
	}
	
	// TODO: implement this
	public void RemoveAll(Area area)
	{
		
	}
	
	public bool IsSpawned(GameObject obj)
	{
		return Chunks.GetOrCreate(obj.Tile).GetEntities<GameObject>(obj.Tile, EntityType.STATIC_OBJECT, EntityType.DYNAMIC_OBJECT).Contains(obj);
	}
	
	public bool IsSpawned(GroundItem item)
	{
		return Chunks.GetOrCreate(item.Tile).GetEntities<GroundItem>(item.Tile, EntityType.GROUND_ITEM).Contains(item);
	}
	
	// TODO: implement this
	public GroundItem? GetGroundItem()
	{
		
	}
	
	public GameObject? GetObject(Tile tile, int type)
	{
		return Chunks.Get(tile, true).GetEntities<GameObject>(tile, EntityType.STATIC_OBJECT, EntityType.DYNAMIC_OBJECT).FirstOrDefault((it) => it.Type == type);
	}
	
	// TODO: implement this
	public Player? GetPlayerForName(string username)
	{
		for (int i=0; i<Players.Capacity; i++)
		{
			
		}
	}
	
	public T? GetService<T>(Type type, bool searchSubclasses = false) where T : IService
	{
		if (searchSubclasses) {
			return (T?) Services.FirstOrDefault((serv) => type.IsAssignableFrom(serv!.GetType()), null);
		}
		return (T?) Services.FirstOrDefault((serv) => serv!.GetType() == type, null);
	}
	
	
}