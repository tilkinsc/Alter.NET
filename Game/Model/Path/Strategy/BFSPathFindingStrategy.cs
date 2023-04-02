using Game.Model.Collision;

namespace Game.Model.Path.Strategy;

class BFSPathFindingStrategy : PathFindingStrategy
{
	
	private class Node
	{
		public Tile Tile { get; private set; }
		public Node? Parent;
		
		public int Cost = 0;
		
		public Node(Tile tile, Node? parent)
		{
			Tile = tile;
			Parent = parent;
		}
	}
	
	public BFSPathFindingStrategy(CollisionManager collision)
			: base(collision)
	{
	}
	
	private bool IsTileBlocked(Tile node, Tile link)
	{
		return !Collision.CanTraverse(node, Direction.Between(node, link), false);
	}
	
	private bool IsStepBlocked(Tile node, Tile link, int width, int length, bool clipNode, bool clipLink)
	{
		if (!clipNode && !clipLink)
			return false;
		
		for (int x=0; x<width; x++)
		{
			for (int z=0; z<length; z++)
			{
				Tile transform = node.Transform(x, z);
				if (clipNode && IsTileBlocked(transform, link))
					return true;
				if (clipLink && IsTileBlocked(link, transform))
					return true;
			}
		}
		return false;
	}
	
	private bool IsTargetDirectionBlocked(Tile node, Tile end, int targetWidth, int targetLength, HashSet<Direction> blockedDirection)
	{
		int x = node.X;
		int z = node.Z;
		int dx = x - end.X;
		int dz = z - end.Z;
		
		Direction face;
		if (dx == -1)
			face = Direction.EAST;
		else if (dx == targetWidth)
			face = Direction.WEST;
		else if (dz == -1)
			face = Direction.NORTH;
		else if (dz == targetLength)
			face = Direction.SOUTH;
		else
			return false;
		
		return blockedDirection.Contains(face.GetOpposite());
	}
	
	private bool IsDiagonalTile(Tile current, Tile end, int targetWidth, int targetLength)
	{
		int curX = current.X;
		int curZ = current.Z;
		int endX = end.X;
		int endZ = end.Z;
		
		bool southWest = curX == (endX - 1) && curZ == (endZ - 1);
		bool southEast = curX == (endX + targetWidth) && curZ  == (endZ - 1);
		bool northWest = curX == (endX - 1) && curZ == (endZ + targetLength);
		bool northEast = curX == (endX + targetWidth) && curZ == (endZ + targetLength);
		
		return southWest || southEast || northWest || northEast;
	}
	
	private bool IsTileOverlapping(Tile tile, Tile target, int targetWidth, int targetLength)
	{
		int curX = tile.X;
		int curZ = tile.Z;
		int endX = target.X;
		int endZ = target.Z;
		
		return curX >= endX && curX < endX + targetWidth && curZ >= endZ && curZ < endZ + targetLength;
	}
	
	private bool IsDirectionBlocked(Tile node, Tile end, int targetWidth, int targetLength, bool projectilePath)
	{
		int x = node.X;
		int z = node.Z;
		int dx = x - end.X;
		int dz = z - end.Z;
		
		Direction face;
		if (dx == -1)
			face = Direction.EAST;
		else if (dx == targetWidth)
			face = Direction.WEST;
		else if (dz == -1)
			face = Direction.NORTH;
		else if (dz == targetLength)
			face = Direction.SOUTH;
		else
			return false;
		
		return Collision.IsBlocked(node, face, projectilePath);
	}
	
