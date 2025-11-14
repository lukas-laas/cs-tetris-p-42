
interface ITetromino
{
    int X { get; set; }
    int Y { get; set; }
    int[,] Shape { get; }
    string Color { get; init; }
    void Rotate();
    List<(int, int)> GetTileCoords();
    void SetPosition(int x, int y);

    bool CanMove(int deltaX, int deltaY, CollisionGrid collisionGrid, int boardWidth, int boardHeight);
}

class Tetromino(int[,] shape) : ITetromino
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[,] Shape { get; private set; } = shape;

    public string Color { get; init; } = AnsiColor.GetNextColor();

    public virtual void Rotate()
    {
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
    /// Returns list of block positions relative to tetromino position
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

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool CanMove(int deltaX, int deltaY, Board board)
    {
        return CanMove(deltaX, deltaY, board.CollisionGrid, board.Width, board.Height);
    }
    public bool CanMove(int deltaX, int deltaY, CollisionGrid collisionGrid, int boardWidth, int boardHeight)
    {
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
}

class TetrominoI : Tetromino
{
    public TetrominoI() : base(new int[,] {
            { 0, 0, 0, 0},
            { 1, 1, 1, 1},
            { 0, 0, 0, 0},
            { 0, 0, 0, 0},
        })
    {
        Color = AnsiColor.CyanCode;
    }
}

class TetrominoJ : Tetromino
{
    public TetrominoJ() : base(new int[,] {
            { 1, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    {
        Color = AnsiColor.BlueCode;
    }
}

class TetrominoL : Tetromino
{
    public TetrominoL() : base(new int[,] {
            { 0, 0, 1},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    {
        Color = AnsiColor.OrangeCode;
    }
}

class TetrominoO : Tetromino
{
    public TetrominoO() : base(new int[,] {
            { 0, 1, 1, 0},
            { 0, 1, 1, 0},
            { 0, 0, 0, 0},
        })
    {
        Color = AnsiColor.YellowCode;
    }

    public override void Rotate()
    {
        // O Tetromino does not rotate}
    }
}

class TetrominoS : Tetromino
{
    public TetrominoS() : base(new int[,] {
            { 0, 1, 1},
            { 1, 1, 0},
            { 0, 0, 0},
        })
    {
        Color = AnsiColor.GreenCode;
    }
}

class TetrominoT : Tetromino
{
    public TetrominoT() : base(new int[,] {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    {
        Color = AnsiColor.MagentaCode;
    }
}

class TetrominoZ : Tetromino
{
    public TetrominoZ() : base(new int[,] {
            { 1, 1, 0},
            { 0, 1, 1},
            { 0, 0, 0},
        })
    { }
}

// Custom tetrominoes goes hard 
// TODO: Make sure rotation works correctly
class TetrominoThiccI : Tetromino
{
    public TetrominoThiccI() : base(new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
        })
    { }
}

class TetrominoSmallI : Tetromino
{
    public TetrominoSmallI() : base(new int[,] {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 },

        })
    { }
}

class TetrominoLowerI : Tetromino
{
    public TetrominoLowerI() : base(new int[,] {
            { 0, 0, 0, 0 },
            { 1, 1, 0, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
        })
    { }
}

class TetrominoIII : Tetromino
{
    public TetrominoIII() : base(new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
        })
    { }
}

class TetrominoBlocc : Tetromino
{
    public TetrominoBlocc() : base(new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }

    public override void Rotate()
    {
        // Blocc Tetromino does not rotate}
    }
}

class TetrominoDonut : Tetromino
{
    public TetrominoDonut() : base(new int[,] {
            { 0, 1, 1, 1, 0 },
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }

    public override void Rotate()
    {
        // Donut Tetromino does not rotate}
    }
}

class TetrominoDot : Tetromino
{
    public TetrominoDot() : base(new int[,] {
            { 0, 1, 0},
            { 0, 0, 0},
        })
    { }

    public override void Rotate()
    {
        // Dot Tetromino does not rotate}
    }
}

class TetrominoArch : Tetromino
{
    public TetrominoArch() : base(new int[,] {
            { 0, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 0 },
        })
    { }
}