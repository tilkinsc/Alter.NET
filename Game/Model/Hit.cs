namespace Game.Model;

class Hit
{
	
	public class Hitmark
	{
		public int Type;
		public int Damage;
		
		public Hitmark(int type, int damage)
		{
			Type = type;
			Damage = damage;
		}
	}
	
	public int HitbarType = 0;
	public int HitbarPercentage = -1;
	public int HitbarMaxPercentage = -1;
	public int HitbarDepleteSpeed = 0;
	public int HitbarDelay = 0;
	
	public bool HideHitbar = false;
	
	public int ClientDelay = 0;
	public int DamageDelay = 0;
		
	public List<Hitmark> Hitmarks;
	
	public Hit(List<Hitmark> hitmarks, int clientDelay = 0, int damageDelay = 0)
	{
		Hitmarks = hitmarks;
		ClientDelay = clientDelay;
		DamageDelay = damageDelay;
	}
	
	// TODO: Enforce hitmark values
	public bool Validate()
	{
		return true;
	}
	
	// TODO: reinstate builder
	.
	
}
