namespace QueryPack.Criterias.ImMemory.Ordering
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Create instance of comparer used in order operation
    /// </summary>
    public interface IComparerFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IComparer{TProperty}"/>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        IComparer<TProperty> GetComparer<TProperty>();
        /// <summary>
        /// Validate if factory is able to create comparer for given type
        /// </summary>
        bool CanCreate(Type param);
    }
}