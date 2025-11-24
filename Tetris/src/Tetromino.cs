
class WallKickTable
{
    public List<(int, int)> row0R = [];
    public List<(int, int)> rowR0 = [];
    public List<(int, int)> rowR2 = [];
    public List<(int, int)> row2R = [];
    public List<(int, int)> row2L = [];
    public List<(int, int)> rowL2 = [];
    public List<(int, int)> rowL0 = [];
    public List<(int, int)> row0L = [];

    public (int, int) GetOffset(int rotationState, int testIndex)
    {
        int fromState = rotationState;
        int toState = (rotationState + 1) % 4; // TODO handle if multi directional rotation is implemented
        return GetOffset(fromState, toState, testIndex);
    }
    public (int, int) GetOffset(int fromState, int toState, int testIndex)
    {
        if (fromState == 0 && toState == 1)
            return row0R[testIndex];
        else if (fromState == 1 && toState == 0)
            return rowR0[testIndex];
        else if (fromState == 1 && toState == 2)
            return rowR2[testIndex];
        else if (fromState == 2 && toState == 1)
            return row2R[testIndex];
        else if (fromState == 2 && toState == 3)
            return row2L[testIndex];
        else if (fromState == 3 && toState == 2)
            return rowL2[testIndex];
        else if (fromState == 3 && toState == 0)
            return rowL0[testIndex];
        else if (fromState == 0 && toState == 3)
            return row0L[testIndex];
        else
            throw new ArgumentException("Invalid rotation states");
    }

    public static WallKickTable MakeJLSTZTable()
    {
        // https://tetris.wiki/Super_Rotation_System#Wall_Kicks
        return new WallKickTable()
        {
            row0R = [(0, 0), (-1, 0), (-1, +1), (0, -2), (-1, -2)],
            rowR0 = [(0, 0), (+1, 0), (+1, -1), (0, +2), (+1, +2)],
            rowR2 = [(0, 0), (+1, 0), (+1, -1), (0, +2), (+1, +2)],
            row2R = [(0, 0), (-1, 0), (-1, +1), (0, -2), (-1, -2)],
            row2L = [(0, 0), (+1, 0), (+1, +1), (0, -2), (+1, -2)],
            rowL2 = [(0, 0), (-1, 0), (-1, -1), (0, +2), (-1, +2)],
            rowL0 = [(0, 0), (-1, 0), (-1, -1), (0, +2), (-1, +2)],
            row0L = [(0, 0), (+1, 0), (+1, +1), (0, -2), (+1, -2)],
        };
    }

    public static WallKickTable MakeITable()
    {
        // https://tetris.wiki/Super_Rotation_System#Wall_Kicks
        return new WallKickTable()
        {
            row0R = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowR0 = [(0, 0), (+2, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowR2 = [(0, 0), (-1, 0), (+2, 0), (-1, +2), (+2, -1)],
            row2R = [(0, 0), (+1, 0), (-2, 0), (+1, -2), (-2, +1)],
            row2L = [(0, 0), (+2, 0), (-1, 0), (+2, +1), (-1, -2)],
            rowL2 = [(0, 0), (-2, 0), (+1, 0), (-2, -1), (+1, +2)],
            rowL0 = [(0, 0), (+1, 0), (-2, 0), (+1, -2), (-2, +1)],
            row0L = [(0, 0), (-1, 0), (+2, 0), (-1, +2), (+2, -1)],
        };
    }
}

class Polyomino
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[,] Shape { get; private set; }

    public int SpawnXOffset { get; set; } = 0;
    public int SpawnYOffset { get; set; } = 0;

    public string Color { get; private set; }

    public WallKickTable WallKickOffsets = WallKickTable.MakeJLSTZTable();
    private int rotationState = 0;

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

    public virtual void Rotate()
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

        Shape = rotated;
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
    public TetrominoI() : base(new int[,] {
            { 0, 0, 0, 0},
            { 1, 1, 1, 1},
            { 0, 0, 0, 0},
            { 0, 0, 0, 0},
        }, AnsiColor.CyanCode)
    {
        WallKickOffsets = WallKickTable.MakeITable();
    }
}

class TetrominoJ : Polyomino
{
    public TetrominoJ() : base(new int[,] {
            { 1, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.BlueCode)
    { }
}

class TetrominoL : Polyomino
{
    public TetrominoL() : base(new int[,] {
            { 0, 0, 1},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.OrangeCode)
    { }
}

class TetrominoO : Polyomino
{
    public TetrominoO() : base(new int[,] {
            { 0, 1, 1, 0},
            { 0, 1, 1, 0},
            { 0, 0, 0, 0},
        }, AnsiColor.YellowCode)
    { }

    public override void Rotate()
    {
        // O Tetromino does not rotate}
    }
}

class TetrominoS : Polyomino
{
    public TetrominoS() : base(new int[,] {
            { 0, 1, 1},
            { 1, 1, 0},
            { 0, 0, 0},
        }, AnsiColor.GreenCode)
    { }
}

class TetrominoT : Polyomino
{
    public TetrominoT() : base(new int[,] {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.MagentaCode)
    { }
}

class TetrominoZ : Polyomino
{
    public TetrominoZ() : base(new int[,] {
            { 1, 1, 0},
            { 0, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.RedCode)
    { }
}


/** 
 * Custom polyominoes goes hard 
 * TODO: Make sure rotation works correctly
 */
class OctominoThiccI : Polyomino
{
    public OctominoThiccI() : base(new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
        })
    { }
}

class DominoSmallI : Polyomino
{
    public DominoSmallI() : base(new int[,] {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },

        })
    { }
}

class TrominoLowerI : Polyomino
{
    public TrominoLowerI() : base(new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 0, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
        })
    { }
}

class OctominoIII : Polyomino
{
    public OctominoIII() : base(new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
        }, AnsiColor.WhiteCode)
    {
        SpawnXOffset = -2;
        SpawnYOffset = -2;
    }
}

class NonominoBlocc : Polyomino
{
    public NonominoBlocc() : base(new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }

    public override void Rotate()
    {
        // Blocc does not rotate}
    }
}

class OctominoDonut : Polyomino
{
    public OctominoDonut() : base(new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }

    public override void Rotate()
    {
        // Donut does not rotate}
    }
}

class MonominoDot : Polyomino
{
    public MonominoDot() : base(new int[,] {
            { 0, 1, 0},
            { 0, 0, 0},
        })
    { }

    public override void Rotate()
    {
        // Dot does not rotate}
    }
}

class PentominoArch : Polyomino
{
    public PentominoArch() : base(new int[,] {
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }
}

class PentominoX : Polyomino
{
    public PentominoX() : base(new int[,] {
        { 1, 0, 1 },
        { 0, 1, 0 },
        { 1, 0, 1 },
    })
    { }
}