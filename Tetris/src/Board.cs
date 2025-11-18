
class Board
{
    // Handles "physics" of pieces on the board

    // Color info (affected by buffs/debuffs)

    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 40;
    public int VisibleHeight { get; set; } = 20;

    private CollisionGrid collisionGrid = [];
    private List<Tile> settledTiles = [];
    private List<Polyomino> fallingPolyominoes = []; // usually just one but debuffs might change that

    public CollisionGrid CollisionGrid => collisionGrid;
    public List<Tile> SettledTiles => settledTiles;
    public List<Polyomino> FallingPolyominoes => fallingPolyominoes;

    public Board()
    {
        for (int y = 0; y < Height; y++)
        {
            collisionGrid.Add([.. new bool[Width]]);
        }
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> allTiles = [.. settledTiles];
        foreach (Polyomino polyomino in fallingPolyominoes)
        {
            allTiles.AddRange(polyomino.GetTiles());
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

    public void AddPolyomino(Polyomino polyomino, int? xPosition = null, int? yPosition = null)
    {
        int x = xPosition ?? (Width / 2 - 2) + polyomino.SpawnXOffset;
        int y = yPosition ?? Height - VisibleHeight - 2 + polyomino.SpawnYOffset;

        fallingPolyominoes.Add(polyomino);
        polyomino.SetPosition(x, y);
        UpdateCollisionGrid();
    }

    public void Tick()
    {
        // Move falling polyominoes down by one if possible
        List<Polyomino> iterablePolyominoes = new List<Polyomino>(fallingPolyominoes);
        foreach (Polyomino polyomino in iterablePolyominoes)
        {
            if (polyomino.CanMove(0, 1, collisionGrid, Width, Height))
            {
                polyomino.SetPosition(polyomino.X, polyomino.Y + 1);
                continue;
            }

            // Cannot move down, settle the polyomino
            settledTiles.AddRange(polyomino.GetTiles());
            fallingPolyominoes.Remove(polyomino);

            UpdateCollisionGrid();
            for (int y = 0; y < collisionGrid.Count; y++)
            {
                if (!collisionGrid[y].All((b) => b == true)) continue;

                // Remove tiles on the cleared row
                settledTiles.RemoveAll((tile) => tile.Y == y);

                // Move all tiles above the cleared row down by 1
                foreach (var tile in settledTiles)
                {
                    if (tile.Y < y)
                        tile.Y += 1;
                }

                UpdateCollisionGrid();
                y -= 1; // Recheck line in case resettled tiles clear for sanity
            }
        }
    }
}