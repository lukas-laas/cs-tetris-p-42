internal class Program
{
    private static void Main(string[] args)
    {
        // https://tetris.wiki/Tetris_Guideline

        Board board = new();
        board.AddTetromino(new TetrominoI());
        board.AddTetromino(new TetrominoJ());
        board.AddTetromino(new TetrominoS());
        board.Tick();
        board.Tick();
        board.Tick();

        Renderer.RenderBoard(board);
    }
}