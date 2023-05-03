namespace QueryPack.Criterias.ImMemory.Ordering.Impl
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public class EnumNameComparerFactory : IComparerFactory
    {
        public bool CanCreate(Type param) => param.IsEnum;

        public IComparer<TProperty> GetComparer<TProperty>() => new EnumComparer<TProperty>();

        class EnumComparer<TProperty> : IComparer<TProperty>
        {
            public int Compare(TProperty left, TProperty right)
            {
                var type = typeof(TProperty);
                var fieldLeft = type.GetField(left.ToString(), BindingFlags.Static | BindingFlags.Public);
                var fieldRight = type.GetField(right.ToString(), BindingFlags.Static | BindingFlags.Public);
                var leftValue = fieldLeft.GetCustomAttribute<DescriptionAttribute>()?.Description ?? left.ToString();
                var rightValue = fieldRight.GetCustomAttribute<DescriptionAttribute>()?.Description ?? right.ToString();

                return string.Compare(leftValue, rightValue);
            }
        }
    }
}