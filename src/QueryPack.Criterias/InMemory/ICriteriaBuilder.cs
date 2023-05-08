namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// Builds predicate for in-memory text search
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal interface ICriteriaBuilder<TEntity> where TEntity : class
    {
        /// <summary>
        /// Builds predicate based on query <see cref="ITextSearch"/>
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Expression<Func<TEntity, bool>> Build(ITextSearch query);
    }
}