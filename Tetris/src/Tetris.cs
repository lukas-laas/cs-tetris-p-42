class Tetris
{
    // Next block(s) and such
    private List<Tetromino> queue = [];
    // board
    public Board Board { get; } = new();

    // Buffs and debuffs
    public int Dt;
    // score
    private int score;
    private int money;
    public void Tick()
    {
        if (Dt < 0)
        {
            Board.Tick();
        }
    }

    public void Move(Input direction)
    {
        // Move block
    }
}