using Cache.Index;
using Cache.Util;

namespace Cache.FS.Jagex;


class RLDiskStorage : RLStorage
{
	
	public const string MAIN_FILE_CACHE_DAT = "main_file_cache.dat2";
	public const string MAIN_FILE_CACHE_IDX = "main_file_cache.idx";
	
	public string Folder;
	
	public RLDataFile Data;
	public RLIndexFile Index255;
	public List<RLIndexFile> IndexFiles = new List<RLIndexFile>();
	
	public RLDiskStorage(string folder)
	{
		Folder = folder;
		
		Data = new RLDataFile(new FileStream(Path.Join(Folder, MAIN_FILE_CACHE_DAT), FileMode.Open, FileAccess.ReadWrite));
		Index255 = new RLIndexFile(255, new FileStream(Path.Join(Folder, MAIN_FILE_CACHE_IDX), FileMode.Open, FileAccess.ReadWrite));
	}
	
	private RLIndexFile GetIndex(int i)
	{
		foreach (RLIndexFile idx in IndexFiles)
		{
			if (idx.IndexFileID == i) {
				return idx;
			}
		}
		
		RLIndexFile indexFile = new RLIndexFile(i, new FileStream(Path.Join(Folder, MAIN_FILE_CACHE_IDX + i), FileMode.Open, FileAccess.ReadWrite));
		IndexFiles.Add(indexFile);
		return indexFile;
	}
	
	public void Init(RLStore store)
	{
		for (int i=0; i<Index255.GetIndexCount(); i++)
		{
			store.AddIndex(i);
			GetIndex(i);
		}
		
		// assert store.GetIndexes().Size() == IndexFiles.Size();
	}
	
	public void Dispose()
	{
		Data.Dispose();
		Index255.Dispose();
		foreach (RLIndexFile indexFile in IndexFiles)
		{
			indexFile.Dispose();
		}
	}
	
	public byte[]? ReadIndex(int indexId)
	{
		RLIndexEntry? entry = Index255.Read(indexId);
		if (entry == null)
			return null;
		
		return Data.Read(Index255.IndexFileID, entry.ID, entry.Sector, entry.Length);
	}
	
	private void LoadIndex(RLIndex index)
	{
		byte[]? indexData = ReadIndex(index.ID);
		if (indexData == null)
			return;
		
		RLContainer? res = RLContainer.Decompress(indexData, null);
		if (res == null)
			return;
		byte[]? data = res.Data;
		if (data == null)
			return;
		
		RLIndexData id = new RLIndexData();
		id.Load(data);
		
		index.Protocol = id.Protocol;
		index.Revision = id.Revision;
		index.Named = id.Named;
		
		if (id.Archives == null)
			return;
		
		foreach (RLArchiveData ad in id.Archives)
		{
			RLArchive archive = index.AddArchive(ad.ID);
			archive.NameHash = ad.NameHash;
			archive.CRC = ad.CRC;
			archive.Revision = ad.Revision;
			archive.FileData = ad.Files;
			
			// assert ad.GetFiles().Length > 0;
		}
		
		index.CRC = res.CRC;
		index.Compression = res.Compression;
		// assert res.Revision == -1;
	}
	
	public void Load(RLStore store)
	{
		foreach (RLIndex index in store.Indexes)
		{
			LoadIndex(index);
		}
	}
	
	public byte[]? LoadArchive(RLArchive archive)
	{
		RLIndex index = archive.Index;
		RLIndexFile indexFile = GetIndex(index.ID);
		
		// assert indexFile.GetIndexFileId() == index.GetId();
		
		RLIndexEntry? entry = indexFile.Read(archive.ArchiveID);
		if (entry == null)
			return null;
		
		// assert entry.GetID() == archive.GetArchiveID();
		
		return Data.Read(index.ID, entry.ID, entry.Sector, entry.Length);
	}
	
	private void SaveIndex(RLIndex index)
	{
		RLIndexData indexData = index.ToIndexData();
		byte[]? data = indexData.WriteIndexData();
		if (data == null)
			return;
		
		RLContainer container = new RLContainer(index.Compression, -1);
		container.Compress(data, null);
		byte[]? compressedData = container.Data;
		if (compressedData == null)
			return;
		RLDataFileWriteResult res = Data.Write(Index255.IndexFileID, index.ID, compressedData);
		
		Index255.Write(new RLIndexEntry(Index255, index.ID, res.Sector, res.CompressedLength));
		
		RLCRC32 crc = new RLCRC32();
		crc.Update(compressedData, 0, compressedData.Length);
		index.CRC = crc.Finalize();
	}
	
	
	public void Save(RLStore save)
	{
		foreach (RLIndex i in save.Indexes)
		{
			SaveIndex(i);
		}
	}

	public void SaveArchive(RLArchive archive, byte[] data)
	{
		RLIndex index = archive.Index;
		RLIndexFile indexFile = GetIndex(index.ID);
		// assert indexFile.GetIndexFieldID() == index.GetID();
		
		RLDataFileWriteResult res = Data.Write(index.ID, archive.ArchiveID, data);
		indexFile.Write(new RLIndexEntry(indexFile, archive.ArchiveID, res.Sector, res.CompressedLength));
		
		byte compression = data[0];
		int compressedSize = BitConverter.ToInt32(data, 1);
		
		int length = 1 // compression type
				+ 4    // compressed size
				+ compressedSize
				+ (compression != RLCompressionType.NONE ? 4 : 0);
		
		RLCRC32 crc = new RLCRC32();
		crc.Update(data, 0, length);
		archive.CRC = crc.Finalize();
	}
	
	
	
}