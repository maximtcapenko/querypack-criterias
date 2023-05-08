namespace QueryPack.Criterias.Examples.Predicates
{
    using System.Collections.Generic;
    using Impl;

    internal class ExpressionOrderExample
    {
        class OrderUsers : IOrder
        {
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
            var queryModel = new OrderUsers
            {
                OrderBy = new Dictionary<string, OrderDirection>
                {
                    ["Name"] = OrderDirection.Asc,
                    ["Email"] = OrderDirection.Desc
                }
            };

            var users = Enumerable.Range(0, 20).Select(e => User.New()).ToList();
            users.ForEach(u => Console.WriteLine(u));
            var query = users.AsQueryable();

            var orderBuilder = new OrderQueryVisitorBuilder<User, OrderUsers>();
            orderBuilder.AddOrder(e => new { e.Name, e.Role }, 
                config => config.From(e => e.OrderBy));

            var visitor = orderBuilder.GetVisitor(queryModel);
            query = visitor.Visit(query);

            Console.WriteLine(query);
        }
    }
}