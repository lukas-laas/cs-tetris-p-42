

class WelcomeRenderer(GameState gameState)
{
    private readonly GameState gameState = gameState;
    private readonly List<Player> players = [.. gameState.Players.Where(p => !p.IsAI)];

    public void Render(GameModeSelect gameModeSelected)
    {
        string buffer = "";

        // Title
        buffer += GameRenderer.MakeTitle(); // Borrow title from main game renderer since it's pwetty
        buffer += "\n\n";

        switch (gameModeSelected)
        {
            case GameModeSelect.Singleplayer:
                buffer += RenderUtils.Center2DString(AnsiColor.BgWhite(AnsiColor.Black("Singleplayer mode  -  Press ENTER to start".PadVisibleRight(45))));
                buffer += RenderUtils.Center2DString("Multiplayer mode".PadVisibleRight(45));
                break;

            case GameModeSelect.Multiplayer:
                buffer += RenderUtils.Center2DString("Singleplayer mode".PadVisibleRight(45));
                buffer += RenderUtils.Center2DString(AnsiColor.BgWhite(AnsiColor.Black("Multiplayer mode  -  Press ENTER to start".PadVisibleRight(45))));
                break;
        }

        buffer += "\n\n";

        // Descriptions
        switch (gameModeSelected)
        {
            case GameModeSelect.Singleplayer:

                buffer += RenderUtils.Center2DString("You against the AGI (artificial gaming intelligence).");
                buffer += RenderUtils.Center2DString("""

                Player controls:
                  A / ←: Move left
                  D / →: Move right
                  W / ↑: Rotate piece
                  S / ↓: Soft drop
                  Q / -: Use ability
                """);
                break;

            case GameModeSelect.Multiplayer:
                buffer += RenderUtils.Center2DString("""
                Compete against another player to see who can get the highest 
                score and more importantly, who can survive the longest!
                """);

                buffer += "\n";

                buffer += RenderUtils.Center2DString(RenderUtils.Merge2DStrings([
                    """
                    Controls (Player 1):
                      A: Move left
                      D: Move right
                      W: Rotate piece
                      S: Soft drop
                      Q: Use ability
                    """,
                    """
                    Controls (Player 2):
                      ←: Move left
                      →: Move right
                      ↑: Rotate piece
                      ↓: Soft drop
                      -: Use ability
                    """
                ]));
                break;
        }

        RenderUtils.Render(buffer);
    }
}