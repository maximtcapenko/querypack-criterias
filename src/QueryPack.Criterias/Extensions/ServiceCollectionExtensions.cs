namespace QueryPack.Criterias.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Query;
    using Query.Impl;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="ICriteriaBuilder{,}"/> and <see cref="IQueryVisitorBuilder{,}"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddCriteriaBuilders(this ServiceCollection services, params Assembly[] assemblies)
        {
            var registerCriteriaBuilderMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(RegisterCriteriaBuilder), BindingFlags.Static | BindingFlags.NonPublic);
            var registerQueryCriteriaMethod = typeof(ServiceCollectionExtensions).GetMethod(nameof(RegisterQueryConfiguration), BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var serviceType in assemblies.SelectMany(e => e.GetTypes()))
            {
                ReflectionUtils.ProcessGenericInterfaceImpls(serviceType, typeof(ICriteriaBuilder<,>), (@interface, implementation, name) =>
                {
                    if (implementation.IsGenericType)
                        return;

                    var genericMethod = registerCriteriaBuilderMethod.MakeGenericMethod(@interface.GetGenericArguments());
                    genericMethod.Invoke(null, new object[] { services, implementation });
                });

                ReflectionUtils.ProcessGenericInterfaceImpls(serviceType, typeof(IQueryConfiguration<,>), (@interface, implementation, name) =>
                {
                    if (implementation.IsGenericType)
                        return;

                    var genericMethod = registerQueryCriteriaMethod.MakeGenericMethod(@interface.GetGenericArguments());
                    genericMethod.Invoke(null, new object[] { services, implementation });
                });
            }
            return services;
        }

        private static void RegisterCriteriaBuilder<TEntity, TSearch>(ServiceCollection services, Type implementation)
            where TEntity : class
            where TSearch : class
        {
            services.AddSingleton(typeof(ICriteriaBuilder<TEntity, TSearch>), implementation);
        }

        private static void RegisterQueryConfiguration<TEntity, TSearch>(ServiceCollection services, Type implementation)
            where TEntity : class
            where TSearch : class
        {
            var configuration = Activator.CreateInstance(implementation) as IQueryConfiguration<TEntity, TSearch>;

            var builder = new GenericQueryCriteriaBuilder<TEntity, TSearch>();
            configuration.Configure(builder);

            var visitorBuilder = builder.GetQueryVisitorBuilder();
            services.AddSingleton(typeof(IQueryVisitorBuilder<TEntity, TSearch>), visitorBuilder);
        }
    }
}