using Exceptions;
using Game.FS.Def;
using Game.Model.Combat;
using Game.Sync.Block;

namespace Game.Model.Entity;

class Npc : Pawn
{
	public const int RESET_PAWN_FACE_DELAY = 25;
	public const int DEFAULT_NPC_STAT_COUNT = 5;
	
	private bool Active;
	public Player? Owner;
	public bool Respawns;
	public int WalkRadius;
	private int Hitpoints = 10;
	public NpcCombatDef CombatDef;
	public CombatClass CombatClass = CombatClass.MELEE;
	public AttackStyle AttackStyle = AttackStyle.CONTROLLED;
	public CombatStyle CombatStyle = CombatStyle.STAB;
	public Stats Stats;
	public Func<Npc, Player, bool> AggroCheck;
	public NpcDef Def;
	public string Name => Def.Name;
	public List<object> Species => CombatDef.Species;
	
	public int ID { get; private set; }
	public World World { get; private set; }
	public Tile SpawnTile { get; private set; }
	
	private Npc(int id, World world, Tile spawnTile)
			: base(world)
	{
		ID = id;
		World = world;
		SpawnTile = spawnTile;
		Tile = spawnTile;
		Stats = new Stats(World.GameContext.NPCStatCount);
		Def = World.Definitions.Get<NpcDef>(id);
		EntityType = EntityType.NPC;
	}
	
	public Npc(int id, Tile tile, World world)
			: this(id, world, new Tile(tile))
	{
		Tile = tile;
	}
	
	public Npc(Player owner, int id, Tile tile, World world)
			: this(id, world, new Tile(tile))
	{
		Tile = tile;
		Owner = owner;
	}
	
	public override bool IsRunning() => false;
	public override int GetSize() => World.Definitions.Get<NpcDef>(ID).Size;
	public override int GetCurrentHP() => Hitpoints;
	public override int GetMaxHP() => CombatDef.Hitpoints;
	public override void SetCurrentHP(int level) => Hitpoints = level;
	
	public override void AddBlock(UpdateBlockType block)
	{
		UpdateBlockStructure bits = World.NpcUpdateBlocks.UpdateBlocks[block];
		BlockBuffer.AddBit(bits.Bit);
	}
	
	public override bool HasBlock(UpdateBlockType block)
	{
		UpdateBlockStructure bits = World.NpcUpdateBlocks.UpdateBlocks[block];
		return BlockBuffer.HasBit(bits.Bit);
	}
	
	public override void Cycle()
	{
		if (!Timers.IsEmpty())
			TimerCycle();
		HitsCycle();
	}
	
	public int GetTransform(Player player)
	{
		if (Def.VarBit != -1) {
			VarBitDef varbitDef = World.Definitions.Get<VarBitDef>(Def.VarBit);
			int state = player.Varps.GetBit(varbitDef.Varp, varbitDef.StartBit, varbitDef.EndBit);
			return Def.Transforms[state];
		}
		if (Def.Varp != -1) {
			int state = player.Varps.GetState(Def.Varp);
			return Def.Transforms[state];
		}
		return ID;
	}
	
	public bool SetActive(bool active)
	{
		this.Active = active;
	}
	
	public bool IsActive => Active;
	
	public bool IsSpawned => Index > 0;
	
	class Stats
	{
		private int[] CurrentLevels;
		private int[] MaxLevels;
		private int NStats;
		
		public Stats(int nStats)
		{
			NStats = nStats;
			CurrentLevels = new int[NStats];
			Array.Fill(CurrentLevels, 1);
			MaxLevels = new int[NStats];
			Array.Fill(MaxLevels, 1);
		}
		
		public int GetCurrentLevel(int skill) => CurrentLevels[skill];
		public int GetMaxLevel(int skill) => CurrentLevels[skill];
		public void SetCurrentLevel(int skill, int level) => CurrentLevels[skill] = level;
		public void SetMaxLevel(int skill, int level) => MaxLevels[skill] = level;
		
		public void AlterCurrentLevel(int skill, int value, int capValue = 0)
		{
			if (capValue == 0 || capValue < 0 && value < 0 || capValue > 0 && value >= 0)
				throw new IllegalStateException("Cap value and alter value must always be the same signum (+ or -)");
			int curLevel = GetCurrentLevel(skill);
			int altered;
			if (capValue > 0) {
				altered = Math.Min(curLevel + value, GetMaxLevel(skill) + capValue);
			} else if (capValue < 0) {
				altered = Math.Max(curLevel + value, GetMaxLevel(skill) + capValue);
			} else {
				altered = Math.Min(curLevel, curLevel + value);
			}
			int newLevel = Math.Max(0, altered);
			if (newLevel != curLevel) {
				SetCurrentLevel(skill, newLevel);
			}
		}
		
		public void DecrementCurrentLevel(int skill, int value, bool capped)
		{
			AlterCurrentLevel(skill, -value, capped ? -value : 0);
		}
		
		public void IncrementCurrentLevel(int skill, int value, bool capped)
		{
			AlterCurrentLevel(skill, value, capped ? 0 : value);
		}
		
	}
	
}