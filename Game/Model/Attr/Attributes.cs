using Game.Model.Container;
using Game.Model.Entity;

namespace Game.Model.Attr;

static class Attributes
{
	public static AttributeKey<string> LAST_LOGIN_ATTR = new AttributeKey<string>("last_login");
	public static AttributeKey<string> MEMBERS_EXPIRES_ATTR = new AttributeKey<string>("members_expires");
	public static AttributeKey<bool> NEW_ACCOUNT_ATTR = new AttributeKey<bool>();
	public static AttributeKey<string> FREE_BOND_CLAIMED_ATTR = new AttributeKey<string>("bond_claimed");
	public static AttributeKey<bool> APPEARANCE_SET_ATTR = new AttributeKey<bool>("appearance_set");
	public static AttributeKey<bool> NO_CLIP_ATTR = new AttributeKey<bool>();
	public static AttributeKey<bool> PROTECT_ITEM_ATTR = new AttributeKey<bool>();
	public static AttributeKey<int> DISPLAY_MODE_CHANGE_ATTR = new AttributeKey<int>();
	public static AttributeKey<int> RESET_FACING_PAWN_DISTANCE_ATTR = new AttributeKey<int>();
	public static AttributeKey<WeakReference<Pawn>> FACING_PAWN_ATTR = new AttributeKey<WeakReference<Pawn>>();
	public static AttributeKey<WeakReference<Npc>> NPC_FACING_US_ATTR = new AttributeKey<WeakReference<Npc>>();
	public static AttributeKey<Shop.Shop> CURRENT_SHOP_ATTR = new AttributeKey<Shop.Shop>();
	public static AttributeKey<WeakReference<Pawn>> COMBAT_TARGET_FOCUS_ATTR = new AttributeKey<WeakReference<Pawn>>();
	public static AttributeKey<WeakReference<Pawn>> KILLER_ATTR = new AttributeKey<WeakReference<Pawn>>();
	public static AttributeKey<WeakReference<Pawn>> LAST_HIT_ATTR = new AttributeKey<WeakReference<Pawn>>();
	public static AttributeKey<WeakReference<Pawn>> LAST_HIT_BY_ATTR = new AttributeKey<WeakReference<Pawn>>();
	public static AttributeKey<int> POISON_TICKS_LEFT_ATTR = new AttributeKey<int>(persistenceKey: "poison_ticks_left", resetOnDeath: true);
	public static AttributeKey<int> ANTIFIRE_POTION_CHARGES_ATTR = new AttributeKey<int>(persistenceKey: "antifire_potion_charges", resetOnDeath: true);
	public static AttributeKey<bool> DRAGONFIRE_IMMUNITY_ATTR = new AttributeKey<bool>(persistenceKey: "dragonfire_immunity", resetOnDeath: true);
	public static AttributeKey<string> COMMAND_ATTR = new AttributeKey<string>();
	public static AttributeKey<List<string>> COMMAND_ARGS_ATTR = new AttributeKey<List<string>>();
	public static AttributeKey<int> INTERACTING_OPT_ATTR = new AttributeKey<int>();
	public static AttributeKey<int> INTERACTING_SLOT_ATTR = new AttributeKey<int>();
	public static AttributeKey<WeakReference<GroundItem>> INTERACTING_GROUNDITEM_ATTR = new AttributeKey<WeakReference<GroundItem>>();
	public static AttributeKey<WeakReference<ItemTransaction>> GROUNDITEM_PICKUP_TRANSACTION = new AttributeKey<WeakReference<ItemTransaction>>();
	// TODO: removed the out keyword from GameObject
	public static AttributeKey<WeakReference<GameObject>> INTERACTING_OBJ_ATTR = new AttributeKey<WeakReference<GameObject>>();
	public static AttributeKey<WeakReference<Npc>> INTERACTING_NPC_ATTR = new AttributeKey<WeakReference<Npc>>();
	public static AttributeKey<WeakReference<Player>> INTERACTING_PLAYER_ATTR = new AttributeKey<WeakReference<Player>>();
	public static AttributeKey<int> INTERACTING_ITEM_SLOT = new AttributeKey<int>();
	public static AttributeKey<int> INTERACTING_ITEM_ID = new AttributeKey<int>();
	public static AttributeKey<WeakReference<Item.Item>> INTERACTING_ITEM = new AttributeKey<WeakReference<Item.Item>>();
	public static AttributeKey<int> OTHER_ITEM_SLOT_ATTR = new AttributeKey<int>();
	public static AttributeKey<int> OTHER_ITEM_ID_ATTR = new AttributeKey<int>();
	public static AttributeKey<WeakReference<Item.Item>> OTHER_ITEM_ATTR = new AttributeKey<WeakReference<Item.Item>>();
	public static AttributeKey<int> INTERACTING_COMPONENT_PARENT = new AttributeKey<int>();
	public static AttributeKey<int> INTERACTING_COMPONENT_CHILD = new AttributeKey<int>();
	public static AttributeKey<int> LEVEL_UP_SKILL_ID = new AttributeKey<int>();
	public static AttributeKey<int> LEVEL_UP_INCREMENT = new AttributeKey<int>();
	public static AttributeKey<Double> LEVEL_UP_OLD_XP = new AttributeKey<Double>();
}