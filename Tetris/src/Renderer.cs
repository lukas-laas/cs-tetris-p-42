
class Renderer
{
    public static void RenderBoard(Board board)
    {
        CollisionGrid grid = board.CollisionGrid;
        List<Tetromino> tetrominoes = board.Tetrominoes;
        List<Tetromino> fallingTetrominoes = board.FallingTetrominoes;

        const int TitleWidth = 24;
        string titlePadLeft = new(' ', (int)Math.Floor((Console.WindowWidth - TitleWidth) / 2d));

        string buffer = "";
        buffer += $"""
        
        {titlePadLeft}▄▄▄▄ ▄▄▄ ▄▄▄▄ ▄▄▖ ▗▖ ▗▄▖ 
        {titlePadLeft} ▐▌  █▄   ▐▌  █▂█ ▐▌ ▀▙▝
        {titlePadLeft} ▐▌  █▄▄  ▐▌  █▀▙ ▐▌ ▜▄▛


        """;


        for (int y = board.VisibleHeight; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                if (grid[y][x])
                {
                    buffer += "#"; // occupied
                }
                else
                {
                    buffer += "."; // empty
                }
            }
            buffer += "\n";
        }

        Console.Clear(); // clear and draw to mitigate stutter and general unpleasantries
        Console.WriteLine(buffer);
    }
}