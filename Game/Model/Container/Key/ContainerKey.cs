namespace Game.Model.Container.Key;

class ContainerKey
{
	
	public string Name { get; private set; }
	public int Capacity { get; private set; }
	public ContainerStackType StackType { get; private set; }
	
	public ContainerKey(string name, int capacity, ContainerStackType stackType)
	{
		Name = name;
		Capacity = capacity;
		StackType = stackType;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as ContainerKey);
	}
	
	public bool Equals(ContainerKey? containerKey)
	{
		if (containerKey == null)
			return false;
		if (containerKey.Name == Name)
			return true;
		return false;
	}
	
	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
	
}