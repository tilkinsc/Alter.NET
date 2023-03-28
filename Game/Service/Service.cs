using Game.Model;

namespace Game.Service;

interface IService
{
	public void Init(Server server, World world);
	public void PostLoad(Server server, World world);
	public void BindNet(Server server, World world);
	public void Terminate(Server server, World world);
}
