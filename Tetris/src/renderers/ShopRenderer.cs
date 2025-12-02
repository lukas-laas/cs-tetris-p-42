

class ShopRenderer(GameState gameState)
{
    private readonly GameState gameState = gameState;
    private readonly List<Player> players = [.. gameState.Players.Where(p => !p.IsAI)];
    private static readonly int itemWidth = 30;
    private static readonly int shelfWidth = itemWidth * 2 + 3 + 4; // 3 and 4 for borders and padding

    public void Render(List<Shelves> shopStates)
    {
        string buffer = "";

        buffer += "\n";
        buffer += MakeTitle();
        buffer += "\n\n";

        buffer += RenderUtils.Center2DString(
            RenderUtils.Merge2DStrings(
                [.. players.Select(p => MakeShoppingStand(p, shopStates))],
                8, false)
        );
        buffer += "\n";
        buffer += "\n";
        buffer += RenderUtils.Center2DString(MakeReadyPlayerDisplay());

        RenderUtils.Render(buffer);
    }

    private string MakeReadyPlayerDisplay()
    {
        int nonAIPlayerCount = gameState.Players.Count(p => !p.IsAI);
        int readyPlayerCount = gameState.ReadyPlayers.Count(p => !p.IsAI);
        string readyText = $"Ready players [{readyPlayerCount}/{nonAIPlayerCount}]";
        return readyText;
    }

    private static string MakeTitle()
    {
        return RenderUtils.Center2DString("============ SHOP ============");
    }

    private static string MakeShoppingStand(Player player, List<Shelves> shopStates)
    {
        Shop shop = player.Shop;

        Shelves shelves = shopStates.First(s => s.Shop == shop);
        List<Shelf> shopItems = shelves.ShelvesList;

        string buffer = "";

        string title = $"{player.Name.Possessive()} Shop";
        int cartValue = shopItems.Where(shelf => shelf.Side == Side.Basket).Sum(shelf => shelf.Product.price);
        string balance = $"Balance: {player.Money}cu";
        string cartTotal = $"Cart total: {cartValue}cu";

        buffer += $"{title.PadVisibleLeft((shelfWidth + title.VisibleLength()) / 2).PadVisibleRight(shelfWidth)}\n";
        buffer += $" {balance.PadVisibleRight(shelfWidth - cartTotal.VisibleLength() - 2)}{cartTotal}\n";
        buffer += $"╭───────────── SHOP ─────────────┬───────────── CART ─────────────╮\n";

        // Items
        for (int i = 0; i < shopItems.Count; i++)
        {
            buffer += "│ ";
            Shelf shelf = shopItems[i];
            bool isSelected = shelves.ShelfIndex == i;

            // Shop side
            if (shelf.Side == Side.Stand) buffer += MakeProductDisplay(shelf, isSelected, player, cartValue);
            else buffer += "".PadVisibleRight(itemWidth);

            // Middle barrier
            if (isSelected)
            {
                buffer += shelf.Side == Side.Stand ? " > " : " < ";
            }
            else buffer += " │ ";

            // Cart side
            if (shelf.Side == Side.Basket) buffer += MakeProductDisplay(shelf, isSelected, player, cartValue, isCart: true);
            else buffer += "".PadVisibleRight(itemWidth);

            buffer += " │\n";
        }

        // Ready row (one step below the last shop item)
        bool isReadySelected = shelves.ShelfIndex == shop.Products.Count;
        string readyLabel = isReadySelected ? AnsiColor.Black(AnsiColor.BgWhite(" READY ")) : " READY ";
        string readyText = $"{readyLabel.PadVisibleLeft((itemWidth + "READY".Length) / 2).PadVisibleRight(itemWidth)}";
        buffer += $"│ {readyText} │ {"".PadVisibleRight(itemWidth)} │\n";

        // Padding
        buffer += $"│ {"".PadVisibleRight(itemWidth)} │ {"".PadVisibleRight(itemWidth)} │\n";
        buffer += $"├─{new string('─', itemWidth)}─┴─{new string('─', itemWidth)}─┤\n";

        // Item description
        for (int i = 0; i < shopItems.Count; i++)
        {
            Shelf shelf = shopItems[i];
            if (shelves.ShelfIndex == i)
            {
                string description = shelf.Product.description;
                List<string> descriptionLines = [.. RenderUtils.WrapText(description, shelfWidth - 4).Split('\n')];
                foreach (string line in descriptionLines)
                {
                    buffer += $"│ {line.PadVisibleRight(shelfWidth - 4)} │\n";
                }
                // buffer += $"│ {"".PadVisibleRight(shelfWidth - 4)} │\n";
                buffer += $"├─{new string('─', shelfWidth - 4)}─┤\n";
            }
        }

        // Instructions
        buffer += $"│ {"Use Up/Down to select an item.".PadVisibleRight(shelfWidth - 4)} │\n";
        buffer += $"│ {"Use Left/Right to put back/to cart.".PadVisibleRight(shelfWidth - 4)} │\n";
        buffer += $"│ {"Select READY to mark yourself as ready to continue.".PadVisibleRight(shelfWidth - 4)} │\n";
        buffer += $"╰{new string('─', shelfWidth - 2)}╯\n";

        return buffer;
    }

    private static string MakeProductDisplay(Shelf shelf, bool isSelected, Player player, int cartValue, bool isCart = false)
    {
        string displayPrice = AnsiColor.Gray("N/A");
        if (shelf.Product.price != int.MaxValue)
        {
            if (isCart)
            {
                // Items already in the cart keep a neutral color
                displayPrice = $"{shelf.Product.price}cu";
            }
            else
            {
                int effectiveMoney = player.Money - cartValue;

                displayPrice = shelf.Product.price <= effectiveMoney ?
                    AnsiColor.Green($"{shelf.Product.price}cu")
                    :
                    AnsiColor.Red($"{shelf.Product.price}cu");
            }
        }
        string line = $"{shelf.Product.Name.PadVisibleRight(itemWidth - displayPrice.VisibleLength() - 1)} {displayPrice}";

        return isSelected ? AnsiColor.Black(AnsiColor.BgWhite(line)) : line;
    }
}