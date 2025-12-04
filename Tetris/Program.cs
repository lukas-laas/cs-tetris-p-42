/** 
 * Please have a look at the ../readme.md file for information about this project.
 */

internal class Program
{
    private static void Main()
    {
        // Rough guidelines followed:
        // https://tetris.wiki/Tetris_Guideline

        // Game entry point
        try
        {
            Restart();
        }
        catch (Exception e)
        {
            Log.Add(e.ToString());
        }
    }

    public static void Restart()
    {
        _ = new GameState();
    }

}