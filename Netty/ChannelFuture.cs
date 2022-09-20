using DotNetty.Transport.Channels;

namespace Netty;

interface ChannelFuture : IFuture
{
	
	public IChannel Channel();
	
	public bool IsVoid();
	
}