
class Tetromino
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[,] Shape { get; private set; }

    public int SpawnXOffset { get; set; } = 0;
    public int SpawnYOffset { get; set; } = 0;

    public string Color { get; private set; }

    public Tetromino(int[,] shape)
    {
        Shape = shape;
        Color = AnsiColor.GetNextColor();
    }
    public Tetromino(int[,] shape, string color)
    {
        Shape = shape;
        Color = color;
    }

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

    /// <summary>
    /// Gets the height of the tetromino shape not counting empty rows in the definition matrix
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
    /// Gets the width of the tetromino shape not counting empty columns in the definition matrix
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

class TetrominoI : Tetromino
{
    public TetrominoI() : base(new int[,] {
            { 0, 0, 0, 0},
            { 1, 1, 1, 1},
            { 0, 0, 0, 0},
            { 0, 0, 0, 0},
        }, AnsiColor.CyanCode)
    { }
}

class TetrominoJ : Tetromino
{
    public TetrominoJ() : base(new int[,] {
            { 1, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.BlueCode)
    { }
}

class TetrominoL : Tetromino
{
    public TetrominoL() : base(new int[,] {
            { 0, 0, 1},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.OrangeCode)
    { }
}

class TetrominoO : Tetromino
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

class TetrominoS : Tetromino
{
    public TetrominoS() : base(new int[,] {
            { 0, 1, 1},
            { 1, 1, 0},
            { 0, 0, 0},
        }, AnsiColor.GreenCode)
    { }
}

class TetrominoT : Tetromino
{
    public TetrominoT() : base(new int[,] {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.MagentaCode)
    { }
}

class TetrominoZ : Tetromino
{
    public TetrominoZ() : base(new int[,] {
            { 1, 1, 0},
            { 0, 1, 1},
            { 0, 0, 0},
        }, AnsiColor.RedCode)
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
        }, AnsiColor.WhiteCode)
    {
        SpawnXOffset = -2;
        SpawnYOffset = -2;
    }
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

class TetrominoX : Tetromino
{
    public TetrominoX() : base(new int[,] {
        { 1, 0, 1 },
        { 0, 1, 0 },
        { 1, 0, 1 },
    })
    { }
}