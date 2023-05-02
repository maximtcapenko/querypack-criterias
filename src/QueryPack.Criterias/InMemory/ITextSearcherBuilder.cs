namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Linq.Expressions;

    public interface ITextSearcherBuilder<TEntity> where TEntity : class
    {
        ///<summary>
        /// Includes field in text search
        ///</summary>
        IRestriction<TEntity> IncludeField<TProperty>(Expression<Func<TEntity, TProperty>> criteria);
    }
}