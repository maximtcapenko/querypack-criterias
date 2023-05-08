namespace QueryPack.Criterias
{
    using System.Collections.Generic;

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
        Dictionary<string, OrderDirection> OrderBy { get; set; }
    }
}