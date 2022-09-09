namespace Cache.FS.Jagex;

class RLIndexEntry : IEquatable<RLIndexEntry>
{
	
	public RLIndexFile IndexFile;
	public int ID;
	public int Sector;
	public int Length;
	
	public RLIndexEntry(RLIndexFile indexFile, int id, int sector, int length)
	{
		IndexFile = indexFile;
		ID = id;
		Sector = sector;
		Length = length;
	}
	
	public override int GetHashCode()
	{
		int hash = 7;
		hash = 19 * hash + IndexFile.GetHashCode();
		hash = 19 * hash + ID;
		hash = 19 * hash + Sector;
		hash = 19 * hash + Length;
		return hash;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLIndexEntry);
	}
	
	public bool Equals(RLIndexEntry? obj)
	{
		if (obj == null)
			return false;
		if (!IndexFile.Equals(obj))
			return false;
		if (ID != obj.ID || Sector != obj.Sector || Length != obj.Sector)
			return false;
		return true;
	}
	
}
