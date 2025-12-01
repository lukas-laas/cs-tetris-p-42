
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