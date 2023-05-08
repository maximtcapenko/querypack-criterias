namespace QueryPack.Criterias.Impl
{
    using System;
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
                var paramExpression = Expression.Parameter(typeof(TEntity), "e");
                var method = GetType().GetMethod(nameof(this.SetOrder), BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var expression in _property.GetPropertyExpressions())
                {
                    var path = ExpressionsExtension.ResolveMemberPath(expression);
                    if (container.TryGetValue(path, out var direction))
                    {
                        var generic = method.MakeGenericMethod((expression.Member as PropertyInfo).PropertyType);
                        query = (IQueryable<TEntity>)generic.Invoke(this, new object[] { paramExpression, expression, query, direction });
                    }
                }
            }

            return query;
        }

        private IQueryable<TEntity> SetOrder<TMember>(ParameterExpression param, MemberExpression member, IQueryable<TEntity> query, OrderDirection direction)
        {
            var property = Expression.Lambda<Func<TEntity, TMember>>(member, param);
            if (query.Expression.Type == typeof(IOrderedQueryable<TEntity>))
            {
                if (direction == OrderDirection.Desc)
                    return (query as IOrderedQueryable<TEntity>).ThenByDescending(property);
                else
                    return (query as IOrderedQueryable<TEntity>).ThenBy(property);
            }
            else
            {
                if (direction == OrderDirection.Desc)
                    return query.OrderByDescending(property);
                else
                    return query.OrderBy(property);
            }
        }
    }
}