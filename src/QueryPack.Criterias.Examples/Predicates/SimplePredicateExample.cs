namespace QueryPack.Criterias.Examples.Predicates
{
    using Impl;

    internal class SimplePredicateExample
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


        public void Run()
        {
            var builder = new GenericCriteriaBuilder<User, SearchUser>();
            builder.With(m => e => e.Name == m.UserName)
                   .Or(b => b.With(m => e => e.Email.StartsWith(m.UserEmail))
                             .And(m => e => e.Name == "test"));

            var expression = builder.Build(new SearchUser { UserName = "jhon", UserEmail = "jhon@email.com" });

            Console.WriteLine($"Run example of {nameof(SimplePredicateExample)}");
            Console.WriteLine(expression);
            Console.WriteLine("end");
        }
    }
}
