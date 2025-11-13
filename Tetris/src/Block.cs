
interface ITetromino
{
    int X { get; }
    int Y { get; }
    int[,] Shape { get; }
    void Rotate();
}

class Tetromino : ITetromino
{
    public int X { get; private set; }
    public int Y { get; private set; }
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
        // O Tetromino does not rotate
    }
}
