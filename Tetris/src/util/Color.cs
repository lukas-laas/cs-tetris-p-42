
/// <summary>
/// ANSI color codes for console text coloring without using the Console.*Color properties.
/// 
/// References:
/// https://en.wikipedia.org/wiki/ANSI_escape_code#colors
/// </summary>
class AnsiColor
{
    public const string ResetCode = "\u001b[0m";
    public const string RedCode = "\u001b[31m";
    public const string GreenCode = "\u001b[32m";
    public const string YellowCode = "\u001b[93m";
    public const string OrangeCode = "\u001b[38;2;253;151;13m";
    public const string BlueCode = "\u001b[34m";
    public const string MagentaCode = "\u001b[35m";
    public const string CyanCode = "\u001b[36m";

    public static string Red(string text) => Apply(text, RedCode);
    public static string Green(string text) => Apply(text, GreenCode);
    public static string Yellow(string text) => Apply(text, YellowCode);
    public static string Orange(string text) => Apply(text, OrangeCode);
    public static string Blue(string text) => Apply(text, BlueCode);
    public static string Magenta(string text) => Apply(text, MagentaCode);
    public static string Cyan(string text) => Apply(text, CyanCode);

    public static string Apply(string text, string colorCode)
    {
        return $"{colorCode}{text}{ResetCode}";
    }

    // Very OOP... :eyes:
    private static readonly string[] order = [RedCode, GreenCode, YellowCode, BlueCode, MagentaCode, CyanCode];
    static private int nextColorIndex = 0;
    public static string GetNextColor()
    {
        return order[nextColorIndex++ % order.Length];
    }
}