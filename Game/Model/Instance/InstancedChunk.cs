namespace Game.Model.Instance;

class InstancedChunk
{
	
	public int Packed;
	
	public int Rotation => (Packed >> 1) & 0x3;
	
	public InstancedChunk(int packed)
	{
		Packed = packed;
	}
	
}