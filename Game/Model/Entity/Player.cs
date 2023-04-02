using Game.Model.Container;
using Game.Model.Priv;
using Game.Model.Varp;

namespace Game.Model.Entity;

class Player : Pawn
{
	
	public const int NORMAL_VIEW_DISTANCE = 15;
	public const int LARGE_VIEW_DISTANCE = 127;
	public const int TILE_VIEW_DISTANCE = 32;
	
	public string Username = "";
	public Privilege Privilege = Privilege.DEFAULT;
	public Coordinate? LastKnownRegionBase;
	public bool Initiated;
	public int LastIndex = -1;
	public int BountyPoints = 0;
	public PlayerUID? UID;
	public ItemContainer Bonds = new ItemContainer(World.Definitions, BOND_POUCH_KEY);
	public ItemContainer Inventory = new ItemContainer(World.Definitions, INVENTORY_KEY);
	public ItemContainer Equipment = new ItemContainer(World.Definitions, EQUIPMENT_KEY);
	public ItemContainer Bank = new ItemContainer(World.Definitions, BANK_KEY);
	public Dictionary<ContainerKey, ItemContainer> Containers = new Dictionary<ContainerKey, ItemContainer>();
	public InterfaceSet Interfaces = new InterfaceSet(this, World.Plugins);
	public VarpSet Varps = new VarpSet(World.Definitions.GetCount(typeof(VarpDef)));
	public List<String?> Options = new List<string?>(10);
	public bool ShopDirty;
	public bool LargeViewport;
	public Player[] GPILocalPlayers = new Player[2048];
	public int[] GPILocalIndexes = new int[2048];
	public int GPILocalCount;
	public int[] GPIExternalIndexes = new int[2048];
	public int GPIExternalCount;
	public int[] GPIInactivityFlags = new int[2048];
	public int[] GPITileHashMultipliers = new int[2048];
	public List<Npc> LocalNPCs = new List<Npc>();
	public Appearance.Appearance Appearance = Model.Appearance.Appearance.DEFAULT_MALE;
	public int[] Animations = new int[] { 808, 823, 819, 820, 821, 822, 824 };
	public double Weight;
	public int SkillIcon = -1;
	public double RunEnergy = 100.0;
	public int CombatLevel = 3;
	public int GameMode = 0;
	public double XPRate = 1.0;
	public int LastMapBuildTime = 0;
	public Social Social = new Social();
	
	public bool IsOnline { get => Index > 0; }
	
	private volatile bool PendingLogout;
	private volatile bool SetDisconnectionTimer;
	private SkillSet Skillset = new SkillSet(World.GameContext.SkillCount);
	private bool Appearimation;
	
	
	public Player(World world)
			: base(world)
	{
		Containers.Add(BOND_POUCH_KEY, Bonds);
		Containers.Add(INVENTORY_KEY, Inventory);
		Containers.Add(EQUIPMENT_KEY, Equipment);
		Containers.Add(BANK_KEY, Bank);
		EntityType = EntityType.PLAYER;
		
	}
	
	
	
}