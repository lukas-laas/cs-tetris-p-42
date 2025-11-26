
class GameState
{
    public List<Board> Games { get; } = [
        new(new() {
            { "a", Input.Left },
            { "d", Input.Right },
            { "w", Input.Rotate },
            { "s", Input.SoftDrop }
        }),
        new(new() {
            { "ArrowLeft",  Input.Left },
            { "ArrowRight", Input.Right },
            { "ArrowUp",    Input.Rotate },
            { "ArrowDown",  Input.SoftDrop }
        }),
        // new(new() {
        //     { "A", Input.Left },
        //     { "D", Input.Right },
        //     { "W", Input.Rotate },
        //     { "S", Input.SoftDrop }
        // }),
        // new(new() {
        //     { "LeftArrow",  Input.Left },
        //     { "RightArrow", Input.Right },
        //     { "UpArrow",    Input.Rotate },
        //     { "DownArrow",  Input.SoftDrop }
        // }),
    ];
    private readonly KeyInput inputHandler = new();

    public GameState()
    {
        Renderer renderer = new(this);
        bool gaming = true;

        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // TODO: Switch case to switch between menu, game and shop, might want to live in Program, not sure
        while (gaming)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string[] pressedKeys = inputHandler.ReadAll();

            Games.ForEach(board => board.Tick(pressedKeys));

            renderer.Render();
            Thread.Sleep(20);
        }
    }
}