	private HashSet<Tile> GetEndTiles(PathRequest request)
	{
		Tile end = request.End;
		int targetWidth = request.TargetWidth;
		int targetLength = request.TargetLength;
		int TouchRadius = request.TouchRadius;
		bool projectilePath = request.ProjectilePath;
		
		bool clipDiagonals = request.ClipFlags.Contains(PathRequest.ClipFlag.DIAGONAL);
		bool clipDirections = request.ClipFlags.Contains(PathRequest.ClipFlag.DIRECTIONS);
		bool clipOverlapping = request.ClipFlags.Contains(PathRequest.ClipFlag.OVERLAP);
		
		HashSet<Tile> validTiles = new HashSet<Tile>();
		if (targetWidth == 0 && targetLength == 0)
		{
			validTiles.Add(end);
		}
		else
		{
			for (int x=-1; x<=targetWidth; x++)
			{
				for (int z=-1; z<=targetLength; z++)
				{
					Tile tile = end.Transform(x, z);
					if (clipDiagonals && IsDiagonalTile(tile, end, targetWidth, targetLength)
							|| clipDirections && IsTargetDirectionBlocked(tile, end, targetWidth, targetLength, new HashSet<Direction>(request.BlockedDirections))
							|| clipOverlapping && IsTileOverlapping(tile, end, targetWidth, targetLength))
					{
						continue;
					}
					if (!IsDirectionBlocked(tile,  end, targetWidth, targetLength,  projectilePath))
						validTiles.Add(tile);
				}
			}
		}
		
		if (TouchRadius > 1)
		{
			for (int x=-TouchRadius; x<=TouchRadius; x++)
			{
				for (int z=-TouchRadius; z<=TouchRadius; z++)
				{
					if (x > 0 && x < targetWidth && z > 0 && z < targetLength)
						continue;
					Tile tile = end.Transform(x, z);
					validTiles.Add(tile);
				}
			}
		}
		return validTiles;
	}
	
	public override Route CalculateRoute(PathRequest request)
	{
		Tile start = request.Start;
		Tile end = request.End;
		
		int sourceWidth = request.SourceWidth;
		int sourceLength = request.SourceLength;
		bool projectilePath = request.ProjectilePath;
		bool projectile = projectilePath || request.TouchRadius > 1;
		
		bool clipNode = request.ClipFlags.Contains(PathRequest.ClipFlag.NODE);
		bool clipLink = request.ClipFlags.Contains(PathRequest.ClipFlag.LINKED_NODE);
		
		HashSet<Tile> validEndTiles = GetEndTiles(request);
		
		Queue<Node> nodes  = new Queue<Node>();
		HashSet<Node> closed = new HashSet<Node>();
		Node? tail = null;
		int searchLimit = 256 * 10;
		bool success = false;
		
		nodes.Enqueue(new Node(start, null));
		
		Direction[] order = (Direction[]) Direction.RS_ORDER.Clone();
		
		while (nodes.Count > 0)
		{
			if (Cancel)
				return new Route(new Queue<Tile>(), false, start);
			if (searchLimit-- == 0)
				break;
			
			Node head = nodes.Dequeue();
			bool inRange = validEndTiles.Contains(head.Tile) && (!projectile || Collision.Raycast(head.Tile, end, projectilePath));
			if (inRange)
			{
				tail = head;
				success = true;
				break;
			}
			
			order.OrderBy((iter) => {
				Tile step = head.Tile.Step(iter);
				return step.GetDelta(end) + step.GetDelta(head.Tile);
			});
			
			foreach (Direction direction in order)
			{
				Tile tile = head.Tile.Step(direction);
				Node node = new Node(tile, head);
				if (!closed.Contains(node) && start.IsWithinRadius(tile, MAX_DISTANCE) && !IsStepBlocked(head.Tile, tile, sourceWidth, sourceLength, clipNode, clipLink))
				{
					node.Cost = head.Cost + 1;
					nodes.Enqueue(node);
					closed.Add(node);
				}
			}
		}
		
		if (tail == null && closed.Count > 0)
		{
			Node min = closed.MinBy((node) => node.Tile.GetDistance(end))!;
			HashSet<Node> valid = new HashSet<Node>();
			foreach (Node node in closed)
			{
				if (node.Tile.GetDistance(end) <= min.Tile.GetDistance(end))
				{
					valid.Add(node);
				}
			}
			if (valid.Count > 0)
			{
				tail = valid.MinBy((node) => node.Tile.GetDelta(start));
			}
		}
		
		Tile? last = tail?.Tile;
		Queue<Tile> path = new Queue<Tile>();
		while (tail?.Parent != null)
		{
			path.Enqueue(tail.Tile);
			tail = tail.Parent;
		}
		path = new Queue<Tile>(path.Reverse());
		
		return new Route(path, success, last ?? start);
	}
	
}