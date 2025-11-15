
class Renderer
{
    private readonly Games games;

    private List<Board> Boards { get; set; }
    private List<int> CanvasWidths { get; set; }
    private readonly int aspectRatioCorrection = 2; // Console characters are taller than they are wide
    private readonly int boardSpacing = 4;

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

        // Title
        const int TitleWidth = 24;
        string titlePadLeft = new(' ', Math.Max(Console.WindowWidth - TitleWidth, 0) / 2);
        buffer += $"""

        {titlePadLeft}{AnsiColor.BorderBlue("╔══════════════════════════╗")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red("▄▄▄▄")} {AnsiColor.Orange("▄▄▄")} {AnsiColor.Yellow("▄▄▄▄")} {AnsiColor.Green("▄▄▖")} {AnsiColor.Cyan("▗▖")} {AnsiColor.Magenta("▗▄▖")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄ ")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▂█")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▀▙▝")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")} {AnsiColor.Red(" ▐▌ ")} {AnsiColor.Orange("█▄▄")} {AnsiColor.Yellow(" ▐▌ ")} {AnsiColor.Green("█▀▙")} {AnsiColor.Cyan("▐▌")} {AnsiColor.Magenta("▜▄▛")} {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("║")}                          {AnsiColor.BorderBlue("║")}
        {titlePadLeft}{AnsiColor.BorderBlue("▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀")}

        """;

        // Debug info above each board
        buffer += $" {DebugLine("Falling", (b) => b.FallingTetrominoes.Count.ToString())}\n";
        buffer += $" {DebugLine("Settled", (b) => b.SettledTetrominoes.Count.ToString())}\n";

        // Color extraction from the tetrominoes on each board
        List<string[,]> colorGrids = [];
        for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
        {
            Board board = Boards[boardIndex];
            string[,] colorGrid = new string[board.Height, board.Width];
            List<Tetromino> tetrominoes = board.GetAllTetrominoes();
            foreach (Tetromino tetromino in tetrominoes)
            {
                foreach ((int x, int y) in tetromino.GetTileCoords())
                {
                    if (x >= 0 && x < board.Width && y >= 0 && y < board.Height)
                        colorGrid[y, x] = tetromino.Color;
                }
            }
            colorGrids.Add(colorGrid);
        }

        // Boards
        int highest = Boards.Select(b => b.Height).Max();
        int highestVisible = Boards.Select(b => b.Height - b.VisibleHeight).Min();
        for (int y = 0; y < highest; y++)
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
                    continue;
                }

                // If this is the first row above the visible area, draw a roof
                if (y == board.Height - board.VisibleHeight)
                {
                    int canvasWidth = CanvasWidths[boardIndex];
                    buffer += BoardRoof(canvasWidth);
                    continue;
                }

                string row = "";
                for (int x = 0; x < board.Width; x++)
                {
                    row += colorGrid[y, x] != null ?
                        AnsiColor.Apply(new string('▓', 2), colorGrid[y, x])
                        :
                        new string(' ', 2);
                }
                buffer += BoardWall(row);
            }
            buffer += "\n";
        }
        buffer += "   "; // Line number padding adjustment
        // Floors 
        for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
        {
            int canvasWidth = CanvasWidths[boardIndex];
            buffer += BoardFloor(canvasWidth);
        }
        buffer += "\n";

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }

    public void RefetchBoards()
    {
        this.Boards = games.GetAllBoards();
    }

    private static string BoardRoof(int width)
        => AnsiColor.BorderBlue($"╭{new string('─', width)}╮");

    private static string BoardWall(string content)
        => AnsiColor.BorderBlue("│") + content + AnsiColor.BorderBlue("│");

    private static string BoardFloor(int width)
        => AnsiColor.BorderBlue($"▀{new string('▀', width)}▀");

    /** Value getter runs on a Board */
    private string DebugLine(string label, Func<Board, string> valueGetter)
    {
        string line = "   "; // Line number padding
        for (int boardIndex = 0; boardIndex < Boards.Count; boardIndex++)
        {
            int canvasWidth = CanvasWidths[boardIndex];
            line += AnsiColor.Gray($"{label}:{valueGetter(Boards[boardIndex])}").PadRight(canvasWidth + 2 + boardSpacing);
        }
        return line;
    }
}