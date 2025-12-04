

class GameOverRenderer
{
    public static void Render(List<Player> drawees, List<Player> players)
    {
        string buffer = "";

        buffer += RenderUtils.Center2DString("It's a DRAW!");

        Render(buffer, players);
    }

    public static void Render(Player winner, List<Player> players)
    {
        string buffer = "";

        buffer += RenderUtils.Center2DString($"Winner: {winner.Name}!");

        Render(buffer, players);
    }

    private static void Render(string childContent, List<Player> players)
    {
        string buffer = "";

        // Title
        buffer += GameRenderer.MakeTitle(); // Borrow title from main game renderer since it's pwetty
        buffer += "\n\n";

        buffer += RenderUtils.Center2DString("=== GAME OVER ===");
        buffer += "\n";

        buffer += childContent;
        buffer += "\n";

        buffer += "\n";
        buffer += RenderUtils.Center2DString(MakeScoreboard(players));
        buffer += "\n";
        buffer += "\n";

        buffer += RenderUtils.Center2DString("Press any key to play again...");

        RenderUtils.Render(buffer);
    }

    private static string MakeScoreboard(List<Player> players)
    {
        const int width = 35;

        List<Player> sortedPlayers = [.. players.OrderByDescending(p => p.Score)];

        string buffer = "";
        string headerFirstHalf = "Scoreboard ";
        string headerSecondHalf = "score";
        int headerPadding = width - (headerFirstHalf.Length + headerSecondHalf.Length);
        buffer += $"{headerFirstHalf}{new string(' ', headerPadding)}{headerSecondHalf}\n";
        foreach (Player player in sortedPlayers)
        {
            string firstHalf = $"- {player.Name} ";
            string secondHalf = $" {player.Score}";

            int padding = width - (firstHalf.Length + secondHalf.Length);
            buffer += $"{firstHalf}{new string(' ', padding)}{secondHalf}\n";
        }

        return buffer;
    }
}