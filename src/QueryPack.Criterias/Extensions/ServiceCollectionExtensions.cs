namespace QueryPack.Criterias.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCriteriaBuilders(this ServiceCollection services, params Assembly[] assemblies)
        {
            var method = typeof(ServiceCollectionExtensions).GetMethod(nameof(Register), BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var serviceType in assemblies.SelectMany(e => e.GetTypes()))
            {
                ReflectionUtils.ProcessGenericInterfaceImpls(serviceType, typeof(ICriteriaBuilder<,>), (@interface, implementation, name) =>
                {
                    var genericMethod = method.MakeGenericMethod(@interface.GetGenericArguments());
                    genericMethod.Invoke(null, new object[] { services, implementation});
                });
            }
            return services;
        }

        private static void Register<TEntity, TSearch>(ServiceCollection services, Type implementation)
            where TEntity : class
            where TSearch : class
        {
            services.AddSingleton(typeof(ICriteriaBuilder<TEntity, TSearch>), implementation);
        }
    }
}