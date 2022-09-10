using System.Text;
using Exceptions;

namespace Util;

// TODO: checkme
class HuffmanCodec
{
	
	private byte[] Sizes;
	
	public int[] Masks;
	public int[] Keys;
	
	public HuffmanCodec(byte[] sizes)
	{
		Sizes = sizes;
		int i_2 = sizes.Length;
		Masks = new int[i_2];
		int[] ints_3 = new int[33];
		Keys = new int[8];
		int i_4 = 0;
		
		for (int i_5=0; i_5<i_2; i_5++)
		{
			int b_6 = sizes[i_5];
			if (b_6 != 0) {
				int i_7 = 1 << 32 - b_6;
				int i_8 = ints_3[b_6];
				Masks[i_5] = i_8;
				int i_9 = 0;
				int i_10 = 0;
				int i_11 = 0;
				int i_12 = 0;
				if ((i_8 & i_7) != 0)
					i_9 = ints_3[b_6 - 1];
				else {
					i_9 = i_8 | i_7;
					i_10 = b_6 - 1;
					while (i_10 >= 1)
					{
						i_11 = ints_3[i_10];
						if (i_11 != i_8)
							break;
						i_12 = 1 << 32 - i_10;
						if ((i_11 & i_12) != 0) {
							ints_3[i_10] = ints_3[i_10 - 1];
							break;
						}
						ints_3[i_10] = i_11 | i_12;
						i_10--;
					}
				}
				ints_3[b_6] = i_9;
				i_10 = b_6 + 1;
				while (i_10 <= 32)
				{
					if (ints_3[i_10] == i_8)
						ints_3[i_10] = i_9;
					i_10++;
				}
				i_10 = 0;
				i_11 = 0;
				while (i_11 < b_6)
				{
					i_12 = int.MinValue >> i_11; // Integer.MIN_VALUE.ushr(i_11);
					if ((i_8 & i_12) != 0) {
						if (Keys[i_10] == 0)
							Keys[i_10] = i_4;
						i_10 = Keys[i_10];
					} else
						++i_10;
					if (i_10 >= Keys.Length) {
						int[] ints_13 = new int[Keys.Length * 2];
						// TODO: make sure this is correct
						for (int i_14=0; i_14<Keys.Length; i_14++) // for (i_14 in keys.indices)
							ints_13[i_14] = Keys[i_14];
						Keys = ints_13;
					}
					i_11++;
				}
				Keys[i_10] = ~i_5;
				if (i_10 >= i_4)
					i_4 = i_10 + 1;
			}
		}
	}
	
	public int Compress(string text, ref byte[] output)
	{
		int key = 0;
		byte[] input = Encoding.ASCII.GetBytes(text);
		int bitpos = 0;
		for (int pos=0; pos<text.Length; pos++)
		{
			int data = input[pos] & 255;
			int size = Sizes[data];
			int mask = Masks[data];
			if (size == 0)
				throw new RuntimeException("No codeword for data value $data");
			int remainder = bitpos & 7;
			key = key & (-remainder >> 31);
			var offset = bitpos >> 3;
			bitpos += size;
			int i_41_ = (-1 + (remainder - -size) >> 3) + offset;
			remainder += 24;
			key = key | (mask >> remainder);
			output[offset] = (byte) key;
			if (~i_41_ < ~offset) {
				remainder -= 8;
				key = mask >> remainder;
				output[++offset] = (byte) key;
				if (~offset > ~i_41_) {
					remainder -= 8;
					key = mask >> remainder;
					output[++offset] = (byte) key;
					if (~offset > ~i_41_) {
						remainder -= 8;
						key = mask >> remainder;
						output[++offset] = (byte) key;
						if (i_41_ > offset) {
							remainder -= 8;
							key = mask << -remainder;
							output[++offset] = (byte) key;
						}
					}
				}
			}
		}
		// TODO: unsure order of operations
		return 7 + bitpos >> 3;
	}
	
	public int Decompress(byte[] compressed, ref byte[] decompressed, int decompressedLength)
	{
		int decompressedLen = decompressedLength;
		int i_2 = 0;
		int i_4 = 0;
		if (decompressedLength == 0)
			return 0;
		else {
			int i_7 = 0;
			decompressedLen += i_4;
			int i_8 = i_2;
			while (true) {
				int b_9 = compressed[i_8];
				if (b_9 < 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				int i_10 = 0;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x40) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x20) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x10) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x8) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x4) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x2) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				if ((b_9 & 0x1) != 0)
					i_7 = Keys[i_7];
				else
					++i_7;
				i_10 = Keys[i_7];
				if (i_10 < 0) {
					decompressed[i_4++] = (byte) ~i_10;
					if (i_4 >= decompressedLength)
						break;
					i_7 = 0;
				}
				++i_8;
			}
			return i_8 + 1 - i_2;
		}
	}
	
}