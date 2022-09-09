namespace Cache.Util;

class RLXtea
{
	
	public const int GOLDEN_RATIO = -1640531527;
	
	public const int ROUNDS = 32;
	
	public readonly int[] Key;
	
	public RLXtea(int[] key)
	{
		Key = key;
	}
	
	public byte[] Encrypt(byte[] data, int len)
	{
		MemoryStream buffer = new MemoryStream(data);
		MemoryStream output = new MemoryStream(len);
		
		BinaryReader stream = new BinaryReader(buffer);
		BinaryWriter stream2 = new BinaryWriter(output);
		
		int numBlocks = len / 8;
		for (int block=0; block<numBlocks; block++)
		{
			long v0 = stream.ReadInt32();
			long v1 = stream.ReadInt32();
			int sum = 0;
			for (int i=0; i<ROUNDS; i++)
			{
				v0 += (((v1 << 4) ^ ((long) ((ulong) v1 >> 5))) + v1) ^ (sum + Key[sum & 3]);
				sum += GOLDEN_RATIO;
				v1 += (((v0 << 4) ^ ((long) ((ulong) v0 >> 5))) + v0) ^ (sum + Key[((long) ((ulong) sum >> 11)) & 3]);
			}
			stream2.Write((int) v0);
			stream2.Write((int) v1);
		}
		stream2.Write(buffer.ToArray());
		return output.ToArray();
	}
	
	public byte[] Decrypt(byte[] data, int len)
	{
		MemoryStream buffer = new MemoryStream(data);
		MemoryStream output = new MemoryStream(len);
		
		BinaryReader stream = new BinaryReader(buffer);
		BinaryWriter stream2 = new BinaryWriter(output);
		
		int numBlocks = len / 8;
		for (int block=0; block<numBlocks; block++)
		{
			long v0 = stream.ReadInt32();
			long v1 = stream.ReadInt32();
			int sum = -957401312; // GOLDEN_RATIO * ROUNDS
			for (int i=0; i<ROUNDS; i++)
			{
				v1 -= (((v0 << 4) ^ ((long) ((ulong) v0 >> 5))) + v0) ^ (sum + Key[((long) ((ulong) sum >> 11)) & 3]);
				sum -= GOLDEN_RATIO;
				v0 -= (((v1 << 4) ^ ((long) ((ulong) v1 >> 5))) + v1) ^ (sum + Key[sum & 3]);
			}
			stream2.Write((int) v0);
			stream2.Write((int) v1);
		}
		stream2.Write(buffer.ToArray());
		return output.ToArray();
	}
	
}