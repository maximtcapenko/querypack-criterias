namespace QueryPack.Criterias.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    public static class ExpressionsExtension
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;

            var parameter = first.Parameters[0];
            var visitor = new SubstExpressionVisitor(parameter);

            var body = Expression.And(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
                return second;

            var parameter = first.Parameters[0];

            var visitor = new SubstExpressionVisitor(parameter);            
            var body = Expression.Or(first.Body, visitor.Visit(second.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static string GetMemberPath(MemberExpression memberExpression, Func<string, string> transformer = null)
        {
            var path = new List<string>();

            do
            {
                var memberName = memberExpression.Member.Name;
                path.Add(transformer == null ? memberName : transformer(memberName));
                memberExpression = memberExpression.Expression as MemberExpression;
            }
            while (memberExpression != null);

            var sb = new StringBuilder();
            var i = path.Count - 1;
            for (; i > 0; --i)
            {
                sb.Append(path[i]);
                sb.Append('.');
            }
            sb.Append(path[i]);
            return sb.ToString();
        }
    }

    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public SubstExpressionVisitor(ParameterExpression parameterExpression)
        {
            _parameter = parameterExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }
}
