internal class Program
{
    private static void Main(string[] args)
    {
        // https://tetris.wiki/Tetris_Guideline

        Board board = new();
        board.AddTetromino(new TetrominoI(), xPosition: 0);
        board.AddTetromino(new TetrominoJ(), yPosition: 16);
        board.AddTetromino(new TetrominoS(), xPosition: 7);

        board.Tick();
        board.Tick();
        board.Tick();
        board.Tick();
        board.Tick();

        Renderer.RenderBoard(board);
    }
}