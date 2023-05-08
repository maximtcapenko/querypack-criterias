namespace QueryPack.Criterias.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using Query;

    public class OrderQueryVisitorBuilder<TEntity, TModel> : IQueryVisitorBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        private IQueryVisitorBuilder<TEntity, TModel> _queryVisitorBuilder;

         public void AddOrder<TProperty>(Expression<Func<TEntity, TProperty>> property,
          Action<IOrderBuilder<TModel>> configureDelegate)
        {
            var builder = new InnerOrderVisitorBuilder<TProperty>(property);
            configureDelegate?.Invoke(builder);
            _queryVisitorBuilder = builder;
        }

        public IQueryVisitor<TEntity> GetVisitor(TModel model) => _queryVisitorBuilder?.GetVisitor(model);
        
        class InnerOrderVisitorBuilder<TProperty> : 
            IOrderBuilder<TModel>, IRestriction<TModel>,  IQueryVisitorBuilder<TEntity, TModel>
        {
            private readonly Expression<Func<TEntity, TProperty>> _property;
            private Func<TModel, Dictionary<string, OrderDirection>> _directionFactory;
            private TModel _queryModel;
            private Func<TModel, bool> _innerValidate = e => true;
            private Func<TModel, bool> _validator;

            public InnerOrderVisitorBuilder(Expression<Func<TEntity, TProperty>> property)
            {
                _property = property;
            }

            public IRestriction<TModel> From(Func<TModel, OrderDirection> directionFactory)
            {
                _directionFactory = (query) => _property.ResolveMemberNames()
                .ToDictionary(e => e.OriginalName, e => directionFactory(query));
                return this;
            }

            public IRestriction<TModel> From(Func<TModel, Dictionary<string, OrderDirection>> direction)
            {
                _innerValidate = (query) =>
                {
                    var result = direction(query);
                    var paths = _property.ResolveMemberNames();

                    return result.Any(e => paths.Any(p => e.Key.Equals(p.OriginalName, 
                        StringComparison.CurrentCultureIgnoreCase)));
                };

                _directionFactory = (query) => direction(query);

                return this;
            }

            public IQueryVisitor<TEntity> GetVisitor(TModel queryModel)
                 => new OrderQueryVisitor<TEntity, TProperty, TModel>(queryModel, _property, _validator, _directionFactory);

            public void When(Expression<Func<TModel, bool>> condition)
                 => _validator = (query) => condition.Compile()(query) && _innerValidate(query);
        }
    }
}