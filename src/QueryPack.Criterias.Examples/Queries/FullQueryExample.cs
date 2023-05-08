namespace QueryPack.Criterias.Examples.Queries
{
    using Query;
    using QueryPack.Criterias.Query.Impl;

    internal class FullQueryExample
    {
        class UserQuery : IOrder
        {
            public string UserName { get; set; }
            public Dictionary<string, OrderDirection> OrderBy { get; set; }
        }

        enum Role
        {
            SuperUser,
            Admin,
            User
        }

        class User
        {
            static Random rundomizer = new Random();
            static string[] names = { "mike", "daren", "lee", "david" };
            public string Email { get; set; }
            public string Name { get; set; }
            public Role Role { get; set; }

            public static User New()
             => new User
             {
                 Name = names.OrderBy(e => rundomizer.Next()).First(),
                 Email = "",
                 Role = Enum.GetValues<Role>().OrderBy(e => rundomizer.Next()).First()
             };

            public override string ToString() => $"{Name}:{Email}-{Role}";
        }
        public void Run()
        {
            var queryModel = new UserQuery
            {
                UserName = "mike",
                OrderBy = new Dictionary<string, OrderDirection>
                {
                    ["Role"] = OrderDirection.Desc
                }
            };
            var configuration = new UserQueryConfiguration();
            var builder = new GenericQueryCriteriaBuilder<User, UserQuery>();
            configuration.Configure(builder);
            var visitorBuilder = builder.GetQueryVisitorBuilder();

            var visior = visitorBuilder.GetVisitor(queryModel);

            var users = Enumerable.Range(0, 20).Select(e => User.New()).ToList();

            var query = visior.Visit(users.AsQueryable());
            Console.WriteLine(query);
            var results = query.ToList();
        }

        class UserQueryConfiguration : IQueryConfiguration<User, UserQuery>
        {
            public void Configure(IQueryCriteriaBuilder<User, UserQuery> builder)
            {
                builder.AddPredicate(m => e => e.Name.StartsWith(m.UserName),
                    r => r.When(m => !string.IsNullOrEmpty(m.UserName)));

                builder.AddOrder(e => new { e.Name, e.Role }, builder => builder.From(e => e.OrderBy));
            }
        }
    }
}