using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Game.Model;
using Game.System;
using Net.Codec.Handshake;

namespace Game.Protocol;

class GameHandler : ChannelHandlerAdapter
{
	
	// TODO: ValueOf seems to be missing
	public static readonly AttributeKey<ServerSystem> SYSTEM_KEY = AttributeKey.ValueOf("system");
	
	private Store Filestore;
	private World World;
	
	public GameHandler(Store filestore, World world)
	{
		Filestore = filestore;
		World = world;
	}
	
	public override void ChannelInactive(IChannelHandlerContext ctx)
	{
		ServerSystem session = ctx.Channel.GetAttribute(SYSTEM_KEY).GetAndRemove();
		session.Terminate();
		ctx.Channel.CloseAsync();
	}
	
	public override void ChannelRead(IChannelHandlerContext ctx, object msg)
	{
		try {
			IAttribute<ServerSystem> attribute = ctx.Channel.GetAttribute(SYSTEM_KEY);
			ServerSystem? system = attribute.Get();
			if (system != null) {
				system.ReceiveMessage(ctx, msg);
			} else if (msg is HandshakeMessage) {
				HandshakeMessage message = (HandshakeMessage) msg;
				switch((HandshakeType) message.ID)
				{
					case HandshakeType.FILESTORE:
						attribute.Set(new FilestoreSystem(ctx.Channel, Filestore));
						break;
					case HandshakeType.LOGIN:
						attribute.Set(new LoginSystem(ctx.Channel, World));
						break;
				}
			}
		} catch (Exception e) {
			Console.Error.WriteLine($"Error reading message {msg} from channel {ctx.channel()} {e}");
		}
	}
	
	public override void ExceptionCaught(IChannelHandlerContext ctx, Exception cause)
	{
		// TODO: fill out
	}
	
}