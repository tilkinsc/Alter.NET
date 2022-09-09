using Game.Model;

namespace Game.Model;

class ForcedMovement : IEquatable<ForcedMovement>
{
	
	public Tile InitialTile;
	public List<Tile> Destinations;
	public int ClientDuration1;
	public int ClientDuration2;
	public int DirectionAngle;
	
	public ForcedMovement(Tile initialTile, List<Tile> destinations, int clientDuration1, int clientDuration2, int directionAngle)
	{
		InitialTile = initialTile;
		Destinations = destinations;
		ClientDuration1 = clientDuration1;
		ClientDuration2 = clientDuration2;
		DirectionAngle = directionAngle;
	}
	
	public Tile GetFinalDestination() => Destinations.Last();
	public int GetMaxDuration() => Math.Max(ClientDuration1, ClientDuration2);
	public int GetDiffX1() => InitialTile.X - Destinations[0].X;
	public int GetDiffZ1() => InitialTile.Z - Destinations[0].Z;
	public int GetDiffX2()
	{
		if (Destinations.Count >= 2) {
			return InitialTile.X - Destinations[1].X;
		}
		return 0;
	}
	public int GetDiffZ2()
	{
		if (Destinations.Count >= 2) {
			return InitialTile.Z - Destinations[1].Z;
		}
		return 0;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as ForcedMovement);
	}
	
	public bool Equals(ForcedMovement? other)
	{
		if (other == null)
			return false;
			
		// TODO: missing destinations.ContextEquals(other.Destinations)
		if (InitialTile != other.InitialTile || ClientDuration1 != other.ClientDuration1 || ClientDuration2 != other.ClientDuration2 || DirectionAngle != other.DirectionAngle)
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		int hash = InitialTile.GetHashCode();
		// TODO: this isnt the hash code of content
		hash = 31 * hash + Destinations.GetHashCode();
		hash = 31 * hash + ClientDuration1;
		hash = 31 * hash + ClientDuration2;
		hash = 31 * hash + DirectionAngle;
		return hash;
	}
	
	public static ForcedMovement Of(Tile src, Tile dst, int clientDuration1, int clientDuration2, int directionAngle)
	{
		return new ForcedMovement(src, new List<Tile>(new Tile[] { dst }), clientDuration1, clientDuration1, directionAngle);
	}
	
	public static ForcedMovement Of(Tile src, Tile dst1, Tile dst2, int clientDuration1, int clientDuration2, int directionAngle)
	{
		return new ForcedMovement(src, new List<Tile>(new Tile[] { dst1, dst2 }), clientDuration1, clientDuration2, directionAngle);
	}
	
}
