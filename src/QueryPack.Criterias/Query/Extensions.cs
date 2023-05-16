namespace QueryPack.Criterias.Query
{
    using System;
    using System.Linq.Expressions;
    using Criterias.Impl;

    ///<summary>
    /// Query CriteriaBuilder Extensions
    ///</summary>
    public static class QueryCriteriaBuilderExtensions
    {
        ///<summary>
        /// Adds an order statement to the query visitors pipeline
        ///</summary>
        public static IQueryCriteriaBuilder<TEntity, TModel> AddOrder<TEntity, TModel, TProperty>(this IQueryCriteriaBuilder<TEntity, TModel> self, Expression<Func<TEntity, TProperty>> property,
          Action<IOrderBuilder<TModel>> orderBuilder)
          where TEntity : class
          where TModel : class
        {
            var orderQueryVisitorBuilder = new OrderQueryVisitorBuilder<TEntity, TModel>();
            orderQueryVisitorBuilder.AddOrder(property, orderBuilder);
            self.Add(orderQueryVisitorBuilder);

            return self;
        }

        ///<summary>
        /// Adds a predicate statement to the query visitors pipeline
        ///</summary>
        public static ICriteriaConfigurator<TEntity, TModel> AddPredicate<TEntity, TModel>(this IQueryCriteriaBuilder<TEntity, TModel> self,
          Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
          Action<IRestriction<TModel>> restriction = default)
          where TEntity : class
          where TModel : class
        {
            var criteriaBuilder = new GenericCriteriaBuilder<TEntity, TModel>();
            var configurer = criteriaBuilder.With(criteriaFactory, restriction);
            self.Add(new PredicateQueryVisitorBuilder<TEntity, TModel>(criteriaBuilder));

            return configurer;
        }
    }
}