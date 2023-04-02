
## Contributing

Alter.NET has a strict style guide.

The style guide is in place for these reasons:
* Keep on track with the Alter source
* Prevent code from looking like a Frankenstien

1. All variables should be in order of their Alter definition, unless they are defined below the constructor.
2. All variables are to be defined above the constructor in order of their Alter definition.
3. Retain all variable/class names to the best of your ability, unless they are shorthand names (i.e. attr -> attribute)
4. Functions, if, while, and for statements should be defined like this,
```cs
public void Example()
{
	if (someGuardClause)
		return;
	
	if (someExpression)
	{
		while (someExpression)
		{
			
		}
		for (int i=0; i<5; i++)
		{
			DoSomething();
		}
		foreach (ClassName className in someList)
		{
			
		}
	}
}
```
5. All variable and functions should be PascalCase, see #8, #9.
6. Class fields defined with val should be properties with private set: `public int Example { get; private set; }`
7. Class fields defined with var or no declarator should be fields.
8. Private class fields should be camelCase prefixed with an underscore: `private int _example;`
9. Function parameters and local variables should be camelCase.
10. Enums that are more than just one int field should be a class. Enum class fields must be implemeneted with readonly. See Game.Model.Direction.cs for an example.


