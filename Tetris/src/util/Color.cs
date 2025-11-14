
/// <summary>
/// ANSI color codes for console text coloring without using the Console.*Color properties.
/// </summary>
class AnsiColor
{
    public const string Reset = "\u001b[0m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Yellow = "\u001b[33m";
    public const string Blue = "\u001b[34m";
    public const string Magenta = "\u001b[35m";
    public const string Cyan = "\u001b[36m";

    public static string Apply(string text, string color)
    {
        return $"{color}{text}{Reset}";
    }

    public static string GetRandomColor()
    {
        string[] colors = [Red, Green, Yellow, Blue, Magenta, Cyan];
        Random rand = new();
        return colors[rand.Next(colors.Length)];
    }
}