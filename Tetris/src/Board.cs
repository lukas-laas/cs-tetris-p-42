
class Board
{
    // Handles "physics" of pieces on the board

    // Color info (affected by buffs/debuffs)

    private int width = 10;
    private int height = 40;
    private int visibleHeight = 20;
    private List<List<bool>> collisionGrid;
    private List<Block> blocks; // Settled blocks on the board only to render colors
    private List<Block> fallingBlocks; // usually just one but debuffs might change that

    public Board()
    {
        collisionGrid = [.. new List<List<bool>>(height).Select(_ => new List<bool>(width).Select(__ => false).ToList())];
    }

}