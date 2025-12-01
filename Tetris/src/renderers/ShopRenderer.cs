

class ShopRenderer(GameState gameState)
{
    private readonly List<Player> players = gameState.Players;
    private static readonly int itemWidth = 30;
    private static readonly int shelfWidth = itemWidth * 2 + 3 + 4; // 3 and 4 for borders and padding

    public void Render(List<Shelves> shopStates)
    {
        string buffer = "";

        buffer += "\n";
        buffer += MakeTitle();
        buffer += "\n\n";

        buffer += RenderUtils.Center2DString(RenderUtils.Merge2DStrings([.. players.Select(p => MakeShoppingStand(p, shopStates))], 8));

        RenderUtils.Render(buffer);
    }

    private static string MakeTitle()
    {
        return RenderUtils.Center2DString("============ SHOP ============");
    }

    private static string MakeShoppingStand(Player player, List<Shelves> shopStates)
    {
        if (player.Shop is null) throw new Exception("Player has no shop!");
        Shop shop = player.Shop;

        List<Shelf> shopItems = shopStates.First(s => s.Shop == shop).ShelvesList;

        string buffer = "";

        string title = $"{player.Name.Possessive()} Shop";
        string balance = $"Balance: {player.Money}cu";
        string cartTotal = $"Cart total: {shopItems.Where(shelf => shelf.Side == Side.Basket).Sum(shelf => shelf.Product.price)}cu";

        buffer += $"{title.PadVisibleLeft((shelfWidth + title.VisibleLength()) / 2).PadVisibleRight(shelfWidth)}\n";
        buffer += $" {balance.PadVisibleRight(shelfWidth - cartTotal.VisibleLength() - 2)}{cartTotal}\n";
        buffer += $"╭───────────── SHOP ─────────────┬───────────── CART ─────────────╮\n";
        for (int i = 0; i < shopItems.Count; i++)
        {
            buffer += "│ ";
            Shelf shelf = shopItems[i];
            bool isSelected = shop.ShelfIndex == i;

            // Shop side
            if (shelf.Side == Side.Stand) buffer += MakeProductDisplay(shelf, isSelected, player);
            else buffer += "".PadVisibleRight(itemWidth);

            // Middle barrier
            if (isSelected)
            {
                buffer += shelf.Side == Side.Stand ? " > " : " < ";
            }
            else buffer += " │ ";

            // Cart side
            if (shelf.Side == Side.Basket) buffer += MakeProductDisplay(shelf, isSelected, player);
            else buffer += "".PadVisibleRight(itemWidth);

            buffer += " │\n";
        }

        // Bottom
        buffer += $"│ {"".PadVisibleRight(itemWidth)} │ {"".PadVisibleRight(itemWidth)} │\n";
        buffer += $"├─{new string('─', itemWidth)}─┴─{new string('─', itemWidth)}─┤\n";
        buffer += $"│ {"Use Up/Down to select an item.".PadVisibleRight(shelfWidth - 4)} │\n";
        buffer += $"│ {"Use Left/Right to put back/to cart.".PadVisibleRight(shelfWidth - 4)} │\n";
        buffer += $"╰{new string('─', shelfWidth - 2)}╯\n";

        return buffer;
    }

    private static string MakeProductDisplay(Shelf shelf, bool isSelected, Player player)
    {
        string displayPrice = AnsiColor.Gray("N/A");
        if (shelf.Product.price != int.MaxValue)
        {
            displayPrice = shelf.Product.price <= player.Money ?
                AnsiColor.Green($"{shelf.Product.price}cu")
                :
                AnsiColor.Red($"{shelf.Product.price}cu");
        }
        string line = $"{shelf.Product.name.PadVisibleRight(itemWidth - displayPrice.VisibleLength() - 1)} {displayPrice}";

        return isSelected ? AnsiColor.Black(AnsiColor.BgWhite(line)) : line;
    }
}