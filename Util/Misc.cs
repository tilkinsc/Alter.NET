using System.Linq;
using System.Text.RegularExpressions;

namespace Util;

class Misc
{
	
	public static readonly int[] DIRECTION_DELTA_X = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
	public static readonly int[] DIRECTION_DELTA_Z = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
	
	public int GetPlayerWalkingDirection(int dx, int dy)
	{
		if (dx == -1 && dy == -1)
			return 0;
		if (dx == 0 && dy == -1)
			return 1;
		if (dx == 1 && dy == -1)
			return 2;
		if (dx == -1 && dy == 0)
			return 3;
		if (dx == 1 && dy == 0)
			return 4;
		if (dx == -1 && dy == 1)
			return 5;
		if (dx == 0 && dy == 1)
			return 6;
		if (dx == 1 && dy == 1)
			return 7;
		return -1;
	}
	
	public int GetPlayerRunningDirection(int dx, int dy)
	{
		if (dx == -2 && dy == -2)
			return 0;
		if (dx == -1 && dy == -2)
			return 1;
		if (dx == 0 && dy == -2)
			return 2;
		if (dx == 1 && dy == -2)
			return 3;
		if (dx == 2 && dy == -2)
			return 4;
		if (dx == -2 && dy == -1)
			return 5;
		if (dx == 2 && dy == -1)
			return 6;
		if (dx == -2 && dy == 0)
			return 7;
		if (dx == 2 && dy == 0)
			return 8;
		if (dx == -2 && dy == 1)
			return 9;
		if (dx == 2 && dy == 1)
			return 10;
		if (dx == -2 && dy == 2)
			return 11;
		if (dx == -1 && dy == 2)
			return 12;
		if (dx == 0 && dy == 2)
			return 13;
		if (dx == 1 && dy == 2)
			return 14;
		if (dx == 2 && dy == 2)
			return 15;
		return -1;
	}
	
	// TODO: fill in this function
	// fun IntRange.toArray(): Array<Int> {
	// 	return toList().toTypedArray()
	// }
	
	// TODO: verify if the regex works
	public string GetIndefiniteArticle(string word)
	{
		char first = word.ToLower().First();
		bool vowel = first == 'a' || first == 'e' || first == 'i' || first == 'o' || first == 'u';
		bool numeric = new Regex(".*[0-9].*").IsMatch(word.First().ToString());
		bool some = new string[] { "bolts", "arrows", "coins", "vambraces", "chaps", "grapes", "silk", "bread", "grey wolf fur", "spice" }.Any(word.Contains);
		if (numeric)
			return $" {word}";
		if (vowel)
			return $"an {word}";
		if (some)
			return $"some {word}";
		return $" {word}";
	}
	
}