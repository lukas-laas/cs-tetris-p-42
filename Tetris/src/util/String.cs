
static class StringExtensions
{
    public static string Possessive(this string name)
    {
        if (name.EndsWith('s'))
        {
            return name + "'";
        }
        else
        {
            return name + "'s";
        }
    }

    /// <summary>
    /// Pads the string on the right to achieve the desired total visible length, accounting for non-visible characters such as ANSI escape codes.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="totalWidth"></param>
    /// <returns></returns>
    public static string PadVisibleRight(this string str, int totalWidth)
    {
        int visibleLength = RenderUtils.GetVisibleLength(str);
        int paddingNeeded = totalWidth - visibleLength;
        if (paddingNeeded > 0)
        {
            return str + new string(' ', paddingNeeded);
        }
        else
        {
            return str;
        }
    }

    /// <summary>
    /// Pads the string on the left to achieve the desired total visible length, accounting for non-visible characters such as ANSI escape codes.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="totalWidth"></param>
    /// <returns></returns>
    public static string PadVisibleLeft(this string str, int totalWidth)
    {
        int visibleLength = RenderUtils.GetVisibleLength(str);
        int paddingNeeded = totalWidth - visibleLength;
        if (paddingNeeded > 0)
        {
            return new string(' ', paddingNeeded) + str;
        }
        else
        {
            return str;
        }
    }

    public static int VisibleLength(this string str)
    {
        return RenderUtils.GetVisibleLength(str);
    }
}