
class Games
{
    private List<Tetris> tetrises = [new Tetris(), new Tetris()];
    public List<Tetris> Tetrises => tetrises;

    // overarching game state
    public Games()
    {
        // Game controls
        var control1 = new Dictionary<string, Input> {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Rotate },
            { "S", Input.SoftDrop }
        };

        var control2 = new Dictionary<string, Input> {
            { "LeftArrow", Input.Left },
            { "RightArrow", Input.Right },
            { "UpArrow", Input.Rotate },
            { "DownArrow", Input.SoftDrop }
        };
        Input direction1;
        Input direction2;

        Renderer renderer = new(this);
        bool gaming = true;

        long lastTick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int shopTime = 500 * 20 * 5; // Temporary value, Probably around ~5 default tetrominoe spawn

        // TODO: Switch case to switch between menu, game and shop, might want to live in Program, not sure
        while (gaming)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            // if (currentTime - lastTick > shopTime)
            // {
            //     lastTick = currentTime;
            //     gaming = false;
            // }

            string key = KeyInput.Read() ?? "";
            {
                if (control1.TryGetValue(key, out direction1))
                {
                    tetrises[0].Move(direction1);
                }

                if (control2.TryGetValue(key, out direction2))
                {
                    tetrises[1].Move(direction2);
                }
            }
            tetrises[0].Tick();
            tetrises[1].Tick();
            renderer.Render();
            Thread.Sleep(20);
        }
    }

    public List<Board> GetAllBoards()
      => [.. tetrises.Select(t => t.Board)];
}