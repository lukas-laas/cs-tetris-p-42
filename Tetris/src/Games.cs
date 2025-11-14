using System.Xml;

class Games
{
    // overarching game state
    public Games()
    {
        const int players = 2;
        List<Tetris> tetrises = new(players);

        // Game controls
        var control1 = new Dictionary<string, Input> {
            { "A", Input.Left },
            { "D", Input.Right },
            { "W", Input.Rotate },
            { "S", Input.SoftDrop }
        };

        var control2 = new Dictionary<string, Input> {
            { "LeftArrow", Input.Left },
            { "RightArror", Input.Right },
            { "UpArrow", Input.Rotate },
            { "DownArrow", Input.SoftDrop }
        };
        Input direction1;
        Input direction2;

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
        }
    }
}