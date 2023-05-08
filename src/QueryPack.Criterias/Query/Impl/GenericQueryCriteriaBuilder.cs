namespace QueryPack.Criterias.Query.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Criterias.Impl;

    public class GenericQueryCriteriaBuilder<TEntity, TModel> : IQueryCriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        private readonly AggregateQueryVisitorBuilder _internalQueryVisitorBuilder = new AggregateQueryVisitorBuilder();

        public IQueryVisitorBuilder<TEntity, TModel> GetQueryVisitorBuilder() => _internalQueryVisitorBuilder;

        public IQueryCriteriaBuilder<TEntity, TModel> AddOrder<TProperty>(Expression<Func<TEntity, TProperty>> property,
            Action<IOrderBuilder<TModel>> orderBuilder)
        {
            var orderQueryVisitorBuilder = new OrderQueryVisitorBuilder<TEntity, TModel>();
            orderQueryVisitorBuilder.AddOrder(property, orderBuilder);
            _internalQueryVisitorBuilder.Add(orderQueryVisitorBuilder);

            return this;
        }

        public ICriteriaConfigurer<TEntity, TModel> AddPredicate(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default)
        {
            var criteriaBuilder = new GenericCriteriaBuilder<TEntity, TModel>();
            var configurer = criteriaBuilder.With(criteriaFactory, restriction);
            _internalQueryVisitorBuilder.Add(new PredicateQueryVisitorBuilder<TEntity, TModel>(criteriaBuilder));

            return configurer;
        }


        class AggregateQueryVisitorBuilder : IQueryVisitorBuilder<TEntity, TModel>
        {
            private readonly List<IQueryVisitorBuilder<TEntity, TModel>> _queryVisitorBuilders = new List<IQueryVisitorBuilder<TEntity, TModel>>();

            public IQueryVisitor<TEntity> GetVisitor(TModel queryModel)
                => new AggregateQueryVisitor(queryModel, _queryVisitorBuilders);

            public AggregateQueryVisitorBuilder Add(IQueryVisitorBuilder<TEntity, TModel> visitorBuilder)
            {
                _queryVisitorBuilders.Add(visitorBuilder);
                return this;
            }
        }

        class AggregateQueryVisitor : IQueryVisitor<TEntity>
        {
            private readonly IEnumerable<IQueryVisitorBuilder<TEntity, TModel>> _queryVisitorBuilders;
            private readonly TModel _queryModel;

            public AggregateQueryVisitor(TModel queryModel,
                IEnumerable<IQueryVisitorBuilder<TEntity, TModel>> queryVisitorBuilders)
            {
                _queryVisitorBuilders = queryVisitorBuilders;
                _queryModel = queryModel;
            }

            public IQueryable<TEntity> Visit(IQueryable<TEntity> query)
            {
                foreach (var builder in _queryVisitorBuilders)
                {
                    var visitor = builder.GetVisitor(_queryModel);
                    query = visitor.Visit(query);
                }

                return query;
            }
        }
    }
}