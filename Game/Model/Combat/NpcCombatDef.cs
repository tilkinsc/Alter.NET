namespace Game.Model.Combat;

class NpcCombatDef
{
	
	public const int DEFAULT_HITPOINTS = 10;
	public const int DEFAULT_ATTACK_SPEED = 4;
	public const int DEFAULT_RESPAWN_DELAY = 25;
	public const int DEFAULT_ATTACK_ANIMATION = 422;
	public const int DEFAULT_BLOCK_ANIMATION = 424;
	public const int DEFAULT_DEATH_ANIMATION = 836;
	
	public int Hitpoints;
	public List<int> Stats;
	public int AttackSpeed;
	public int AttackAnimation;
	public int BlockAnimation;
	public List<int> DeathAnimation;
	public int RespawnDelay;
	public int AggressiveRadius;
	public int AggroTargetDelay;
	public int AggressiveTimer;
	public double PoisonChance;
	public double VenomChance;
	public bool PoisonImmunity;
	public bool VenomImmunity;
	public int SlayerReq;
	public double SlayerXP;
	public List<int> Bonuses;
	public Set<Any> Species;
	
	public NpcCombatDef(int hitpoints, List<int> stats, int attackSpeed, int attackAnimation,
				int blockAnimation, List<int> deathAnimation, int respawnDelay, int aggressiveRadius,
				int aggroTargetDelay, int aggressiveTimer, double poisonChance, double venomChance,
				bool poisonImmunity, bool venomImmunity, int slayerReq, double slayerXP,
				List<int> bonuses, Set<Any> species
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
		new List<int> { 1, 1, 1, 1, 1},
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
		new Set<Any>()
	);
	
}