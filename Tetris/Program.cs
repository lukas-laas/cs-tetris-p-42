internal class Program
{
    private static void Main()
    {
        // Rough guidelines followed:
        // https://tetris.wiki/Tetris_Guideline

        // Game entry point
        try
        {
            _ = new GameState();
        }
        catch (Exception e)
        {
            Log.Add(e.ToString());
        }
    }

}