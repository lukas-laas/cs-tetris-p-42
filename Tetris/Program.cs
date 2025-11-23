internal class Program
{
    private static readonly string logDirectory = "logs";
    private static readonly string logFileName = Path.Join(logDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss") + ".log");

    private static void Main(string[] args)
    {
        // https://tetris.wiki/Tetris_Guideline

        // Log file
        Directory.CreateDirectory(logDirectory);
        File.Create(logFileName);

        // Game entry point
        _ = new GameState();
    }

    public static void Log(string message)
    {
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        File.AppendAllText(logFileName, $"[{dateTime}] {message}{Environment.NewLine}");
    }
}