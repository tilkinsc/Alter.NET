using Cache.Index;
using Exceptions;

namespace Cache.FS;

class RLArchive : IEquatable<RLArchive>
{
	
	public readonly RLIndex Index;
	
	public readonly int ArchiveID;
	
	public int NameHash;
	public int CRC;
	public int Revision;
	public int Compression;
	public RLFileData[]? FileData;
	public byte[]? Hash;
	
	public RLArchive(RLIndex index, int id)
	{
		this.Index = index;
		this.ArchiveID = id;
	}
	
	public override int GetHashCode()
	{
		int hash = 7;
		hash = 47 * hash + ArchiveID;
		hash = 47 * hash + NameHash;
		hash = 47 * hash + Revision;
		return hash;
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLArchive);
	}
	
	public bool Equals(RLArchive? arc)
	{
		if (arc == null)
			return false;
		if (ArchiveID != arc.ArchiveID)
			return false;
		if (NameHash != arc.NameHash)
			return false;
		if (Revision != arc.Revision)
			return false;
		return true;
	}
	
	public byte[]? Decompress(byte[]? data)
	{
		return Decompress(data, null);
	}
	
	public byte[]? Decompress(byte[]? data, int[]? keys)
	{
		if (data == null)
			return null;
		
		byte[]? encryptedData = data;
		
		RLContainer? container = RLContainer.Decompress(encryptedData, keys);
		if (container == null) {
			// logger.warn($"Unable to decrypt archive {this}");
			return null;
		}
		
		byte[]? decompressedData = container.Data;
		
		if (CRC != container.CRC) {
			// logger.warn("CRC mismatch for archive {Index.ID}/{ArchiveID}");
			throw new IOException($"CRC mismatch for archive {Index.ID}/{ArchiveID}");
		}
		
		if (container.Revision != -1 && Revision != container.Revision) {
			// logger.warn($"revision mismatch for archive {Index.ID}/{ArchiveID}, expected {Revision} was {container.Revision}");
			Revision = container.Revision;
		}
		
		Compression = container.Compression;
		return decompressedData;
	}
	
	public RLArchiveFiles GetFiles(byte[] data)
	{
		return GetFiles(data, null);
	}
	
	public RLArchiveFiles GetFiles(byte[]? data, int[]? keys)
	{
		byte[]? decompressedData = Decompress(data, keys);
		if (FileData == null)
			throw new IllegalStateException("FileData was null");
		if (decompressedData == null)
			throw new IllegalArgumentException("Could not decompress data");
		
		RLArchiveFiles files = new RLArchiveFiles();
		foreach (RLFileData fileEntry in FileData)
		{
			RLFSFile file = new RLFSFile(fileEntry.ID);
			file.NameHash = fileEntry.NameHash;
			files.AddFile(file);
		}
		files.LoadContents(decompressedData);
		return files;
	}
	
}