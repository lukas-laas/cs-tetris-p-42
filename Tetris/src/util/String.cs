
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
}