namespace QueryPack.Criterias
{
    using System.Collections.Generic;

    /// <summary>
    /// Order Direction
    /// </summary>
    public enum OrderDirection
    {
        Asc = 1,
        Desc
    }
    /// <summary>
    /// The basic interface which contains the definition of order statements
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Order definitions container
        /// </summary>
        Dictionary<string, OrderDirection> OrderBy { get; set; }
    }
}