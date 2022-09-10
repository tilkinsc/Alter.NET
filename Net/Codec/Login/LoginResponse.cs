namespace Net.Codec.Login;

class LoginResponse
{
	
	public int Index;
	public int Privilege;
	
	public LoginResponse(int index, int privilege)
	{
		Index = index;
		Privilege = privilege;
	}
	
}