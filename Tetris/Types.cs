global using CollisionGrid = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;
global using ControlScheme = System.Collections.Generic.Dictionary<string, Input>;
enum Input
{
    Left,
    Right,
    Rotate,
    SoftDrop, // Hard drop not implemented, maybe l8r B)
};
enum Orientation
{
    Zero,
    Right,
    Two,
    Left,
}
enum Side
{
    Stand, // Left
    Basket, // Right
};
class Shelf(Side side, IProduct product)
{
    public Side Side { get; set; } = side;
    public IProduct Product { get; set; } = product;
}
class Shelves(Shop shop, List<Shelf> shelves)
{
    public Shop Shop { get; set; } = shop;
    public List<Shelf> ShelvesList { get; set; } = shelves;
}