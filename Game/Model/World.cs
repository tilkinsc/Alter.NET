using Cache.FS;
using Game.FS;
using Game.FS.Def;
using Game.Model.Attr;
using Game.Model.Collision;
using Game.Model.Entity;
using Game.Model.Region;
using Game.Model.Timer;
using Game.Service.Xtea;

namespace Game.Model;

class World
{
	
	public GameContext GameContext;
	public DevContext DevContext;
	public RLStore FileStore;
	public DefinitionSet Definitions;
	public List<Service> Services;
	public CoroutineDispatcher CoroutineDispatcher;
	public QueueTaskSet Queues;
	public PawnList<Player> Players;
	public PawnList<Npc> Npcs;
	public ChunkSet Chunks;
	public CollisionManager Collision;
	public InstancedMapAllocator InstanceAllocator;
	public PluginRepository Plugins;
	public PrivilegeSet Privileges;
	public XTeaKeyService? XTeaKeyService;
	public UpdateBlockSet PlayerUpdateBlocks;
	public UpdateBlockSet NpcUpdateBlocks;
	public SecureRandom Random;
	public TimerMap Timers;
	public AttributeMap Attributes;
	public List<GroundItem> GroundItems;
	public List<GroundItem> GroundItemQueues;
	public int CurrentCycle = 0;
	public bool MultiThreadPathfinding = false;
	public int RebootTimer = -1;
	
	public World(GameContext gameContext, DevContext devContext)
	{
		GameContext = gameContext;
		DevContext = devContext;
		Queues = new WorldQueueTaskSet();
		Players = new PawnList(new int[gameContext.PlayerLimit]);
		Npcs = new PawnList(new Npc[UInt16.MaxValue]);
		Collision = new CollisionManager(Chunks);
		InstanceAllocator = new InstancedMapAllocator();
		Plugins = new PluginRepository(this);
		Privileges = new PrivilegeSet();
		PlayerUpdateBlocks = new UpdateBlockSet();
		NpcUpdateBlocks = new UpdateBlockSet();
		Random = new SecureRandom();
		Timers = new TimerMap();
		Attributes = new AttributeMap();
		GroundItems = new List<GroundItem>();
		GroundItemQueues = new List<GroundItem>();
	}
	
	
	val huffman by lazy {
		val binary = filestore.getIndex(IndexType.BINARY)!!
		val archive = binary.FindArchiveByName("huffman")!!
		val file = archive.getFiles(filestore.storage.loadArchive(archive)!!).files[0]
		HuffmanCodec(file.contents)
	}
	
	// TODO: check this
	public void Init()
	{
		foreach (List<GameService> service in GetService(typeof(GameService)))
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
	
	
	
}