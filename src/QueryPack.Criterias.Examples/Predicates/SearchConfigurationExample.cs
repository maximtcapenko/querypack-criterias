namespace QueryPack.Criterias.Examples.Predicates
{
    using Impl;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;

    internal class SearchConfigurationExample
    {
        class SearchUser
        {
            public string UserName { get; set; }

            public string UserEmail { get; set; }
        }

        class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        class UserSearchBuilder : GenericCriteriaBuilder<User, SearchUser>
        {
            public UserSearchBuilder()
            {
                With(m => e => e.Name.StartsWith(m.UserName),
                    r => r.When(m => !string.IsNullOrEmpty(m.UserName)));
            }
        }

        public void Run()
        {
            var services = new ServiceCollection();
            services.AddCriteriaBuilders(typeof(SearchConfigurationExample).Assembly);
            var provider = services.BuildServiceProvider();

            var searchBuilder = provider.GetRequiredService<ICriteriaBuilder<User, SearchUser>>();
            var expression = searchBuilder.Build(new SearchUser
            {
                UserName = "jhon"
            });

            Console.WriteLine($"Run example of {nameof(SearchConfigurationExample)}");
            Console.WriteLine(expression);
            Console.WriteLine("end");
        }
    }
}
