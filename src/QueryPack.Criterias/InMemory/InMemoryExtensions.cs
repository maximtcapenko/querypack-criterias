namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Impl;
    using Ordering;
    using Ordering.Impl;

    ///<summary>
    /// In-Memory Extensions
    ///</summary>
    public static class InMemoryExtensions
    {
        ///<summary>
        /// In memory search by user defined criterias
        ///</summary>
        public static IEnumerable<TEntity> UseInMemoryTextSearchBy<TEntity>(this IEnumerable<TEntity> entities,
            Action<ITextSearcherBuilder<TEntity>> builder, ITextSearch query)
            where TEntity : class
        {
            var builderInstance = new InMemoryTextSearcherBuilder<TEntity>();
            builder(builderInstance);

            return builderInstance.BuildAndSearch(query, entities);
        }
        ///<summary>
        /// In memory order by user defined criterias
        ///</summary>
        public static IEnumerable<TEntity> UseInMemoryOrderBy<TEntity>(this IEnumerable<TEntity> entities, IOrder query, Action<OrderOptions> optionsConfigurer = null)
            where TEntity : class
        {
            var orderOptions = new OrderOptions();
            optionsConfigurer?.Invoke(orderOptions);

            var builderInstance = new InMemoryTextOrderBuilder<TEntity>(orderOptions);

            return builderInstance.Build(entities, query);
        }
        /// <summary>
        /// In memory order by user defined criterias
        /// </summary>
        public static IEnumerable<TEntity> UseInMemoryOrderBy<TEntity, TProperty>(this IEnumerable<TEntity> entities, IOrder query, Expression<Func<TEntity, TProperty>> allowedProperties)
         where TEntity : class
        {
            var builderInstance = new InMemoryTextOrderBuilder<TEntity, TProperty>(allowedProperties);

            return builderInstance.Build(entities, query);
        }
    }
}