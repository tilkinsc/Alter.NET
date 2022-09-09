using System.Text;
using System.Text.RegularExpressions;

namespace Cache.Util;

class RLNamer
{
	
	private HashSet<string> Used = new HashSet<string>();
	
	public string? Name(string? name, int id)
	{
		name = Sanitize(name);
		if (name == null)
			return null;
		
		if (Used.Contains(name)) {
			name = name + "_" + id;
			// assert !used.Contains(name);
		}
		
		Used.Add(name);
		
		return name;
	}
	
	private static string? Sanitize(string? str)
	{
		if (string.IsNullOrEmpty(str))
			return null;
		
		string s = Regex.Replace(
				RemoveTags(str)
				.ToUpper()
				.Replace(' ', '_'),
			"[^a-zA-Z0-9_]",
			"");
		if (string.IsNullOrEmpty(s))
			return null;
		
		if (char.IsDigit(s[0])) {
			return "_" + s;
		}
		return s;
	}
	
	public static string RemoveTags(string str)
	{
		StringBuilder builder = new StringBuilder(str.Length);
		bool inTag = false;
		
		for (int i=0; i<str.Length; i++)
		{
			char currentChar = str[i];
			
			if (currentChar == '<') {
				inTag = true;
			} else if (currentChar == '>') {
				inTag = false;
			} else if (!inTag) {
				builder.Append(currentChar);
			}
		}
		
		return builder.ToString();
	}
	
}
