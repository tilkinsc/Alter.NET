using Game.Model.Collision;
using Util;

namespace Game.Model.Path.Strategy;

class SimplePathFindingStrategy : PathFindingStrategy
{
	
	public SimplePathFindingStrategy(CollisionManager collision)
			: base(collision)
	{
	}
	
	private bool CanTraverse(CollisionManager collision, Tile tile, int width, int length, Direction direction, bool projectile)
	{
		for (int x=0; x<width; x++)
		{
			for (int z=0; z<length; z++)
			{
				Tile transform = tile.Transform(x, z);
				if (!collision.CanTraverse(transform, direction, projectile) || !collision.CanTraverse(transform.Step(direction), direction.GetOpposite(), projectile))
					return false;
			}
		}
		return true;
	}
	
	private bool AreBordering(Tile tile1, int size1, Tile tile2, int size2)
	{
		return AabbUtil.AreBordering(tile1.X, tile1.Z, size1, size1, tile2.X, tile2.Z, size2, size2);
	}
	
	private bool AreDiagonal(Tile tile1, int size1, Tile tile2, int size2)
	{
		return AabbUtil.AreDiagonal(tile1.X, tile1.Z, size1, size1, tile2.X, tile2.Z, size2, size2);
	}
	
	private bool AreOverlapping(Tile tile1, int size1, Tile tile2, int size2)
	{
		return AabbUtil.AreOverlapping(tile1.X, tile1.Z, size1, size1, tile2.X, tile2.Z, size2, size2);
	}
	
	private bool AreCoordinatesInRange(int coord1, int size1, int coord2, int size2)
	{
		return !(coord1 + size1 < coord2 || coord1 > coord2 + size2);
	}
	
	public override Route CalculateRoute(PathRequest request)
	{
		Tile start = request.Start;
		Tile end = request.End;
		
		bool projectile = request.ProjectilePath;
		int sourceWidth = request.SourceLength;
		int sourceLength = request.SourceLength;
		int targetWidth = Math.Max(request.TouchRadius, request.TargetWidth);
		int targetLength = Math.Max(request.TouchRadius, request.TargetLength);
		
		List<Tile> path = new List<Tile>();
		bool success = false;
		
		int searchLimit = 2;
		while (searchLimit-- > 0)
		{
			Tile tail = path.Count > 0 ? path[0] : start;
			if (AreBordering(tail, sourceWidth, end, targetWidth) && !AreDiagonal(tail, sourceWidth, end, targetWidth)
					&& Collision.Raycast(tail, end, projectile))
			{
				success = true;
				break;
			}
			
			Direction eastOrWest = (tail.X < end.X) ? Direction.EAST : Direction.WEST;
			Direction northOrSouth = (tail.Z < end.Z) ? Direction.NORTH : Direction.SOUTH;
			bool overlapped = false;
			if (AreOverlapping(tail, sourceWidth, end, targetWidth))
			{
				eastOrWest = eastOrWest.GetOpposite();
				northOrSouth = northOrSouth.GetOpposite();
				overlapped = true;
			}
			
			while ((!AreCoordinatesInRange(tail.Z, sourceLength, end.Z, targetLength)
						|| AreDiagonal(tail, sourceLength, end, targetLength)
						|| AreOverlapping(tail, sourceLength, end, targetLength))
					&& (overlapped || !AreOverlapping(tail.Step(northOrSouth), sourceLength, end, targetLength))
					&& CanTraverse(Collision, tail, sourceWidth, sourceLength, northOrSouth, projectile))
			{
				tail = tail.Step(northOrSouth);
				path.Add(tail);
			}
			
			while ((!AreCoordinatesInRange(tail.X, sourceWidth, end.X, targetWidth)
						|| AreDiagonal(tail, sourceWidth, end, targetWidth)
						|| AreOverlapping(tail, sourceWidth, end, targetWidth))
					&& (overlapped || !AreOverlapping(tail.Step(eastOrWest), sourceWidth, end, targetWidth))
					&& CanTraverse(Collision, tail, sourceWidth, sourceLength, eastOrWest, projectile))
			{
				tail = tail.Step(eastOrWest);
				path.Add(tail);
			}
		}
		
		return new Route(new Queue<Tile>(path), success, path.Count > 0 ? path[0] : start);
	}
	
	
}