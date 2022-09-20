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
			ctx.WriteAndFlushAsync(temp).ContinueWith(ChannelFutureListener.CLOSE.OperationComplete, null);
		}
	}
	
	private int[] DecodeCRCs(IByteBuffer xteaBuf)
	{
		int[] crcs = new int[CacheCrcs.Length];
		for (int i=0; i<CRCORDER.Length; i++)
		{
			int idx = CRCORDER[i];
			switch (idx)
			{
				case 7:
				case 2:
					crcs[idx] = xteaBuf.ReadIntIME();
					break;
				case 14:
				case 17:
				case 8:
				case 19:
				case 4:
				case 3:
					crcs[idx] = xteaBuf.ReadIntIME();
					break;
				case 6:
				case 20:
				case 18:
				case 16:
				case 12:
				case 9:
				case 10:
					crcs[idx] = xteaBuf.ReadIntLE();
					break;
				case 15:
				case 1:
				case 0:
				case 5:
				case 13:
				case 11:
					crcs[idx] = xteaBuf.ReadInt();
					break;
			}
		}
		return crcs;
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
			ctx.WriteAndFlushAsync(temp).ContinueWith(ChannelFutureListener.CLOSE.OperationComplete, null);
			return;
		}
		
		int[] xteaKeys = new int[4] {
			secureBuf.ReadInt(),
			secureBuf.ReadInt(),
			secureBuf.ReadInt(),
			secureBuf.ReadInt()
		};
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
			
			byte[] data = new byte[xteaKeys.Length];
			secureBuf.ReadBytes(data);
			IByteBuffer xteaBuf = Unpooled.WrappedBuffer(Xtea.Decipher(xteaKeys, data, 0, data.Length));
			string username = xteaBuf.ReadString();
			
			if (reportedSeed != ServerSeed) {
				xteaBuf.ResetReaderIndex();
				xteaBuf.SkipBytes(PayloadLength);
				IByteBuffer temp = ctx.Channel.Allocator.Buffer(1);
				temp.WriteByte((int) LoginResultType.BAD_SESSION_ID);
				ctx.WriteAndFlushAsync(temp).ContinueWith(ChannelFutureListener.CLOSE.OperationComplete, null);
				return;
			}
			
			int clientSettings = xteaBuf.ReadByte();
			bool clientResizable = (clientSettings >> 1) == 1;
			int clientWidth = xteaBuf.ReadUnsignedShort();
			int clientHeight = xteaBuf.ReadUnsignedShort();
			
			xteaBuf.SkipBytes(24);
			xteaBuf.ReadString();
			xteaBuf.SkipBytes(4);
			xteaBuf.SkipBytes(10);
			xteaBuf.SkipBytes(2);
			xteaBuf.SkipBytes(1);
			xteaBuf.SkipBytes(3);
			xteaBuf.SkipBytes(2);
			xteaBuf.ReadJagexString();
			xteaBuf.ReadJagexString();
			xteaBuf.ReadJagexString();
			xteaBuf.ReadJagexString();
			xteaBuf.SkipBytes(1);
			xteaBuf.SkipBytes(2);
			xteaBuf.ReadJagexString();
			xteaBuf.ReadJagexString();
			xteaBuf.SkipBytes(2);
			xteaBuf.SkipBytes(12);
			xteaBuf.SkipBytes(4);
			xteaBuf.ReadJagexString();
			xteaBuf.SkipBytes(12);
			
			int[] crcs = DecodeCRCs(xteaBuf);
			LoginRequest request = new LoginRequest(ctx.Channel, username, password ?? "", ServerRevision, xteaKeys, clientResizable, authCode, "".ToUpper(), clientWidth, clientHeight, Reconnecting);
			output.Add(request);
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
				ctx.WriteAndFlushAsync(buf).ContinueWith(ChannelFutureListener.CLOSE.OperationComplete, null);
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