
namespace QueryPack.Criterias.Query
{
    using System.Linq;
    /// <summary>
    /// Query Visitor
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQueryVisitor<TEntity> where TEntity : class
    {
        /// <summary>
        /// Visits the given query and returns the modified query
        /// </summary>
         IQueryable<TEntity> Visit(IQueryable<TEntity> query);
    }
}