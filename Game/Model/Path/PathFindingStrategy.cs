using Game.Model.Collision;

namespace Game.Model.Path;

abstract class PathFindingStrategy
{
	
	public static int MAX_DISTANCE = 20;
	
	public CollisionManager Collision;
	public volatile bool Cancel = false;
	
	public PathFindingStrategy(CollisionManager collision)
	{
		Collision = collision;
	}
	
	public abstract Route CalculateRoute(PathRequest request);
	
}