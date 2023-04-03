using Game.Model.Combat;
using Game.Model.Timer;
using Game.Model.Attr;
using Game.Sync.Block;
using Game.Model.Path;
using Game.Model.Bits;

namespace Game.Model.Entity;

abstract class Pawn : BaseEntity
{
	
	public int Index = -1;
	public UpdateBlockBuffer BlockBuffer = new UpdateBlockBuffer();
	public Tile? LastTile;
	public Tile? LastChunkTile;
	public bool HasMoved = false;
	public MovementQueue MovementQueue { get; private set; }
	public StepDirection? Steps;
	public Direction LastFacingDirection = Direction.SOUTH;
	public Direction FaceDirection => LastFacingDirection;
	public LockState Lock = LockState.NONE;
	public AttributeMap Attributes { get; private set; } = new AttributeMap();
	public TimerMap Timers { get; private set; } = new TimerMap();
	public QueueTaskSet Queues = new PawnQueueTaskSet();
	public int[] EquipmentBonuses { get; private set; } = new int[14];
	public int PrayerIcon = -1;
	private int TransmogID = -1;
	private List<Hit> PendingHits = new List<Hit>();
	public DamageMap DamageMap { get; private set; } = new DamageMap();
	public bool Invisible = false;
	private FutureRoute? FutureRoute;
	
	public World World { get; private set; }
	
	public Pawn(World world)
	{
		World = world;
		MovementQueue = new MovementQueue(this);
	}
	
	public abstract void Cycle();
	public bool IsDead() => GetCurrentHP() == 0;
	public bool IsAlive() => !IsDead();
	public abstract bool IsRunning();
	public abstract int GetSize();
	public abstract int GetCurrentHP();
	public abstract int GetMaxHP();
	public abstract void SetCurrentHP(int level);
	public abstract void AddBlock(UpdateBlockType block);
	public abstract bool HasBlock(UpdateBlockType block);
	
	public void FullLock() => Lock = LockState.FULL;
	public void Unlock() => Lock = LockState.NONE;
	public bool IsLocked() => Lock != LockState.NONE;
	
	public int GetTransmogID() => TransmogID;
	
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
	
	public Tile GetFrontFacingTile(Pawn target, int offset = 0)
	{
		return GetFrontFacingTile(target.GetCenterTile(), offset);
	}
	
	public void Attack(Pawn target)
	{
		ResetInteractions();
		InterruptQueues();
		
		Attributes[Attr.Attributes.COMBAT_TARGET_FOCUS_ATTR] = new WeakReference(target);
		if (EntityType.IsPlayer || this is Npc && !World.Plugins.ExecuteNpcCombat(this)) {
			World.Plugins.ExecuteCombat(this);
		}
	}
	
	public void AddHit(Hit hit) => PendingHits.Add(hit);
	public void ClearHits() => PendingHits.Clear();
	
	public void TimerCycle()
	{
		List<TimerKey> toRemove = new List<TimerKey>();
		foreach (KeyValuePair<TimerKey, int> timers in Timers.Timers)
		{
			TimerKey key = timers.Key;
			int time = timers.Value;
			if (time <= 0)
			{
				if (key == RESET_PAWN_FACING_TIMER)
				{
					ResetFacePawn();
				}
				else
				{
					World.Plugins.ExecuteTimer(this, key);
				}
				if (!Timers.Has(key))
				{
					toRemove.Add(key);
				}
			}
		}
		for (int i=0; i<toRemove.Count; i++)
		{
			Timers.Timers.Remove(toRemove[i]);
		}
	}
	
