namespace Game.Message;

interface MessageHandler<T> where T : Message
{
	
	public void Handle(Client client, World world, T message);
	
	// TODO: object[] is used for Any[] which may not work
	public void Log(Client client, string format, params object[] args);
	// TODO: No body filled out
	
}