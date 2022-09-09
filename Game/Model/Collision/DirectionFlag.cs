namespace Game.Model.Collision;

class DirectionFlag
{
	public Direction Direction;
	public bool IsImpenetrable;
	
	public DirectionFlag(Direction direction, bool impenetrable)
	{
		Direction = direction;
		IsImpenetrable = impenetrable;
	}
}