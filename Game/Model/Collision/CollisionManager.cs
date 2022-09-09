using Exceptions;
using Game.FS;
using Game.Model.Entity;
using Game.Model.Region;

namespace Game.Model.Collision;

class CollisionManager
{
	
	public const int BLOCKED_TILE = 0x1;
	public const int BRIDGE_TILE = 0x2;
	public const int ROOF_TILE = 0x4;
	
	public ChunkSet Chunks;
	public bool CreateChunksIfNeeded;
	
	public CollisionManager(ChunkSet chunks, bool createChunksIfNeeded = false)
	{
		Chunks = chunks;
		CreateChunksIfNeeded = createChunksIfNeeded;
	}
	
	public bool IsClipped(Tile tile)
	{
		return Chunks.Get(tile, CreateChunksIfNeeded)!.IsClipped(tile);
	}
	
	public bool IsBlocked(Tile tile, Direction direction, bool projectile)
	{
		return Chunks.Get(tile, CreateChunksIfNeeded)!.IsBlocked(tile, direction, projectile);
	}
	
	public bool CanTraverse(Tile tile, Direction direction, bool projectile)
	{
		Chunk? chunk = Chunks.Get(tile, CreateChunksIfNeeded);
		if (chunk == null)
			throw new IllegalStateException("Unable to gather chunk");
		
		if (chunk.IsBlocked(tile, direction, projectile))
			return false;
		
		if (direction.IsDiagonal()) {
			foreach (Direction other in direction.GetDiagonalComponents())
			{
				Tile diagonalTile = tile.Step(other);
				Chunk diagonalChunk = Chunks.Get(diagonalTile, CreateChunksIfNeeded)!;
				if (diagonalChunk.IsBlocked(diagonalTile, other.GetOpposite(), projectile))
					return false;
			}
		}
		return true;
	}
	
	public bool Raycast(Tile start, Tile target, bool projectile)
	{
		if (start.Height != target.Height)
			throw new IllegalArgumentException("Tiles must be on the same height level");
		
		int x0 = start.X;
		int y0 = start.Z;
		int x1 = target.X;
		int y1 = target.Z;
		int height = start.Height;
		
		int dx = Math.Abs(x1 - x0);
		int dy = Math.Abs(y1 - y0);
		
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		
		int err = dx - dy;
		int err2 = 0;
		
		Tile prev = new Tile(x0, y0, height);
		while (x0 != x1 || y0 != y1)
		{
			err2 = err << 1;
			
			if (err2 > -dy) {
				err -= dy;
				x0 += sx;
			}
			if (err2 < dx) {
				err += dx;
				y0 += sy;
			}
			
			Tile next = new Tile(x0, y0, height);
			Direction dir = Direction.Between(prev, next);
			if (!CanTraverse(prev, dir, projectile || !CanTraverse(next, dir.GetOpposite(), projectile)))
				return false;
			prev = next;
		}
		return true;
	}
	
	public int RaycastTiles(Tile start, Tile target)
	{
		if (start.Height != target.Height)
			throw new IllegalArgumentException("Tiles must be on the same height level");
		
		int x0 = start.X;
		int y0 = start.Z;
		int x1 = target.X;
		int y1 = target.Z;
		int height = start.Height;
		
		int dx = Math.Abs(x1 - x0);
		int dy = Math.Abs(y1 - y0);
		
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		
		int err = dx - dy;
		int err2 = 0;
		
		int tiles = 0;
		
		while (x0 != x1 || y0 != y1)
		{
			err2 = err << 1;
			if (err2 > -dy) {
				err -= dy;
				dx += sx;
			}
			if (err2 < dx) {
				err += dx;
				y0 += sy;
			}
			tiles++;
		}
		return tiles;
	}
	
	private void Flag(CollisionType type, CollisionMatrix matrix, int localX, int localY, CollisionFlag flag)
	{
		if (type == CollisionType.ADD) {
			matrix.AddFlag(localX, localY, flag);
		} else {
			matrix.RemoveFlag(localX, localY, flag);
		}
	}
	
	public void ApplyCollision(DefinitionSet definitions, GameObject obj, CollisionType updateType)
	{
		CollisionUpdate.Builder builder = new();
		builder.SetType(updateType);
		builder.PutObject(definitions, obj);
		ApplyUpdate(builder.Build());
	}
	
	// TODO: fill this out
	public void ApplyUpdate(CollisionUpdate update)
	{
		CollisionType type = update.Type;
		Dictionary<Tile, List<DirectionFlag>> map = update.Flags;
		
		Chunk? chunk = null;
		foreach (KeyValuePair<Tile, List<DirectionFlag>> entry in map)
		{
			Tile tile = entry.Key;
			
			if (chunk == null || !chunk.Contains(tile)) {
				chunk = Chunks.Get(tile, CreateChunksIfNeeded);
			}
			
			int localX = tile.X % Chunk.CHUNK_SIZE;
			int localZ = tile.Z % Chunk.CHUNK_SIZE;
			
			CollisionMatrix matrix = chunk!.GetMatrix(tile.Height);
			List<CollisionFlag> pawns = CollisionFlag.PAWN_FLAGS;
			List<CollisionFlag> projectiles = CollisionFlag.PROJECTILE_FLAGS;
			foreach (DirectionFlag flag in entry.Value)
			{
				Direction direction = flag.Direction;
				if (direction == Direction.NONE) {
					continue;
				}
				
				int orientation = direction.OrientationValue;
				if (flag.IsImpenetrable) {
					Flag(type, matrix, localX, localZ, projectiles[orientation]);
				}
				
				Flag(type, matrix, localX, localZ, pawns[orientation]);
			}
		}
	}
	
}
