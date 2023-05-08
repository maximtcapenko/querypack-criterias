namespace QueryPack.Criterias.Query
{
    /// <summary>
    /// Creates query visitor builder
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryCriteriaBuilder<TEntity, TModel>
        where TEntity : class 
        where TModel : class
    {
        ///<summary>
        /// Adds a custom defined statement <see cref="IQueryVisitorBuilder{TEntity,TModel}"/> to the query visitors pipeline
        ///</summary>
        IQueryCriteriaBuilder<TEntity, TModel> Add(IQueryVisitorBuilder<TEntity, TModel> queryVisitorBuilder);
    }
}