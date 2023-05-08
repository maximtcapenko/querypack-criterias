namespace QueryPack.Criterias
{
    using System.Collections.Generic;

    public enum OrderDirection
    {
        Asc = 1,
        Desc
    }
    
    public interface IOrder
    {
        Dictionary<string, OrderDirection> OrderBy { get; set; }
    }
}