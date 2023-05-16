namespace QueryPack.Criterias.Query
{
    /// <summary>
    /// Query Configuration
    /// </summary>
    public interface IQueryConfiguration<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// Configures query builder <see cref="IQueryCriteriaBuilder{Entity, TModel}"/>
        /// </summary>
        /// <param name="builder"></param>
        void Configure(IQueryCriteriaBuilder<TEntity, TModel> builder);
    }
}