	// TODO: implement this
	public void HitsCycle()
	{
		List<Hit> toRemove = new List<Hit>();
		foreach (Hit hit in PendingHits)
		{
			if (IsDead())
				break;
			
			if (Lock.DelaysDamage())
			{
				hit.DamageDelay = Math.Max(0, hit.DamageDelay - 1);
				continue;
			}
			
			if (hit.DamageDelay-- == 0)
			{
				if (!hit.CancelCondition())
				{
					BlockBuffer.Hits.Add(hit);
					AddBlock(UpdateBlockType.HITMARK);
					
					foreach (Hitmark hitmark in hit.Hitmarks)
					{
						int hp = GetCurrentHP();
						if (hitmark.Damage > hp)
						{
							hitmark.Damage = hp;
						}
						
						if (BitStorages.INFINITE_VARS_STORAGE.Get(this, InfiniteVarsType.HP) == 0)
						{
							SetCurrentHP(hp - hitmark.Damage);
						}
						
						if (GetCurrentHP() <= 0)
						{
							for (int i=0; i<hit.Actions.Count; i++)
							{
								// TODO: not sure if correct
								hit.Actions[i] = new Action(hit);
							}
							if (EntityType.IsPlayer())
							{
								ExecutePlugin(PlayerDeathAction.DeathPlugin);
							}
							else
							{
								ExecutePlugin(NpcDeathAction.DeathPlugin);
							}
							toRemove.Add(hit);
							goto End;
						}
					}
					for (int i=0; i<hit.Actions.Count; i++)
					{
						// TODO: not sure if correct
						hit.Actions[i] = new Action(hit);
					}
				}
				toRemove.Add(hit);
			}
		}
	End:
		if (IsDead() && PendingHits.Count > 0)
		{
			PendingHits.Clear();
		}
	}
	
	public void HandleFutureRoute()
	{
		if (FutureRoute?.IsCompleted == true && FutureRoute?.Strategy?.Cancel == false)
		{
			WalkPath(FutureRoute.Route!.Path, FutureRoute.StepType, FutureRoute.DetectCollision);
			FutureRoute = null;
		}
	}
	
	public void WalkPath(Queue<Tile> path, StepType stepType, bool detectCollision)
	{
		if (path.Count == 0)
		{
			if (this is Player)
			{
				Write(SetMapFlagMessage(255, 255));
			}
			return;
		}
		
		if (Timers.Has(Timer.Timers.FROZEN_TIMER))
		{
			if (this is Player)
			{
				WriteMessage(MAGIC_STOPS_YOU_FROM_MOVING);
			}
			return;
		}
		
		if (Timers.Has(Timer.Timers.STUN_TIMER))
		{
			return;
		}
		
		MovementQueue.Clear();
		
		Tile? tail = null;
		path.TryDequeue(out Tile? next);
		while (next != null)
		{
			MovementQueue.AddStep(next, stepType, detectCollision);
			path.TryDequeue(out Tile? poll);
			if (poll == null)
			{
				tail = next;
			}
			next = poll;
		}
		
		if (tail == null || tail.SameAs(Tile!))
		{
			if (this is Player)
			{
				Write(SetMapFlagMessage(255, 255));
			}
			MovementQueue.Clear();
			return;
		}
		
		if (this is Player && LastKnownRegionBase != null)
		{
			Write(SetMapFlagMessage(tail.X - LastKnownRegionBase.X, tail.Z - LastKnownRegionBase.Z));
		}
	}
	
	public void WalkTo(int x, int z, StepType stepType = StepType.NORMAL, bool detectCollision = true)
	{
		if (Tile.X == x && Tile.Z == z)
			return;
		
		if (Timers.Has(Timer.Timers.FROZEN_TIMER))
		{
			if (this is Player)
			{
				WriteMessage(MAGIC_STOPS_YOU_FROM_MOVING);
			}
			return;
		}
		
		if (Timers.Has(Timer.Timers.STUN_TIMER))
		{
			return;
		}
		
		var multiThread = World.MultiThreadPathfinding;
		var request = PathRequest.CreateWalkRequest(this, x, z, false, detectCollision);
		var strategy = CreatePathFindingStrategy(multiThread);
		
		if (multiThread)
		{
			MovementQueue.Clear();
		}
		FutureRoute?.Strategy?.Cancel = true;
		
		if (multiThread)
		{
			FutureRoute = FutureRoute.of(strategy, request, stepType, detectCollision);
		}
		else
		{
			var route = strategy.CalculateRoute(request);
			WalkPath(route.Path, stepType, detectCollision);
		}
	}
	
	public void WalkTo(Tile tile, StepType stepType = StepType.NORMAL, bool detectCollision = true)
	{
		WalkTo(tile.X, tile.Z, stepType, detectCollision);
	}
	
	public async Task WalkTo(QueueTask it, Tile tile, StepType stepType = StepType.NORMAL, bool detectCollision = true)
	{
		WalkTo(it, tile.X, tile.Z, stepType, detectCollision);
	}
	
	public async Task WalkTo(QueueTask it, int x, int z, StepType stepType = StepType.NORMAL, bool detectCollision = true)
	{
		
	}
	
}
