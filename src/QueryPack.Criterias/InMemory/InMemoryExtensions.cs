namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Collections.Generic;
    using Impl;

    public static class InMemoryExtensions
    {
        ///<summary>
        /// In memory search by user defined criteria
        ///</summary>
        public static IEnumerable<TEntity> UseInMemoryTextSearchBy<TEntity>(this IEnumerable<TEntity> entities, 
            Action<ITextSearcherBuilder<TEntity>> builder, ITextSearch query) 
            where TEntity : class
        {
            var builderInstance = new InMemoryTextSearcherBuilder<TEntity>();
            builder(builderInstance);

            return builderInstance.BuildAndSearch(query, entities);
        }
    }
}