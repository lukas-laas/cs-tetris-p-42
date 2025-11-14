
class Renderer
{
    private readonly Games games;

    public Renderer(Games games)
    {
        this.games = games;

        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");

        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false;
    }

    public void Render()
    {
        const int AspectRatioCorrection = 2; // Console characters are taller than they are wide

        List<Board> boards = games.GetAllBoards();

        List<int> canvasWidths = [.. boards.Select(b => b.Width * AspectRatioCorrection)];
        List<string[,]> colorGrids = [];

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

        // Debug info
        for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
        {
            Board board = boards[boardIndex];
            buffer += $"   Settled:{board.SettledTetrominoes.Count}  Falling:{board.FallingTetrominoes.Count}";
        }
        buffer += "\n";

        // Color extraction
        for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
        {
            Board board = boards[boardIndex];
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
        // for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
        // {
        //     int canvasWidth = canvasWidths[boardIndex];
        //     buffer += $"{AnsiColor.BorderBlue($"╭{new string('─', canvasWidth)}╮")}"; // top border
        // }
        // buffer += "\n";

        int highest = boards.Select(b => b.Height).Max();
        int highestVisible = boards.Select(b => b.VisibleHeight).Max();

        for (int y = 0; y < highest; y++)
        {
            
            if (y < highestVisible) continue;

            for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
            {
                Board board = boards[boardIndex];
                string[,] colorGrid = colorGrids[boardIndex];

                buffer += $"{AnsiColor.BorderBlue("│")}"; // Left border
                for (int x = 0; x < board.Width; x++)
                {
                    // 
                    buffer += colorGrid[y, x] != null ?
                        AnsiColor.Apply(new string('▓', 2), colorGrid[y, x])
                        :
                        new string(' ', 2);
                }
                buffer += $"{AnsiColor.BorderBlue("│")}"; // Right border
            }
            buffer += "\n";
        }

        for (int boardIndex = 0; boardIndex < boards.Count; boardIndex++)
        {
            int canvasWidth = canvasWidths[boardIndex];
            buffer += boardFloor(canvasWidth);
        }
        buffer += "\n";

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }

    private string boardRoof(int width)
        => AnsiColor.BorderBlue($"╭{new string('─', width)}╮");

    private string boardWall(int width)
        => AnsiColor.BorderBlue("│") + new string(' ', width) + AnsiColor.BorderBlue("│");

    private string boardWall(string content)
        => AnsiColor.BorderBlue("│") + content + AnsiColor.BorderBlue("│");

    private string boardFloor(int width)
        => AnsiColor.BorderBlue($"▀{new string('▀', width)}▀");
}