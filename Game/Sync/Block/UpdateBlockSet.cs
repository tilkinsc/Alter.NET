using JSON;

namespace Game.Sync.Block;

class UpdateBlockSet
{
	
	public int UpdateOpcode = -1;
	public int LargeSceneUpdateOpcode = -1;
	public int UpdateBlockExcessMark = -1;
	public List<UpdateBlockType> UpdateBlockOrder = new List<UpdateBlockType>();
	public Dictionary<UpdateBlockType, UpdateBlockStructure> UpdateBlocks = new Dictionary<UpdateBlockType, UpdateBlockStructure>();
	
	public void Load(ServerPropertiesJSON properties)
	{
		var?
	}
	
}
