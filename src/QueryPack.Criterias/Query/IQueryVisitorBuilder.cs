namespace QueryPack.Criterias.Query
{
    public interface IQueryVisitorBuilder<TEntity, TModel> 
        where TEntity : class
        where TModel : class
    {
        IQueryVisitor<TEntity> GetVisitor(TModel queryModel);
    }
}