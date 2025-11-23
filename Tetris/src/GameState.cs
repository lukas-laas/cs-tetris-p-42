
class GameState
{
    public List<Board> Games { get; } = [
        new(new() {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Rotate },
            { "S", Input.SoftDrop }
        }),
        new(new() {
            { "LeftArrow", Input.Left },
            { "RightArrow", Input.Right },
            { "UpArrow", Input.Rotate },
            { "DownArrow", Input.SoftDrop }
        }),
    ];

    public GameState()
    {
        Renderer renderer = new(this);
        bool gaming = true;

        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // TODO: Switch case to switch between menu, game and shop, might want to live in Program, not sure
        while (gaming)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string key = KeyInput.Read() ?? "";

            Games.ForEach(board => board.Tick(key));

            renderer.Render();
            Thread.Sleep(20);
        }
    }
}