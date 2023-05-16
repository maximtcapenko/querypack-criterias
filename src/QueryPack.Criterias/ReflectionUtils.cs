namespace QueryPack.Criterias
{
    using System;
    using System.Linq;

    /// <summary>
    /// Reflection Utils
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Applies action to classes which implement defined interface
        /// </summary>
        public static void ProcessGenericInterfaceImpls(Type serviceType, Type interfaceType, Action<Type, Type, string> action)
        {
            if (serviceType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType))
            {
                var types = serviceType.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);

                foreach (var type in types)
                {
                    action(type, serviceType, null);
                }
            }
        }
        /// <summary>
        /// Gets nullable type
        /// </summary>
        public static Type GetNullableType(Type type)
            => type switch
            {
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) => t,
                Type t when t.IsClass => t,
                Type t => typeof(Nullable<>).MakeGenericType(t)
            };
    }
}