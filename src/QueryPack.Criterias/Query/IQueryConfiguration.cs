namespace QueryPack.Criterias.Query
{
    using System;

    public interface IQueryConfiguration<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
         void Configure(IQueryCriteriaBuilder<TEntity, TModel> builder);
    }
}