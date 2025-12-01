
class GameRenderer
{
    private readonly List<Player> players;

    private static readonly int aspectRatioCorrection = 2; // Console characters are taller than they are wide
    private static readonly int boardSpacing = 16;

    private static readonly Dictionary<string, string> boardBorder = new()
    {
        {"topLeft",          "╭"},
        {"topRight",         "╮"},
        {"topHorizontal",    "─"},
        {"side",             "│"},
        {"bottomLeft",       "▀"}, // Alt ╰
        {"bottomRight",      "▀"}, // Alt ╯
        {"bottomHorizontal", "▀"}, // Alt ─
    };
    private static readonly char occupiedTileChar = '▓'; // Alt '▓' '█'
    private static readonly char emptyTileChar = ' ';
    private static readonly Dictionary<string, string> queueBorder = new()
    {
        {"topLeft",          "╔"},
        {"topRight",         "╗"},
        {"topHorizontal",    "═"},
        {"side",             "║"},
        {"bottomLeft",       "╚"},
        {"bottomRight",      "╝"},
        {"bottomHorizontal", "═"},
    };

    public GameRenderer(GameState gameState)
    {
        this.players = gameState.Players;

        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");

        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false; // Will not restore on exit, but oh well
    }

    public void Render()
    {
        string buffer = "";

        // Title
        buffer += MakeTitle();
        buffer += "\n";

        // Boards
        buffer += RenderUtils.Center2DString(RenderUtils.Merge2DStrings([.. players.Select(MakeBoard)], boardSpacing));

        RenderUtils.Render(buffer);
    }

    private static string MakeTitle()
    {
        string title = $"""

        {AnsiColor.BorderBlue("╔═══════════════════════════════╗")}
        {AnsiColor.BorderBlue("║")} {AnsiColor.Red("▄▄▄▄")} {AnsiColor.Orange("▄▄▄")} {AnsiColor.Yellow("▄▄▄▄")} {AnsiColor.Green("▄▄▖")} {AnsiColor.Cyan("▗▖")} {AnsiColor.Magenta("▗▄▖")} {AnsiColor.White(" ▖▗ ")} {AnsiColor.BorderBlue("║")}
        {AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄ ")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▂█")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▀▙▝")} {AnsiColor.White("▝▛▜▘")} {AnsiColor.BorderBlue("║")}
        {AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄▄")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▀▙")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▜▄▛")} {AnsiColor.White("▝▛▜▘")} {AnsiColor.BorderBlue("║")}
        {AnsiColor.BorderBlue("║")}                               {AnsiColor.BorderBlue("║")}
        {AnsiColor.BorderBlue("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀")}

        """;

        return RenderUtils.Center2DString(title);
    }

    private static string[,] MakeColorGrid(Board board)
    {
        string[,] colorGrid = new string[board.Height, board.Width];
        foreach (Tile tile in board.GetAllTiles())
        {
            if (tile.X < 0 || tile.X >= board.Width || tile.Y < 0 || tile.Y >= board.Height)
            {
                throw new Exception($"Tile out of bounds: ({tile.X}, {tile.Y}) on board of size ({board.Width}, {board.Height})");
            }
            colorGrid[tile.Y, tile.X] = tile.Color;
        }
        return colorGrid;
    }

    private static string MakeBoard(Player player)
    {
        Board board = player.Board;

        int innerCanvasWidth = board.Width * aspectRatioCorrection;

        // Info lines (prepended later)
        string infoLines = "";
        infoLines += InfoLine("Score", player.Score);
        infoLines += InfoLine("Money", player.Money);

        string buffer = "";

        // Top border
        buffer += AnsiColor.BorderBlue(
            boardBorder["topLeft"]
            + new string(boardBorder["topHorizontal"][0], innerCanvasWidth)
            + boardBorder["topRight"]
            + "\n"
        );

        // Body
        string[,] colorGrid = MakeColorGrid(board);
        for (int y = 0; y < board.Height; y++)
        {
            if (y < board.Height - board.VisibleHeight) continue; // Skip if not visible

            string row = "";
            for (int x = 0; x < board.Width; x++)
            {
                row += colorGrid[y, x] != null ?
                    AnsiColor.Apply(new string(occupiedTileChar, 2), colorGrid[y, x])
                    // AnsiColor.Apply("[]", colorGrid[y, x])
                    :
                    new string(emptyTileChar, 2);
            }
            buffer += AnsiColor.BorderBlue(boardBorder["side"])
                + $"{row}"
                + AnsiColor.BorderBlue(boardBorder["side"])
                + "\n";
        }

        // Bottom border
        buffer += AnsiColor.BorderBlue(
            boardBorder["bottomLeft"]
            + new string(boardBorder["bottomHorizontal"][0], innerCanvasWidth)
            + boardBorder["bottomRight"]
            + "\n"
        );

        // Add queue
        string queueString = MakeQueue(board);
        buffer = RenderUtils.Merge2DStrings([buffer, queueString], 2, false);

        // Prepend info lines at the end
        buffer = infoLines + buffer;
        return buffer;
    }

    private static string MakeQueue(Board board)
    {
        Queue<Polyomino> queue = board.Queue;

        int queueWidth = 5 * aspectRatioCorrection;

        string buffer = "";

        // Top border
        buffer += AnsiColor.BorderBlue(
            queueBorder["topLeft"]
            + new string(queueBorder["topHorizontal"][0], queueWidth)
            + queueBorder["topRight"]
            + "\n"
        );

        // Body
        foreach (Polyomino polyomino in queue)
        {
            List<(int, int)> coords = polyomino.GetTileCoords();

            const int width = 4;
            int height = polyomino.GetHeight();

            int startY = polyomino.GetTileCoords().Min(coord => coord.Item2);

            string row = "";
            for (int y = 0; y < Math.Clamp(height, 2, 5); y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (coords.Contains((polyomino.X + x, polyomino.Y + y + startY)))
                    {
                        row += AnsiColor.Apply(new string(occupiedTileChar, 2), polyomino.Color);
                    }
                    else
                    {
                        row += new string(emptyTileChar, 2);
                    }
                }

                // Content line
                buffer += AnsiColor.BorderBlue(queueBorder["side"])
                    + $" {row} "
                    + AnsiColor.BorderBlue(queueBorder["side"])
                    + "\n";

                row = ""; // Reset for reuse
            }

            // Empty lines between polyominoes
            buffer += AnsiColor.BorderBlue(queueBorder["side"])
                + $"{new string(emptyTileChar, queueWidth)}"
                + AnsiColor.BorderBlue(queueBorder["side"])
                + "\n";
        }

        // Bottom border
        buffer += AnsiColor.BorderBlue(
            queueBorder["bottomLeft"]
            + new string(queueBorder["bottomHorizontal"][0], queueWidth)
            + queueBorder["bottomRight"]
            + "\n"
        );
        return buffer;
    }
    private static string InfoLine(string label, string value)
        => AnsiColor.Gray($" {label}:{value}\n");

    private static string InfoLine(string label, int value)
        => InfoLine(label, value.ToString());
}