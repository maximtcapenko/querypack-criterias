namespace QueryPack.Criterias
{
    using System;
    using System.Linq.Expressions;

    public interface ICriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        ///<summary>
        /// Initializes criteria builder pipeline
        ///</summary>
        ICriteriaConfigurer<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        ///<summary>
        /// Builds predicate based on search model
        ///</summary>
        Expression<Func<TEntity, bool>> Build(TModel search);
    }

    public interface IRestriction<TModel>
         where TModel : class
    {
        ///<summary>
        /// Validates restriction
        ///</summary>
        void When(Expression<Func<TModel, bool>> validation);
    }

    public interface ICriteriaConfigurer<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="criteriaFactory"></param>
        /// <param name="restriction">Restriction configurer</param>
        /// <returns></returns>
        ICriteriaConfigurer<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="group">Group criterias configurer</param>
        /// <returns></returns>
        ICriteriaConfigurer<TEntity, TModel> And(Action<IGroupCriteriaConfigurer<TEntity, TModel>> group);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="criteriaFactory"></param>
        /// <param name="restriction">Restriction configurer</param>
        /// <returns></returns>
        ICriteriaConfigurer<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="group">Group criterias configurer</param>
        /// <returns></returns>
        ICriteriaConfigurer<TEntity, TModel> Or(Action<IGroupCriteriaConfigurer<TEntity, TModel>> group);
    }

    public interface IGroupCriteriaConfigurer<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        IScopeCriteriaConfigurer<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }

    public interface IScopeCriteriaConfigurer<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="predicateFactory"></param>
        /// <param name="restriction">Group criterias configurer</param>
        /// <returns></returns>
        IScopeCriteriaConfigurer<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="predicateFactory"></param>
        /// <param name="restriction">Group criterias configurer</param>
        /// <returns></returns>
        IScopeCriteriaConfigurer<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }
}
