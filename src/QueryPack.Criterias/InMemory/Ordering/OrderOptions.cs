namespace QueryPack.Criterias.ImMemory.Ordering
{
    using System.Collections.Generic;
    
    public class OrderOptions
    {
        public List<IComparerFactory> OrderCompares { get; } = new List<IComparerFactory>();

        public void AddComparerFactory(IComparerFactory comparerFactory)
        {
            OrderCompares.Add(comparerFactory);
        }
    }
}