using Exceptions;
using Game.Model.Entity;

namespace Game.Model;

enum StepType
{
	NORMAL,
	FORCED_WALK,
	FORCE_RUN
}

class StepDirection
{
	
	public Direction? WalkDirection { get; private set; }
	public Direction? RunDirection { get; private set; }
	
	public StepDirection(Direction? walkDirection, Direction? runDirection)
	{
		WalkDirection = walkDirection;
		RunDirection = runDirection;
	}
	
}

class Step
{
	
	public Tile Tile { get; private set; }
	public StepType Type { get; private set; }
	public bool DetectCollision { get; private set; }
	
	public Step(Tile tile, StepType type, bool detectCollision)
	{
		Tile = tile;
		Type = type;
		DetectCollision = detectCollision;
	}
	
}

class MovementQueue
{
	
	// TODO: this is supposed to be a Deque<Step>
	public LinkedList<Step> Steps = new LinkedList<Step>();
	
	public Pawn Pawn { get; private set; }
	
	public MovementQueue(Pawn pawn)
	{
		Pawn = pawn;
	}
	
	public bool HasDestination()
	{
		return Steps.Count > 0;
	}
	
	public Tile? PeekLast()
	{
		LinkedListNode<Step>? last = Steps.Last;
		if (last == null)
			return null;
		return last.Value.Tile;
	}
	
	public Step? PeekLastStep()
	{
		LinkedListNode<Step>? last = Steps.Last;
		if (last == null)
			return null;
		return last.Value;
	}
	
	public void Clear() => Steps.Clear();
	
	private void AddStep(Tile current, Tile next, StepType type, bool detectCollision)
	{
		int dx = next.X - current.X;
		int dz = next.Z - current.Z;
		int delta = Math.Max(Math.Abs(dx), Math.Abs(dz));
		
		for (int i=0; i<delta; i++)
		{
			if (dx < 0) {
				dx++;
			} else if (dx > 0) {
				dx--;
			}
			if (dz < 0) {
				dz++;
			} else if (dz > 0) {
				dz--;
			}
			Steps.AddLast(new Step(next.Transform(-dx, -dz), type, detectCollision));
		}
	}
	
	public void AddStep(Tile step, StepType type, bool detectCollision)
	{
		Tile? current = Steps.Any() ? Steps.Last!.Value.Tile : Pawn.Tile;
		if (current == null)
			throw new IllegalStateException("Pawn list empty and pawn doesn't have a tile");
		AddStep(current, step, type, detectCollision);
	}
	
	// TODO: fill this in
	public void Cycle()
	{
		.
	}
	
}
