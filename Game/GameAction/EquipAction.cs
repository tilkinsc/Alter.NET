using Game.Model.Entity;

namespace Game.GameAction;

class EquipAction
{
	
	private static string[] abc = new string[] {
		"attack", "defence", "strength", "hitpoints", "ranged", "prayer",
		"magic", "cooking", "woodcutting", "fletching", "fishing", "firemaking",
		"crafting", "Smithing", "mining", "herblore", "agility", "thieving",
		"slayer", "farming", "runecrafting", "hunter", "construction"
	};
	
	public enum Result
	{
		UNHANDLED,
		PLUGIN,
		NO_FREE_SPACE,
		FAILED_REQUIREMENTS,
		SUCCESS,
		INVALID_ITEM
	}
	
	public Result Equip(Player p, Item item, int inventorySlot = -1)
	{
		
	}
	
}