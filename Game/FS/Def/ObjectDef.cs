using System.Text;
using Util;
using Game.Model.Entity;

namespace Game.FS.Def;

class ObjectDef : Definition
{
	
	public String Name = "";
	public int Width = 1;
	public int Length = 1;
	public bool IsSolid = true;
	public bool IsImpenetrable = true;
	public bool IsInteractive = false;
	public bool IsObstructive = false;
	public int ClipMask = 0;
	public int VarBit = -1;
	public int Varp = -1;
	public int Animation = -1;
	public bool IsRotated = false;
	public int Category = -1;
	public List<string?> Options = new List<string?>(new string[5] { "", "", "", "", "" });
	public List<int>? Transforms;
	public Dictionary<int, object> Parameters = new Dictionary<int, object>();
	public string? Examine;
	
	public ObjectDef(int id)
		: base(id)
	{
	}
	
	public int GetRotatedWidth(GameObject obj)
	{
		if ((obj.Rotation & 0x1) == 1) {
			return Length;
		}
		return Width;
	}
	
	public int GetRotatedLength(GameObject obj)
	{
		if ((obj.Rotation & 0x1) == 1) {
			return Width;
		}
		return Length;
	}
	
	public override void Decode(MemoryStream buffer, int opcode)
	{
		BinaryReader stream = new BinaryReader(buffer, Encoding.ASCII, true);
		switch (opcode)
		{
			case 1:
			{
				byte count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
					stream.ReadByte();
				}
				break;
			}
			case 2:
			{
				Name = stream.ReadString();
				break;
			}
			case 5:
			{
				byte count = stream.ReadByte();
				for (int i=0; i<count; i++)
					stream.ReadUInt16();
				break;
			}
			case 14:
			{
				Width = stream.ReadByte();
				break;
			}
			case 15:
			{
				Length = stream.ReadByte();
				break;
			}
			case 17:
			{
				IsSolid = false;
				break;
			}
			case 18:
			{
				IsImpenetrable = false;
				break;
			}
			case 19:
			{
				IsInteractive = stream.ReadByte() == 1;
				break;
			}
			case 24:
			{
				Animation = stream.ReadUInt16();
				break;
			}
			case 27:
			{
				break;
			}
			case 28:
			{
				stream.ReadByte();
				break;
			}
			case 29:
			{
				stream.ReadByte();
				break;
			}
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			{
				string str = stream.ReadString();
				Options[opcode - 30] = str;
				if (str.Equals("null") || str.Equals("hidden")) {
					Options[opcode - 30] = null;
				}
				break;
			}
			case 39:
			{
				stream.ReadByte();
				break;
			}
			case 40:
			{
				byte count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
					stream.ReadUInt16();
				}
				break;
			}
			case 41:
			{
				byte count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
					stream.ReadUInt16();
				}
				break;
			}
			case 61:
			{
				Category = stream.ReadUInt16();
				break;
			}
			case 62:
			{
				IsRotated = true;
				break;
			}
			case 65:
			case 66:
			case 67:
			case 68:
			{
				stream.ReadUInt16();
				break;
			}
			case 69:
			{
				ClipMask = stream.ReadByte();
				break;
			}
			case 70:
			case 71:
			case 72:
			{
				stream.ReadUInt16();
				break;
			}
			case 73:
			{
				IsObstructive = true;
				break;
			}
			case 75:
			{
				stream.ReadByte();
				break;
			}
			case 77:
			case 92:
			{
				VarBit = stream.ReadUInt16();
				if (VarBit == 65535) {
					VarBit = -1;
				}
				
				Varp = stream.ReadUInt16();
				if (Varp == 65535) {
					Varp = -1;
				}
				
				int terminatingTransform = -1;
				if (opcode == 92) {
					terminatingTransform = stream.ReadUInt16();
					if (terminatingTransform == 65535) {
						terminatingTransform = -1;
					}
				}
				
				int count = stream.ReadByte();
				int[] trans = new int[count + 2];
				for (int i=0; i<count + 1; i++)
				{
					int transform = stream.ReadUInt16();
					if (transform == 65535)
						transform = -1;
					trans[i] = transform;
				}
				trans[count+1] = terminatingTransform;
				Transforms = new List<int>(trans);
				break;
			}
			case 78:
			{
				stream.ReadUInt16();
				stream.ReadByte();
				break;
			}
			case 79:
			{
				stream.ReadUInt16();
				stream.ReadUInt16();
				stream.ReadByte();
				int count = stream.ReadByte();
				for (int i=0; i<count; i++)
				{
					stream.ReadUInt16();
				}
				break;
			}
			case 81:
			{
				stream.ReadByte();
				break;
			}
			case 82:
			{
				stream.ReadUInt16();
				break;
			}
			case 249:
			{
				Parameters.Merge<int, object>(ReadParams(buffer));
				break;
			}
		}
	}
}
