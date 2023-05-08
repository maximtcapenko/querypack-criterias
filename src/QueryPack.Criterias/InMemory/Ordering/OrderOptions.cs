namespace QueryPack.Criterias.ImMemory.Ordering
{
    using System.Collections.Generic;
    
    public class OrderOptions
    {
        public List<IComparerFactory> OrderCompares { get; } = new List<IComparerFactory>();
        ///<summary>
        ///Adds comparer factory <see cref="IComparerFactory"/>
        ///</summary>
        ///<param name="comparerFactory"></param>
        public void AddComparerFactory(IComparerFactory comparerFactory)
        {
            OrderCompares.Add(comparerFactory);
        }
    }
}