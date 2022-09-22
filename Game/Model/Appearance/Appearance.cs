using static Game.Model.Appearance.Looks;

namespace Game.Model.Appearance;

class Appearance : IEquatable<Appearance>
{
	
	public static readonly int[] DEFAULT_COLORS = new int[] { 0, 27, 9, 0, 0 };
	public static readonly int[] DEFAULT_MALE_LOOKS = new int[] { 15, 9, 3, 8, 0, 3, 1 };
	public static readonly int[] DEFAULT_FEMALE_LOOKS = new int[] { 0, 0, 0, 0, 0, 0 };
	public static readonly Appearance DEFAULT_MALE = new Appearance(DEFAULT_MALE_LOOKS, DEFAULT_COLORS, Gender.MALE);
	public static readonly Appearance DEFAULT_FEMALE = new Appearance(DEFAULT_FEMALE_LOOKS, DEFAULT_COLORS, Gender.FEMALE);
	
	public int[] Looks;
	public int[] Colors;
	public Gender Gender;
	
	public Appearance(int[] looks, int[] colors, Gender gender)
	{
		Looks = looks;
		Colors = colors;
		Gender = gender;
	}
	
	public int GetLook(int option)
	{
		switch (Gender)
		{
			case Gender.MALE:
				return option switch {
					0 => GetHeads(Gender)[Looks[0]],
					1 => GetJaws(Gender)[Looks[1]],
					2 => GetTorsos(Gender)[Looks[2]],
					3 => GetArms(Gender)[Looks[3]],
					4 => GetHands(Gender)[Looks[4]],
					5 => GetLegs(Gender)[Looks[5]],
					6 => GetFeets(Gender)[Looks[6]],
					_ => -1
				};
			case Gender.FEMALE:
				return option switch {
					0 => GetHeads(Gender)[Looks[0]],
					2 => GetTorsos(Gender)[Looks[2]],
					3 => GetArms(Gender)[Looks[3]],
					4 => GetHands(Gender)[Looks[4]],
					5 => GetLegs(Gender)[Looks[5]],
					6 => GetFeets(Gender)[Looks[6]],
					_ => -1
				};
		};
		return -1;
	}
	
	public override bool Equals(object? other)
	{
		return Equals(other as Appearance);
	}
	
	public bool Equals(Appearance? other)
	{
		if (other == null)
			return false;
		// TODO: doesnt check Looks content equals
		// TODO: doesnt check Colors contents equals
		if (Gender != other.Gender)
			return false;
		return true;
	}
	
	public override int GetHashCode()
	{
		// TODO: doesnt get Looks content hash code
		// TODO: doesnt get Colors content hash code
		int hash = Looks.GetHashCode();
		hash = 31 * hash + Colors.GetHashCode();
		hash = 31 * hash + Gender.GetHashCode();
		return hash;
	}
	
}