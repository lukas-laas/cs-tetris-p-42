
class Board
{
    public int Width { get; } = 10;
    public int Height { get; } = 40;
    public int VisibleHeight { get; set; } = 20;

    public List<Func<Polyomino>> PolyominoPool { get; private set; } = [
        () => new TetrominoI(),
        () => new TetrominoJ(),
        () => new TetrominoL(),
        () => new TetrominoO(),
        () => new TetrominoS(),
        () => new TetrominoT(),
        () => new TetrominoZ(),
        () => new OctominoThiccI(),
        () => new DominoSmallI(),
        () => new TrominoLowerI(),
        () => new OctominoIII(),
        () => new NonominoBlocc(),
        () => new OctominoDonut(),
        () => new MonominoDot(),
        () => new PentominoArchBTW(),
        () => new PentominoX(),
        () => new TetrominoO(),
    ];

    public Queue<Polyomino> Queue { get; private set; } = [];
    public CollisionGrid CollisionGrid { get; private set; } = [];
    public List<Tile> SettledTiles { get; private set; } = [];
    public List<Polyomino> FallingPolyominoes { get; private set; } = []; // usually just one but debuffs might change that

    public int Score { get; set; } = 0;
    public int Money { get; set; } = 0;

    public int Dt { get; set; } = 500; // Delta time between ticks in ms
    private long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    private readonly ControlScheme controlScheme;
    private string[] ValidKeys => [.. controlScheme.Keys];

    public Board(ControlScheme controlScheme)
    {
        this.controlScheme = controlScheme;

        // Initialize collision grid
        for (int y = 0; y < Height; y++)
        {
            CollisionGrid.Add([.. new bool[Width]]);
        }

        // Fill the queue with initial polyominoes
        AddToQueue(4);
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> allTiles = [.. SettledTiles];
        foreach (Polyomino polyomino in FallingPolyominoes)
        {
            allTiles.AddRange(polyomino.GetTiles());
        }

        return allTiles;
    }

    private void UpdateCollisionGrid()
    {
        // Clear grid
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                CollisionGrid[y][x] = false;
            }
        }

        // Set occupied cells
        foreach (Tile tile in SettledTiles)
        {
            int y = tile.Y;
            int x = tile.X;
            if (y >= 0 && y < Height && x >= 0 && x < Width)
            {
                CollisionGrid[y][x] = true;
            }
        }
    }

    private void SpawnPolyomino(Polyomino polyomino, int? xPosition = null, int? yPosition = null)
    {
        int x = xPosition ?? Width / 2 - 2 + polyomino.SpawnXOffset;
        int y = yPosition ?? Height - VisibleHeight - 2 + polyomino.SpawnYOffset;

        FallingPolyominoes.Add(polyomino);
        polyomino.SetPosition(x, y);
        UpdateCollisionGrid();
    }

    public void Tick(string keyString)
    {
        Move(keyString); // Uncapped movement speed

        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - lastTick < Dt) return;
        lastTick = currentTime;

        if (FallingPolyominoes.Count == 0)
        {
            AddToQueue();
            SpawnPolyomino(Queue.Dequeue());
        }

        PhysicsTick();
    }

    private void PhysicsTick()
    {
        // Move falling polyominoes down by one if possible
        List<Polyomino> iterablePolyominoes = [.. FallingPolyominoes]; // Copy to avoid modification during iteration
        foreach (Polyomino polyomino in iterablePolyominoes)
        {
            if (polyomino.CanMove(0, 1, this))
            {
                polyomino.Y += 1;
                continue;
            }

            // Cannot move down, settle the polyomino
            SettledTiles.AddRange(polyomino.GetTiles());
            FallingPolyominoes.Remove(polyomino);

            // Track tiles instead of rows when clearing for features that require it like color clearing
            int tilesCleared = 0;

            UpdateCollisionGrid();
            for (int y = 0; y < CollisionGrid.Count; y++)
            {
                if (!CollisionGrid[y].All((b) => b == true)) continue;

                // Remove tiles on the cleared row
                SettledTiles.RemoveAll((tile) => tile.Y == y);
                tilesCleared += Width;

                // Move all tiles above the cleared row down by 1
                foreach (var tile in SettledTiles)
                {
                    if (tile.Y < y) tile.Y += 1;
                }

                UpdateCollisionGrid();
                y -= 1; // Recheck line in case resettled tiles clear for sanity
            }

            // Update score based on lines cleared
            Score += tilesCleared switch
            {
                0 => 0,
                10 => 100,
                20 => 300,
                30 => 500,
                40 => 800,
                _ => tilesCleared * 20,
            };
        }
    }

    private void Move(string keyString)
    {
        if (!ValidKeys.Contains(keyString)) return;

        Input selectedInput = controlScheme[keyString]; // Not sure if a copy is necessary TODO - look into it

        foreach (Polyomino polyomino in FallingPolyominoes)
        {
            switch (selectedInput)
            {
                case Input.Left:
                    if (polyomino.CanMove(-1, 0, this)) polyomino.X--;
                    break;
                case Input.Right:
                    if (polyomino.CanMove(1, 0, this)) polyomino.X++;
                    break;
                case Input.SoftDrop:
                    if (polyomino.CanMove(0, 1, this)) polyomino.Y++;
                    break;
                case Input.Rotate:
                    polyomino.Rotate(this);
                    break;
                default:
                    throw new Exception("Unrecognized input");
            }
        }
    }

    private void AddToQueue(int Count)
    {
        for (int i = 0; i < Count; i++) AddToQueue();
    }
    private void AddToQueue()
    {
        Queue.Enqueue(GetPolyomino());
    }

    private Polyomino GetPolyomino()
    {
        Random rng = new();
        int index = rng.Next(0, PolyominoPool.Count);
        return PolyominoPool[index]();
    }
}