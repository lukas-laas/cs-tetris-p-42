
class GameState
{
    public List<Player> Players { get; } = [
        new ("Lukas", new (new () {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Rotate },
            { "S", Input.SoftDrop }
        })),
        new ("Vena", new (new () {
            { "LeftArrow",  Input.Left },
            { "RightArrow", Input.Right },
            { "UpArrow",    Input.Rotate },
            { "DownArrow",  Input.SoftDrop }
        })),
    ];

    public GameState()
    {
        Renderer renderer = new(this);
        bool gaming = true;

        Players[0].Shop = new(Players[0], [Players[1]]);
        Players[1].Shop = new(Players[1], [Players[0]]);

        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // TODO: Switch case to switch between menu, game and shop, might want to live in Program, not sure
        while (gaming)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string key = KeyInput.Read() ?? "";

            Players.ForEach(player => player.Tick(key));

            renderer.Render();
            Thread.Sleep(20);
        }
    }
}