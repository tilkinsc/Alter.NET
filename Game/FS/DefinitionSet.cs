using Cache;
using Cache.Definitions;
using Cache.Definitions.Loaders;
using Cache.FS;
using Cache.Region;
using DotNetty.Buffers;
using Exceptions;
using Game.FS.Def;
using Game.Model;
using Game.Model.Collision;
using Game.Model.Region;
using Game.Service.Xtea;
using Util;

namespace Game.FS;


class DefinitionSet
{
	
	public Dictionary<Type, Dictionary<int, object>> Definitions = new Dictionary<Type, Dictionary<int, object>>();
	
	private XTeaKeyService? XTeaKeyService = null;
	
	public void LoadAll(RLStore store)
	{
		Load<AnimDef>(store, typeof(AnimDef));
		Load<VarBitDef>(store, typeof(VarBitDef));
		Load<EnumDef>(store, typeof(EnumDef));
		Load<StructDef>(store, typeof(StructDef));
		Load<NpcDef>(store, typeof(NpcDef));
		Load<ItemDef>(store, typeof(ItemDef));
		Load<ObjectDef>(store, typeof(ObjectDef));
	}
	
	public void LoadRegions(World world, ChunkSet chunks, int[] regions)
	{
		int loaded = 0;
		foreach (int region in regions)
		{
			if (chunks.ActiveRegions.AddIfNotExist(region)) {
				if (CreateRegion(world, region)) {
					loaded++;
				}
			}
		}
	}
	
	public void Load<T>(RLStore store, Type type) where T : Definition
	{
		RLConfigType? configType = null;
		if (type == typeof(VarpDef)) {
			configType = RLConfigType.VARPLAYER;
		} else if (type == typeof(VarBitDef)) {
			configType = RLConfigType.VARBIT;
		} else if (type == typeof(EnumDef)) {
			configType = RLConfigType.ENUM;
		} else if (type == typeof(StructDef)) {
			configType = RLConfigType.STRUCT;
		} else if (type == typeof(NpcDef)) {
			configType = RLConfigType.NPC;
		} else if (type == typeof(ObjectDef)) {
			configType = RLConfigType.OBJECT;
		} else if (type == typeof(ItemDef)) {
			configType = RLConfigType.ITEM;
		} else if (type == typeof(AnimDef)) {
			configType = RLConfigType.SEQUENCE;
		} else {
			throw new IllegalArgumentException($"Unhandled class type {type.FullName}");
		}
		RLIndex? configs = store.GetIndex(RLIndexType.CONFIGS);
		if (configs == null) {
			throw new FileNotFoundException("Cache was not found");
		}
		RLArchive archive = configs.GetArchive(configType.GetID())!;
		List<RLFSFile> files = archive.GetFiles(store.Storage.LoadArchive(archive)!).Files;
		
		Dictionary<int, T> definitions = new Dictionary<int, T>();
		for (int i=0; i<files.Count; i++)
		{
			definitions[files[i].FileID] = CreateDefinition<T>(type, files[i].FileID, files[i].Contents!);
		}
		Definitions[type] = definitions.ToDictionary(v => v.Key, v => (object) v.Value!)!;
	}
	
	public T CreateDefinition<T>(Type type, int id, byte[] data) where T : Definition
	{
		Definition? def = null;
		if (type == typeof(VarpDef)) {
			def = new VarpDef(id);
		} else if (type == typeof(VarBitDef)) {
			def = new VarBitDef(id);
		} else if (type == typeof(EnumDef)) {
			def = new EnumDef(id);
		} else if (type == typeof(StructDef)) {
			def = new StructDef(id);
		} else if (type == typeof(NpcDef)) {
			def = new NpcDef(id);
		} else if (type == typeof(ObjectDef)) {
			def = new ObjectDef(id);
		} else if (type == typeof(ItemDef)) {
			def = new ItemDef(id);
		} else if (type == typeof(AnimDef)) {
			def = new AnimDef(id);
		} else {
			throw new IllegalArgumentException($"Unhandled class type {type.FullName}");
		}
		
		IByteBuffer buf = Unpooled.WrappedBuffer(data);
		def.Decode(buf);
		buf.Release();
		return (T) def;
	}
	
