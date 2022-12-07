using Exceptions;

namespace Game.Model.Entity;

class Projectile : BaseEntity
{
	
	public Pawn? TargetPawn;
	public Tile TargetTile;
	public int GFX;
	public int StartHeight;
	public int EndHeight;
	public int Steepness;
	public int Angle;
	public int Delay;
	public int Lifespan;
	
	private Projectile(Pawn? targetPawn, Tile targetTile, int gfx, int startHeight, int endHeight, int steepness, int angle, int delay, int lifespan)
			: base()
	{
		EntityType = EntityType.PROJECTILE;
		TargetPawn = targetPawn;
		TargetTile = targetTile;
		GFX = gfx;
		StartHeight = startHeight;
		EndHeight = endHeight;
		Steepness = steepness;
		Angle = angle;
		Delay = delay;
		Lifespan = lifespan;
	}
	
	public class Builder
	{
		
		private Tile? Start;
		private Pawn? TargetPawn;
		private Tile? TargetTile;
		private int GFX = -1;
		private int StartHeight;
		private int EndHeight;
		private int Angle;
		private int Steepness;
		private int Delay;
		private int Lifespan = -1;
		
		public Projectile Build()
		{
			if (Start == null)
				throw new IllegalStateException("Start must be set to a tile");
			if (TargetPawn == null && TargetTile == null)
				throw new IllegalStateException("Target must be set to a tile or pawn");
			if (GFX == -1)
				throw new IllegalStateException("GFX must be set");
			
			Tile start = Start;
			Tile target = TargetPawn != null ? TargetPawn.Tile! : TargetTile!;
			
			if (Lifespan == -1) {
				Lifespan = start.GetDistance(target) * 5;
			}
			
			Projectile proj = new Projectile(TargetPawn, target, GFX, StartHeight, EndHeight, Steepness, Angle, Delay, Lifespan);
			proj.Tile = start;
			return proj;
		}
		
		public Builder SetStart(Tile start)
		{
			Start = start;
			return this;
		}
		
		public Builder SetTarget(Pawn pawn)
		{
			TargetPawn = pawn;
			return this;
		}
		
		public Builder SetTarget(Tile tile)
		{
			TargetTile = tile;
			return this;
		}
		
		public Builder SetGFX(int gfx)
		{
			GFX = gfx;
			return this;
		}
		
		public Builder SetStartHeight(int startHeight)
		{
			StartHeight = startHeight;
			return this;
		}
		
		public Builder SetEndHeight(int endHeight)
		{
			EndHeight = endHeight;
			return this;
		}
		
		public Builder SetAngle(int angle)
		{
			Angle = angle;
			return this;
		}
		
		public Builder SetSteepness(int steepness)
		{
			Steepness = steepness;
			return this;
		}
		
		public Builder SetDelay(int delay)
		{
			Delay = delay;
			return this;
		}
		
		public Builder SetLifespan(int lifespan)
		{
			Lifespan = lifespan;
			return this;
		}
		
		public Builder SetTiles(Tile start, Tile target)
		{
			Start = start;
			TargetTile = target;
			return this;
		}
		
		public Builder SetTiles(Tile start, Pawn target)
		{
			Start = start;
			TargetPawn = target;
			return this;
		}
		
		public Builder SetHeights(int startHeight, int endHeight)
		{
			StartHeight = startHeight;
			EndHeight = endHeight;
			return this;
		}
		
		public Builder SetSlope(int angle, int steepness)
		{
			Angle = angle;
			Steepness = steepness;
			return this;
		}
		
		public Builder SetTimes(int delay, int lifespan)
		{
			Delay = delay;
			Lifespan = lifespan;
			return this;
		}
		
	}
	
}