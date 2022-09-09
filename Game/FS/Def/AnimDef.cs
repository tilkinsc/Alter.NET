namespace Game.FS.Def;

class AnimDef : Definition
{
	
	public List<int> FrameIDs = new List<int>();
	public List<int> FrameLengths = new List<int>();
	public int Priority = -1;
	public int LengthInCycles = 0;
	
	public AnimDef(int id)
			: base(id)
	{
	}
	
	public override void Decode(MemoryStream buffer, int opcode)
	{
		BinaryReader stream = new BinaryReader(buffer);
		switch (opcode)
		{
			case 1:
			{
				int frameCount = stream.ReadUInt16();
				int totalFrameLength = 0;
				FrameIDs = new List<int>(frameCount);
				FrameLengths = new List<int>(frameCount);
				
				for (int i=0; i<frameCount; i++)
				{
					FrameLengths[i] = stream.ReadUInt16();
					if (i < frameCount - 1 || FrameLengths[i] < 200) {
						totalFrameLength += FrameLengths[i];
					}
				}
				
				for (int i=0; i<frameCount; i++)
				{
					FrameIDs[i] = stream.ReadUInt16();
				}
				
				for (int i=0; i<frameCount; i++)
				{
					FrameIDs[i] += stream.ReadUInt16() << 16;
				}
				
				LengthInCycles = (int) Math.Ceiling((totalFrameLength * 20.0) / 600.0);
				break;
			}
			case 2:
			{
				stream.ReadUInt16();
				break;
			}
			case 3:
			{
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadByte();
				}
				break;
			}
			case 5:
			{
				stream.ReadByte();
				break;
			}
			case 6:
			case 7:
			{
				stream.ReadUInt16();
				break;
			}
			case 8:
			case 9:
			{
				stream.ReadByte();
				break;
			}
			case 10:
			{
				Priority = stream.ReadByte();
				break;
			}
			case 11:
			{
				stream.ReadByte();
				break;
			}
			case 12:
			{
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
					stream.ReadUInt16();
				}
				break;
			}
			case 13:
			{
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					// TODO: check if correct
					stream.ReadUnsignedMedium();
				}
				break;
			}
		}
	}
	
}