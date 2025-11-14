class Tetris
{
    // Next block(s) and such
    private List<Tetromino> queue = [];
    // board
    public Board Board { get; } = new();

    // Buffs and debuffs

    // score
    private int score;
    private int money;
    private int dt = 500;

    private long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    public void Tick()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - lastTick > dt)
        {
            Board.Tick();
        }
    }

    public void SetDt(int dt)
    {
        this.dt = dt;
    }

    public void Move(Input direction)
    {
        // Move block
    }
}