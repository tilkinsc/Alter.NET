namespace Cache.Region;


class RLPosition : IEquatable<RLPosition>
{
	
	public readonly int X;
	public readonly int Y;
	public readonly int Z;
	
	public RLPosition(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
	
	public override string ToString()
	{
		return $"Position{{x={X}, y={Y}, z={Z}}}";
	}
	
	public override bool Equals(object? obj)
	{
		return Equals(obj as RLPosition);
	}
	
	public bool Equals(RLPosition? pos)
	{
		if (pos == null)
			return false;
		return pos.X == X && pos.Y == Y && pos.Z == Z;
	}

	public override int GetHashCode()
	{
		int hash = 7;
		hash = 67 * hash + X;
		hash = 67 * hash + Y;
		hash = 67 * hash + Z;
		return hash;
	}

}