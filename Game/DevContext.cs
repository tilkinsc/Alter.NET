namespace Game;

class DevContext
{
	public bool DebugExamines;
	public bool DebugObjects;
	public bool DebugButtons;
	public bool DebugItemActions;
	public bool DebugMagicSpells;
	
	public DevContext(bool debugExamines, bool debugObjects, bool debugButtons, bool debugItemActions, bool debugMagicSpells)
	{
		DebugExamines = debugExamines;
		DebugObjects = debugObjects;
		DebugButtons = debugButtons;
		DebugItemActions = debugItemActions;
		DebugMagicSpells = debugMagicSpells;
	}
}
