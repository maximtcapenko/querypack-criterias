using QueryPack.Criterias.Examples.Predicates;

var simpleExample = new SimplePredicateExample();
simpleExample.Run();

var configExample = new SearchConfigurationExample();
configExample.Run();

var textSearchExample = new  InMemoryTextSearchExample();
textSearchExample.Run();

var inMemOrderExample = new  InMemoryOrderExample();
inMemOrderExample.Run();

var orderExample = new ExpressionOrderExample();
orderExample.Run();