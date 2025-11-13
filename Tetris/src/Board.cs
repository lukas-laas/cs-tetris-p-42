
class Board
{
    // Handles "physics" of pieces on the board

    // Color info (affected by buffs/debuffs)

    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 40;
    public int VisibleHeight { get; private set; } = 20;

    private CollisionGrid collisionGrid = [];
    private List<Tetromino> tetrominoes = []; // Settled blocks on the board only to render colors
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

}