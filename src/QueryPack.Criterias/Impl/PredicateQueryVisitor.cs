using System.Linq;

namespace QueryPack.Criterias.Impl
{
    using Query;

    internal class PredicateQueryVisitor<TEntity, TModel> : IQueryVisitor<TEntity> 
        where TEntity : class
        where TModel : class
    {
        private readonly TModel _queryModel;
        private readonly ICriteriaBuilder<TEntity, TModel> _criteriaBuilder;

        public PredicateQueryVisitor(TModel queryModel, ICriteriaBuilder<TEntity, TModel> criteriaBuilder)
        {
            _criteriaBuilder = criteriaBuilder;
            _queryModel = queryModel;
        }

        public IQueryable<TEntity> Visit(IQueryable<TEntity> query)
        {
            var predicate = _criteriaBuilder.Build(_queryModel);
            return query.Where(predicate);
        }
    }
}