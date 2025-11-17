
class Renderer
{
    private readonly Games games;

    private List<Tetris> tetrises;
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
    private static readonly char occupiedTileChar = '▓';
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

    public Renderer(Games games)
    {
        this.games = games;
        this.tetrises = games.Tetrises;

        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");

        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false;
    }

    public void Render()
    {
        string buffer = "";

        // Title
        buffer += MakeTitle();

        buffer += "\n";

        // Boards
        buffer += Merge2DStrings([.. tetrises.Select(MakeBoard)], boardSpacing);

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }

    private static string MakeTitle()
    {
        // Title
        const int TitleWidth = 24;
        string titlePadLeft = new(' ', (Console.WindowWidth - TitleWidth) / 2 - 1);
        return $"""

        {titlePadLeft}{AnsiColor.BorderBlue("╔══════════════════════════╗")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red("▄▄▄▄")} {AnsiColor.Orange("▄▄▄")} {AnsiColor.Yellow("▄▄▄▄")} {AnsiColor.Green("▄▄▖")} {AnsiColor.Cyan("▗▖")} {AnsiColor.Magenta("▗▄▖")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄ ")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▂█")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▀▙▝")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄▄")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▀▙")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▜▄▛")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")}                          {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀")}

        """;
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

    private static string MakeBoard(Tetris tetris)
    {
        Board board = tetris.Board;

        string buffer = "";

        // Info lines
        buffer += InfoLine("Score", tetris.Score);
        buffer += InfoLine("Falling", board.FallingTetrominoes.Count);
        buffer += InfoLine("Settled tiles", board.SettledTiles.Count);

        // Top border
        buffer += MakeBoardRoof(board.Width);
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
                    :
                    new string(emptyTileChar, 2);
            }
            buffer += MakeBoardWall(row);
        }
        // Bottom border
        buffer += MakeBoardFloor(board.Width);
        return buffer;
    }

    private static string Merge2DStrings(List<string> parts, int spacing = 16)
    {
        List<List<string>> linesGroupedByPart = [.. parts.Select(part => part.Split('\n').ToList())];

        // Normalize line counts with leading empty lines
        int largestHeight = linesGroupedByPart.Max(part => part.Count);
        linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup => Enumerable.Repeat(string.Empty, largestHeight - lineGroup.Count).Concat(lineGroup).ToList())];

        // Normalize line lengths with trailing spaces to groups widest line
        linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup =>
        {
            int maxLength = lineGroup.Select(l => l.Length).Max();
            return lineGroup.Select(line => line.PadRight(maxLength)).ToList();
        })];

        // Merge line by line
        string buffer = "";
        for (int lineIndex = 0; lineIndex < largestHeight; lineIndex++)
        {
            string line = "";
            foreach (var group in linesGroupedByPart)
            {
                line += group[lineIndex] + new string(' ', spacing);
            }
            buffer += line.TrimEnd() + "\n"; // Trim trailing spaces
        }
        return buffer;
    }

    private static string MakeBoardRoof(int width)
        => AnsiColor.BorderBlue($"{boardBorder["topLeft"]}{new string(boardBorder["topHorizontal"][0], width * aspectRatioCorrection)}{boardBorder["topRight"]}\n");

    private static string MakeBoardWall(string content)
        => $"{AnsiColor.BorderBlue(boardBorder["side"])}{content}{AnsiColor.BorderBlue(boardBorder["side"])}\n";

    private static string MakeBoardFloor(int width)
        => AnsiColor.BorderBlue($"{boardBorder["bottomLeft"]}{new string(boardBorder["bottomHorizontal"][0], width * aspectRatioCorrection)}{boardBorder["bottomRight"]}\n");

    private static string InfoLine(string label, string value)
        => AnsiColor.Gray($"{label}:{value}\n");

    private static string InfoLine(string label, int value)
        => InfoLine(label, value.ToString());
}