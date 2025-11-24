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
    Zero = 0,
    Right = 1,
    Two = 2,
    Left = 3,
}