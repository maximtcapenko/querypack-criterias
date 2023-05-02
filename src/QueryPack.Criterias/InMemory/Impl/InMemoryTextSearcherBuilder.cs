namespace QueryPack.Criterias.ImMemory.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Criterias.Extensions;

    internal class InMemoryTextSearcherBuilder<TEntity> : ITextSearcherBuilder<TEntity>
           where TEntity : class
    {
        private readonly List<ICriteriaBuilder<TEntity>> _criteriaBuilders = new List<ICriteriaBuilder<TEntity>>();

        public IRestriction<TEntity> IncludeField<TProperty>(Expression<Func<TEntity, TProperty>> criteria)
        {
            var criteriaInstance = new InternalCriteriaBuilder<TProperty>(criteria);
            _criteriaBuilders.Add(criteriaInstance);

            return criteriaInstance;
        }

        public IEnumerable<TEntity> BuildAndSearch(ITextSearch query, IEnumerable<TEntity> entities)
        {
            if (!_criteriaBuilders.Any()) return entities;

            var criteria = _criteriaBuilders[0].Build(query);

            for (int i = 1; i < _criteriaBuilders.Count; i++)
                criteria = criteria.Or(_criteriaBuilders[i].Build(query));

            return entities.AsQueryable().Where(criteria);
        }

        private class InternalCriteriaBuilder<TProperty> : IRestriction<TEntity>, ICriteriaBuilder<TEntity>
        {
            private static readonly MethodInfo LikeMethodInfo = typeof(StringExtensions).GetMethod(nameof(StringExtensions.Like));

            private readonly Expression<Func<TEntity, TProperty>> _criteria;
            private Expression<Func<TEntity, bool>> _validator;

            public InternalCriteriaBuilder(Expression<Func<TEntity, TProperty>> criteria)
            {
                _criteria = criteria;
            }

            public void When(Expression<Func<TEntity, bool>> condition)
            {
                _validator = condition;
            }

            public Expression<Func<TEntity, bool>> Build(ITextSearch query)
            {
                var searchExpression = Expression.Constant($"{query.TextSearch}");
                var parameter = _criteria.Parameters.First();

                Expression expression = null;
                foreach (var searchPropExpression in _criteria.GetPropertyExpressions())
                {
                    var likeMethod = LikeMethodInfo.MakeGenericMethod((searchPropExpression.Member as PropertyInfo).PropertyType);
                    var likeCallExpression = Expression.Call(likeMethod, searchPropExpression, searchExpression);
                    expression = expression == null ? likeCallExpression as Expression : Expression.Or(expression, likeCallExpression);
                }

                if (expression == null) return default;

                if (_validator != null)
                {
                    var call = Expression.Invoke(_validator, parameter);
                    var checkExpression = Expression.Equal(call, Expression.Constant(true));
                    var returnTarget = Expression.Label(typeof(bool));
                    expression = Expression.IfThen(checkExpression, Expression.Return(returnTarget, expression, typeof(bool)));
                    expression = Expression.Block(expression, Expression.Label(returnTarget, Expression.Default(typeof(bool))));
                }
                var result = Expression.Lambda<Func<TEntity, bool>>(expression, new[] { parameter });

                return result;
            }
        }
    }
}