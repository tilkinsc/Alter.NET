using System.Text;

namespace Game.Model.Timer;

static class TimeConstants
{
	
	public const long CYCLES_PER_MINUTE = 100;
	public const long CYCLES_PER_HOUR = 6000;
	public const long CYCLES_PER_DAY = 144000;
	public const long CYCLES_PER_WEEK = 1008000;
	public const long CYCLES_PER_YEAR = 52416000;
	
	public const long SECOND = 1000;
	public const long MINUTE = 60000;
	public const long HOUR = 3600000;
	public const long DAY = 86400000;
	public const long WEEK = 604800000;
	public const long YEAR = 31449600000;
	
	public static long? SecondsToCycles(long seconds)
	{
		long secs = (long) (seconds * 0.6);
		return secs > 0 ? secs : null;
	}
	
	public static long? MinutesToCycles(long minutes)
	{
		long mins = minutes * CYCLES_PER_MINUTE;
		return mins > 0 ? mins : null;
	}
	
	public static long? HoursToCycles(long hours)
	{
		long hrs = hours * CYCLES_PER_HOUR;
		return hrs > 0 ? hrs : null;
	}
	
	public static long? DaysToCycles(long days)
	{
		long d = days * CYCLES_PER_DAY;
		return d > 0 ? d : null;
	}
	
	public static long? WeeksToCycles(long weeks)
	{
		long wks = weeks * CYCLES_PER_WEEK;
		return wks > 0 ? wks : null;
	}
	
	public static long? YearsToCycles(long years)
	{
		long yrs = years * CYCLES_PER_YEAR;
		return yrs > 0 ? yrs : null;
	}
	
	public static long? CyclesToSeconds(long cycles)
	{
		long secs = (long) Math.Round(cycles / 0.6);
		return secs > 0 ? secs : null;
	}
	
	public static long? CyclesToMinutes(long cycles)
	{
		long mins = (long) (cycles / (double) CYCLES_PER_MINUTE);
		return mins > 0 ? mins : null;
	}
	
	public static long? CyclesToHours(long cycles)
	{
		long hrs = (long) (cycles / (double) CYCLES_PER_HOUR);
		return hrs > 0 ? hrs : null;
	}
	
	public static long? CyclesToDays(long cycles)
	{
		long days = (long) (cycles / (double) CYCLES_PER_DAY);
		return days > 0 ? days : null;
	}
	
	public static long? CyclesToWeeks(long cycles)
	{
		long wks = (long) (cycles / (double) CYCLES_PER_WEEK);
		return wks > 0 ? wks : null;
	}
	
	public static long? CyclesToYears(long cycles)
	{
		long yrs = (long) (cycles / (double) CYCLES_PER_YEAR);
		return yrs > 0 ? yrs : null;
	}
	
	public static string GetCyclesContext(long cycles)
	{
		long value = cycles;
		double amount = 0.0;
		if (value < CYCLES_PER_MINUTE*3) {
			amount = value / 1.666;
			if (amount > 1.0)
				return $"{amount} seconds";
			else return $"{amount} second";
		}
		if (value < CYCLES_PER_HOUR) {
			amount = value / CYCLES_PER_MINUTE;
			if (amount > 1.0)
				return $"{amount} minutes";
			else return $"{amount} minute";
		}
		if (value < CYCLES_PER_DAY) {
			amount = value / CYCLES_PER_HOUR;
			if (amount > 1.0)
				return $"{amount} hours";
			else return $"{amount} hour";
		}
		if (value < CYCLES_PER_WEEK) {
			amount = value / CYCLES_PER_DAY;
			if (amount > 1.0)
				return $"{amount} days";
			else return $"{amount} day";
		}
		if (value < CYCLES_PER_WEEK*26) {
			amount = value / (CYCLES_PER_WEEK * 4);
			if (amount > 1.0)
				return $"{amount} months";
			else return $"{amount} month";
		}
		amount = value / CYCLES_PER_YEAR;
		if (amount > 1.0)
			return $"{amount} years";
		else return $"{amount} year";
	}
	
	public static long TimePassed(long last)
	{
		return Util.Time.CurrentTimeMillis() - last;
	}
	
	public static string GetTimeContext(long time)
	{
		long value = time;
		double amount = 0.0;
		if (value < MINUTE) {
			amount = value / SECOND;
			if (value < 1.0)
				return $"{amount} seconds";
			else return $"{amount} second";
		}
		if (value < HOUR) {
			amount = value / MINUTE;
			if (value < 1.0)
				return $"{amount} minutes";
			else return $"{amount} minute";
		}
		if (value < DAY) {
			amount = value / HOUR;
			if (value < 1.0)
				return $"{amount} hours";
			else return $"{amount} hour";
		}
		if (value < DAY * 84) {
			amount = value / DAY;if (value < 1.0)
				return $"{amount} days";
			else return $"{amount} day";
		}
		if (value < YEAR) {
			amount = value / (DAY * 28);
			if (value < 1.0)
				return $"{amount} months";
			else return $"{amount} month";
		}
		amount = value / YEAR;
		if (value < 1.0)
			return $"{amount} years";
		else return $"{amount} year";
	}
	
	public static string GetBriefTimeContext(long time)
	{
		string complete = GetCompleteTimeContext(time);
		if (complete.Count((ch) => ch == ' ') > 2) {
			string[] parts = complete.Split(' ');
			return $"{parts[0]} {parts[1]} {parts[2]}";
		}
		return complete;
	}
	
	public static string GetCompleteTimeContext(long time)
	{
		StringBuilder sb = new StringBuilder();
		long value = time;
		double amount = 0.0;
		if (value >= YEAR) {
			amount = (value / YEAR);
			if (amount > 1)
				sb.Append($"{amount} years");
			else sb.Append($"{amount} year");
			value %= YEAR;
			if (value != 0)
				sb.Append(" ");
		}
		if (value >= WEEK) {
			amount = (value / WEEK);
			if (amount > 1)
				sb.Append($"{amount} weeks");
			else sb.Append($"{amount} week");
			value %= WEEK;
			if (value != 0)
				sb.Append(" ");
		}
		if (value >= DAY) {
			amount = (value / DAY);
			if (amount > 1)
				sb.Append($"{amount} days");
			else sb.Append($"{amount} day");
			value %= DAY;
			if (value != 0)
				sb.Append(" ");
		}
		if (value >= HOUR) {
			amount = (value / HOUR);
			if (amount > 1)
				sb.Append($"{amount} hours");
			else sb.Append($"{amount} hour");
			value %= HOUR;
			if (value != 0)
				sb.Append(" ");
		}
		if (value >= MINUTE) {
			amount = (value / MINUTE);
			if (amount > 1)
				sb.Append($"{amount} minutes");
			else sb.Append($"{amount} minute");
			value %= MINUTE;
			if (value != 0)
				sb.Append(" ");
		}
		if (value >= SECOND) {
			amount = (value / SECOND);
			if (amount > 1)
				sb.Append($"{amount} seconds");
			else sb.Append($"{amount} second");
			value %= SECOND;
		}
		return sb.ToString();
	}
	
}
