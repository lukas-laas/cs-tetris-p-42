
interface ITetromino
{
    int X { get; }
    int Y { get; }
    int[,] Shape { get; }
    void Rotate();
}

class Tetromino : ITetromino
{
    public int X { get; set; }
    public int Y { get; set; }
    public int[,] Shape { get; private set; }

    public Tetromino(int[,] shape)
    {
        Shape = shape;
    }

    public void Rotate()
    {
        int[,] rotationMatrix = new int[,] { { 0, -1 }, { 1, 0 } };
        // Do rotatey stuff
    }

    /// <summary>
    /// Returns list of block positions relative to tetromino position
    /// </summary>
    public List<(int, int)> GetBlocks()
    {
        List<(int X, int Y)> blocks = [];
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (Shape[y, x] != 0)
                {
                    blocks.Add((x, y));
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
}

class TetrominoI : Tetromino
{
    public TetrominoI() : base(new int[,] {
            { 0, 0, 0, 0},
            { 1, 1, 1, 1},
            { 0, 0, 0, 0},
            { 0, 0, 0, 0},
        })
    { }
}

class TetrominoJ : Tetromino
{
    public TetrominoJ() : base(new int[,] {
            { 1, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    { }
}

class TetrominoL : Tetromino
{
    public TetrominoL() : base(new int[,] {
            { 0, 0, 1},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    { }
}

class TetrominoO : Tetromino
{
    public TetrominoO() : base(new int[,] {
            { 0, 1, 1, 0},
            { 0, 1, 1, 0},
            { 0, 0, 0, 0},
        })
    { }

    new public static void Rotate()
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
    { }
}

class TetrominoT : Tetromino
{
    public TetrominoT() : base(new int[,] {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 0, 0},
        })
    { }
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