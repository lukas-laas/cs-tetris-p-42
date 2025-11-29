global using CollisionGrid = System.Collections.Generic.List<System.Collections.Generic.List<bool>>;
global using KeyCode = SDL2.SDL.SDL_Keycode;
global using ControlScheme = System.Collections.Generic.Dictionary<SDL2.SDL.SDL_Keycode, Input>;
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