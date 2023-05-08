namespace QueryPack.Criterias.Query
{
    using System;
    using System.Linq.Expressions;

    public interface IQueryCriteriaBuilder<TEntity, TModel>
        where TEntity : class 
        where TModel : class
    {
        ICriteriaConfigurer<TEntity, TModel> AddPredicate(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
 
        IQueryCriteriaBuilder<TEntity, TModel> AddOrder<TProperty>(Expression<Func<TEntity, TProperty>> property,
          Action<IOrderBuilder<TModel>> orderBuilder);
    }
}