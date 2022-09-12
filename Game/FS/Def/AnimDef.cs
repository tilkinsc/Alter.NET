using DotNetty.Buffers;

namespace Game.FS.Def;

class AnimDef : Definition
{
	
	public int[]? FrameIDs;
	public int[]? FrameLengths;
	public int Priority = -1;
	public int LengthInCycles = 0;
	
	public AnimDef(int id)
			: base(id)
	{
	}
	
	public override void Decode(IByteBuffer buf, int opcode)
	{
		switch (opcode)
		{
			case 1:
			{
				int frameCount = buf.ReadUnsignedShort();
				int totalFrameLength = 0;
				FrameIDs = new int[frameCount];
				FrameLengths = new int[frameCount];
				
				for (int i=0; i<frameCount; i++)
				{
					FrameLengths[i] = buf.ReadUnsignedShort();
					if (i < frameCount - 1 || FrameLengths[i] < 200) {
						totalFrameLength += FrameLengths[i];
					}
				}
				
				for (int i=0; i<frameCount; i++)
				{
					FrameIDs[i] = buf.ReadUnsignedShort();
				}
				
				for (int i=0; i<frameCount; i++)
				{
					FrameIDs[i] += buf.ReadUnsignedShort() << 16;
				}
				
				LengthInCycles = (int) Math.Ceiling((totalFrameLength * 20.0) / 600.0);
				break;
			}
			case 2:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 3:
			{
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadByte();
				}
				break;
			}
			case 5:
			{
				buf.ReadByte();
				break;
			}
			case 6:
			case 7:
			{
				buf.ReadUnsignedShort();
				break;
			}
			case 8:
			case 9:
			{
				buf.ReadByte();
				break;
			}
			case 10:
			{
				Priority = buf.ReadByte();
				break;
			}
			case 11:
			{
				buf.ReadByte();
				break;
			}
			case 12:
			{
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadUnsignedShort();
					buf.ReadUnsignedShort();
				}
				break;
			}
			case 13:
			{
				int count = buf.ReadByte();
				for (int i=0; i<count; i++)
				{
					buf.ReadMedium();
				}
				break;
			}
		}
	}
	
}