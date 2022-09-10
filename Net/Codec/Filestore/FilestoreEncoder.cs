namespace Net.Codec.Filestore;

class FilestoreEncoder : MessageToByteEncoder<FilestoreResponse>
{
	
	public override void Encode(ChannelHandlerContext ctx, FilestoreResponse msg, MemoryStream output)
	{
		BinaryWriter writer = new BinaryWriter(output);
		writer.Write((byte) msg.Index);
		writer.Write((short) msg.Archive);
		
		foreach (byte b in msg.Data)
		{
			if (output.Position % 512 == 0) {
				writer.Write((sbyte) -1);
			}
			writer.Write(b);
		}
	}
	
}