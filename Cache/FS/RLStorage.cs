namespace Cache.FS;

interface RLStorage : IDisposable
{
	
	public void Init(RLStore store);
	
	public void Load(RLStore store);
	public void Save(RLStore save);
	
	public byte[]? LoadArchive(RLArchive archive);
	
	public void SaveArchive(RLArchive archive, byte[] data);
	
}