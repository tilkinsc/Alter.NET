namespace Game.Model.Attr;

abstract class AttributeKey : IEquatable<AttributeKey>
{
	
	public string? PersistenceKey;
	public bool ResetOnDeath;
	public bool Temporary;
	
	public AttributeKey(string? persistenceKey = null, bool resetOnDeath = false, bool temp = false)
	{
		PersistenceKey = persistenceKey;
		ResetOnDeath = resetOnDeath;
		Temporary = temp;
	}
	
	public sealed override bool Equals(object? obj)
	{
		return Equals(obj as AttributeKey);
	}
	
	public bool Equals(AttributeKey? other)
	{
		if (other == null)
			return false;
		if (PersistenceKey != null) {
			return PersistenceKey == other.PersistenceKey && other.ResetOnDeath == ResetOnDeath;
		}
		return base.Equals(other);
	}
	
	public sealed override int GetHashCode()
	{
		if (PersistenceKey != null)
			return PersistenceKey.GetHashCode();
		return base.GetHashCode();
	}
	
	public override string ToString()
	{
		return "ToString for AttributeKey not implemented";
	}
	
}

class AttributeKey<T> : AttributeKey
{
	
	public AttributeKey(string? persistenceKey = null, bool resetOnDeath = false, bool temp = false)
		: base(persistenceKey, resetOnDeath, temp)
	{
	}
	
}
