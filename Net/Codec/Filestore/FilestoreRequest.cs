namespace Net.Codec.Filestore;

class FilestoreRequest
{
	
	public int Index { get; private set; }
	public int Archive { get; private set; }
	public bool Priority { get; private set; }
	
	public FilestoreRequest(int index, int archive, bool priority)
	{
		Index = index;
		Archive = archive;
		Priority = priority;
	}
	
}