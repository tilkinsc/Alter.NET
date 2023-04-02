namespace Game.Model.Priv;

class Privilege
{
	public const string DEV_POWER = "dev";
	public const string DONOR_POWER = "donor";
	public const string ADMIN_POWER = "admin";
	public const string OWNER_POWER = "owner";
	
	public static readonly Privilege DEFAULT = new Privilege(0, 0, "Player", new HashSet<string>());
	
	public int ID { get; private set; }
	public int Icon { get; private set; }
	public string Name { get; private set; }
	public HashSet<string> Powers { get; private set; }
	
	public Privilege(int id, int icon, string name, HashSet<string> powers)
	{
		ID = id;
		Icon = icon;
		Name = name;
		Powers = powers;
	}
	
}