using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Net.Codec.Login;
using Netty;

namespace Net.Codec.Filestore;

class FilestoreDecoder : StatefulFrameDecoder<FilestoreDecoderState>
{
	
	public const int ARCHIVE_REQUEST_URGENT = 0;
	public const int ARCHIVE_REQUEST_NEUTRAL = 1;
	public const int CLIENT_INIT_GAME = 2;
	public const int CLIENT_LOAD_SCREEN = 3;
	public const int CLIENT_INIT_OPCODE = 6;
	
	private int ServerRevision;
	
	public FilestoreDecoder(int serverRevision)
			: base(FilestoreDecoderState.REVISION_REQUEST)
	{
		ServerRevision = serverRevision;
	}
	
	private void DecodeRevisionRequest(IChannelHandlerContext ctx, IByteBuffer buf)
	{
		if (buf.ReadableBytes >= 4) {
			int revision = buf.ReadInt();
			if (revision != ServerRevision) {
				ctx.WriteAndFlushAsync(LoginResultType.REVISION_MISMATCH).ContinueWith(ChannelFutureListener.CLOSE.OperationComplete!, null);
			} else {
				SetState(FilestoreDecoderState.ARCHIVE_REQUEST);
				ctx.WriteAndFlushAsync(LoginResultType.ACCEPTABLE);
			}
		}
	}
	
	private void DecodeArchiveRequest(IByteBuffer buf, List<object> output)
	{
		if (!buf.IsReadable())
			return;
		
		buf.MarkReaderIndex();
		int opcode = buf.ReadByte();
		switch (opcode)
		{
			case CLIENT_INIT_GAME:
			case CLIENT_LOAD_SCREEN:
			case CLIENT_INIT_OPCODE:
				buf.SkipBytes(3);
				break;
			case ARCHIVE_REQUEST_NEUTRAL:
			case ARCHIVE_REQUEST_URGENT:
				if (buf.ReadableBytes >= 3) {
					int index = buf.ReadByte();
					int archive = buf.ReadUnsignedShort();
					
					FilestoreRequest request = new FilestoreRequest(index, archive, opcode == ARCHIVE_REQUEST_URGENT);
					output.Add(request);
				} else {
					buf.ResetReaderIndex();
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