namespace Game.Model.Container.Key;

static class ContainerKeys
{
	
	public static readonly ContainerKey BOND_POUCH_KEY = new ContainerKey("bonds", 200, ContainerStackType.NO_STACK);
	public static readonly ContainerKey INVENTORY_KEY = new ContainerKey("inventory", 28, ContainerStackType.NORMAL);
	public static readonly ContainerKey EQUIPMENT_KEY = new ContainerKey("equipment", 14, ContainerStackType.NORMAL);
	public static readonly ContainerKey BANK_KEY = new ContainerKey("bank", 800, ContainerStackType.STACK);
	
}