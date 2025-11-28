
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

        RenderUtils.Render(buffer);
    }

    private static string MakeTitle()
    {
        return RenderUtils.Center2DString("============ SHOP ============");
    }
}