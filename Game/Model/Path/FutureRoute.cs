namespace Game.Model.Path;

class FutureRoute
{
	
	public volatile bool IsCompleted = false;
	
	public PathFindingStrategy Strategy;
	public StepType StepType;
	public bool DetectCollision;
	
	private FutureRoute(PathFindingStrategy strategy, StepType stepType, bool detectCollision)
	{
		Strategy = strategy;
		StepType = stepType;
		DetectCollision = detectCollision;
	}
	
	?
	
}