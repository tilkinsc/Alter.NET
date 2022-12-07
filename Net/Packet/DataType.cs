namespace Net.Packet;

class DataType
{
	public static readonly DataType BYTE = new DataType(1);
	public static readonly DataType SHORT = new DataType(2);
	public static readonly DataType MEDIUM = new DataType(3);
	public static readonly DataType INT = new DataType(4);
	public static readonly DataType LONG = new DataType(8);
	public static readonly DataType BYTES = new DataType(-1);
	public static readonly DataType SMART = new DataType(-1);
	public static readonly DataType STRING = new DataType(-1);
	
	public int Bytes;
	
	public DataType(int bytes)
	{
		Bytes = bytes;
	}
}