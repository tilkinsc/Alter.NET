using Game.Model.Entity;
using Game.Model.Entity;
using Util;

namespace Game.Model.Combat;

class DamageStack
{
	public int TotalDamage;
	public long LastHit;
	
	public DamageStack(int totalDamage, long lastHit)
	{
		TotalDamage = totalDamage;
		LastHit = lastHit;
	}
}

class DamageMap : Dictionary<Pawn, DamageStack>
{
	
	public int GetDamageFrom(Pawn pawn)
	{
		if (TryGetValue(pawn, out DamageStack? val)) {
			return val.TotalDamage;
		}
		return 0;
	}
	
	public void Add(Pawn pawn, int damage)
	{
		if (TryGetValue(pawn, out DamageStack? val)) {
			val.TotalDamage += damage;
			return;
		}
		this[pawn] = new DamageStack(damage, Time.CurrentTimeMillis());
	}
	
	public List<Pawn> GetAllByEntityType(EntityType type, long? timeFrameMS = null)
	{
		List<Pawn> pawn = new List<Pawn>();
		foreach (KeyValuePair<Pawn, DamageStack> pairs in this)
		{
			if (pairs.Key.EntityType != type)
				continue;
			if (timeFrameMS != null && Time.CurrentTimeMillis() - pairs.Value.LastHit > timeFrameMS)
				continue;
			pawn.Add(pairs.Key);
		}
		return pawn;
	}
	
	public Pawn? GetMostDamage()
	{
		Pawn? pawn = null;
		int currentDamage = 0;
		foreach (KeyValuePair<Pawn, DamageStack> pairs in this)
		{
			if (pairs.Value.TotalDamage > currentDamage) {
				currentDamage = pairs.Value.TotalDamage;
				pawn = pairs.Key;
			}
		}
		return pawn;
	}
	
	public Pawn? GetMostDamage(EntityType type, long? timeFrameMS = null)
	{
		Pawn? pawn = null;
		int currentDamage = 0;
		foreach (KeyValuePair<Pawn, DamageStack> pairs in this)
		{
			if (pairs.Key.EntityType != type)
				continue;
			if (timeFrameMS != null && Time.CurrentTimeMillis() - pairs.Value.LastHit > timeFrameMS)
				continue;
			if (pairs.Value.TotalDamage > currentDamage) {
				currentDamage = pairs.Value.TotalDamage;
				pawn = pairs.Key;
			}
		}
		return pawn;
	}
	
}
