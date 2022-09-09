using Exceptions;
using Game.FS;
using Game.FS.Def;
using Game.Model.Entity;
using Game.Model.Collision;
using Game.Model.Entity;
using Game.Model.Region.Update;

namespace Game.Model.Region;


class Chunk
{
	
	public const int CHUNK_SIZE = 8;
	public const int CHUNKS_PER_REGION = 13;
	public const int CHUNK_VIEW_RADIUS = 3;
	public const int REGION_SIZE = CHUNK_SIZE * CHUNK_SIZE;
	public const int MAX_VIEWPORT = CHUNK_SIZE * CHUNKS_PER_REGION;
	
	public ChunkCoords Coords;
	public int Heights;
	
	public List<CollisionMatrix> Matrices =
		CollisionMatrix.CreateMatrices(Tile.TOTAL_HEIGHT_LEVELS, CHUNK_SIZE, CHUNK_SIZE);
	
	public List<Tile> BlockedTiles = new List<Tile>();
	public Dictionary<Tile, List<BaseEntity>>? Entities;
	public List<EntityUpdate>? Updates;
	
	public Chunk(ChunkCoords coords, int heights)
	{
		Coords = coords;
		Heights = heights;
	}
	
	private void CopyMatrices(Chunk other)
	{
		for (int i=0; i<other.Matrices.Count; i++)
		{
			Matrices[i] = new CollisionMatrix(other.Matrices[i]);
		}
	}
	
	public Chunk(Chunk other)
		: this(other.Coords, other.Heights)
	{
		CopyMatrices(other);
	}
	
	public void CreateEntityContainers()
	{
		Entities = new Dictionary<Tile, List<BaseEntity>>();
		Updates = new List<EntityUpdate>();
	}
	
	public CollisionMatrix GetMatrix(int height)
	{
		return Matrices[height];
	}
	
	public void SetMatrix(int height, CollisionMatrix matrix)
	{
		Matrices[height] = matrix;
	}
	
	public bool Contains(Tile tile)
	{
		return Coords == tile.ChunkCoords;
	}
	
	public bool IsBlocked(Tile tile, Direction direction, bool projectile)
	{
		
		return Matrices[tile.Height].IsBlocked(tile.X % CHUNK_SIZE, tile.Z % CHUNK_SIZE, direction, projectile);
	}
	
	public bool IsClipped(Tile tile)
	{
		return Matrices[tile.Height].IsClipped(tile.X % CHUNK_SIZE, tile.Z % CHUNK_SIZE);
	}
	
	private bool CanBeViewed(Player p, BaseEntity entity)
	{
		if (p.Tile.Height != entity.Tile.Height) {
			return false;
		}
		if (entity.EntityType.IsGroundItem()) {
			GroundItem item = (GroundItem) entity;
			return item.IsPublic() || item.IsOwnedBy(p);
		}
		return true;
	}
	
	private void SendUpdate(World world, EntityUpdate update)
	{
		List<ChunkCoords> surrounding = Coords.GetSurroundingCoords();
		foreach (ChunkCoords coords in surrounding)
		{
			Chunk chunk = world.Chunks.Get(coords, false);
			List<var?> clients = chunk.GetEntities<Client>(EntityType.CLIENT);
			foreach (Client client in clients)
			{
				if (!CanBeViewed(client, update.Entity)) {
					continue;
				}
				var? local = client.LastKnownRegionBase.ToLocal(Coords.ToTile());
				client.Write(UpdateZonePartialFollowsMessage(local.X, local.Z));
				client.Write(update.ToMessage());
			}
		}
	}
	
