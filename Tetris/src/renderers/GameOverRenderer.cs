

class GameOverRenderer
{
    public static void Render(bool isDraw)
    {
        string buffer = "";

        if (!isDraw) throw new Exception("Cannot be called unless there's a draw.");

        

        Render(buffer);
    }

    public static void Render(Player winner, List<Player> losers)
    {
        string buffer = "";


        Render(buffer);
    }

    private static void Render(string childContent)
    {
        string buffer = "";

        // Title
        buffer += GameRenderer.MakeTitle(); // Borrow title from main game renderer since it's pwetty
        buffer += "\n\n";

        buffer += RenderUtils.Center2DString("=== GAME OVER ===");
        buffer += "\n\n";

        buffer += childContent;
        buffer += "\n\n";

        RenderUtils.Render(buffer);
    }
}