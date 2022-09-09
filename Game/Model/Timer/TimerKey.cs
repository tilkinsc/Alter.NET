namespace Game.Model.Timer;

class TimerKey : IEquatable<TimerKey>
{
	
	public string? PersistenceKey;
	public bool TickOffline;
	public bool ResetOnDeath;
	public bool Temporary;
	
	public TimerKey(string? persistenceKey = null, bool tickOffline = true, bool resetOnDeath = false, bool temp = false)
	{
		PersistenceKey = persistenceKey;
		TickOffline = tickOffline;
		ResetOnDeath = resetOnDeath;
		Temporary = temp;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as TimerKey);
	}
	
	public bool Equals(TimerKey? other)
	{
		if (other == null)
			return false;
		if (PersistenceKey != null) {
			return PersistenceKey == other.PersistenceKey && TickOffline == other.TickOffline && ResetOnDeath == other.ResetOnDeath;
		}
		return base.Equals(other);
	}
	
	public override int GetHashCode()
	{
		if (PersistenceKey == null) {
			return base.GetHashCode();
		}
		int hash = PersistenceKey.GetHashCode();
		hash = hash * 31 + TickOffline.GetHashCode();
		hash = hash * 31 + ResetOnDeath.GetHashCode();
		return hash;
	}
	
}
