internal class Program
{
    private static void Main(string[] args)
    {
        // Rough guidelines followed:
        // https://tetris.wiki/Tetris_Guideline

        Log.Add("Game started.");

        // Game entry point
        _ = new GameState();
    }

}