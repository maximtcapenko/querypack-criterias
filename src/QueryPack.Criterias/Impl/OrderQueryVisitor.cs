namespace QueryPack.Criterias.Impl
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;
    using Query;

    internal class OrderQueryVisitor<TEntity, TProperty, TModel> : IQueryVisitor<TEntity> where TEntity : class
    {
        private readonly Expression<Func<TEntity, TProperty>> _property;
        private readonly TModel _queryModel;
        private readonly Func<TModel, bool> _validator;
        private readonly Func<TModel, Dictionary<string, OrderDirection>> _directionFactory;

        private static ConcurrentDictionary<MemberExpression, Delegate> _setOrderCache = new ConcurrentDictionary<MemberExpression, Delegate>();
        private static  MethodInfo _setOrderMethod = typeof(ExpressionsExtension).GetMethod(nameof(ExpressionsExtension.SetOrder), BindingFlags.Static | BindingFlags.Public);

        public OrderQueryVisitor(TModel queryModel, Expression<Func<TEntity, TProperty>> property, Func<TModel, bool> validator,
            Func<TModel, Dictionary<string, OrderDirection>> directionFactory)
        {
            _directionFactory = directionFactory;
            _queryModel = queryModel;
            _property = property;
            _validator = validator;
        }

        public IQueryable<TEntity> Visit(IQueryable<TEntity> query)
        {
            var validVisit = true;

            if (_validator != null)
                validVisit = _validator(_queryModel);

            if (validVisit)
            {
                var container = _directionFactory(_queryModel);
                var paramExpression = _property.Parameters.First();

                foreach (var expression in _property.GetPropertyExpressions())
                {
                    var path = ExpressionsExtension.ResolveMemberPath(expression);
                    if (container.TryGetValue(path, out var direction))
                    {
                        var compiledMethod = _setOrderCache.GetOrAdd(expression, (expression) =>
                        {
                            var generic = _setOrderMethod.MakeGenericMethod(typeof(TEntity), (expression.Member as PropertyInfo).PropertyType);
                            return MethodFactory.CreateGenericMethod<IQueryable<TEntity>>(generic);
                        });

                        query = ((Func<object, object[], IQueryable<TEntity>>)compiledMethod)(null, new object[] { query, paramExpression, expression, direction });
                    }
                }
            }

            return query;
        }
    }
}