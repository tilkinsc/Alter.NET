using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Net.Codec;

abstract class StatefulFrameDecoder<T> : ByteToMessageDecoder where T : System.Enum
{
	
	private T State;
	
	public StatefulFrameDecoder(T state)
	{
		State = state;
	}
	
	public void SetState(T state)
	{
		State = state;
	}
	
	public abstract void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output, T state);
	
	protected override void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		Decode(ctx, buf, output, State);
	}
	
}