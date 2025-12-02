using System.Text.RegularExpressions;

static class RenderUtils
{
    public static string lastFrame = "";

    public static readonly Regex ansiRegex = new("\u001b\\[[0-9;]*m");

    public static void Render(string content)
    {
        RenderUtils.lastFrame = content;
        Console.Clear();
        Console.WriteLine(content);
    }

    public static string Merge2DStrings(List<string> parts, int spacing = 16, bool bottomAlign = true)
    {
        List<List<string>> linesGroupedByPart = [.. parts.Select(part => part.Split('\n').ToList())];

        // Normalize line counts with leading empty lines
        int largestHeight = linesGroupedByPart.Max(part => part.Count);
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
            return lineGroup.Select(line => line.PadVisibleRight(maxVisibleLength)).ToList();
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
        return padding + input2d.Replace("\n", "\n" + padding).TrimEnd() + "\n";
    }

    public static int GetVisibleLength(string text)
        => ansiRegex.Replace(text, string.Empty).Length;

    public static void DimCanvas()
    {
        string dimmedLastFrame = ansiRegex.Replace(lastFrame, match =>
        {
            string inner = match.Value.Replace("\u001b[", "").Replace("m", "");
            string[] parts = inner.Split(';');

            // If rgb, dim via rgb manipulation
            if (parts.Length == 5)
            {
                if (int.TryParse(parts[2], out int r) &&
                    int.TryParse(parts[3], out int g) &&
                    int.TryParse(parts[4], out int b))
                {
                    r = (int)(r * 0.6);
                    g = (int)(g * 0.6);
                    b = (int)(b * 0.6);
                    return $"\u001b[38;2;{r};{g};{b}m";
                }
            }

            // Else, normal dim code insertion
            return match.Value.Insert(2, "2;");

        });
        Render(dimmedLastFrame);
    }

    /** 
     * Moves cursor to modify canvas in place to write a large number made of unicode block characters.
     */
    public static void WriteLargeNumberInPlace(int number)
    {
        DimCanvas();

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;
        int baseY = (consoleHeight - 10) / 2;

        string selectedNumber = number switch
        {
            0 => zero,
            1 => one,
            2 => two,
            3 => three,
            _ => throw new ArgumentOutOfRangeException(nameof(number), "Only numbers 0-3 are supported."),
        };

        int offsetX = (consoleWidth - GetVisibleLength(selectedNumber.Split('\n').First())) / 2;

        string[] lines = selectedNumber.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            Console.SetCursorPosition(offsetX, baseY + i);
            Console.WriteLine(line);
        }
    }

    private static readonly string zero = $"""
    {AnsiColor.Green("▗▇▇▇▇▖  ▗▇▇▇▇▖  ▇▇")}
    {AnsiColor.Green("█▘  ▝█  █▘  ▝█  ██")}
    {AnsiColor.Green("█       █    █  ██")}
    {AnsiColor.Green("█  ███  █    █  ██")}
    {AnsiColor.Green("█▖  ▗█  █▖  ▗█    ")}
    {AnsiColor.Green("▝████▘  ▝████▘  ██")}
    """;

    private static readonly string one = $"""
    {AnsiColor.Orange(" ▗▇▇  ")}
    {AnsiColor.Orange(" ███  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("  ██  ")}
    {AnsiColor.Orange("██████")}
    """;

    private static readonly string two = $"""
    {AnsiColor.Yellow(" ▄▇▇▄ ")}
    {AnsiColor.Yellow("██▘ ██")}
    {AnsiColor.Yellow("  ▗██▘")}
    {AnsiColor.Yellow(" ▗██▘ ")}
    {AnsiColor.Yellow("▗██▘  ")}
    {AnsiColor.Yellow("██████")}
    """;

    private static readonly string three = $"""
    {AnsiColor.Red(" ▄▇▇▄ ")}
    {AnsiColor.Red("██  ██")}
    {AnsiColor.Red(" ▗▄▄█▘")}
    {AnsiColor.Red(" ▝▀▀█▖")}
    {AnsiColor.Red("██  ██")}
    {AnsiColor.Red("▝████▘")}
    """;

    public static string WrapText(string text, int maxLineLength)
    {
        List<string> words = [.. text.Split(' ')];
        string buffer = "";
        string currentLine = "";

        foreach (string word in words)
        {
            int wordLength = GetVisibleLength(word);
            int currentLineLength = GetVisibleLength(currentLine);

            if (currentLineLength + wordLength + 1 <= maxLineLength)
            {
                if (currentLineLength > 0)
                {
                    currentLine += " ";
                }
                currentLine += word;
            }
            else
            {
                if (currentLineLength > 0)
                {
                    buffer += currentLine + "\n";
                }
                currentLine = word;
            }
        }

        if (currentLine.Length > 0)
        {
            buffer += currentLine + "\n";
        }

        return buffer.TrimEnd('\n');
    }
}