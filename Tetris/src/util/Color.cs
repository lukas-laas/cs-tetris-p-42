
/// <summary>
/// ANSI color codes for console text coloring without using the Console.*Color properties.
/// 
/// References:
/// https://en.wikipedia.org/wiki/ANSI_escape_code#colors
/// </summary>
class AnsiColor
{
    public const string RedCode = "\u001b[31m";
    public static string Red(string text) => Apply(text, RedCode);

    public const string GreenCode = "\u001b[32m";
    public static string Green(string text) => Apply(text, GreenCode);

    public const string YellowCode = "\u001b[93m";
    public static string Yellow(string text) => Apply(text, YellowCode);

    public const string OrangeCode = "\u001b[38;2;253;151;13m";
    public static string Orange(string text) => Apply(text, OrangeCode);

    public const string BlueCode = "\u001b[34m";
    public static string Blue(string text) => Apply(text, BlueCode);

    public const string MagentaCode = "\u001b[35m";
    public static string Magenta(string text) => Apply(text, MagentaCode);

    public const string CyanCode = "\u001b[36m";
    public static string Cyan(string text) => Apply(text, CyanCode);



    // Colors not in color rotation
    public const string WhiteCode = "\u001b[37m";
    public static string White(string text) => Apply(text, WhiteCode);

    public const string PinkCode = "\u001b[38;2;255;105;180m";
    public static string Pink(string text) => Apply(text, PinkCode);

    public const string ArchBlueCode = "\u001b[38;2;22;145;207m";
    public static string ArchBlue(string text) => Apply(text, ArchBlueCode);

    public const string BorderBlueCode = "\u001b[38;2;49;103;204m";
    public static string BorderBlue(string text) => Apply(text, BorderBlueCode);

    public const string GrayCode = "\u001b[90m";
    public static string Gray(string text) => Apply(text, GrayCode);

    public const string XRedCode = "\u001b[38;2;255;0;0m";
    public static string XRed(string text) => Apply(text, XRedCode);

    public const string BlackCode = "\u001b[30m";
    public static string Black(string text) => Apply(text, BlackCode);


    // BG colors
    public const string BgWhiteCode = "\u001b[47m";
    public static string BgWhite(string text) => Apply(text, BgWhiteCode);

    public const string ResetCode = "\u001b[0m";
    public static string Reset(string text) => $"{ResetCode}{text}{ResetCode}";


    public static string Apply(string text, string colorCode)
        => $"{colorCode}{text}{ResetCode}";

    public static string Apply(string text, int r, int g, int b)
        => Apply(text, $"\u001b[38;2;{Math.Clamp(r, 0, 255)};{Math.Clamp(g, 0, 255)};{Math.Clamp(b, 0, 255)}m");

    // Very OOP... :eyes:
    private static readonly string[] colorRotation = [RedCode, GreenCode, OrangeCode, YellowCode, BlueCode, MagentaCode, CyanCode];
    static private int nextColorIndex = 0;
    public static string GetNextColor()
    {
        return colorRotation[Math.Abs(nextColorIndex++) % colorRotation.Length];
    }
}