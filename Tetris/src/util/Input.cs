class KeyInput
{
    public static string? Read()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key.ToString() != "") Log.Add(key.Key.ToString());

            return key.Key.ToString();
        }
        return null;
    }
}
