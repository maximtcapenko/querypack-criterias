namespace QueryPack.Criterias.Examples.Predicates
{
    using System.Collections.Generic;
    using ImMemory;
    using ImMemory.Ordering.Impl;

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

    internal class InMemoryOrderExample
    {
        public void Run()
        {
            var query = new OrderUsers
            {
                OrderBy = new Dictionary<string, OrderDirection>
                {
                    ["name"] = OrderDirection.Asc,
                    ["role"] = OrderDirection.Desc
                }
            };

            var users = Enumerable.Range(0, 20).Select(e => User.New()).ToList();
            users.ForEach(u => Console.WriteLine(u));
            Console.WriteLine("-------ordered by name[acs] then role[desc]---------");
            var ordered = users.UseInMemoryOrderBy(query,
                    optionsConfigurer => optionsConfigurer.AddComparerFactory(new EnumNameComparerFactory())).ToList();
            
            ordered.ForEach(u => Console.WriteLine(u));
        }
    }
}