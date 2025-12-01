
enum Side
{
    Stand, // Left
    Basket, // Right
};
class Shelf(Side side, IProduct product)
{
    public Side Side { get; set; } = side;
    public IProduct Product { get; set; } = product;
}

class ShopRenderer(GameState gameState)
{
    private readonly List<Player> players = gameState.Players;

    public void Render()
    {
        string buffer = "";

        buffer += "\n";
        buffer += MakeTitle();
        buffer += "\n\n";

        buffer += RenderUtils.Center2DString(RenderUtils.Merge2DStrings([.. players.Select(MakeShoppingStand)], 8));

        RenderUtils.Render(buffer);
    }

    private static string MakeTitle()
    {
        return RenderUtils.Center2DString("============ SHOP ============");
    }

    private static string MakeShoppingStand(Player player)
    {
        if (player.Shop is null) throw new Exception("Player has no shop!");
        Shop shop = player.Shop;

        List<Shelf> shopItems = [.. shop.Products.Select(product => new Shelf(Side.Stand, product))];

        const int itemWidth = 20;
        const int shelfWidth = itemWidth * 2 + 3 + 4; // 3 and 4 for borders and padding

        string buffer = "";
        buffer += $"{player.Name.PadLeft((shelfWidth + player.Name.Length) / 2).PadRight(shelfWidth)}\n"; // TODO Make prettier
        buffer += $"╭──────── SHOP ────────┬──────── CART ────────╮\n";

        for (int i = 0; i < shopItems.Count; i++)
        {
            buffer += "│ ";
            Shelf shelf = shopItems[i];
            bool isSelected = shop.ShelfIndex == i;

            // Shop side
            if (shelf.Side == Side.Stand)
            {
                buffer += $"{shelf.Product.name} - {shelf.Product.price}cu".PadRight(itemWidth);
            }
            else buffer += $"{"",itemWidth}";

            // Middle barrier
            if (isSelected)
            {
                buffer += shelf.Side == Side.Stand ? " > " : " < ";
            }
            else buffer += " │ ";

            // Cart side
            if (shelf.Side == Side.Basket)
            {
                buffer += $"{shelf.Product.name} - {shelf.Product.price}cu".PadRight(itemWidth);
            }
            else buffer += $"{"",itemWidth}";
            buffer += " │\n";
        }

        // Bottom
        buffer += $"│ {"",itemWidth} │ {"",itemWidth} │\n";
        buffer += $"╰─{new string('─', itemWidth)}─┴─{new string('─', itemWidth)}─╯\n";

        return buffer;
    }
}