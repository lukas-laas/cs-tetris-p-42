internal class Program
{
    private static void Main(string[] args)
    {
        // https://tetris.wiki/Tetris_Guideline

        Board board = new();
        Renderer.RenderBoard(board);
    }
}