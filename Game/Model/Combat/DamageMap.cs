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

class DamageMap
{
	
	private Dictionary<Pawn, DamageStack> _map = new Dictionary<Pawn, DamageStack>(0);
	
	public void Add(Pawn pawn, int damage)
	{
		int total = 0;
		if (_map.TryGetValue(pawn, out DamageStack? val)) {
			total += val?.TotalDamage ?? 0;
		}
		_map[pawn] = new DamageStack(total, Time.CurrentTimeMillis());
	}
	
	public List<Pawn> GetAll(EntityType type, long? timeFrameMS = null)
	{
		List<Pawn> pawn = new List<Pawn>();
		foreach (KeyValuePair<Pawn, DamageStack> pairs in _map)
		{
			if (pairs.Key.EntityType != type)
				continue;
			if (timeFrameMS == null || (Time.CurrentTimeMillis() - pairs.Value.LastHit < timeFrameMS))
				pawn.Add(pairs.Key);
		}
		return pawn;
	}
	
	public int GetDamageFrom(Pawn pawn)
	{
		_map.TryGetValue(pawn, out DamageStack? damageStack);
		return damageStack?.TotalDamage ?? 0;
	}
	
	public Pawn? GetMostDamage()
	{
		Pawn? pawn = null;
		int biggestDamage = 0;
		foreach (KeyValuePair<Pawn, DamageStack> pairs in _map)
		{
			if (pairs.Value.TotalDamage > biggestDamage) {
				biggestDamage = pairs.Value.TotalDamage;
				pawn = pairs.Key;
			}
		}
		return pawn;
	}
	
	public Pawn? GetMostDamage(EntityType type, long? timeFrameMS = null)
	{
		Pawn? pawn = null;
		int biggestDamage = 0;
		foreach (KeyValuePair<Pawn, DamageStack> pairs in _map)
		{
			if (pairs.Key.EntityType != type)
				continue;
			if (timeFrameMS == null || Time.CurrentTimeMillis() - pairs.Value.LastHit < timeFrameMS)
			{
				if (pairs.Value.TotalDamage > biggestDamage) {
					biggestDamage = pairs.Value.TotalDamage;
					pawn = pairs.Key;
				}
			}
		}
		return pawn;
	}
	
}
