
class Renderer
{
    public static void RenderBoard(Board board)
    {
        const int AspectRatioCorrection = 2; // console characters are taller than they are wide
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

        buffer += $"╭{new string('─', CanvasWidth)}╮\n"; // top border

        // For each row
        for (int y = board.VisibleHeight; y < board.Height; y++)
        {
            buffer += "│"; // left border

            // For each column within the row
            List<bool> row = board.CollisionGrid[y];
            for (int x = 0; x < board.Width; x++)
            {
                Console.WriteLine(row);
                buffer += board.CollisionGrid[y][x] ? "▓▓" : "  ";
            }
            buffer += "│"; // right border
            buffer += "\n";
        }

        buffer += $"╰{new string('─', CanvasWidth)}╯\n"; // bottom border

        Console.Clear(); // clear and draw to mitigate stutter and general unpleasantries
        Console.WriteLine(buffer);
    }
}