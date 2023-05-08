
namespace QueryPack.Criterias.Query
{
    using System.Linq;
    /// <summary>
    /// Visits the given query and returns the modified query
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQueryVisitor<TEntity> where TEntity : class
    { 
        IQueryable<TEntity> Visit(IQueryable<TEntity> query);
    }
}