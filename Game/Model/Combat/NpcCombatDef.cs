namespace Game.Model.Combat;

class NpcCombatDef
{
	
	public const int DEFAULT_HITPOINTS = 10;
	public const int DEFAULT_ATTACK_SPEED = 4;
	public const int DEFAULT_RESPAWN_DELAY = 25;
	public const int DEFAULT_ATTACK_ANIMATION = 422;
	public const int DEFAULT_BLOCK_ANIMATION = 424;
	public const int DEFAULT_DEATH_ANIMATION = 836;
	
	public int Hitpoints { get; private set; }
	public List<int> Stats { get; private set; }
	public int AttackSpeed { get; private set; }
	public int AttackAnimation { get; private set; }
	public int BlockAnimation { get; private set; }
	public List<int> DeathAnimation { get; private set; }
	public int RespawnDelay { get; private set; }
	public int AggressiveRadius { get; private set; }
	public int AggroTargetDelay { get; private set; }
	public int AggressiveTimer { get; private set; }
	public double PoisonChance { get; private set; }
	public double VenomChance { get; private set; }
	public bool PoisonImmunity { get; private set; }
	public bool VenomImmunity { get; private set; }
	public int SlayerReq { get; private set; }
	public double SlayerXP { get; private set; }
	public List<int> Bonuses { get; private set; }
	public HashSet<object> Species { get; private set; }
	
	public NpcCombatDef(int hitpoints, List<int> stats, int attackSpeed, int attackAnimation,
				int blockAnimation, List<int> deathAnimation, int respawnDelay, int aggressiveRadius,
				int aggroTargetDelay, int aggressiveTimer, double poisonChance, double venomChance,
				bool poisonImmunity, bool venomImmunity, int slayerReq, double slayerXP,
				List<int> bonuses, HashSet<object> species
			)
	{
		Hitpoints = hitpoints;
		Stats = stats;
		AttackSpeed = attackSpeed;
		AttackAnimation = attackAnimation;
		BlockAnimation = blockAnimation;
		DeathAnimation = deathAnimation;
		RespawnDelay = respawnDelay;
		AggressiveRadius = aggressiveRadius;
		AggroTargetDelay = aggroTargetDelay;
		AggressiveTimer = aggressiveTimer;
		PoisonChance = poisonChance;
		VenomChance = venomChance;
		PoisonImmunity = poisonImmunity;
		VenomImmunity = venomImmunity;
		SlayerReq = slayerReq;
		SlayerXP = slayerXP;
		Bonuses = bonuses;
		Species = species;
	}
	
	public static readonly NpcCombatDef DEFAULT = new NpcCombatDef(
		DEFAULT_HITPOINTS,
		new List<int> { 1, 1, 1, 1, 1 },
		DEFAULT_ATTACK_SPEED,
		DEFAULT_ATTACK_ANIMATION,
		DEFAULT_BLOCK_ANIMATION,
		new List<int> { DEFAULT_DEATH_ANIMATION },
		DEFAULT_RESPAWN_DELAY,
		0,
		0,
		0,
		0.0,
		0.0,
		false,
		false,
		1,
		0.0,
		new List<int>(),
		new HashSet<object>()
	);
	
}