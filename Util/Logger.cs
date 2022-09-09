using System.Runtime.CompilerServices;
using System.Text;

namespace Util;

static class Logger
{
	
	private const string DEFAULT_OUTPUT = "./logger.log";
	
	private static StreamWriter? writerStream;
	private static bool loggable = false;
	
	public static void Init(Stream? stream = null)
	{
		try {
			if (stream != null) {
				writerStream = new StreamWriter(stream, Encoding.UTF8);
			} else {
				writerStream = new StreamWriter(DEFAULT_OUTPUT, true, Encoding.UTF8);
			}
			Console.SetOut(writerStream);
			Console.SetError(writerStream);
			loggable = true;
		} catch (Exception e) {
			Console.WriteLine(e);
			writerStream = null;
		}
	}
	
	public static void Destroy()
	{
		if (writerStream != null) {
			writerStream.Flush();
			writerStream.Close();
		}
	}
	
	public static void Log(string data,
			[CallerMemberName] string name = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (writerStream == null)
			return;
		Console.WriteLine($"[{DateTime.Now}] [Log] {Path.GetFileName(sourceFilePath)}.{name}#{sourceLineNumber}: ${data}");
	}
	
	public static void Error(string data,
			[CallerMemberName] string name = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (writerStream == null)
			return;
		Console.WriteLine($"[{DateTime.Now}] [Error] {Path.GetFileName(sourceFilePath)}.{name}#{sourceLineNumber}: ${data}");
	}
	
	public static void Warn(string data,
			[CallerMemberName] string name = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
	{
		if (writerStream == null)
			return;
		Console.WriteLine($"[{DateTime.Now}] [Warn] {Path.GetFileName(sourceFilePath)}.{name}#{sourceLineNumber}: ${data}");
	}
	
}
