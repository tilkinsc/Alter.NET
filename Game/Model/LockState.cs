namespace Game.Model;

enum LockState
{
	NONE = 0,
	DELAY_ACTIONS,
	FULL_WITH_ITEM_INTERACTION,
	FULL_WITH_DAMAGE_IMMUNITY,
	FULL_WITH_LOGOUT,
	FULL
}

static class LockStateExtensions
{
	public static bool CanLogout(this LockState state) => state == LockState.NONE || state == LockState.FULL_WITH_LOGOUT;
	public static bool CanMove(this LockState state) => state == LockState.NONE;
	public static bool CanAttack(this LockState state) => state == LockState.NONE;
	public static bool CanBeAttacked(this LockState state) => state != LockState.FULL_WITH_DAMAGE_IMMUNITY;
	public static bool CanDropItems(this LockState state) => state == LockState.NONE;
	public static bool CanGroundItemInteract(this LockState state) => state == LockState.NONE;
	public static bool CanNpcInteract(this LockState state) => state == LockState.NONE;
	public static bool CanPlayerInteract(this LockState state) => state == LockState.NONE;
	public static bool CanItemInteract(this LockState state) => state == LockState.NONE || state == LockState.FULL_WITH_ITEM_INTERACTION;
	public static bool CanInterfaceInteract(this LockState state) => state == LockState.NONE;
	public static bool CanUsePrayer(this LockState state) => state == LockState.NONE || state == LockState.DELAY_ACTIONS;
	public static bool CanRestoreRunEnergy(this LockState state) => state != LockState.DELAY_ACTIONS;
	public static bool CanTeleport(this LockState state) => state == LockState.NONE;
	
	public static bool DelaysPrayer(this LockState state) => state == LockState.DELAY_ACTIONS;
	public static bool DelaysDamage(this LockState state) => state == LockState.DELAY_ACTIONS;
}
