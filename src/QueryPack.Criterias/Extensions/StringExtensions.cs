namespace QueryPack.Criterias.Extensions
{
    using System;

    public static class StringExtensions
    {
        public static bool Like<T>(this T property, string pattern)
        {
            return property.ToString().StartsWith(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}