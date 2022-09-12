using System.Numerics;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Net.Codec.Login;

class LoginDecoder : StatefulFrameDecoder<LoginDecoderState>
{
	
	public const int LOGIN_OPCODE = 16;
	public const int RECONNECT_OPCODE = 18;
	public static readonly int[] CRCORDER = new int[21] {
		6, 15, 14, 7, 20,
		17, 8, 18, 2, 16,
		12, 9, 1, 0, 10,
		5, 19, 4, 3, 13,
		11
	};
	
	private int ServerRevision;
	private int[] CacheCrcs;
	private long ServerSeed;
	private BigInteger? RSAExponent;
	private BigInteger? RSAModulus;
	
	private int PayloadLength = -1;
	private bool Reconnecting = false;
	
	private long BufferPosition = 0;
	
	public LoginDecoder(int serverRevision, int[] cacheCrcs, long serverSeed,
						BigInteger? rsaExponent, BigInteger? rsaModulus)
			: base(LoginDecoderState.HANDSHAKE)
	{
		ServerRevision = serverRevision;
		CacheCrcs = cacheCrcs;
		ServerSeed = serverSeed;
		RSAExponent = rsaExponent;
		RSAModulus = rsaModulus;
	}
	
	private void DecodeHandshake(IChannelHandlerContext ctx, IByteBuffer buf)
	{
		BinaryReader stream = new BinaryReader(buf);
		if (buf.Length > 0) {
			int opcode = stream.ReadByte();
			if (opcode == LOGIN_OPCODE || opcode == RECONNECT_OPCODE) {
				Reconnecting = opcode == RECONNECT_OPCODE;
				SetState(LoginDecoderState.HEADER);
			} else {
				ctx.WriteResponse(LoginResultType.BAD_SESSION_ID);
			}
		}
	}
	
	private void DecodePayload(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		if (buf.Length >= PayloadLength) {
			BufferPosition = buf.Position;
			
			
		}
	}
	
	private void DecodeHeader(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		BinaryReader stream = new BinaryReader(buf);
		if (buf.Length >= 3) {
			int size = stream.ReadUInt16();
			if (buf.Length >= size) {
				int revision = stream.ReadInt32();
				stream.ReadInt32();
				stream.ReadByte();
				stream.ReadByte();
				if (revision == ServerRevision) {
					PayloadLength = size - (4 * 4);
					DecodePayload(ctx, buf, output);
				} else {
					ctx.WriteResponse(LoginResultType.REVISION_MISMATCH);
				}
			} else {
				buf.Seek(BufferPosition, SeekOrigin.Begin);
			}
		}
	}
	
	public override void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output, LoginDecoderState state)
	{
		BufferPosition = buf.Position;
		switch (state)
		{
			case LoginDecoderState.HANDSHAKE:
				DecodeHandshake(ctx, buf);
				break;
			case LoginDecoderState.HEADER:
				DecodeHeader(ctx, buf, output);
				break;
		}
	}
	
}