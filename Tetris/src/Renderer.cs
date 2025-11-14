
class Renderer
{
    public static void RenderBoard(Board board)
    {
        const int AspectRatioCorrection = 2; // Console characters are taller than they are wide
        int CanvasWidth = board.Width * AspectRatioCorrection;

        string buffer = "";

        // Title
        const int TitleWidth = 24;
        string titlePadLeft = new(' ', Math.Max(Console.WindowWidth - TitleWidth, 0) / 2);
        buffer += $"""

        {titlePadLeft}╔══════════════════════════╗
        {titlePadLeft}║ ▄▄▄▄ ▄▄▄ ▄▄▄▄ ▄▄▖ ▗▖ ▗▄▖ ║
        {titlePadLeft}║  ▐▌  █▄   ▐▌  █▂█ ▐▌ ▀▙▝ ║
        {titlePadLeft}║  ▐▌  █▄▄  ▐▌  █▀▙ ▐▌ ▜▄▛ ║
        {titlePadLeft}║                          ║
        {titlePadLeft}▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀

        """;

        string boardPadLeft = new(' ', 8);

        buffer += $"{boardPadLeft}╭{new string('─', CanvasWidth)}╮\n"; // top border

        // For each row
        for (int y = 0; y < board.Height; y++)
        {
            // Skip hidden rows
            if (y < board.Height - board.VisibleHeight) continue;

            buffer += $"{boardPadLeft}│"; // Left border

            List<bool> row = board.CollisionGrid[y];
            bool anythingOnThisRow = row.Any(cell => cell);

            // For each column within the row
            for (int x = 0; x < board.Width; x++)
            {
                Console.WriteLine(row);
                buffer += board.CollisionGrid[y][x] ? new string('▓', 2) : new string(' ', 2);
            }
            buffer += "│"; // Right border
            buffer += "\n";
        }

        buffer += $"{boardPadLeft}╰{new string('─', CanvasWidth)}╯\n"; // Bottom border

        Console.Clear(); // Clear and draw close together to mitigate stutter and visual unpleasantries
        Console.WriteLine(buffer);
    }
}