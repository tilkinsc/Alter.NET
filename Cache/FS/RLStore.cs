using Cache.FS.Jagex;
using Exceptions;

namespace Cache.FS;

class RLStore : IDisposable, IEquatable<RLStore>
{
	
	public readonly RLStorage Storage;
	
	public List<RLIndex> Indexes = new List<RLIndex>();
	
	public RLStore(string folder)
	{
		Storage = new RLDiskStorage(folder);
		Storage.Init(this);
	}
	
	public RLStore(RLStorage storage)
	{
		this.Storage = storage;
		Storage.Init(this);
	}
	
	public void Dispose()
	{
		Storage.Dispose();
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as RLStore);
	}
	
	public bool Equals(RLStore? store)
	{
		if (store == null)
			return false;
		if (!Indexes.Equals(store.Indexes))
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		int hash = 5;
		hash = 79 * hash + Indexes.GetHashCode();
		return hash;
	}
	
	public RLIndex AddIndex(int id)
	{
		foreach (RLIndex i in Indexes)
		{
			if (i.ID == id)
				throw new IllegalArgumentException($"Index {id} already exists");
		}
		
		RLIndex index = new RLIndex(id);
		Indexes.Add(index);
		return index;
	}
	
	public void RemoveIndex(RLIndex index)
	{
		// assert Indexes.Contains(index);
		Indexes.Remove(index);
	}
	
	public void Load()
	{
		Storage.Load(this);
	}
	
	public void Save()
	{
		Storage.Save(this);
	}
	
	public RLIndex? GetIndex(RLIndexType type)
	{
		return FindIndex(type.GetID());
	}
	
	public RLIndex? FindIndex(int id)
	{
		foreach (RLIndex i in Indexes)
		{
			if (i.ID == id)
				return i;
		}
		return null;
	}
}
