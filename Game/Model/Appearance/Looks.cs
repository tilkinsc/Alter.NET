using Exceptions;

namespace Game.Model.Appearance;

static class Looks
{
	
	public static readonly int[] MALE_HEADS = new int[] {
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		129, 130, 131, 132, 133, 134,
		151,
		144, 145, 146, 147, 148, 149, 150
	};
	
	public static readonly int[] FEMALE_HEADS = new int[] {
		45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55,
		119, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128,
		152, 143
	};
	
	public static readonly int[] MALE_JAWS = new int[] {
		10, 11, 12, 13,
		15, 16, 17,
		111, 112, 113, 114, 115, 116, 117,
		14
	};
	
	public static readonly int[] FEMALE_JAWS = new int[] {
		-1
	};
	
	public static readonly int[] MALE_TORSOS = new int[] {
		18, 19, 20, 21, 22, 23, 24, 25,
		105, 106, 107, 108, 109, 110
	};
	
	public static readonly int[] FEMALE_TORSOS = new int[] {
		56, 57, 58, 59, 60,
		89, 90, 91, 92, 93, 94
	};
	
	public static readonly int[] MALE_ARMS = new int[] {
		26, 27, 28, 29, 30, 31, 32,
		84, 85, 86, 87, 88
	};
	
	public static readonly int[] FEMALE_ARMS = new int[] {
		61, 62, 63, 64, 65, 66,
		95, 96, 97, 98, 99
	};
	
	public static readonly int[] MALE_HANDS = new int[] {
		33, 34
	};
	
	public static readonly int[] FEMALE_HANDS = new int[] {
		67, 68
	};
	
	public static readonly int[] MALE_LEGS = new int[] {
		36, 37, 38, 39, 40, 41,
		100, 101, 102, 103, 104
	};
	
	public static readonly int[] FEMALE_LEGS = new int[] {
		70, 71, 72, 73,
		76, 77, 78,
		135, 136, 137, 138, 139, 140
	};
	
	public static readonly int[] MALE_FEETS = new int[] {
		42, 43
	};
	
	public static readonly int[] FEMALE_FEETS = new int[] {
		79, 80
	};
	
	public static int[] GetHeads(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_HEADS,
			Gender.FEMALE => FEMALE_HEADS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetJaws(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_JAWS,
			Gender.FEMALE => FEMALE_JAWS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetTorsos(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_TORSOS,
			Gender.FEMALE => FEMALE_TORSOS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetArms(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_ARMS,
			Gender.FEMALE => FEMALE_ARMS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetHands(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_HANDS,
			Gender.FEMALE => FEMALE_HANDS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetLegs(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_LEGS,
			Gender.FEMALE => FEMALE_LEGS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
	public static int[] GetFeets(Gender gender)
	{
		return gender switch {
			Gender.MALE => MALE_FEETS,
			Gender.FEMALE => FEMALE_FEETS,
			_ => throw new IllegalArgumentException("Unexpected Gender")
		};
	}
	
}