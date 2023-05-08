
namespace QueryPack.Criterias.Query
{
    using System.Linq;

    public interface IQueryVisitor<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Visit(IQueryable<TEntity> query);
    }
}