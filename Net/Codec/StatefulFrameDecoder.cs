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
	
	public abstract void Decode(ChannelHandlerContext, MemoryStream buf, List<object> output, T state);
	
	public override void Decode(ChannelHandlerContext ctx, MemoryStream buf, List<object> output)
	{
		Decode(ctx, buf, output, state);
	}
	
}