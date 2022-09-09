using Exceptions;
using Game.Model.Entity;

namespace Game.Model.Path;

class PathRequest
{
	
	public Tile Start;
	public int SourceWidth;
	public int SourceLength;
	public Tile End;
	public int TargetWidth;
	public int TargetLength;
	public int TouchRadius;
	public bool ProjectilePath;
	public List<ClipFlag> ClipFlags;
	public List<Direction> BlockedDirections;
	
	public enum ClipFlag
	{
		DIAGONAL,
		DIRECTIONS,
		OVERLAP,
		NODE,
		LINKED_NODE
	}
	
	private PathRequest(Tile start, int sourceWidth, int sourceLength, Tile end, int targetWidth, int targetLength, int touchRadius, bool projectilePath, List<ClipFlag> clipFlags, List<Direction> blockedDirections)
	{
		Start = start;
		SourceWidth = sourceWidth;
		SourceLength = sourceLength;
		End = end;
		TargetWidth = targetWidth;
		TargetLength = targetLength;
		TouchRadius = touchRadius;
		ProjectilePath = projectilePath;
		ClipFlags = clipFlags;
		BlockedDirections = blockedDirections;
	}
	
	public static PathRequest CreateWalkRequest(Pawn pawn, int x, int z, bool projectile, bool detectCollision)
	{
		return new Builder()
				.SetPoints(new Tile(pawn.Tile!), new Tile(x, z, pawn.Tile!.Height))
				.SetSourceSize(pawn.GetSize(), pawn.GetSize())
				.SetTargetSize(0, 0)
				.SetProjectilePath(projectile)
				.ClipPathNodes(detectCollision, detectCollision)
				.Build();
	}
	
	public class Builder
	{
		
		private Tile? Start;
		private Tile? End;
		private int SourceWidth = -1;
		private int SourceLength = -1;
		private int TargetWidth = -1;
		private int TargetLength = -1;
		private int TouchRadius = -1;
		private bool ProjectilePath = false;
		private List<ClipFlag> ClipFlags = new List<ClipFlag>();
		private List<Direction> BlockedDirections = new List<Direction>();
		
		public PathRequest Build()
		{
			if (Start == null || End == null)
				throw new IllegalStateException("Points must be set");
			if (SourceWidth == -1 || SourceLength == -1)
				throw new IllegalStateException("Source size must be set");
			if (TargetWidth == -1 || TargetLength == -1)
				throw new IllegalStateException("Target size must be set");
			
			if (TouchRadius == -1)
				TouchRadius = 0;
			
			return new PathRequest(Start, SourceWidth, SourceLength, End, TargetWidth, TargetLength, TouchRadius, ProjectilePath, ClipFlags, BlockedDirections);
		}
		
		public Builder SetPoints(Tile start, Tile end)
		{
			if (Start == null || End == null)
				throw new IllegalStateException("Points have already been set");
			Start = start;
			End = end;
			return this;
		}
		
		public Builder SetSourceSize(int width, int length)
		{
			if (SourceWidth != -1 || SourceLength != -1)
				throw new IllegalStateException("Source size has already been set");
			SourceWidth = width;
			SourceLength = length;
			return this;
		}
		
		public Builder SetTargetSize(int width, int length)
		{
			if (TargetWidth != -1 || TargetLength != -1)
				throw new IllegalStateException("Target size has already been set");
			TargetWidth = width;
			TargetLength = length;
			return this;
		}
		
		public Builder SetTouchRadius(int touchRadius)
		{
			if (TouchRadius != -1)
				throw new IllegalStateException("Touch radius has already been set");
			TouchRadius = touchRadius;
			return this;
		}
		
		public Builder FindProjectilePath()
		{
			if (ProjectilePath)
				throw new IllegalStateException("Projectile path flag has already been set");
			ProjectilePath = true;
			return this;
		}
		
		public Builder SetProjectilePath(bool projectile)
		{
			ProjectilePath = projectile;
			return this;
		}
		
		public Builder ClipDiagonalTiles()
		{
			if (ClipFlags.Contains(ClipFlag.DIAGONAL))
				throw new IllegalArgumentException("Diagonal tiles have already been flagged for clipping");
			ClipFlags.Add(ClipFlag.DIAGONAL);
			return this;
		}
		
		public Builder ClipDirections(params Direction[] blockedDirections)
		{
			if (ClipFlags.Contains(ClipFlag.DIRECTIONS))
				throw new IllegalStateException("A set of directions have already been flagged for clipping");
			ClipFlags.Add(ClipFlag.DIRECTIONS);
			BlockedDirections.AddRange(blockedDirections);
			return this;
		}
		
		public Builder ClipOverlapTiles()
		{
			if (ClipFlags.Contains(ClipFlag.OVERLAP))
				throw new IllegalStateException("Overlapped tiles have already been flagged for clipping");
			ClipFlags.Add(ClipFlag.OVERLAP);
			return this;
		}
		
		public Builder ClipPathNodes(bool node, bool link)
		{
			if (ClipFlags.Contains(ClipFlag.NODE) || ClipFlags.Contains(ClipFlag.LINKED_NODE))
				throw new IllegalStateException("Path nodes have already been flagged for clipping");
			if (node) {
				ClipFlags.Add(ClipFlag.NODE);
			}
			if (link) {
				ClipFlags.Add(ClipFlag.LINKED_NODE);
			}
			return this;
		}
		
	}
	
}
