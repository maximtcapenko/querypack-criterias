namespace QueryPack.Criterias.Impl
{
    using Query;

    public class PredicateQueryVisitorBuilder<TEntity, TModel> : IQueryVisitorBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        private readonly ICriteriaBuilder<TEntity, TModel> _criteriaBuilder;

        public PredicateQueryVisitorBuilder(ICriteriaBuilder<TEntity, TModel> criteriaBuilder)
        {
            _criteriaBuilder = criteriaBuilder;
        }

        public IQueryVisitor<TEntity> GetVisitor(TModel queryModel)
            => new PredicateQueryVisitor<TEntity, TModel>(queryModel, _criteriaBuilder);
    }
}