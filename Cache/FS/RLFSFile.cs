namespace Cache.FS;

public class RLFSFile : IEquatable<RLFSFile>
{
	
	public int FileID;
	public int NameHash;
	public byte[]? Contents;
	
	public RLFSFile(int fileID)
	{
		FileID = fileID;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLFSFile);
	}
	
	public bool Equals(RLFSFile? files)
	{
		if (files == null)
			return false;
		if (FileID == files.FileID
				&& NameHash == files.NameHash
				&& (
					(Contents == null && files.Contents == null)
					|| Enumerable.SequenceEqual(Contents!, files.Contents!)
				))
			return true;
		return false;
	}
	
	public override int GetHashCode()
	{
		int hash = 7;
		hash = 97 * hash + FileID;
		hash = 97 * hash + NameHash;
		// TODO: doesnt get the hash of the contents
		hash = 97 * hash + Contents!.GetHashCode();
		return hash;
	}
	
}