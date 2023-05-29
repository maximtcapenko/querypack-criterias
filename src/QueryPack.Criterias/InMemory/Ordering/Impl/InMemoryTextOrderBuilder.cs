namespace QueryPack.Criterias.ImMemory.Ordering.Impl
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Criterias.Extensions;

    internal class InMemoryTextOrderBuilder<TEntity> where TEntity : class
    {
        private static readonly MethodInfo _setOrderMethod =
            typeof(ExpressionsExtension).GetMethod(nameof(ExpressionsExtension.SetInMemoryOrder), BindingFlags.Public | BindingFlags.Static);
        private static ConcurrentDictionary<Expression, Delegate> _setOrderCache = new ConcurrentDictionary<Expression, Delegate>();

        private readonly OrderOptions _orderOptions;

        public InMemoryTextOrderBuilder() { }

        public InMemoryTextOrderBuilder(OrderOptions orderOptions)
        {
            _orderOptions = orderOptions;
        }

        public IEnumerable<TEntity> Build(IEnumerable<TEntity> entities, IOrder query)
        {
            if (query.OrderBy == null || !query.OrderBy.Any())
                return entities;

            var result = entities.AsQueryable();
            var parameterExpression = Expression.Parameter(typeof(TEntity));
            foreach (var item in query.OrderBy.OrderBy(e => e.Value))
            {
                string path = item.Key;

                path = ResolveInternal(item.Key);
                if (string.IsNullOrEmpty(path))
                    continue;

                (var propertyExpression, var propertyType) = parameterExpression.GetPropertyExpressionAndTypeFromPath(path);

                var compiledMethod = _setOrderCache.GetOrAdd(propertyExpression, (expression) =>
                {
                    var generic = _setOrderMethod.MakeGenericMethod(typeof(TEntity), propertyType);
                    return MethodFactory.CreateGenericMethod<IQueryable<TEntity>>(generic);
                });

                result = ((Func<object, object[], IQueryable<TEntity>>)compiledMethod)(this, new object[] { result, parameterExpression, propertyExpression, _orderOptions, item.Value  });
            }

            return result;
        }

        protected virtual string ResolveInternal(string propertyName) => propertyName;
    }

    internal class InMemoryTextOrderBuilder<TEntity, TProperty> : InMemoryTextOrderBuilder<TEntity>
        where TEntity : class
    {
        private readonly Expression<Func<TEntity, TProperty>> _allowedProperties;
        private readonly IEnumerable<MemberNames> _allowedMemberNames;

        public InMemoryTextOrderBuilder(Expression<Func<TEntity, TProperty>> allowedProperties)
        {
            _allowedProperties = allowedProperties;
            _allowedMemberNames = _allowedProperties.ResolveMemberNames();
        }

        protected override string ResolveInternal(string propertyName)
        {
            var obj = _allowedMemberNames.FirstOrDefault(m => m.ProjectedName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            if (obj == null) return propertyName;

            return obj.OriginalName;
        }
    }
}