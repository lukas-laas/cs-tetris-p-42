
class Board
{
    // Handles "physics" of pieces on the board

    // Color info (affected by buffs/debuffs)

    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 40;
    public int VisibleHeight { get; set; } = 20;

    private CollisionGrid collisionGrid = [];
    private List<Tile> settledTiles = [];
    private List<Tetromino> fallingTetrominoes = []; // usually just one but debuffs might change that

    public CollisionGrid CollisionGrid => collisionGrid;
    public List<Tile> SettledTiles => settledTiles;
    public List<Tetromino> FallingTetrominoes => fallingTetrominoes;

    public Board()
    {
        for (int y = 0; y < Height; y++)
        {
            collisionGrid.Add([.. new bool[Width]]);
        }
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> allTiles = [];
        allTiles.AddRange(settledTiles);
        foreach (Tetromino tetromino in fallingTetrominoes)
        {
            allTiles.AddRange(tetromino.GetTiles());
        }
        return allTiles;
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
        foreach (Tile tile in settledTiles)
        {
            int y = tile.Y;
            int x = tile.X;
            if (y >= 0 && y < Height && x >= 0 && x < Width)
            {
                collisionGrid[y][x] = true;
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
        // Move falling tetrominoes down by one if possible
        List<Tetromino> iterableTetrominoes = [.. fallingTetrominoes];
        foreach (Tetromino tetromino in iterableTetrominoes)
        {
            if (tetromino.CanMove(0, 1, collisionGrid, Width, Height))
                tetromino.SetPosition(tetromino.X, tetromino.Y + 1);
            else // Cannot move down, settle the tetromino
            {
                settledTiles.AddRange(tetromino.GetTiles());
                fallingTetrominoes.Remove(tetromino);
                // Check rows for clear
                // Update collision
                UpdateCollisionGrid();
                // Check rows for full row
                for (int y = 0; y < collisionGrid.Count; y++)
                {
                    if (collisionGrid[y].All((b) => b == true))
                    {
                        collisionGrid.RemoveAt(y);
                    }
                }
                // Add empty to top
            }
        }

        int filledSlot = 0;
        for (int y = 0; y < Height; y++)
        {
            filledSlot = 0;
            for (int x = 0; x < Width; x++)
            {
                if (collisionGrid[y][x] == true) filledSlot++;
                if (filledSlot == Width)
                {
                    // TODO: implement, if this is not the right place I'm sorry Vena..
                    // Board.clearRow(y);
                }
            }
        }

        UpdateCollisionGrid();
    }
}