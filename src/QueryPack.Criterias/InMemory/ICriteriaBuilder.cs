namespace QueryPack.Criterias.ImMemory
{
    using System;
    using System.Linq.Expressions;

    internal interface ICriteriaBuilder<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, bool>> Build(ITextSearch query);
    }
}