	public int GetCount(Type type)
	{
		return Definitions[type].Count;
	}
	
	public T? Get<T>(int id) where T : Definition
	{
		return (Definitions[typeof(T)])[id] as T;
	}
	
	public bool CreateRegion(World world, int id)
	{
		if (XTeaKeyService == null) {
			XTeaKeyService = world.GetService(typeof(XTeaKeyService));
		}
		
		RLIndex? regionIndex = world.FileStore.GetIndex(RLIndexType.MAPS);
		
		int x = id >> 8;
		int z = id & 0xFF;
		
		RLArchive? mapArchive = regionIndex.FindArchiveByName($"m{x}_{z}");
		if (mapArchive == null)
			return false;
		
		RLArchive? landArchive = regionIndex.FindArchiveByName($"l{x}_{z}");
		if (landArchive == null)
			return false;
		
		byte[]? mapData = mapArchive.Decompress(world.FileStore.Storage.LoadArchive(mapArchive));
		if (mapData == null)
			return false;
		
		RLMapDefinition mapDefinition = new RLMapLoader().Load(x, z, mapData);
		
		RLRegion cacheRegion = new RLRegion(id);
		cacheRegion.LoadTerrain(mapDefinition);
		
		List<Tile> blocked = new List<Tile>();
		List<Tile> bridges = new List<Tile>();
		for (int height=0; height<4; height++)
		{
			for (int lx=0; lx<64; lx++)
			{
				for (int lz=0; lz<64; lz++)
				{
					int tileSetting = cacheRegion.GetTileSetting(height, lx, lz);
					Tile tile = new Tile(cacheRegion.BaseX + lx, cacheRegion.BaseY + lz, height);
					
					if ((tileSetting & CollisionManager.BLOCKED_TILE) == CollisionManager.BLOCKED_TILE) {
						blocked.Add(tile);
					}
					
					if ((tileSetting & CollisionManager.BRIDGE_TILE) == CollisionManager.BRIDGE_TILE) {
						bridges.Add(tile);
						blocked.Remove(tile.Transform(-1));
					}
				}
			}
		}
		
		CollisionUpdate.Builder blockedTileBuilder = new CollisionUpdate.Builder();
		blockedTileBuilder.Type = CollisionType.ADD;
		foreach (Tile tile in blocked)
		{
			world.Chunks.GetOrCreate(tile).BlockedTiles.Add(tile);
			blockedTileBuilder.PutTile(tile, false, Direction.NESW);
		}
		world.Collision.ApplyUpdate(blockedTileBuilder.Build());
		
		if (XTeaKeyService == null) {
			return true;
		}
		
		int[] keys = XTeaKeyService.Get(id);
		if (keys == null)
			keys = XTeaKeyService.EMPTY_KEYS;
		// try {
			
			byte[]? landData = landArchive.Decompress(world.FileStore.Storage.LoadArchive(landArchive), keys);
			RLLocationsDefinition locDef = new RLLocationsLoader().Load(x, z, landData);
			cacheRegion.LoadLocations(locDef);
			
			foreach (RLLocation loc in cacheRegion.Locations)
			{
				Tile tile = new Tile(loc.Position.X, loc.Position.Y, loc.Position.Z);
				if (bridges.Contains(tile.Transform(1))) {
					return@forEach
				}
				StaticObject obj = new StaticObject(loc.ID, loc.Type, loc.Orientation, bridges.Contains(tile) ? tile.Transform(-1) : tile);
				world.Chunks.GetOrCreate(tile).AddEntity(world, obj, obj.Tile);
			}
			return true;
		// } catch (IOException e) {
			
		// 	return false;
		// }
		
	}
	
}
