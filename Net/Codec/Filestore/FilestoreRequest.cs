namespace Net.Codec.Filestore;

class FilestoreRequest
{
	
	public int Index;
	public int Archive;
	public bool Priority;
	
	public FilestoreRequest(int index, int archive, bool priority)
	{
		Index = index;
		Archive = archive;
		Priority = priority;
	}
	
}