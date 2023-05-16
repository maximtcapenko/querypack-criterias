namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Text Searcher Builder
    /// </summary>
    public interface ITextSearcherBuilder<TEntity> where TEntity : class
    {
        ///<summary>
        /// Includes field in text search
        ///</summary>
        IRestriction<TEntity> IncludeField<TProperty>(Expression<Func<TEntity, TProperty>> criteria);
    }
}