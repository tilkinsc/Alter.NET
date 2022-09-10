namespace Net.Codec.Game;


enum GameDecoderState
{
	OPCODE,
	LENGTH,
	PAYLOAD
}