namespace QueryPack.Criterias.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Member names container
    /// </summary>
    public class MemberNames
    {
        public MemberNames(string projectedName, string originalName)
        {
            ProjectedName = projectedName;
            OriginalName = originalName;
        }

        public string ProjectedName { get; }
        public string OriginalName { get; }
    }
    /// <summary>
    /// Expressions Extension
    /// </summary>
    public static class ExpressionsExtension
    {
        /// <summary>
        /// And operand builder
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> arg)
        {
            if (self == null)
                return arg;

            var parameter = self.Parameters[0];
            var visitor = new SubstExpressionVisitor(parameter);

            var body = Expression.And(self.Body, visitor.Visit(arg.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        /// <summary>
        /// Or operand builder
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> arg)
        {
            if (self == null)
                return arg;

            var parameter = self.Parameters[0];

            var visitor = new SubstExpressionVisitor(parameter);
            var body = Expression.Or(self.Body, visitor.Visit(arg.Body));

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static (Expression expression, Type type) GetPropertyExpressionAndTypeFromPath(this ParameterExpression self, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Invalid empty member path", nameof(path));

            // build all successive levels: e.X, e.X.Y, ...
            Expression memberExpression = null;
            var memberExpressions = new List<Expression>();
            foreach (var member in path.Split('.'))
            {
                memberExpression = memberExpression == null
                    ? Expression.PropertyOrField(self, member)
                    : Expression.PropertyOrField(memberExpression, member);

                memberExpressions.Add(memberExpression);
            }

            // use last level to get nullable type
            var memberType = (memberExpression as MemberExpression).Member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => throw new NotSupportedException()
            };

            // shortcut if only one level
            if (memberExpressions.Count == 1)
                return (memberExpression, memberType);

            var nullableMemberType = ReflectionUtils.GetNullableType(memberType);
            var nullExpression = Expression.Constant(null);
            var resultExpression = Expression.Variable(nullableMemberType, "result");
            var initResultExpression = Expression.Assign(resultExpression, Expression.Convert(nullExpression, nullableMemberType));

            // now backwards build checks like e.X == null ? null : e.X.Y ...
            memberExpressions.Reverse();
            for (int i = 1; i < memberExpressions.Count; ++i)
            {
                var currMemberExpr = memberExpressions[i];
                memberExpression = Expression.IfThenElse(
                    Expression.Equal(currMemberExpr, nullExpression),
                    Expression.Assign(resultExpression, Expression.Convert(nullExpression, nullableMemberType)),
                    memberExpression is ConditionalExpression
                        ? memberExpression
                        : Expression.Assign(resultExpression, Expression.Convert(memberExpression, nullableMemberType))
                );
            }

            var expression = Expression.Block(nullableMemberType, new[] { resultExpression }, initResultExpression, memberExpression, resultExpression);
            return (expression, nullableMemberType);
        }

        /// <summary>
        /// Resolves all member expressions from PropertyExpression or NewExpression 
        /// </summary>
        public static IEnumerable<MemberExpression> GetPropertyExpressions<T>(this Expression<T> self)
        {
            if (self.Body is NewExpression newExpression)
                foreach (var arg in newExpression.Arguments.OfType<MemberExpression>())
                    yield return arg;
            else
                yield return self.Body as MemberExpression;
        }

        public static IEnumerable<MemberNames> ResolveMemberNames<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> self)
        {
            if (self is null)
                yield break;

            if (self.Body is NewExpression newExpression)
            {
                for (int i = 0; i < newExpression.Members.Count; ++i)
                {
                    if (ResolveMemberExpression(newExpression.Arguments[i]) is MemberExpression memberExpression)
                        yield return new MemberNames(newExpression.Members[i].Name, ResolveMemberPath(memberExpression));
                }
            }
            else if (ResolveMemberExpression(self) is MemberExpression memberExpression)
            {
                var path = ResolveMemberPath(memberExpression);
                yield return new MemberNames(path, path);
            }
        }

        public static string ResolveMemberPath(MemberExpression arg)
        {
            var path = new List<string>();
            do
            {
                var memberName = arg.Member.Name;
                path.Add(memberName);
                arg = arg.Expression as MemberExpression;
            }
            while (arg != null);

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

        private static Expression ResolveMemberExpression(Expression arg)
            => arg switch
            {
                UnaryExpression ue when ue.NodeType == ExpressionType.Convert => ue.Operand,
                MemberExpression me => me,
                _ => null
            };
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
