using System.Numerics;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Game.Model;

namespace Game.Protocol;

class ClientChannelInitializer : ChannelInitializer<ISocketChannel>
{
	
	private int Revision;
	private BigInteger? RSAExponent;
	private BigInteger? RSAModulus;
	private Store Filestore;
	private World World;
	
	private GlobalTrafficShapingHandler GlobalTrafficHandler = new GlobalTrafficShapingHandler(Executors.NewSingleThreadScheduledExecutor(), 0, 0, 1000);
	
	private GameHandler Handler;
	
	public ClientChannelInitializer(int revision, BigInteger? rsaExponent, BigInteger? rsaModulus, Store filestore, World world)
	{
		Revision = revision;
		RSAExponent = rsaExponent;
		RSAModulus = rsaModulus;
		Filestore = filestore;
		World = world;
		Handler = new GameHandler(filestore, world);
	}
	
	protected override void InitChannel(ISocketChannel ch)
	{
		IChannelPipeline p = ch.Pipeline;
		val crcs = filestore.indexes.map { it.crc }.toIntArray();
		
		p.AddLast("global_traffic", GlobalTrafficHandler);
		p.AddLast("channel_traffic", new ChannelTrafficShapingHandler(0, 1024 * 5, 1000));
		p.AddLast("timeout", new IdleStateHandler(30, 0, 0));
		p.AddLast("handshake_encoder", new HandshakeEncoder());
		p.AddLast("handshake_decoder", new HandshakeDecoder(revision, crcs, rsaExponent, rsaModulus));
		p.AddLast("handler", Handler);
	}
	
}