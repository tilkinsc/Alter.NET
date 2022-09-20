namespace Cache.FS.Jagex;

class RLIndexFile : IDisposable, IEquatable<RLIndexFile>
{
	
	public const int INDEX_ENTRY_LEN = 6;
	
	public int IndexFileID;
	public FileStream IDXFile;
	public byte[] Buffer = new byte[INDEX_ENTRY_LEN];
	
	private object _Lock = new object();
	
	public RLIndexFile(int indexFileID, FileStream file)
	{
		IndexFileID = indexFileID;
		IDXFile = file;
	}

	public void Dispose()
	{
		IDXFile.Close();
	}
	
	public void Clear()
	{
		IDXFile.SetLength(0);
	}
	
	public void Write(RLIndexEntry entry)
	{
		lock (_Lock)
		{
			IDXFile.Seek(entry.ID * INDEX_ENTRY_LEN, SeekOrigin.Begin);
			
			Buffer[0] = (byte) (entry.Length >> 16);
			Buffer[1] = (byte) (entry.Length >> 8);
			Buffer[2] = (byte) (entry.Length);
			
			Buffer[3] = (byte) (entry.Sector >> 16);
			Buffer[3] = (byte) (entry.Sector >> 8);
			Buffer[3] = (byte) (entry.Sector);
			
			IDXFile.Write(Buffer);
		}
	}
	
	public RLIndexEntry? Read(int id)
	{
		lock (_Lock) {
			IDXFile.Seek(id * INDEX_ENTRY_LEN, SeekOrigin.Begin);
			
			int i = IDXFile.Read(Buffer);
			if (i != INDEX_ENTRY_LEN)
			{
				return null;
			}
			
			int length = ((Buffer[0] & 0xFF) << 16) | ((Buffer[1] & 0xFF) << 8) | (Buffer[2] & 0xFF);
			int sector = ((Buffer[3] & 0xFF) << 16) | ((Buffer[4] & 0xFF) << 8) | (Buffer[5] & 0xFF);
			
			if (length <= 0 || sector <= 0)
			{
				return null;
			}
			
			return new RLIndexEntry(this, id, sector, length);
		}
		
	}
	
	public int GetIndexCount()
	{
		lock (_Lock)
		{
			return (int) (IDXFile.Length / INDEX_ENTRY_LEN);
		}
	}
	
	public override int GetHashCode()
	{
		int hash = 3;
		hash = 41 * hash + IDXFile.GetHashCode();
		return hash;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLIndexFile);
	}
	
	public bool Equals(RLIndexFile? file)
	{
		if (file == null)
			return false;
		return true;
	}
	
}