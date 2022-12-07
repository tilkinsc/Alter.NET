using Exceptions;

namespace Game.Model.Instance;

class InstancedMapConfiguration
{
	
	public Tile ExitTile;
	public PlayerUID? Owner;
	public InstancedMapAttribute[] Attributes;
	public bool BypassObjectChunkBounds;
	
	private InstancedMapConfiguration(Tile exitTile, PlayerUID? owner, InstancedMapAttribute[] attributes, bool bypassObjectChunkBounds)
	{
		ExitTile = exitTile;
		Owner = owner;
		Attributes = attributes;
		BypassObjectChunkBounds = bypassObjectChunkBounds;
	}
	
	class Builder
	{
		
		private Tile? ExitTile;
		private PlayerUID? Owner;
		private List<InstancedMapAttribute> Attributes = new List<InstancedMapAttribute>();
		private bool BypassObjectChunkBounds;
		
		public InstancedMapConfiguration Build()
		{
			bool ownerRequired = Attributes.Exists((obj) => obj == InstancedMapAttribute.DEALLOCATE_ON_DEATH || obj == InstancedMapAttribute.DEALLOCATE_ON_LOGOUT);
			
			if (ExitTile == null)
				throw new IllegalStateException("Exit tile must be set");
			if (Owner == null && Attributes.Count > 0)
				throw new IllegalStateException("One or more attributes require an owner to be set");
			
			return new InstancedMapConfiguration(ExitTile, Owner, Attributes.ToArray(), BypassObjectChunkBounds);
		}
		
		public Builder SetExitTile(Tile tile)
		{
			ExitTile = tile;
			return this;
		}
		
		public Builder SetOwner(PlayerUID? owner)
		{
			Owner = owner;
			return this;
		}
		
		public Builder AddAttribute(InstancedMapAttribute attribute, params InstancedMapAttribute[] others)
		{
			Attributes.Add(attribute);
			Attributes.AddRange(others);
			return this;
		}
		
		public Builder SetBypassObjectChunkBounds(bool bypass)
		{
			BypassObjectChunkBounds = bypass;
			return this;
		}
		
	}
	
}