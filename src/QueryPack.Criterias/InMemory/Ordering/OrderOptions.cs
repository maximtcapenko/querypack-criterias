namespace QueryPack.Criterias.ImMemory.Ordering
{
    using System.Collections.Generic;

    ///<summary>
    /// Order Options
    ///</summary>
    public class OrderOptions
    {
        private readonly List<IComparerFactory> _orderCompares = new List<IComparerFactory>();

        ///<summary>
        /// Gets all registered comparers <see cref="IComparerFactory"/>
        ///</summary>
        public IEnumerable<IComparerFactory> OrderCompares => _orderCompares;

        ///<summary>
        ///Adds comparer factory <see cref="IComparerFactory"/>
        ///</summary>
        ///<param name="comparerFactory"></param>
        public void AddComparerFactory(IComparerFactory comparerFactory)
        {
            _orderCompares.Add(comparerFactory);
        }
    }
}