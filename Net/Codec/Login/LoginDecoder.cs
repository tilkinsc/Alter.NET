using System.Numerics;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Netty;
using Util.IO;

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
		if (!buf.IsReadable())
			return;
		int opcode = buf.ReadByte();
		if (opcode == LOGIN_OPCODE || opcode == RECONNECT_OPCODE) {
			Reconnecting = opcode == RECONNECT_OPCODE;
			SetState(LoginDecoderState.HEADER);
		} else {
			IByteBuffer temp = ctx.Channel.Allocator.Buffer(1);
			temp.WriteByte((int) LoginResultType.BAD_SESSION_ID);
			ctx.WriteAndFlushAsync(temp).AddListener(ChannelFutureListener.CLOSE);
		}
	}
	
	private void DecodePayload(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		if (buf.ReadableBytes < PayloadLength)
			return;
		buf.MarkReaderIndex();
		
		IByteBuffer secureBuf;
		if (RSAExponent != null && RSAModulus != null) {
			ushort secureBufLength = buf.ReadUnsignedShort();
			IByteBuffer secure = buf.ReadBytes(secureBufLength);
			BigInteger rsaValue = BigInteger.ModPow(new BigInteger(secure.Array), (BigInteger) RSAExponent, (BigInteger) RSAModulus);
			secureBuf = Unpooled.WrappedBuffer(rsaValue.ToByteArray());
		} else {
			secureBuf = buf;
		}
		
		bool successfulEncryption = secureBuf.ReadByte() == 1;
		if (!successfulEncryption) {
			buf.ResetReaderIndex();
			buf.SkipBytes(PayloadLength);
			IByteBuffer temp = ctx.Channel.Allocator.Buffer(1);
			temp.WriteByte((int) LoginResultType.BAD_SESSION_ID);
			ctx.WriteAndFlushAsync(temp).AddListener(ChannelFutureListener.CLOSE);
			return;
		}
		
		int[] xteaKeys = new int[4] {
			secureBuf.ReadInt(),
			secureBuf.ReadInt(),
			secureBuf.ReadInt(),
			secureBuf.ReadInt()
		}
		long reportedSeed = secureBuf.ReadLong();
		
		int authCode = -1;
		string? password = null;
		int[] previousXteaKeys = new int[4];
		
		if (Reconnecting) {
			for (int i=0; i<previousXteaKeys.Length; i++) {
				previousXteaKeys[i] = secureBuf.ReadInt();
			}
			password = null;
		} else {
			switch (secureBuf.ReadByte()) {
				case 0:
				case 1:
					authCode = secureBuf.ReadUnsignedMedium();
					secureBuf.SkipBytes(1);
					break;
				case 2:
					secureBuf.SkipBytes(4);
					break;
				case 3:
					authCode = secureBuf.ReadInt();
					break;
			}
			secureBuf.SkipBytes(1);
			password = secureBuf.ReadString();
			
			IByteBuffer xteaBuf = buf.Decipher(xteaKeys);
			string username = xteaBuf.ReadString();
			
			if (reportedSeed != ServerSeed) {
				xteaBuf
			}
		}
	}
	
	private void DecodeHeader(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output)
	{
		if (buf.ReadableBytes >= 3)
			return;
		
		int size = buf.ReadUnsignedShort();
		if (buf.ReadableBytes >= size) {
			int revision = buf.ReadInt();
			buf.SkipBytes(4);
			buf.SkipBytes(1);
			buf.ReadByte();
			if (revision == ServerRevision) {
				PayloadLength = size - 16;
				DecodePayload(ctx, buf, output);
			} else {
				IByteBuffer temp = ctx.Channel.Allocator.Buffer(1);
				temp.WriteByte((int) LoginResultType.REVISION_MISMATCH);
				ctx.WriteAndFlushAsync(buf).AddListener(ChannelFutureListener.CLOSE);
			}
		} else {
			buf.ResetReaderIndex();
		}
	}
	
	public override void Decode(IChannelHandlerContext ctx, IByteBuffer buf, List<object> output, LoginDecoderState state)
	{
		buf.MarkReaderIndex();
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