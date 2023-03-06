# QueryPack.Criteria
Is an abstraction layer that allows for the creation and storage of reusable predicates. This layer implements a fluent criteria builder that builds type-safe expression predicates. These predicates can be used to filter and query data from various data sources, that support LINQ 

## Getting Started
1. Install the package into your project
```
dotnet add package QueryPack.Criteria
```
2. Create and configure your criteria builder

```c#
class EntitySearchBuilder : GenericCriteriaBuilder<Entity, SearchModel>
{
   public UserSearchBuilder()
   {
         With(m => e => e.Name.StartsWith(m.Property),
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