
class Games
{
    private List<Tetris> tetrises = [new Tetris(), new Tetris()];

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

        long globalTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        while (true)
        {
            string key = KeyInput.Read() ?? "";
            {
                if (control1.TryGetValue(key, out direction1))
                {
                    tetrises[0].Move(direction1);
                }

                if (control2.TryGetValue(key, out direction2))
                {
                    tetrises[1].Move(direction1);
                }
            }

            renderer.Render();
            Thread.Sleep(400);
        }
    }

    public List<Board> GetAllBoards()
      => [.. tetrises.Select(t => t.Board)];
}