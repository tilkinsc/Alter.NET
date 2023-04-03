namespace Net.Codec.Login;

class LoginResponse
{
	
	public int Index { get; private set; }
	public int Privilege { get; private set; }
	
	public LoginResponse(int index, int privilege)
	{
		Index = index;
		Privilege = privilege;
	}
	
}