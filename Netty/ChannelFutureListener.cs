using System.Threading.Channels;

namespace Netty;

class ChannelFutureListener : GenericFutureListener<ChannelFuture>
{
	
	public static ChannelFutureListener CLOSE = new ChannelFutureListener()
	{
		OperationComplete = (Task a, object? b) => {
			if (a == null)
				return;
			if (a is not ChannelFuture)
				return;
			ChannelFuture future = (ChannelFuture) a;
			future.Channel().CloseAsync();
		}
	};
	
	public static ChannelFutureListener CLOSE_ON_FAILURE = new ChannelFutureListener()
	{
		OperationComplete = (object? a, EventArgs b) => {
			if (a == null)
				return;
			if (a is not ChannelFuture)
				return;
			ChannelFuture future = (ChannelFuture) a;
			if (!future.IsSuccess()) {
				future.Channel().CloseAsync();
			}
		}
	};
	
	public static ChannelFutureListener FIRE_EXCEPTION_ON_FAILURE = new ChannelFutureListener()
	{
		OperationComplete = (object? a, EventArgs b) => {
			if (a == null)
				return;
			if (a is not ChannelFuture)
				return;
			ChannelFuture future = (ChannelFuture) a;
			if (!future.IsSuccess()) {
				future.Channel().Pipeline.FireExceptionCaught(future.Cause());
			}
		}
	};
	
}