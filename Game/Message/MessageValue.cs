using Net.Packet;

namespace Game.Message;

class MessageValue
{
	public string ID;
	public DataOrder Order;
	public DataTransformation Transformation;
	public DataType Type;
	public DataSignature Signature;
	
	public MessageValue(string id, DataOrder order, DataTransformation transformation, DataType type, DataSignature signature)
	{
		ID = id;
		Order = order;
		Transformation = transformation;
		Type = type;
		Signature = signature;
	}
}