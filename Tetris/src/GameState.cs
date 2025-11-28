
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

    private readonly GameRenderer gameRenderer;
    private readonly ShopRenderer shopRenderer;

    public GameState()
    {
        // Instantiate renderers
        gameRenderer = new(this);
        shopRenderer = new(this);

        // Instantiate shops
        Players.ForEach(player => player.Shop = new(player, [.. Players.Where(p => p != player)]));

        // State management variables
        bool shopping = false;
        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        while (true)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - lastTick >= 10 * 1_000)
            {
                shopping = !shopping;
                lastTick = currentTime; // TODO this should only update once shopping is over
            }


            if (shopping)
            {
                ShoppingMode();
            }
            else
            {
                GamingFrame();
            }
        }
    }

    private void GamingFrame()
    {
        int frameTarget = 20; // milliseconds per frame
        long frameStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // Logic
        string key = KeyInput.Read() ?? "";
        Players.ForEach(player => player.Tick(key));

        // Render
        gameRenderer.Render();

        // Throttle
        int remaimingWait = frameTarget - (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameStart);
        if (remaimingWait > 0) Thread.Sleep(remaimingWait);
    }

    private void ShoppingMode()
    {
        while (true)
        {
            int frameTarget = 20; // milliseconds per frame
            long frameStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            shopRenderer.Render();

            // Throttle
            int remaimingWait = frameTarget - (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameStart);
            if (remaimingWait > 0) Thread.Sleep(remaimingWait);
        }
    }
}