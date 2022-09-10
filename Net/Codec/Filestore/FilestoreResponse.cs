namespace Net.Codec.Filestore;

class FilestoreResponse : IEquatable<FilestoreResponse>
{
	
	public int Index;
	public int Archive;
	public byte[] Data;
	
	public FilestoreResponse(int index, int archive, byte[] data)
	{
		Index = index;
		Archive = archive;
		Data = data;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as FilestoreResponse);
	}
	
	public bool Equals(FilestoreResponse? other)
	{
		if (other == null)
			return false;
		// TODO: does not check if Data contents is equal to other.Data contents
		if (Index != other.Index || Archive != other.Archive)
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		int hash = Index;
		hash = 31 * hash + Archive;
		// TODO: does not get the content hash code
		hash = 31 * hash + Data.GetHashCode();
		return hash;
	}
	
}