namespace QueryPack.Criterias.ImMemory.Ordering
{
    using System;
    using System.Collections.Generic;

    public interface IComparerFactory
    {
        IComparer<TProperty> GetComparer<TProperty>();

        bool CanCreate(Type param);
    }
}