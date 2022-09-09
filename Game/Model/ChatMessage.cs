namespace Game.Model;

enum ChatType
{
	NONE = 0,
	AUTOCHAT,
	CLANCHAT
}

enum ChatEffect
{
	NONE = 0,
	WAVE,
	WAVE2,
	SHAKE,
	SCROLL,
	SLIDE
}

enum ChatColor
{
	NONE = 0,
	RED,
	GREEN,
	CYAN,
	PURPLE,
	WHITE,
	FLASH1,
	FLASH2,
	FLASH3,
	GLOW1,
	GLOW2,
	GLOW3
}

class ChatMessage
{
	
	public string Text;
	public int Icon;
	public ChatType Type;
	public ChatEffect Effect;
	public ChatColor Color;
	
	public ChatMessage(string text, int icon, ChatType type, ChatEffect effect, ChatColor color)
	{
		Text = text;
		Icon = icon;
		Type = type;
		Effect = effect;
		Color = color;
	}
	
}