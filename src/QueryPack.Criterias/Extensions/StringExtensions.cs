namespace QueryPack.Criterias.Extensions
{
    using System;

    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Represents StartsWith function
        /// </summary>
        public static bool Like<T>(this T property, string pattern)
        {
            return property.ToString().StartsWith(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}