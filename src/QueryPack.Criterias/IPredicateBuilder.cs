namespace QueryPack.Criterias
{
    using System;
    using System.Linq.Expressions;

    ///<summary>
    /// Criteria Builder
    ///</summary>
    public interface ICriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        ///<summary>
        /// Initializes criteria builder pipeline
        ///</summary>
        ICriteriaConfigurator<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        ///<summary>
        /// Builds predicate based on search model
        ///</summary>
        Expression<Func<TEntity, bool>> Build(TModel search);
    }
    ///<summary>
    /// Restriction
    ///</summary>
    public interface IRestriction<TModel>
         where TModel : class
    {
        ///<summary>
        /// Validates restriction
        ///</summary>
        void When(Expression<Func<TModel, bool>> validation);
    }

    ///<summary>
    /// Criteria Configurator
    ///</summary>
    public interface ICriteriaConfigurator<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="criteriaFactory"></param>
        /// <param name="restriction">Restriction configurator</param>
        /// <returns></returns>
        ICriteriaConfigurator<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="group">Group criterias configurator</param>
        /// <returns></returns>
        ICriteriaConfigurator<TEntity, TModel> And(Action<IGroupCriteriaConfigurator<TEntity, TModel>> group);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="criteriaFactory"></param>
        /// <param name="restriction">Restriction configurator</param>
        /// <returns></returns>
        ICriteriaConfigurator<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> criteriaFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="group">Group criterias configurator</param>
        /// <returns></returns>
        ICriteriaConfigurator<TEntity, TModel> Or(Action<IGroupCriteriaConfigurator<TEntity, TModel>> group);
    }

    ///<summary>
    /// Group Criteria Configurator
    ///</summary>
    public interface IGroupCriteriaConfigurator<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        ///<summary>
        /// Starts predicate builder pipeline
        ///</summary>
        IScopeCriteriaConfigurator<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }
    ///<summary>
    /// Scope Criteria Configurator
    ///</summary>
    public interface IScopeCriteriaConfigurator<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        /// <summary>
        /// And operand
        /// </summary>
        /// <param name="predicateFactory"></param>
        /// <param name="restriction">Group criterias configurator</param>
        IScopeCriteriaConfigurator<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
        /// <summary>
        /// Or operand
        /// </summary>
        /// <param name="predicateFactory"></param>
        /// <param name="restriction">Group criterias configurator</param>
        IScopeCriteriaConfigurator<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default);
    }
}
