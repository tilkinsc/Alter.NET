namespace Net.Codec.Handshake;

class HandshakeMessage
{
	public int ID { get; private set; }
	
	public HandshakeMessage(int id)
	{
		ID = id;
	}
	
}
