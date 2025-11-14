class Input
{
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
