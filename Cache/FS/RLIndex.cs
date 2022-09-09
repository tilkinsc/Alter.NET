using Cache.Index;
using Cache.Util;

namespace Cache.FS;

class RLIndex : IEquatable<RLIndex>
{
	
	public readonly int ID;
	
	public int Protocol = 6;
	public bool Named = true;
	public int Revision;
	public int CRC;
	public int Compression;
	
	public List<RLArchive> Archives = new List<RLArchive>();
	
	public RLIndex(int id)
	{
		ID = id;
	}
	
	public override int GetHashCode()
	{
		int hash = 3;
		hash = 97 * hash + ID;
		hash = 97 * hash + Revision;
		hash = 97 * hash + Archives.GetHashCode();
		return hash;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLIndex);
	}
	
	public bool Equals(RLIndex? idx)
	{
		if (idx == null)
			return false;
		if (ID != idx.ID)
			return false;
		if (Revision != idx.Revision)
			return false;
		if (Archives.Equals(idx.Archives))
			return false;
		return true;
	}
	
	public RLArchive AddArchive(int id)
	{
		RLArchive archive = new RLArchive(this, id);
		Archives.Add(archive);
		return archive;
	}
	
	public RLArchive? GetArchive(int id)
	{
		foreach (RLArchive a in Archives)
		{
			if (a.ArchiveID == id)
				return a;
		}
		return null;
	}
	
	public RLArchive? FindArchiveByName(string name)
	{
		int hash = RLDjb2.Hash(name);
		foreach (RLArchive a in Archives)
		{
			if (a.NameHash == hash)
				return a;
		}
		return null;
	}
	
	public RLIndexData ToIndexData()
	{
		RLIndexData data = new RLIndexData();
		data.Protocol = Protocol;
		data.Revision = Revision;
		data.Named = Named;
		
		RLArchiveData[] archiveDatas = new RLArchiveData[Archives.Count];
		data.Archives = archiveDatas;
		
		int idx = 0;
		foreach (RLArchive archive in Archives)
		{
			RLArchiveData ad = archiveDatas[idx++] = new RLArchiveData();
			ad.ID = archive.ArchiveID;
			ad.NameHash = archive.NameHash;
			ad.CRC = archive.CRC;
			ad.Revision = archive.Revision;
			
			RLFileData[]? files = archive.FileData;
			ad.Files = files;
		}
		return data;
	}
	
}
