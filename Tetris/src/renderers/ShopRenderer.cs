
class ShopRenderer
{
    private readonly List<Player> players;

    public ShopRenderer(GameState gameState)
    {
        this.players = gameState.Players;
    }

    public void Render()
    {
        string buffer = "";

        buffer += MakeTitle() + "\n\n";

        Console.Clear();
        Console.WriteLine(buffer);
    }

    private static string MakeTitle()
    {
        return RenderUtils.Center2DString("============ SHOP ============");
    }
}