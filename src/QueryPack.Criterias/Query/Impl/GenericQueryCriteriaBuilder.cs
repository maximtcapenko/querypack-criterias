namespace QueryPack.Criterias.Query.Impl
{
    using System.Collections.Generic;
    using System.Linq;

    public class GenericQueryCriteriaBuilder<TEntity, TModel> : IQueryCriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        private readonly AggregateQueryVisitorBuilder _internalQueryVisitorBuilder = new AggregateQueryVisitorBuilder();

        public IQueryVisitorBuilder<TEntity, TModel> GetQueryVisitorBuilder() => _internalQueryVisitorBuilder;

        public IQueryCriteriaBuilder<TEntity, TModel> Add(IQueryVisitorBuilder<TEntity, TModel> queryVisitorBuilder)
        {
            _internalQueryVisitorBuilder.Add(queryVisitorBuilder);
            return this;
        }

        class AggregateQueryVisitorBuilder: IQueryVisitorBuilder<TEntity, TModel>
        {
            private readonly List<IQueryVisitorBuilder<TEntity, TModel>> _queryVisitorBuilders = new List<IQueryVisitorBuilder<TEntity, TModel>>();

            public IQueryVisitor<TEntity> GetVisitor(TModel queryModel)
                => new AggregateQueryVisitor(queryModel, _queryVisitorBuilders);

            public void Add(IQueryVisitorBuilder<TEntity, TModel> visitorBuilder)
            {
                _queryVisitorBuilders.Add(visitorBuilder);
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
}