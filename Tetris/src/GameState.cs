
class GameState
{
    public List<Player> Players { get; } = [
        new ("Lukas", new () {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Rotate },
            { "S", Input.SoftDrop }
        }),
        new ("Vena", new () {
            { "LeftArrow",  Input.Left },
            { "RightArrow", Input.Right },
            { "UpArrow",    Input.Rotate },
            { "DownArrow",  Input.SoftDrop }
        }),
        // new (true), // AI Player
    ];

    private readonly GameRenderer gameRenderer;
    private readonly ShopRenderer shopRenderer;

    public GameState()
    {
        // Instantiate shops
        Players.ForEach(player => player.Shop = new(player, [.. Players.Where(p => p != player)]));

        // Instantiate renderers
        gameRenderer = new(this);
        shopRenderer = new(this);

        // State management variables
        bool shopping = false;
        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int secondsBetweenShopping = 0; // TODO - Test 20 seconds

        while (true)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - lastTick >= secondsBetweenShopping * 1_000)
            {
                shopping = true;
            }

            if (shopping)
            {
                ShoppingMode(); // Holds until user exits shop
                shopping = false;
                lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
            else GamingFrame();
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

            string key = KeyInput.Read() ?? "";

            // Stop shopping
            if (key == "Enter") break;

            // Logic
            Players.ForEach(player =>
            {
                if (player.Shop is null) throw new Exception("Player has no shop!");
                Shop shop = player.Shop;

                if (!player.ValidKeys.Contains(key)) return;
                ControlScheme controls = player.IsAI ? [] : player.ControlScheme;

                switch (controls[key])
                {
                    case Input.Rotate:
                        shop.ShelfIndex = (shop.ShelfIndex - 1 + shop.Products.Count) % shop.Products.Count;
                        break;

                    case Input.SoftDrop:
                        shop.ShelfIndex = (shop.ShelfIndex + 1) % shop.Products.Count;
                        break;

                    case Input.Right:
                        // Put in cart
                        break;

                    case Input.Left:
                        // Put back to stand
                        break;

                    default:
                        break;
                }
            });

            shopRenderer.Render();

            // Throttle
            int remaimingWait = frameTarget - (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameStart);
            if (remaimingWait > 0) Thread.Sleep(remaimingWait);
        }

        // Countdown before resuming game
        for (int i = 0; i < 4; i++)
        {
            gameRenderer.Render();
            RenderUtils.WriteLargeNumberInPlace(3 - i);
            Thread.Sleep(1000);
        }
    }
}