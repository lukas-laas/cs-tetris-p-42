
class Renderer
{
    private readonly Board board;
    public Renderer(Board board)
    {
        this.board = board;

        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");

        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false;
    }

    public void RenderBoard()
    {
        const int AspectRatioCorrection = 2; // Console characters are taller than they are wide
        int CanvasWidth = board.Width * AspectRatioCorrection;

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

        // Color extraction
        List<Tetromino> tetrominoes = board.GetAllTetrominoes();
        string[,] colorGrid = new string[board.Height, board.Width];
        foreach (Tetromino tetromino in tetrominoes)
        {
            foreach ((int x, int y) in tetromino.GetTileCoords())
            {
                colorGrid[y, x] = tetromino.Color;
            }
        }

        // Board
        string boardPadLeft = new(' ', 8);
        buffer += $"{boardPadLeft}{AnsiColor.BorderBlue($"╭{new string('─', CanvasWidth)}╮")}\n"; // top border
        for (int y = 0; y < board.Height; y++)
        {
            if (y < board.Height - board.VisibleHeight) continue; // Skip hidden rows

            buffer += $"{boardPadLeft}{AnsiColor.BorderBlue("│")}"; // Left border

            for (int x = 0; x < board.Width; x++)
            {
                buffer += board.CollisionGrid[y][x] ?
                    AnsiColor.Apply(new string('▓', 2), colorGrid[y, x])
                    :
                    new string(' ', 2);
            }
            buffer += $"{AnsiColor.BorderBlue("│")}\n"; // Right border
        }

        buffer += $"{boardPadLeft}{AnsiColor.BorderBlue($"▀{new string('▀', CanvasWidth)}▀")}\n"; // Bottom border

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }
}