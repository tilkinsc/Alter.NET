using DotNetty.Transport.Channels;
using Game.Model;
using Net.Codec.Login;

namespace Game.System;

class LoginSystem : ServerSystem
{
	
	private LoginService? LoginService;
	
	private World World;
	private IChannel Channel;
	
	public LoginSystem(IChannel channel, World world)
	{
		Channel = channel;
		World = world;
	}
	
	public override void ReceiveMessage<T>(IChannelHandlerContext ctx, T msg)
	{
		if (msg is LoginRequest) {
			if (LoginService == null) {
				LoginService = World.GetService(typeof(LoginService));
			}
			LoginService.AddLoginRequest(world, msg);
		}
	}
	
	public override void Terminate() {}
	
}