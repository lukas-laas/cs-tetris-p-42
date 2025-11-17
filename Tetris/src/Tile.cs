class Tile(int x, int y, string color)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public required string Color { get; init; } = color;
}