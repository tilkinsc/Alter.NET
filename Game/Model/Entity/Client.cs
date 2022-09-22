using DotNetty.Transport.Channels;
using Game.Message;
using Net.Codec.Login;

namespace Game.Model.Entity;

class Client : Player
{
	
	public static Client FromRequest(World world, LoginRequest request)
	{
		Client client = new Client(request.Channel, world);
		client.ClientWidth = request.ClientWidth;
		client.ClientHeight = request.ClientHeight;
		client.LoginUsername = request.Username;
		client.Username = request.Username;
		client.UUID = request.UUID;
		client.CurrentXteaKeys = request.XteaKeys;
		return client;
	}
	
	public GameSystem GameSystem;
	public string LoginUsername;
	public string PasswordHash;
	public string UUID;
	public int[] CurrentXteaKeys;
	
	public bool AppletFocused = true;
	public int ClientWidth = 765;
	public int ClientHeight = 503;
	public int CameraPitch = 0;
	public int CameraYaw = 0;
	public bool LogPackets = true;
	
	public IChannel Channel;
	
	public Client(IChannel channel, World world)
			: base(world)
	{
		Channel = channel;
		EntityType = EntityType.CLIENT;
	}
	
	public override void HandleLogout()
	{
		base.HandleLogout();
		World.GetService<PlayerSerializerService>(true).SaveClientData(this);
	}
	
	public override void HandleMessages() => GameSystem.HandleMessages();
	
	public override void Write(params IMessage[] messages)
	{
		foreach (IMessage msg in messages)
		{
			GameSystem.Write(msg);
		}
	}
	
	public override void Write(params object[] messages)
	{
		foreach (object msg in messages)
		{
			Channel.WriteAsync(msg);
		}
	}
	
	public override void ChannelFlush()
	{
		GameSystem.Flush();
	}
	
	public override void ChannelClose()
	{
		GameSystem.Close();
	}
	
	public override string ToString()
	{
		return "ToString not implemented";
	}
	
}