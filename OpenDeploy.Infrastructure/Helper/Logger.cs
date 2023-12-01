namespace OpenDeploy.Infrastructure;

public static class Logger
{
    /// <summary> 写日志基础方法  </summary>  
    public static void Write(string? msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

#if DEBUG
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\t{msg}");
#endif

    }
}
