using DotNetty.Transport.Channels;
using Game.Model;
using Game.Model.Entity;

namespace Game.System;

class GameSystem : ServerSystem
{
	
	public World World;
	public Client Client;
	public GameService Service;
	
	private 
	
	public GameSystem(IChannel channel, World world, Client client, GameService service)
			: base(channel)
	{
		World = world;
		Client = client;
		Service = service;
	}
	
	public override void ReceiveMessage<T>(IChannelHandlerContext ctx, T msg)
	{
		throw new NotImplementedException();
	}
	
	public override void Terminate()
	{
		throw new NotImplementedException();
	}
}