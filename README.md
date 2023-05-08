# QueryPack.Criterias
Is a simple abstraction layer that allows for the creation and storage of reusable predicates. This layer implements a fluent criteria builder that builds type-safe expression predicates. These predicates can be used to filter and query data from various data sources, that support LINQ 

## Getting Started
1. Install the package into your project
```
dotnet add package QueryPack.Criterias
```
2. Create and configure your criteria builder

```c#
class EntitySearchBuilder : GenericCriteriaBuilder<Entity, SearchModel>
{
   public EntitySearchBuilder()
   {
         With(m => e => e.Property.StartsWith(m.Property),
               r => r.When(m => !string.IsNullOrEmpty(m.Property)));
   }
}
```

3. Add the following line to the `Startup`  `Configure` method.

```c#
 services.AddCriteriaBuilders(typeof(YourType).Assembly);
```
4. Inject criteria builder into your service and build search criteria
```c#
ICriteriaBuilder<Entity, SearchModel> _criteriaBuilder;

Expression<Func<Entity, bool>> criteria = _criteriaBuilder.Build(searchModel);
```
5. Implement an in-memory search using the `UseInMemoryTextSearchBy` extension method.
```c#
IEnumerable<Entity> entities;

var results = entities.UseInMemoryTextSearchBy(builder => 
      builder.IncludeField(e => e.Property).When(e => !string.IsNullOrEmpty(e.Property)), 
                new SearchEntity
                {
                    TextSearch = "Test String"
                });
```
6. Add in-memory order using the `UseInMemoryOrderBy` extension method
```c#
IEnumerable<Entity> entities;
var query = new OrderModel
{
       OrderBy = new Dictionary<string, OrderDirection>
       {
            ["fieldA"] = OrderDirection.Asc,
            ["fieldB"] = OrderDirection.Desc
       }
};
var ordered = entities.UseInMemoryOrderBy(query,
                    optionsConfigurer => optionsConfigurer.AddComparerFactory(new EnumNameComparerFactory()));
```
