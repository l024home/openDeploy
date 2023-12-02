namespace OpenDeploy.Infrastructure;

/// <summary> 写日志基础方法  </summary>  
public static class Logger
{
    private static readonly object lockObj = new();

    public static void Info(string? msg)
    {
        Write(msg);
    }

    public static void Error(string? msg)
    {
        Write(msg, ConsoleColor.Red);
    }

    public static void Warn(string? msg)
    {
        Write(msg, ConsoleColor.Green);
    }


    private static void Write(string? msg, ConsoleColor color = ConsoleColor.Gray)
    {
        if (string.IsNullOrEmpty(msg)) { return; }
#if DEBUG
        lock (lockObj)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\t{msg}");
        }
#endif
    }
}
