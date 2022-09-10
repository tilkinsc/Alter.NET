namespace Net.Codec.Login;

class LoginRequest : IEquatable<LoginRequest>
{
	
	public Channel Channel;
	public string Username;
	public string Password;
	public int Revision;
	public int[] XteaKeys;
	public bool ResizableClient;
	public int Auth;
	public string UUID;
	public int ClientWidth;
	public int ClientHeight;
	public bool Reconnecting;
	
	public LoginRequest(Channel channel, string username, string password, int revision,
			int[] xteaKeys, bool resizableClient, int auth, string uuid, int clientWidth,
			int clientHeight, bool reconnecting)
	{
		Channel = channel;
		Username = username;
		Password = password;
		Revision = revision;
		XteaKeys = xteaKeys;
		ResizableClient = resizableClient;
		Auth = auth;
		UUID = uuid;
		ClientWidth = clientWidth;
		ClientHeight = clientHeight;
		Reconnecting = reconnecting;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as LoginRequest);
	}
	
	public bool Equals(LoginRequest? other)
	{
		if (other == null)
			return false;
		// TODO: doesnt check contents equals of XteaKeys and other.XteaKeys
		if (Channel != other.Channel || Username != other.Username || Password != other.Password
				|| Revision != other.Revision || ResizableClient != other.ResizableClient || Auth != other.Auth
				|| UUID != other.UUID || ClientWidth != other.ClientWidth || ClientHeight != other.ClientHeight
				|| Reconnecting != other.Reconnecting)
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		int hash = Channel.GetHashCode();
		hash = 31 * hash + Username.GetHashCode();
		hash = 31 * hash + Password.GetHashCode();
		hash = 31 * hash + Revision;
		// TODO: doesnt add content hashcode of XteaKeys
		hash = 31 * hash + XteaKeys.GetHashCode();
		hash = 31 * hash + ResizableClient.GetHashCode();
		hash = 31 * hash + Auth;
		hash = 31 * hash + UUID.GetHashCode();
		hash = 31 * hash + ClientWidth;
		hash = 31 * hash + ClientHeight;
		hash = 31 * hash + Reconnecting.GetHashCode();
		return hash;
	}
	
}