	private EntityUpdate<T>? CreateUpdateFor<T>(T entity, bool spawn) where T : BaseEntity
	{
		switch (entity.EntityType)
		{
			case EntityType.DYNAMIC_OBJECT:
			case EntityType.STATIC_OBJECT:
			{
				if (spawn)
					return LocAddChangeUpdate(EntityUpdateType.SPAWN_OBJECT, entity as GameObject);
				return LocDelUpdate(EntityUpdateType.REMOVE_OBJECT, entity as GameObject);
			}
			case EntityType.GROUND_ITEM:
			{
				if (spawn)
					return ObjAddUpdate(EntityUpdateType.SPAWN_GROUND_ITEM, entity as GroundItem);
				return ObjDelUpdate(EntityUpdateType.REMOVE_GROUND_ITEM, entity as GroundItem);
			}
			case EntityType.PROJECTILE:
			{
				if (spawn)
					return MapProjAnimUpdate(EntityUpdateType.SPAWN_PROJECTILE, entity as Projectile);
				throw new RuntimeException($"{entity.EntityType} can only be spawned, not removed");
			}
			case EntityType.AREA_SOUND:
			{
				if (spawn)
					return SoundAreaUpdate(EntityUpdateType.PLAY_TILE_SOUND, entity as AreaSound);
				throw new RuntimeException($"{entity.EntityType} can only be spawned, not removed");
			}
			case EntityType.MAP_ANIM:
			{
				if (spawn)
					return MapAnimUpdate(EntityUpdateType.MAP_ANIM, entity as TileGraphic);
				throw new RuntimeException($"{entity.EntityType} can only be spawned, not removed");
			}
		}
		return null;
	}
	
	public void AddEntity(World world, BaseEntity entity, Tile tile)
	{
		if (Entities == null)
			throw new IllegalStateException("Can't fetch uninitialized entities");
		
		if (entity.EntityType.IsTransient()) {
			List<BaseEntity> list;
			if (!Entities.ContainsKey(tile)) {
				list = new List<BaseEntity>();
			} else {
				list = Entities[tile];
			}
			Entities[tile] = list;
		}
		
		EntityUpdate? update = CreateUpdateFor(entity, false);
		if (update != null) {
			if (entity.EntityType == EntityType.STATIC_OBJECT) {
				Updates.Add(update);
			} else {
				Updates.RemoveAll(new Predicate<EntityUpdate>((EntityUpdate obj) => (obj as EntityUpdate)!.Entity == entity));
			}
			SendUpdate(world, update);
		}
		
	}
	
	public void RemoveEntity(World world, BaseEntity entity, Tile tile)
	{
		if (!entity.EntityType.IsTransient()) {
			throw new IllegalStateException("Transient entities cannot be removed from chunks");
		}
		
		if (entity.EntityType.IsObject()) {
			world.Collision.ApplyCollision(world.Definitions, entity as GameObject, CollisionUpdate.Type.REMOVE);
		}
		
		if (Entities == null || Updates == null)
			throw new IllegalStateException("Entities and Updates are not initialized");
		
		if (!Entities.ContainsKey(tile))
			return;
		
		var? update = CreateUpdateFor(entity, false);
		if (update != null) {
			if (entity.EntityType == EntityType.STATIC_OBJECT) {
				Updates.Add(update);
			} else {
				Updates.RemoveAll(update);
			}
			
			SendUpdate(world, update);
		}
	}
	
	public void UpdateGroundItems(World world, GroundItem item, int oldAmount, int newAmount)
	{
		var? update = new ObjCountUpdate(EntityUpdateType.UPDATE_GROUND_ITEM, item, oldAmount, newAmount);
		SendUpdate(world, update);
		
		if (Updates.removeIf { it.entity == item }) {
			Updates.Add(CreateUpdateFor(item, true));
		}
	}
	
	public void SendUpdates(Player p, GameService gameService)
	{
		List<EntityGroupMessage> messages = new List<EntityGroupMessage>();
		
		foreach (var? update in Updates)
		{
			EntityGroupMessage msg = new EntityGroupMessage(update.Type.ID, update.ToMessage());
			if (CanBeViewed(p, update.Entity)) {
				messages.Add(message);
			}
		}
		
		if (messages.Count > 0) {
			var? local = p.LastKnownRegionBase.ToLocal(coords.ToTile());
			p.Write(new UpdateZonePartialEnclosedMessage(local.x, local.z, gameService.MessageEncoders, gameService.MessageStructures, messages.ToArray()));
		}
	}
	
	public List<T> GetEntities<T>(params EntityType[] types)
	{
		
	}
	
	public List<T> GetEntities<T>(Tile tile, params EntityType[] types)
	{
		
	}
	
}