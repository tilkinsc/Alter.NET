using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Net.Codec.Login;
using Util;

namespace Net.Codec.Filestore;

class FilestoreDecoder : StatefulFrameDecoder<FilestoreDecoderState>
{
	
	public const int ARCHIVE_REQUEST_URGENT = 0;
	public const int ARCHIVE_REQUEST_NEUTRAL = 1;
	public const int CLIENT_INIT_GAME = 2;
	public const int CLIENT_LOAD_SCREEN = 3;
	public const int CLIENT_INIT_OPCODE = 6;
	
	private int ServerRevision;
	
	private long BufferPosition = 0;
	
	public FilestoreDecoder(int serverRevision)
			: base(FilestoreDecoderState.REVISION_REQUEST)
	{
		ServerRevision = serverRevision;
	}
	
	private void DecodeRevisionRequest(IChannelHandlerContext ctx, IByteBuffer buf)
	{
		BinaryReader stream = new BinaryReader(buf);
		if (stream.GetRemaining() >= 4) {
			int revision = stream.ReadInt32();
			if (revision != ServerRevision) {
				ctx.WriteAndFlush(LoginResultType.REVISION_MISMATCH).AddListener(ChannelFutureListener.CLOSE);
			} else {
				SetState(FilestoreDecoderState.ARCHIVE_REQUEST);
				ctx.WriteAndFlush(LoginResultType.ACCEPTABLE);
			}
		}
	}
	
	private void DecodeArchiveRequest(IByteBuffer buf, List<object> output)
	{
		BinaryReader stream = new BinaryReader(buf);
		if (!stream.IsReadable())
			return;
		
		BufferPosition = buf.Position;
		int opcode = stream.ReadByte();
		switch (opcode)
		{
			case CLIENT_INIT_GAME:
			case CLIENT_LOAD_SCREEN:
			case CLIENT_INIT_OPCODE:
				buf.Seek(3, SeekOrigin.Current);
				break;
			case ARCHIVE_REQUEST_NEUTRAL:
			case ARCHIVE_REQUEST_URGENT:
				if (stream.GetRemaining() >= 3) {
					int index = stream.ReadByte();
					int archive = stream.ReadUInt16();
					
					FilestoreRequest request = new FilestoreRequest(index, archive, opcode == ARCHIVE_REQUEST_URGENT);
					output.Add(request);
				} else {
					buf.Position = BufferPosition;
				}
				break;
		}
	}
	
	public override void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output, FilestoreDecoderState state)
	{
		switch (state)
		{
			case FilestoreDecoderState.REVISION_REQUEST:
				DecodeRevisionRequest(ctx, buf);
				break;
			case FilestoreDecoderState.ARCHIVE_REQUEST:
				DecodeArchiveRequest(buf, output);
				break;
			
		}
	}
	
}