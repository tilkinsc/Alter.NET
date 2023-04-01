namespace Util;

// I would like this class to eliminate the pointless Box class
// width1 = width1 + x1;
// length1 = length1 + z1;
// width2 = width2 + x2;
// length2 = length2 + z2;
static class AabbUtil
{
	
	public class Box
	{
		public int X;
		public int Z;
		public int Width;
		public int Length;
		
		public int X1 { get => X; }
		public int Z1 { get => Z; }
		public int X2 { get => X + Width; }
		public int Z2 { get => Z + Length; }
		
		public Box(int x, int z, int width, int length)
		{
			X = x;
			Z = z;
			Width = width;
			Length = length;
		}
	}
	
	private static bool Between(int x1, int left, int right)
	{
		return x1 >= left && x1 <= right;
	}
	
	public static bool AreBordering(int x1, int z1, int width1, int length1,
							int x2, int z2, int width2, int length2)
	{
		Box a = new Box(x1, z1, width1 - 1, length1 - 1);
		Box b = new Box(x2, z2, width2 - 1, length2 - 1);
		
		if (Between(b.X1, a.X1, a.X2) && Between(b.Z1, a.Z1, a.Z2) || Between(b.X2, a.X1, a.X2) && Between(b.Z2, a.Z1, a.Z2))
			return false;
		
		if (b.X1 > a.X2 + 1 || b.X2 < a.X1 - 1 || b.Z1 > a.Z2 + 1 || b.Z2 < a.Z1 - 1)
			return false;
		
		return true;
	}
	
	public static bool AreDiagonal(int x1, int z1, int width1, int length1,
							int x2, int z2, int width2, int length2)
	{
		Box a = new Box(x1, z1, width1 - 1, length1 - 1);
		Box b = new Box(x2, z2, width2 - 1, length2 - 1);
		
		if (a.X1 - 1 == b.X2 && a.Z1 - 1 == b.Z2)
			return true;
		
		if (a.X2 + 1 == b.X2 && a.Z1 - 1 == b.Z2)
			return true;
		
		if (a.X1 - 1 == b.X2 && a.Z2 + 1 == b.Z2)
			return true;
		
		if (a.X2 + 1 == b.X2 && a.Z2 + 1 == b.Z2)
			return true;
		
		return false;
	}
	
	public static bool AreOverlapping(int x1, int z1, int width1, int length1,
							int x2, int z2, int width2, int length2)
	{
		Box a = new Box(x1, z1, width1 - 1, length1 - 1);
		Box b = new Box(x2, z2, width2 - 1, length2 - 1);
		
		if (a.X1 > b.X2 || b.X1 > a.X2)
			return false;
		
		if (a.Z1 > b.Z2 || b.Z1 > a.Z2)
			return false;
		
		return true;
	}
	
}