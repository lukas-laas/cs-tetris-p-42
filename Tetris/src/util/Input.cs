class KeyInput
{
    // TODO add key release requirement to pass along the same key again so holding won't send same key multiple times
    public static string? Read()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            return key.Key.ToString();
        }
        return null;
    }
}
