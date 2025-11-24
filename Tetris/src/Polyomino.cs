class Polyomino
{
    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; private set; }
    public int[,] Shape { get; private set; }
    public int SpawnXOffset { get; set; } = 0;
    public int SpawnYOffset { get; set; } = 0;

    public WallKickTable WallKickOffsets = WallKickTable.Make_JLSTZ_Table();

    private Orientation orientation = Orientation.Zero;

    public Polyomino(int[,] shape)
    {
        Shape = shape;
        Color = AnsiColor.GetNextColor();
    }
    public Polyomino(int[,] shape, string color)
    {
        Shape = shape;
        Color = color;
    }

    public virtual void Rotate(Board board)
    {
        // Inspired by https://tetris.wiki/Super_Rotation_System

        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        int[,] rotated = new int[cols, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                rotated[x, rows - 1 - y] = Shape[y, x];
            }
        }

        // Try wall kicks if rotated shape collides
        for (int testIndex = 0; testIndex < WallKickOffsets.row0R.Count; testIndex++)
        {
            // Define test position with wall kick offset and polyomino position
            Orientation toOrientation = (Orientation)(((int)orientation + 1) % 4);
            (int offsetX, int offsetY) = WallKickOffsets.GetOffset(orientation, toOrientation, testIndex);
            int testX = X + offsetX;
            int testY = Y + offsetY;

            // Check collision
            bool collision = false;
            int rotatedRows = rotated.GetLength(0);
            int rotatedCols = rotated.GetLength(1);
            for (int y = 0; y < rotatedRows; y++)
            {
                for (int x = 0; x < rotatedCols; x++)
                {
                    if (rotated[y, x] != 0)
                    {
                        int boardX = testX + x;
                        int boardY = testY + y;

                        // Check boundaries
                        if (boardX < 0 || boardX >= board.Width || boardY < 0 || boardY >= board.Height) // Top bound may be unnecessary since it's way out of the visible height
                        {
                            collision = true;
                            break;
                        }

                        // Check collision grid
                        if (board.CollisionGrid[boardY][boardX])
                        {
                            collision = true;
                            break;
                        }
                    }
                }
                if (collision) break;
            }
            if (!collision)
            {
                // Successful rotation with wall kick
                Shape = rotated;
                orientation = toOrientation;
                X = testX;
                Y = testY;
                return;
            }
        }
    }

    /// <summary>
    /// Returns list of block positions relative to polyomino position
    /// </summary>
    public List<(int, int)> GetTileCoords()
    {
        List<(int, int)> blocks = [];
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (Shape[y, x] != 0)
                {
                    blocks.Add((X + x, Y + y));
                }
            }
        }

        return blocks;
    }

    /// <summary>
    /// Gets the list of tiles that make up this polyomino
    /// </summary>
    public List<Tile> GetTiles()
    {
        List<Tile> tiles = [];
        foreach ((int x, int y) in GetTileCoords())
        {
            tiles.Add(new Tile(x, y, Color));
        }
        return tiles;
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool CanMove(int deltaX, int deltaY, Board board)
    {
        int boardWidth = board.Width;
        int boardHeight = board.Height;
        CollisionGrid collisionGrid = board.CollisionGrid;

        foreach ((int x, int y) in GetTileCoords())
        {
            int newX = x + deltaX;
            int newY = y + deltaY;

            // Check boundaries
            if (newX < 0 || newX >= boardWidth || newY < 0 || newY >= boardHeight)
                return false;

            // Check collision grid
            if (collisionGrid[newY][newX])
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the height of the polyomino shape not counting empty rows in the definition matrix
    /// </summary>
    /// <returns></returns>
    public int GetHeight()
    {
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        int minY = rows;
        int maxY = -1;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (Shape[y, x] != 0)
                {
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (maxY == -1) return 0; // No blocks found

        return maxY - minY + 1;
    }

    /// <summary>
    /// Gets the width of the polyomino shape not counting empty columns in the definition matrix
    /// </summary>
    /// <returns></returns>
    public int GetWidth()
    {
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        int minX = cols;
        int maxX = -1;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (Shape[y, x] != 0)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }
            }
        }

        if (maxX == -1) return 0; // No blocks found

        return maxX - minX + 1;
    }
}

class TetrominoI : Polyomino
{
    public TetrominoI() : base(
        new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
        },
        AnsiColor.CyanCode
    )
    {
        WallKickOffsets = WallKickTable.Make_I_Table();
    }
}

class TetrominoJ : Polyomino
{
    public TetrominoJ() : base(
        new int[,] {
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        AnsiColor.BlueCode
    )
    { }
}

class TetrominoL : Polyomino
{
    public TetrominoL() : base(
        new int[,] {
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        AnsiColor.OrangeCode)
    { }
}

class TetrominoO : Polyomino
{
    public TetrominoO() : base(
        new int[,] {
            { 0, 1, 1, 0 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 },
        },
        AnsiColor.YellowCode
    )
    { }

    public override void Rotate(Board board)
    {
        // O Tetromino does not rotate
    }
}

class TetrominoS : Polyomino
{
    public TetrominoS() : base(
        new int[,] {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 0, 0 },
        },
        AnsiColor.GreenCode
    )
    { }
}

class TetrominoT : Polyomino
{
    public TetrominoT() : base(
        new int[,] {
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        },
        AnsiColor.MagentaCode
    )
    { }
}

class TetrominoZ : Polyomino
{
    public TetrominoZ() : base(
        new int[,] {
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },
        },
        AnsiColor.RedCode
    )
    { }
}


/** 
 * Custom polyominoes goes hard 
 */
class OctominoThiccI : Polyomino
{
    public OctominoThiccI() : base(
        new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
        }
    )
    {
        WallKickOffsets = WallKickTable.Make_ThiccI_Table();
    }
}

class DominoSmallI : Polyomino
{
    public DominoSmallI() : base(
        new int[,] {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },
        }
    )
    { }
}

class TrominoLowerI : Polyomino
{
    public TrominoLowerI() : base(
        new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 0, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
        }
    )
    {
        WallKickOffsets = WallKickTable.Make_I_Table();
    }
}

class OctominoIII : Polyomino
{
    public OctominoIII() : base(
        new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
        }, 
        AnsiColor.WhiteCode
    )
    {
        SpawnXOffset = -2;
        SpawnYOffset = -2;
        WallKickOffsets = WallKickTable.Make_III_Table();
    }
}

class NonominoBlocc : Polyomino
{
    public NonominoBlocc() : base(
        new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        }
    )
    { }

    public override void Rotate(Board board)
    {
        // Blocc does not rotate
    }
}

class OctominoDonut : Polyomino
{
    public OctominoDonut() : base(
        new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        },
        AnsiColor.PinkCode
    )
    { }

    public override void Rotate(Board board)
    {
        // Donut does not rotate
    }
}

class MonominoDot : Polyomino
{
    public MonominoDot() : base(
        new int[,] {
            { 0, 1, 0 },
            { 0, 0, 0 },
        }
    )
    { }

    public override void Rotate(Board board)
    {
        // Dot does not rotate
    }
}

class PentominoArchBTW : Polyomino
{
    public PentominoArchBTW() : base(
        new int[,] {
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        }
    )
    { }
}

class PentominoX : Polyomino
{
    public PentominoX() : base(
        new int[,] {
            { 1, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 1 },
        }, AnsiColor.XRedCode
    )
    { }
}