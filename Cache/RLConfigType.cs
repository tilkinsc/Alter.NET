namespace Cache;


enum RLConfigType
{
	UNDERLAY = 1,
	IDENTKIT = 3,
	OVERLAY = 4,
	INV = 5,
	OBJECT = 6,
	ENUM = 8,
	NPC = 9,
	ITEM = 10,
	PARAMS = 11,
	SEQUENCE = 12,
	SPOTANIM = 13,
	VARBIT = 14,
	VARCLIENT = 19,
	VARCLIENTSTRING = 15,
	VARPLAYER = 16,
	HITSPLAT = 32,
	HEALTHBAR = 33,
	STRUCT = 34,
	AREA = 35,
	DBROW = 38,
	DBTABLE = 39
}

static class RLConfigTypeExtensions
{
	
	public static int GetID(this RLConfigType type) => (int) type;
	public static int GetID(this RLConfigType? type) => type == null ? 0 : (int) type;
	
}
