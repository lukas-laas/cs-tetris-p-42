
class Renderer
{
    public static void RenderBoard(Board board)
    {
        CollisionGrid grid = board.CollisionGrid;
        List<Tetromino> tetrominoes = board.Tetrominoes;
        List<Tetromino> fallingTetrominoes = board.FallingTetrominoes;

        string buffer = "";
        buffer += """
        
            ▄▄▄▄ ▄▄▄ ▄▄▄▄ ▄▄▖ ▗▖ ▗▄▖ 
             ▐▌  █▄   ▐▌  █▂█ ▐▌ ▀▙▝
             ▐▌  █▄▄  ▐▌  █▀▙ ▐▌ ▜▄▛


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