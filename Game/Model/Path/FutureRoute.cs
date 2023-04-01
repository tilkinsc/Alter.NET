namespace Game.Model.Path;

class FutureRoute
{
	
	public volatile bool IsCompleted = false;
	
	public Route? Route;
	
	public PathFindingStrategy Strategy { get; private set; }
	public StepType StepType { get; private set; }
	public bool DetectCollision { get; private set; }
	
	private FutureRoute(PathFindingStrategy strategy, StepType stepType, bool detectCollision)
	{
		Strategy = strategy;
		StepType = stepType;
		DetectCollision = detectCollision;
	}
	
	?
	
}