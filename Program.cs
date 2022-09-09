using Game;
using JSON;
using Util;

namespace Project;

class Program
{
	
	public static void Main(string[] args)
	{
		Logger.Init();
		Server server = new Server();
		server.StartServer();
		server.StartGame();
	}
	
}
