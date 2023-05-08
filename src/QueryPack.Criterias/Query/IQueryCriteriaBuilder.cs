namespace QueryPack.Criterias.Query
{
    using System;
    using System.Linq.Expressions;

    public interface IQueryCriteriaBuilder<TEntity, TModel>
        where TEntity : class 
        where TModel : class
    {
        ///<summary>
        /// Adds a custom defined statement to the query visitors pipeline
        ///</summary>
        IQueryCriteriaBuilder<TEntity, TModel> Add(IQueryVisitorBuilder<TEntity, TModel> queryVisitorBuilder);

    }
}