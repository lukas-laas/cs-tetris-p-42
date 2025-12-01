

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
        buffer += $"{player.Name.PadLeft((shelfWidth + player.Name.Length) / 2).PadRight(shelfWidth)}\n"; // TODO Make prettier
        buffer += $"╭───────────── SHOP ─────────────┬───────────── CART ─────────────╮\n";

        for (int i = 0; i < shopItems.Count; i++)
        {
            buffer += "│ ";
            Shelf shelf = shopItems[i];
            bool isSelected = shop.ShelfIndex == i;
            string displayPrice = shelf.Product.price == int.MaxValue ? "N/A" : $"{shelf.Product.price}cu";

            // Shop side
            if (shelf.Side == Side.Stand) buffer += MakeProductDisplay(shelf.Product, isSelected);
            else buffer += "".PadRight(itemWidth);

            // Middle barrier
            if (isSelected)
            {
                buffer += shelf.Side == Side.Stand ? " > " : " < ";
            }
            else buffer += " │ ";

            // Cart side
            if (shelf.Side == Side.Basket) buffer += MakeProductDisplay(shelf.Product, isSelected);
            else buffer += "".PadRight(itemWidth);

            buffer += " │\n";
        }

        // Bottom
        buffer += $"│ {"".PadRight(itemWidth)} │ {"".PadRight(itemWidth)} │\n";
        buffer += $"├─{new string('─', itemWidth)}─┴─{new string('─', itemWidth)}─┤\n";
        buffer += $"│ {"Use Up/Down to select an item.".PadRight(shelfWidth - 4)} │\n";
        buffer += $"│ {"Use Left/Right to put back/to cart.".PadRight(shelfWidth - 4)} │\n";
        buffer += $"╰{new string('─', shelfWidth - 2)}╯\n";

        return buffer;
    }

    private static string MakeProductDisplay(IProduct product, bool isSelected)
    {
        string displayPrice = product.price == int.MaxValue ? "N/A" : $"{product.price}cu";
        string line = $"{product.name.PadRight(itemWidth - displayPrice.Length - 1)} {displayPrice}";

        return isSelected ? AnsiColor.Black(AnsiColor.BgWhite(line)) : line;
    }
}