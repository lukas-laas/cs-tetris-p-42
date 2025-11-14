
class Board
{
    // Handles "physics" of pieces on the board

    // Color info (affected by buffs/debuffs)

    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 40;
    public int VisibleHeight { get; private set; } = 20;

    private CollisionGrid collisionGrid = [];
    private List<Tetromino> tetrominoes = []; // Settled tetrominoes on the board, to keep track of colors and for updating collision grid
    private List<Tetromino> fallingTetrominoes = []; // usually just one but debuffs might change that

    public CollisionGrid CollisionGrid => collisionGrid;
    public List<Tetromino> Tetrominoes => tetrominoes;
    public List<Tetromino> FallingTetrominoes => fallingTetrominoes;

    public Board()
    {
        for (int y = 0; y < Height; y++)
        {
            collisionGrid.Add([.. new bool[Width]]);
        }
    }

    public List<Tetromino> GetAllTetrominoes()
    {
        return [.. tetrominoes, .. fallingTetrominoes];
    }

    public void UpdateCollisionGrid()
    {
        // Clear grid
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                collisionGrid[y][x] = false;
            }
        }

        // Set occupied cells
        foreach (Tetromino tetromino in GetAllTetrominoes())
        {
            foreach ((int x, int y) in tetromino.GetTileCoords())
            {
                if (y >= 0 && y < Height && x >= 0 && x < Width)
                {
                    collisionGrid[y][x] = true;
                }
            }
        }
    }

    public void AddTetromino(Tetromino tetromino, int? xPosition = null, int? yPosition = null)
    {
        fallingTetrominoes.Add(tetromino);
        tetromino.SetPosition(xPosition ?? (Width / 2 - 2), yPosition ?? Height - VisibleHeight - 2);
        UpdateCollisionGrid();
    }

    public void Tick()
    {
        // Move falling tetrominoes down by one
        foreach (Tetromino tetromino in fallingTetrominoes)
        {
            tetromino.SetPosition(tetromino.X, tetromino.Y + 1);
        }

        UpdateCollisionGrid();
    }
}