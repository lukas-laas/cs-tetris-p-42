class Tetris
{
    // Next block(s) and such
    private List<Tetromino> queue = [];
    // board
    private Board board = new();

    // Buffs and debuffs
    public int Dt;
    // score
    private int score;
    private int money;
    public void Tick()
    {
        if (Dt < 0)
        {
            board.Tick();
        }
    }

    public void Move(Input direction)
    {
        // Move block
    }
}