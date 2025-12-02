/// <summary>
/// The most non OOP class ever...
/// </summary>
static class Log
{
    private static readonly string logDirName = "logs";
    private static readonly string logFileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
    private static readonly string logFilePath = Path.Join(logDirName, logFileName);

    private static readonly DirectoryInfo _logDirInfo = Directory.CreateDirectory(logDirName); // Ensure log directory exists
    private static readonly FileStream logFileStream = File.Create(logFilePath);

    public static void Add(string message)
    {
        // Append message to log file with timestamp
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logMessage = $"[{dateTime}] {message}{Environment.NewLine}";
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(logMessage);
        logFileStream.Write(messageBytes, 0, messageBytes.Length);
        logFileStream.Flush();
    }
}