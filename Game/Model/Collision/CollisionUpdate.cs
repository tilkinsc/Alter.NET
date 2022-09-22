using Exceptions;
using Game.FS;
using Game.FS.Def;
using Game.Model.Entity;

namespace Game.Model.Collision;

enum CollisionType {
	ADD,
	REMOVE
}

class CollisionUpdate
{
	
	public CollisionType Type;
	public Dictionary<Tile, List<DirectionFlag>> Flags;
	
	private CollisionUpdate(CollisionType type, Dictionary<Tile, List<DirectionFlag>> flags)
	{
		Type = type;
		Flags = flags;
	}
	
	public class Builder
	{
		
		private Dictionary<Tile, List<DirectionFlag>> Flags = new Dictionary<Tile, List<DirectionFlag>>();
		private CollisionType? Type = null;
		
		public CollisionUpdate Build()
		{
			if (Type == null)
				throw new IllegalStateException("Type has not been set");
			return new CollisionUpdate((CollisionType) Type, Flags);
		}
		
		public void SetType(CollisionType type)
		{
			if (Type != null)
				throw new IllegalStateException("Type has already been set");
			Type = type;
		}
		
		public void PutTile(Tile tile, bool impenetrable, params Direction[] directions)
		{
			if (!directions.Any())
				throw new IllegalArgumentException("Must supply directions");
			if (Flags.ContainsKey(tile)) {
				foreach (Direction dir in directions)
				{
					Flags[tile].Add(new DirectionFlag(dir, impenetrable));
				}
			} else {
				Flags[tile] = new List<DirectionFlag>();
				foreach (Direction dir in directions)
				{
					Flags[tile].Add(new DirectionFlag(dir, impenetrable));
				}
			}
		}
		
		private void PutWall(Tile tile, bool impenetrable, Direction orientation)
		{
			PutTile(tile, impenetrable, orientation);
			PutTile(tile.Step(orientation), impenetrable, orientation.GetOpposite());
		}
		
		private void PutLargeCornerWall(Tile tile, bool impenetrable, Direction orientation)
		{
			List<Direction> directions = orientation.GetDiagonalComponents();
			PutTile(tile, impenetrable, directions.ToArray());
			foreach (Direction dir in directions)
			{
				PutTile(tile.Step(dir), impenetrable, dir.GetOpposite());
			}
		}
		
		private bool Unwalkable(ObjectDef def, int type)
		{
			bool isSolidFloorDecor = type == ObjectType.FLOOR_DECOR.Value && def.IsInteractive;
			bool isRoof = type > ObjectType.DIAGONAL_INTERACTABLE.Value && type < ObjectType.FLOOR_DECOR.Value && def.IsSolid;
			bool isWall = (type >= ObjectType.LENGTHWISE_WALL.Value && type <= ObjectType.RECTANGULAR_CORNER.Value || type == ObjectType.DIAGONAL_WALL.Value) && def.IsSolid;
			bool isSolidInteractable = (type == ObjectType.DIAGONAL_INTERACTABLE.Value || type == ObjectType.INTERACTABLE.Value) && def.IsSolid;
			return isWall || isRoof || isSolidFloorDecor || isSolidInteractable;
		}
		
		public void PutObject(DefinitionSet definitions, GameObject obj)
		{
			ObjectDef? def = definitions.Get<ObjectDef>(obj.ID);
			if (def == null)
				throw new IllegalStateException("Definitions not loaded before using them");
			
			int type = obj.Type;
			Tile tile = obj.Tile!;
			if (!Unwalkable(def, type)) {
				return;
			}
			
			int x = tile.X;
			int z = tile.Z;
			int height = tile.Height;
			int width = def.Width;
			int length = def.Length;
			bool impenetrable = def.IsImpenetrable;
			int orientation = obj.Rotation;
			
			if (orientation == 1 || orientation == 3) {
				width = def.Length;
				length = def.Width;
			}
			
			if (type == ObjectType.FLOOR_DECOR.Value) {
				if (def.IsInteractive && def.IsSolid) {
					PutTile(new Tile(x, z, height), impenetrable, Direction.NESW.ToArray());
				}
			} else if (type >= ObjectType.DIAGONAL_WALL.Value && type < ObjectType.FLOOR_DECOR.Value) {
				for (int dx=0; dx<width; dx++)
				{
					for (int dz=0; dz<length; dz++)
					{
						PutTile(new Tile(x + dx, z + dz, height), impenetrable, Direction.NESW.ToArray());
					}
				}
			} else if (type == ObjectType.LENGTHWISE_WALL.Value) {
				PutWall(tile, impenetrable, Direction.WNES[orientation]);
			} else if (type == ObjectType.TRIANGULAR_CORNER.Value || type == ObjectType.RECTANGULAR_CORNER.Value) {
				PutWall(tile, impenetrable, Direction.WNES_DIAGONAL[orientation]);
			} else if (type == ObjectType.WALL_CORNER.Value) {
				PutLargeCornerWall(tile, impenetrable, Direction.WNES_DIAGONAL[orientation]);
			}
		}
		
	}
	
}
