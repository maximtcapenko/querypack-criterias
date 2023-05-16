namespace QueryPack.Criterias
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Order Builder
    /// </summary>
    public interface IOrderBuilder<TModel>
       where TModel : class
    {
        /// <summary>
        /// Strict binding of ordering arguments
        /// </summary>
        IRestriction<TModel> From(Func<TModel, OrderDirection> directionFactory);

        /// <summary>
        /// Auto mapping of ordering arguments
        /// </summary>
        IRestriction<TModel> From(Func<TModel, Dictionary<string, OrderDirection>> directionFactory);
    }
}