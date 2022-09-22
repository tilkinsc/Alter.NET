namespace Game.Model.Combat;

class PawnHit
{
	
	public int Hit;
	public bool Landed;
	
	public PawnHit(int hit, bool landed)
	{
		Hit = hit;
		Landed = landed;
	}
	
}