
class GameState
{
    public List<Player> Games { get; } = [
        new ("p1", new (new () {
                { "A", Input.Left },
                { "D", Input.Right },
                { "W", Input.Rotate },
                { "S", Input.SoftDrop }
            })),
        new ("p2", new (new () {
                { "LeftArrow", Input.Left },
                { "RightArrow", Input.Right },
                { "UpArrow", Input.Rotate },
                { "DownArrow", Input.SoftDrop }
            }))
    ];

    public GameState()
    {
        Renderer renderer = new(this);
        bool gaming = true;

        Games[0].Shop = new(Games[0], [Games[1]]);
        Games[1].Shop = new(Games[1], [Games[0]]);

        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // TODO: Switch case to switch between menu, game and shop, might want to live in Program, not sure
        while (gaming)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string key = KeyInput.Read() ?? "";

            if (key == "T")
            {
                Games[0].Shop!.Products[0].Purchase(Games[0]);
                Log.Add($"SPEEDING UP {Games[1].Board.DT}");
            }

            Games.ForEach(player => player.Board.Tick(key));

            renderer.Render();
            Thread.Sleep(20);
        }
    }
}