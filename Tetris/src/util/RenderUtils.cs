
using System.Text.RegularExpressions;

static class RenderUtils
{
    public static readonly Regex ansiRegex = new("\u001b\\[[0-9;]*m");

    public static string Merge2DStrings(List<string> parts, int spacing = 16, bool bottomAlign = true)
    {
        List<List<string>> linesGroupedByPart = [.. parts.Select(part => part.Split('\n').ToList())];

        // Normalize line counts with leading empty lines
        int largestHeight = linesGroupedByPart.Max(part => part.Count);
        // linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup => Enumerable.Repeat(string.Empty, largestHeight - lineGroup.Count).Concat(lineGroup).ToList())];
        if (bottomAlign)
        {
            linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup =>
                Enumerable.Repeat(string.Empty, largestHeight - lineGroup.Count).Concat(lineGroup).ToList()
            )];
        }
        else
        {
            linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup =>
                lineGroup.Concat(Enumerable.Repeat(string.Empty, largestHeight - lineGroup.Count)).ToList()
            )];
        }

        // Normalize line lengths with trailing spaces to groups widest line
        linesGroupedByPart = [.. linesGroupedByPart.Select(lineGroup =>
        {
            int maxVisibleLength = lineGroup.Select(GetVisibleLength).Max();
            return lineGroup.Select(line => PadRightVisible(line, maxVisibleLength)).ToList();
        })];

        // Merge line by line
        string buffer = "";
        for (int lineIndex = 0; lineIndex < largestHeight; lineIndex++)
        {
            string line = "";
            foreach (var group in linesGroupedByPart)
            {
                line += group[lineIndex] + new string(' ', spacing);
            }
            buffer += line.TrimEnd() + "\n"; // Trim trailing spaces
        }
        return buffer;
    }

    public static string Center2DString(string input2d)
    {
        int consoleWidth = Console.WindowWidth;
        int visibleWidth = input2d.Split('\n').Max(GetVisibleLength);
        if (visibleWidth >= consoleWidth) return input2d;

        int paddingLeft = (consoleWidth - visibleWidth) / 2;
        string padding = new(' ', paddingLeft);
        return padding + input2d.Replace("\n", "\n" + padding);
    }

    public static int GetVisibleLength(string text)
        => ansiRegex.Replace(text, string.Empty).Length;

    public static string PadRightVisible(string text, int targetVisibleLength)
    {
        int currentVisibleLength = GetVisibleLength(text);
        if (currentVisibleLength >= targetVisibleLength) return text;

        return text + new string(' ', targetVisibleLength - currentVisibleLength);
    }

    /** 
     * Moves cursor to modify canvas in place to write a large number made of unicode block characters.
     */
    public static void WriteLargeNumberInPlace(int number)
    {
        // move cursor to center of the screen
        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        switch (number)
        {
            case 0:
                Console.SetCursorPosition((consoleWidth - 6) / 2, (consoleHeight - 6) / 2);
                // Console.Write(zero);
                string[] lines = zero.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    Console.SetCursorPosition((consoleWidth - 6) / 2, (consoleHeight - 6) / 2 + i);
                    Console.WriteLine(line);
                }
                break;
            case 1:
                Console.SetCursorPosition((consoleWidth - 6) / 2, (consoleHeight - 6) / 2);
                Console.Write(one);
                break;
            case 2:
                Console.SetCursorPosition((consoleWidth - 6) / 2, (consoleHeight - 6) / 2);
                Console.Write(two);
                break;
            case 3:
                Console.SetCursorPosition((consoleWidth - 6) / 2, (consoleHeight - 6) / 2);
                Console.Write(three);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(number), "Only numbers 0-3 are supported.");
        }
    }

    private static readonly string zero = $"""
    {AnsiColor.Green(" ████ ")}
    {AnsiColor.Green("██   █")}
    {AnsiColor.Green("█ █  █")}
    {AnsiColor.Green("█  █ █")}
    {AnsiColor.Green("█   ██")}
    {AnsiColor.Green(" ████ ")}
    """;

    private static readonly string one = $"""
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange(" ███  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("██████")}
    """;

    private static readonly string two = $"""
    {AnsiColor.Yellow(" ████ ")}
    {AnsiColor.Yellow("██   █")}
    {AnsiColor.Yellow("   ██ ")}
    {AnsiColor.Yellow("  ██  ")}
    {AnsiColor.Yellow(" ██   ")}
    {AnsiColor.Yellow("██████")}
    """;

    private static readonly string three = $"""
    {AnsiColor.Red(" ████ ")}
    {AnsiColor.Red("██  ██")}
    {AnsiColor.Red("   ██ ")}
    {AnsiColor.Red("  ██  ")}
    {AnsiColor.Red("▄  ██ ")}
    {AnsiColor.Red("▀████ ")}
    """;
}