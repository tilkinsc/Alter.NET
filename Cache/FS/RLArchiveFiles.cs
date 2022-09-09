using Exceptions;

namespace Cache.FS;

class RLArchiveFiles : IEquatable<RLArchiveFiles>
{
	
	public List<RLFSFile> Files = new List<RLFSFile>();
	public Dictionary<int, RLFSFile> FileMap = new Dictionary<int, RLFSFile>(); 
	
	public void AddFile(RLFSFile file)
	{
		if (file.FileID == -1)
			throw new IllegalArgumentException("Invalid FSFile");
		
		if (FileMap.ContainsKey(file.FileID))
			throw new IllegalStateException("Duplicate File id found");
		
		
		Files.Add(file);
		FileMap.Add(file.FileID, file);
	}
	
	public void LoadContents(byte[] data)
	{
		// assert !this.getFiles().isEmpty();
		
		
	}
	
	public void Clear()
	{
		Files.Clear();
		FileMap.Clear();
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLStore);
	}
	
	public bool Equals(RLArchiveFiles? files)
	{
		if (files == null)
			return false;
		if (Files.Equals(files))
			return true;
		return false;
	}
	
	public override int GetHashCode()
	{
		int hash = 7;
		hash = 67 * hash + this.Files.GetHashCode();
		return hash;
	}
	
}