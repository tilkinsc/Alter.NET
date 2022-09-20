using DotNetty.Transport.Channels;

namespace Game.System;

abstract class ServerSystem
{
	
	public IChannel Channel;
	
	public ServerSystem(IChannel channel)
	{
		Channel = channel;
	}
	
	public abstract void ReceiveMessage<T>(IChannelHandlerContext ctx, T msg);
	
	public abstract void Terminate();
	
}