
class Renderer
{
    private readonly Games games;

    private List<Board> Boards { get; set; }
    private List<int> CanvasWidths { get; set; }
    private static readonly int aspectRatioCorrection = 2; // Console characters are taller than they are wide
    private static readonly int boardSpacing = 32;

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

    public Renderer(Games games)
    {
        this.games = games;
        this.Boards = games.GetAllBoards();
        this.CanvasWidths = [.. Boards.Select(b => b.Width * aspectRatioCorrection)];

        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");

        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false;
    }

    public void Render()
    {
        string buffer = "";

        int totalCanvasesWidth = CanvasWidths.Count * 2 // Walls
            + CanvasWidths.Sum() // Canvases
            + (boardSpacing * (Boards.Count - 1)) // Spacing in-between
            + 5; // Line number area
        int boardPadLeftSize = (Console.WindowWidth - totalCanvasesWidth) / 2;

        // Title
        buffer += MakeTitle(totalCanvasesWidth);

        // Info lines
        buffer += $" {InfoLine("Score", t => t.Score)}\n";
        buffer += $" {InfoLine("Falling", b => b.FallingTetrominoes.Count.ToString())}\n";
        buffer += $" {InfoLine("Settled tiles", b => b.SettledTiles.Count.ToString())}\n";

        // Color extraction
        List<string[,]> colorGrids = MakeColorGrids(Boards);

        // Boards
        int tallestHeight = Boards.Select(b => b.Height).Max();
        int highestVisible = Boards.Select(b => b.Height - b.VisibleHeight).Min();
        for (int y = 0; y < tallestHeight; y++)
        {
            if (y < highestVisible) continue; // Skip if all are none visible

            // Line number
            buffer += AnsiColor.Gray(y.ToString()) + " ";

            // Bodies of boards
            for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
            {
                Board board = Boards[boardIndex];
                string[,] colorGrid = colorGrids[boardIndex];

                // If before first visible row, draw empty space, no wall
                if (y < board.Height - board.VisibleHeight)
                {
                    int canvasWidth = CanvasWidths[boardIndex];
                    buffer += new string(' ', canvasWidth + 2);
                    buffer += new string(' ', boardSpacing);
                    continue;
                }

                // If this is the first row above the visible area, draw a roof
                if (y == board.Height - board.VisibleHeight)
                {
                    int canvasWidth = CanvasWidths[boardIndex];
                    buffer += MakeBoardRoof(canvasWidth);
                    buffer += new string(' ', boardSpacing);
                    continue;
                }

                string row = "";
                for (int x = 0; x < board.Width; x++)
                {
                    row += colorGrid[y, x] != null ?
                        AnsiColor.Apply(new string(occupiedTileChar, 2), colorGrid[y, x])
                        :
                        new string(emptyTileChar, 2);
                }
                buffer += MakeBoardWall(row);
                buffer += new string(' ', boardSpacing);
            }
            buffer += "\n";
        }
        buffer += "   "; // Line number padding adjustment
        // Floors
        for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
        {
            int canvasWidth = CanvasWidths[boardIndex];
            buffer += MakeBoardFloor(canvasWidth);
            buffer += new string(' ', boardSpacing);
        }
        buffer += "\n";

        // Adjust final padding to fill the console width
        buffer = buffer.Replace("\n", $"\n{new string(' ', boardPadLeftSize)}");

        // Remove trailing spaces on each line for a cleaner look
        buffer = string.Join('\n', buffer.Split('\n').Select(line => line.TrimEnd()));

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }

    private static string MakeTitle(int totalCanvasesWidth)
    {
        // Title
        const int TitleWidth = 24;
        string titlePadLeft = new(' ', (totalCanvasesWidth - TitleWidth) / 2 - 1);
        return $"""

        {titlePadLeft}{AnsiColor.BorderBlue("╔══════════════════════════╗")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red("▄▄▄▄")} {AnsiColor.Orange("▄▄▄")} {AnsiColor.Yellow("▄▄▄▄")} {AnsiColor.Green("▄▄▖")} {AnsiColor.Cyan("▗▖")} {AnsiColor.Magenta("▗▄▖")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄ ")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▂█")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▀▙▝")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄▄")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▀▙")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▜▄▛")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")}                          {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀")}

        """;
    }

    private static List<string[,]> MakeColorGrids(List<Board> boards)
    {
        List<string[,]> colorGrids = [];
        for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
        {
            Board board = boards[boardIndex];
            string[,] colorGrid = new string[board.Height, board.Width];
            foreach (Tile tile in board.GetAllTiles())
            {
                if (tile.X < 0 || tile.X >= board.Width || tile.Y < 0 || tile.Y >= board.Height)
                {
                    throw new Exception($"Tile out of bounds: ({tile.X}, {tile.Y}) on board of size ({board.Width}, {board.Height})");
                }
                colorGrid[tile.Y, tile.X] = tile.Color;
            }
            colorGrids.Add(colorGrid);
        }
        return colorGrids;
    }

    private static string MakeBoardRoof(int width)
        => AnsiColor.BorderBlue($"{boardBorder["topLeft"]}{new string(boardBorder["topHorizontal"][0], width)}{boardBorder["topRight"]}");

    private static string MakeBoardWall(string content)
        => $"{AnsiColor.BorderBlue(boardBorder["side"])}{content}{AnsiColor.BorderBlue(boardBorder["side"])}";

    private static string MakeBoardFloor(int width)
        => AnsiColor.BorderBlue($"{boardBorder["bottomLeft"]}{new string(boardBorder["bottomHorizontal"][0], width)}{boardBorder["bottomRight"]}");

    private string InfoLine(string label, Func<Board, string> valueGetter)
    {
        string line = "   "; // Line number padding
        for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
        {
            int canvasWidth = CanvasWidths[boardIndex];
            line += AnsiColor.Gray($"{label}:{valueGetter(Boards[boardIndex])}".PadRight(canvasWidth + 2));
            line += new string(' ', boardSpacing);
        }
        return line;
    }
    private string InfoLine(string label, Func<Board, int> valueGetter)
        => InfoLine(label, (b) => valueGetter(b).ToString());
    private string InfoLine(string label, Func<Tetris, string> valueGetter)
    {
        string line = "   "; // Line number padding
        for (int tetrisIndex = 0; tetrisIndex < Boards.Count; tetrisIndex++)
        {
            int canvasWidth = CanvasWidths[tetrisIndex];
            line += AnsiColor.Gray($"{label}:{valueGetter(games.Tetrises[tetrisIndex])}".PadRight(canvasWidth + 2));
            line += new string(' ', boardSpacing);
        }
        return line;
    }
    private string InfoLine(string label, Func<Tetris, int> valueGetter)
        => InfoLine(label, (t) => valueGetter(t).ToString());
}