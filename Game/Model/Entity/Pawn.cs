using Game.Model.Combat;
using Game.Model.Timer;
using Game.Model.Attr;
using Game.Sync.Block;
using Game.Model.Path;

namespace Game.Model.Entity;

abstract class Pawn : BaseEntity
{
	
	public UpdateBlockBuffer BlockBuffer = new UpdateBlockBuffer();
	public Tile? LastTile;
	public Tile? LastChunkTile;
	public int Index = -1;
	public bool HasMoved = false;
	public StepDirection? StepDirection;
	public Direction LastFacingDirection = Direction.SOUTH;
	public LockState Lock = LockState.NONE;
	public AttributeMap Attributes = new AttributeMap();
	public TimerMap Timers = new TimerMap();
	// TODO: This one is decayed
	public QueueTaskSet Queues = new PawnQueueTaskSet();
	public List<Hit> PendingHits = new List<Hit>();
	public DamageMap DamageMap = new DamageMap();
	public int[] EquipmentBonuses = new int[14];
	public int PrayerIcon = -1;
	public int TransmogID = -1;
	public bool Invisible = false;
	public FutureRoute? FutureRoute;
	
	public World World;
	public MovementQueue MovementQueue;
	
	public Pawn(World world)
	{
		World = world;
		MovementQueue = new MovementQueue(this);
	}
	
	public abstract void Cycle();
	public abstract bool IsRunning();
	public abstract int GetSize();
	public abstract int GetCurrentHP();
	public abstract void SetCurrentHP(int level);
	public abstract int GetMaxHP();
	public abstract void AddBlock(UpdateBlockType block);
	public abstract bool HasBlock(UpdateBlockType block);
	
	public bool IsDead { get => GetCurrentHP() == 0; }
	public bool IsAlive { get => !IsDead; }
	
	public void FullLock() => Lock = LockState.FULL;
	public void Unlock() => Lock = LockState.NONE;
	public bool IsLocked { get => Lock != LockState.NONE; }
	
	public void SetTransmogID(int transmogID)
	{
		TransmogID = transmogID;
		AddBlock(UpdateBlockType.APPEARANCE);
	}
	
	public bool HasMoveDestination()
	{
		return FutureRoute != null || MovementQueue.HasDestination();
	}
	
	public void StopMovement() => MovementQueue.Clear();
	
	public Tile GetCenterTile()
	{
		return Tile.Transform(GetSize() >> 1, GetSize() >> 1);
	}
	
	public Tile GetFrontFacingTile(Tile target, int offset = 0)
	{
		int size = (GetSize() >> 1);
		Tile center = GetCenterTile();
		
		int granularity = 2048;
		double lutFactor = (granularity / (Math.PI * 2));
		
		double theta = Math.Atan2((double) (target.Z - center.Z), (double) (target.X - center.X));
		double angle = ((((int) (theta * lutFactor)) + offset) & (granularity - 1)) / lutFactor;
		if (angle < 0)
			angle += Math.Tau;
		
		return new Tile(
			(int) Math.Round(center.X + (size * Math.Cos(angle))),
			(int) Math.Round(center.Z + (size * Math.Sin(angle))),
			Tile.Height
		);
	}
	
	public void GetFrontFacingTile(Pawn target, int offset = 0)
			=> GetFrontFacingTile(target.GetCenterTile(), offset);
	
	public void Attack(Pawn target)
	{
		ResetInteractions();
		InterruptQueues();
		
		Attributes[COMBAT_TARGET_FOCUS_ATTR] = new WeakReference(target);
		if (EntityType.IsPlayer() || this is Npc && !World.Plugins.ExecuteNpcCombat(this)) {
			World.Plugins.ExecuteCombat(this);
		}
	}
	
	public void AddHit(Hit hit) => PendingHits.Add(hit);
	public void ClearHits() => PendingHits.Clear();
	
	// TODO: implement this
	public void TimerCycle()
	{
		
	}
	
	// TODO: implement this
	public void HitsCycle()
	{
		
	}
	
}
