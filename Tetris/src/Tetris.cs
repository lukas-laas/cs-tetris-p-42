class Tetris
{
    // Next block(s) and such
    private Queue<Polyomino> queue = [];
    public Queue<Polyomino> Queue => queue;
    // board
    public Board Board { get; } = new();

    // Buffs and debuffs

    // score
    private int score;
    public int Score => score;
    private int money;
    public int Money => money;
    private int dt = 500;

    private long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public Tetris()
    {
        AddToQueue(4);
    }

    public void Tick()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - lastTick < dt) return;
        lastTick = currentTime;

        if (Board.FallingPolyominoes.Count == 0)
        {
            AddToQueue();
            Board.AddPolyomino(queue.Dequeue());
        }
        Board.Tick();
    }

    public void SetDt(int dt)
    {
        this.dt = dt;
    }

    public void Move(Input direction)
    {
        foreach (Polyomino polyomino in Board.FallingPolyominoes)
        {
            switch (direction)
            {
                case Input.Left:
                    if (polyomino.CanMove(-1, 0, Board)) polyomino.X--;
                    break;
                case Input.Right:
                    if (polyomino.CanMove(1, 0, Board)) polyomino.X++;
                    break;
                case Input.SoftDrop:
                    if (polyomino.CanMove(0, 1, Board)) polyomino.Y++;
                    break;
                case Input.Rotate:
                    polyomino.Rotate(); // TODO super rotations
                    break;
                default:
                    throw new Exception("WTF!");
            }
        }
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
            _ => new OctominoThiccI(), // IMPOSSIBLE
        });
    }
    public void AddToQueue(int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            AddToQueue();
        }
    }
}