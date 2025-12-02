
class GameState
{
    public List<Player> Players { get; private set; } = [];

    // KRAV 1:
    // 1: Inkapsling / Informationsgömning
    // 2: Privata fältet används för för att gömma renderarna från omvärlden.
    // 3: Renderarna är enbart side-effekter för GameState och bör inte röras 
    //     någon annanstans än här. I en perfekt värld hade de inte ens 
    //     definierats som fält, snarare bara skapas lokalt vid varje rendering 
    //     men då görs en mass onödig instansiering som kan bekosta prestanda.
    private readonly GameRenderer gameRenderer;
    private readonly ShopRenderer shopRenderer;

    public readonly HashSet<Player> ReadyPlayers = [];
    public static readonly int GameplayDuration = 30;
    public int ElapsedMs { get; private set; } = 0; // Elapsed milliseconds since the current gameplay phase started.

    public GameState()
    {
        // Housekeeping
        if (Console.WindowWidth == 0) throw new Exception("Console window width is 0. Cannot render.");
        if (Console.WindowHeight == 0) throw new Exception("Console window height is 0. Cannot render.");
        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.CursorVisible = false; // Will not restore on exit, but oh well

        // Instantiate players via welcome page
        GameModeSelect gameModeSelected = GameModeSelect.Singleplayer;
        while (true)
        {
            int frameTarget = 20; // milliseconds per frame
            long frameStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            string key = KeyInput.Read() ?? "";
            if (key == "UpArrow" || key == "DownArrow" || key == "W" || key == "S")
            {
                gameModeSelected = gameModeSelected == GameModeSelect.Singleplayer
                    ? GameModeSelect.Multiplayer
                    : GameModeSelect.Singleplayer;
            }
            else if (key == "Enter")
            {
                break;
            }

            WelcomeRenderer welcomeRenderer = new(this);
            welcomeRenderer.Render(gameModeSelected);

            int remaimingWait = frameTarget - (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameStart);
            if (remaimingWait > 0) Thread.Sleep(remaimingWait);
        }

        // Create players based on selection
        switch (gameModeSelected)
        {
            case GameModeSelect.Singleplayer:
                Players.Add(new AIPlayer());
                Players.Add(new HumanPlayer("Player 1", new() {
                    { "A", Input.Left },
                    { "D", Input.Right },
                    { "W", Input.Up },
                    { "S", Input.Down },
                    { "Q", Input.Ability },
                    { "LeftArrow",  Input.Left },
                    { "RightArrow", Input.Right },
                    { "UpArrow",    Input.Up },
                    { "DownArrow",  Input.Down },
                    { "Subtract",   Input.Ability }
                }));
                break;

            case GameModeSelect.Multiplayer:
                // KRAV 5:
                // 1: Beroendeinjektion
                // 2: Player instansieras med med olika kontrollscheman för att stödja
                //     flera spelare på samma maskin. Kontrollschemat injiceras in i
                //     Player-objektet vid konstruktion.
                // 3: Det är en ren och tydlig implementation som gör det enkelt att
                //     skapa nya spelare med olika kontroller utan att behöva hårdkoda
                //     eller ändra i Player-klassen. Det är antingen den här lösningen eller
                //     att direkt sätta ett fält efter konstruktion vilket är mindre önskvärt.
                Players.Add(new HumanPlayer("Player 1", new() {
                    { "A", Input.Left },
                    { "D", Input.Right },
                    { "W", Input.Up },
                    { "S", Input.Down },
                    { "Q", Input.Ability }
                }));
                Players.Add(new HumanPlayer("Player 2", new() {
                    { "LeftArrow",  Input.Left },
                    { "RightArrow", Input.Right },
                    { "UpArrow",    Input.Up },
                    { "DownArrow",  Input.Down },
                    { "Subtract",   Input.Ability }
                }));
                break;
        }

        // Instantiate shops
        Players.ForEach(player => player.Shop = new(player, [.. Players.Where(p => p != player)]));

        // Instantiate renderers
        gameRenderer = new(this);
        shopRenderer = new(this);

        // State management variables
        bool shopping = false;
        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        while (true)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ElapsedMs = (int)(currentTime - lastTick);
            if (ElapsedMs >= GameplayDuration * 1_000) shopping = true;

            if (shopping)
            {
                Players.ForEach(player => player.Shop!.ReStock());
                ShoppingMode(); // Holds until user exits shop
                shopping = false;
                lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                ElapsedMs = 0;
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
        Shelves shelves = shopStates.First(s => s.Shop == shop);

        if (!player.ValidKeys.Contains(key)) return;
        ControlScheme controls = player.IsAI ? [] : player.ControlScheme;

        switch (controls[key])
        {
            case Input.Up:
                // Include extra "Ready" row below the last product
                int totalOptionsUp = shop.Products.Count + 1; // products + ready row
                shelves.ShelfIndex = (shelves.ShelfIndex - 1 + totalOptionsUp) % totalOptionsUp;
                break;

            case Input.Down:
                // Include extra "Ready" row below the last product
                int totalOptionsDown = shop.Products.Count + 1; // products + ready row
                shelves.ShelfIndex = (shelves.ShelfIndex + 1) % totalOptionsDown;
                break;

            case Input.Right:
                // Find selected shelf (only for real product indices)
                if (shelves.ShelfIndex >= shop.Products.Count) break; // ignore READY row for Right

                Shelf selectedShelf = shelves.ShelvesList[shelves.ShelfIndex];
                if (selectedShelf.Side == Side.Basket) break; // Already in basket

                // Money check - include current cart total so you can't exceed balance
                int currentCartTotal = shelves.ShelvesList
                    .Where(shelf => shelf.Side == Side.Basket)
                    .Sum(shelf => shelf.Product.price);
                int prospectiveTotal = currentCartTotal + selectedShelf.Product.price;

                if (prospectiveTotal > player.Money) break; // Not enough money including cart

                // Move to basket
                selectedShelf.Side = Side.Basket;
                break;

            case Input.Left:
                // Find selected shelf (only for real product indices)
                if (shelves.ShelfIndex >= shop.Products.Count) break; // ignore READY row for Left

                Shelf selectedShelf2 = shelves.ShelvesList[shelves.ShelfIndex];
                if (selectedShelf2.Side == Side.Stand) break; // Already in stand

                // Move to stand
                selectedShelf2.Side = Side.Stand;
                break;

            default:
                break;
        }

        // Ready when merely selected
        if (shelves.ShelfIndex == shop.Products.Count)
        {
            ReadyPlayers.Add(player);
        }
        else
        {
            ReadyPlayers.Remove(player);
        }
    }
}