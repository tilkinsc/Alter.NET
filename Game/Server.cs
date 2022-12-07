using System.Diagnostics;
using Game.Model;
using JSON;
using Util;

namespace Game;

class Server
{
	
	public Server()
	{
		
	}
	
	public void StartServer()
	{
		Stopwatch sw = new Stopwatch();
		Logger.Log($"Loading API {GetAPIName()}");
		sw.Start();
		ApiJSON.Instance.Load();
		sw.Stop();
		Logger.Log($"API Loading took {sw.Elapsed}");
	}
	
	public void StartGame()
	{
		Stopwatch sw = new Stopwatch();
		Stopwatch sw2 = new Stopwatch();
		Logger.Log($"Loading Game");
		sw.Start();
		
		sw2.Start();
		GameJSON.Instance.Load();
		GameJSON gm = GameJSON.Instance.Handle;
		GameContext gameContext = new GameContext(
			gm.InitialLaunch,
			gm.Name,
			gm.Revision,
			gm.CycleTime,
			gm.PlayerLimit,
			new Tile(gm.Home),
			gm.SkillCount,
			gm.NPCStatCount,
			gm.RunEnergy,
			gm.GroundItemPublicDelay,
			gm.GroundItemDespawnDelay,
			gm.PreloadMaps
		);
		
		
		DevSettingsJSON.Instance.Load();
		DevSettingsJSON dv = DevSettingsJSON.Instance.Handle;
		DevContext devContext = new DevContext (
			dv.DebugExamines,
			dv.DebugObjects,
			dv.DebugButtons,
			dv.DebugItemActions,
			dv.DebugMagicSpells
		);
		
		World world = new World(gameContext, devContext);
		sw2.Stop();
		Logger.Log($"Loaded world settings took {sw2.Elapsed}");
		
		// File Store
		
		
		
	}
	
	public string GetAPIName()
	{
		return ApiJSON.Instance.Handle.Org;
	}
	
}
