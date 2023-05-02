namespace QueryPack.Criterias.Examples.Predicates
{
    using ImMemory;

    internal class InMemoryTextSearch
    {
        class SearchUser : ITextSearch
        {
            public string TextSearch { get; set; }
        }

        class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        public void Run()
        {
            var users = new List<User>();

            var results = users.UseInMemoryTextSearchBy(builder => 
                builder.IncludeField(e => e.Name).When(e => !string.IsNullOrEmpty(e.Name)), 
                new SearchUser 
                {
                    TextSearch = "jhon"
                });
        }
    }
}