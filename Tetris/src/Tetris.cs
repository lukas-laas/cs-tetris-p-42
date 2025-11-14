class Tetris
{
    // Next block(s) and such
    private Queue<Tetromino> queue = [];
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
        if (currentTime - lastTick < dt) return;

        if (Board.FallingTetrominoes.Count == 0)
        {
            AddToQueue();
            Board.AddTetromino(queue.Dequeue());
        }
        Board.Tick();
    }

    public void SetDt(int dt)
    {
        this.dt = dt;
    }

    public void Move(Input direction)
    {
        // Move block
    }
    public void AddToQueue()
    {
        Random rng = new();
        queue.Enqueue(
        rng.Next(7) switch
        {
            0 => new TetrominoI(),
            1 => new TetrominoJ(),
            2 => new TetrominoL(),
            3 => new TetrominoO(),
            4 => new TetrominoS(),
            5 => new TetrominoT(),
            6 => new TetrominoZ(),
            _ => new TetrominoThiccI(), // IMPOSSIBLE
        });
    }
}