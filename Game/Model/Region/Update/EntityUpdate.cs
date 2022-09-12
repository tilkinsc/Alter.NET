using Game.Model.Entity;
using Game.Message;

namespace Game.Model.Region.Update;

class EntityUpdate
{
	public EntityUpdateType UpdateType;
	
	public EntityUpdate(EntityUpdateType updateType)
	{
		UpdateType = updateType;
	}
	
}

abstract class EntityUpdate<T> : EntityUpdate where T : BaseEntity
{
	
	public T Entity;
	
	public EntityUpdate(EntityUpdateType updateType, T entity)
			: base(updateType)
	{
		Entity = entity;
	}
	
	public abstract IMessage ToMessage();
	
	public override string ToString()
	{
		return "ToString for EntityUpdate not implemented";
	}
	
}
