namespace Game.Model.Collision;

class ObjectType
{
	public static readonly ObjectType LENGTHWISE_WALL = new ObjectType(0, ObjectGroup.WALL);
	public static readonly ObjectType TRIANGULAR_CORNER = new ObjectType(1, ObjectGroup.WALL);
	public static readonly ObjectType WALL_CORNER = new ObjectType(2, ObjectGroup.WALL);
	public static readonly ObjectType RECTANGULAR_CORNER = new ObjectType(3, ObjectGroup.WALL);
	public static readonly ObjectType INTERACTABLE_WALL_DECOR = new ObjectType(4, ObjectGroup.WALL);
	public static readonly ObjectType INTERACTABLE_WALL = new ObjectType(5, ObjectGroup.WALL);
	public static readonly ObjectType DIAGONAL_WALL = new ObjectType(9, ObjectGroup.WALL);
	public static readonly ObjectType INTERACTABLE = new ObjectType(10, ObjectGroup.INTERACTABLE_OBJECT);
	public static readonly ObjectType DIAGONAL_INTERACTABLE = new ObjectType(11, ObjectGroup.INTERACTABLE_OBJECT);
	public static readonly ObjectType FLOOR_DECOR = new ObjectType(22, ObjectGroup.GROUND_DECOR);
	
	public readonly int Value;
	public readonly ObjectGroup Group;
	
	private ObjectType(int value, ObjectGroup group)
	{
		Value = value;
		Group = group;
	}
	
}