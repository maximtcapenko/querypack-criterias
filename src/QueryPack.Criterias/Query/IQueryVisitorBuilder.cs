namespace QueryPack.Criterias.Query
{
    /// <summary>
    /// Query visitor instance builder
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IQueryVisitorBuilder<TEntity, TModel> 
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// Creates an instance of <see cref="QueryPack.Criterias.Query.IQueryVisitor{TEntity}"/>
        /// </summary>
        /// <param name="queryModel">Search model</param>
        IQueryVisitor<TEntity> GetVisitor(TModel queryModel);
    }
}