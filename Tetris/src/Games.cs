class Games
{
    // overarching game state

    // boards via tetris (plural)
    public Games(int players)
    {
        List<Tetris> tetrises = new(players);

        while (true)
        {
            string key = Input.Read() ?? "";
            //Renderer renderer = new(this);
        }
    }
}