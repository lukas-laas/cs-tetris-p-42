
class GameState
{
    public List<Player> Players { get; } = [
        new HumanPlayer("Lukas", new () {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Up },
            { "S", Input.Down }
        }),
        new HumanPlayer("Vena", new () {
            { "LeftArrow",  Input.Left },
            { "RightArrow", Input.Right },
            { "UpArrow",    Input.Up },
            { "DownArrow",  Input.Down }
        }),
        // new AIPlayer(), // AI Player
    ];

    private readonly GameRenderer gameRenderer;
    private readonly ShopRenderer shopRenderer;

    public readonly HashSet<Player> ReadyPlayers = [];

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

        // Players[0].Shop.ProductPool[6]().Purchase();
        // Players[0].Shop.ProductPool[6]().Purchase();
        // Players[0].Shop.ProductPool[6]().Purchase();
        // Players[0].Shop.ProductPool[6]().Purchase();

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
        ReadyPlayers.Clear();
        List<Shelves> shopStates = [.. Players
            .Where(p => !p.IsAI)
            .Select(p => new Shelves(p.Shop!, [.. p.Shop!.Products.Select(prod => new Shelf(Side.Stand, prod))]))];

        while (true)
        {
            int frameTarget = 20; // milliseconds per frame
            long frameStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string key = KeyInput.Read() ?? "";

            // Logic (ignore AI players during shopping)
            Players.Where(p => !p.IsAI).ToList().ForEach(player => ShoppingActions(player, key, shopStates));

            // If all non-AI players are ready, stop shopping
            if (Players.Where(p => !p.IsAI).All(p => ReadyPlayers.Contains(p))) break;

            shopRenderer.Render(shopStates);

            // Throttle
            int remaimingWait = frameTarget - (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameStart);
            if (remaimingWait > 0) Thread.Sleep(remaimingWait);
        }

        // Finalize purchases
        foreach (Shelves shelves in shopStates)
        {
            Shop shop = shelves.Shop;
            Player player = shop.Owner;
            List<Shelf> cartedItems = [.. shelves.ShelvesList.Where(shelf => shelf.Side == Side.Basket)];
            int totalCost = cartedItems.Sum(shelf => shelf.Product.price);
            if (totalCost > player.Money) throw new Exception("Player cannot afford their cart items!");
            cartedItems.ForEach(shelf => shelf.Product.Purchase());
        }

        // Countdown before resuming game
        for (int i = 0; i < 4; i++)
        {
            gameRenderer.Render();
            RenderUtils.WriteLargeNumberInPlace(3 - i);
            Thread.Sleep(1000);
        }
    }

    private void ShoppingActions(Player player, string key, List<Shelves> shopStates)
    {
        if (player.Shop is null) throw new Exception("Player has no shop!");
        Shop shop = player.Shop;

        if (!player.ValidKeys.Contains(key)) return;
        ControlScheme controls = player.IsAI ? [] : player.ControlScheme;

        switch (controls[key])
        {
            case Input.Up:
                // Include extra "Ready" row below the last product
                int totalOptionsUp = shop.Products.Count + 1; // products + ready row
                shop.ShelfIndex = (shop.ShelfIndex - 1 + totalOptionsUp) % totalOptionsUp;
                break;

            case Input.Down:
                // Include extra "Ready" row below the last product
                int totalOptionsDown = shop.Products.Count + 1; // products + ready row
                shop.ShelfIndex = (shop.ShelfIndex + 1) % totalOptionsDown;
                break;

            case Input.Right:
                // Find selected shelf (only for real product indices)
                if (shop.ShelfIndex >= shop.Products.Count) break; // ignore READY row for Right

                Shelf selectedShelf = shopStates.First(s => s.Shop == shop).ShelvesList[shop.ShelfIndex];
                if (selectedShelf.Side == Side.Basket) break; // Already in basket

                // Money check - include current cart total so you can't exceed balance
                List<Shelf> playerShelves = shopStates.First(s => s.Shop == shop).ShelvesList;
                int currentCartTotal = playerShelves
                    .Where(shelf => shelf.Side == Side.Basket)
                    .Sum(shelf => shelf.Product.price);
                int prospectiveTotal = currentCartTotal + selectedShelf.Product.price;

                if (prospectiveTotal > player.Money) break; // Not enough money including cart

                // Move to basket
                selectedShelf.Side = Side.Basket;
                break;

            case Input.Left:
                // Find selected shelf (only for real product indices)
                if (shop.ShelfIndex >= shop.Products.Count) break; // ignore READY row for Left

                Shelf selectedShelf2 = shopStates.First(s => s.Shop == shop).ShelvesList[shop.ShelfIndex];
                if (selectedShelf2.Side == Side.Stand) break; // Already in stand

                // Move to stand
                selectedShelf2.Side = Side.Stand;
                break;

            default:
                break;
        }

        // Ready when merely selected
        if (shop.ShelfIndex == shop.Products.Count)
        {
            ReadyPlayers.Add(player);
        }
        else
        {
            ReadyPlayers.Remove(player);
        }
    }
}