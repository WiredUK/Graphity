# Graphity 
[![Nuget](https://img.shields.io/nuget/v/graphity.svg?style=for-the-badge)](https://www.nuget.org/packages/Graphity) [![AppVeyor](https://img.shields.io/appveyor/ci/WiredUK/graphity.svg?style=for-the-badge)](https://ci.appveyor.com/project/WiredUK/graphity) [![Unit Tests](https://img.shields.io/appveyor/tests/WiredUK/graphity.svg?style=for-the-badge)](https://ci.appveyor.com/project/WiredUK/graphity) [![Code Coverage](https://img.shields.io/codecov/c/github/wireduk/graphity.svg?style=for-the-badge)](https://codecov.io/gh/WiredUK/Graphity)

A .NET Core library that integrates GraphQL and Entity Framework Core with minimal effort.

The aim of this project is to provide a GraphQL endpoint by providing only the DbContext. Further configuration of the schema and queries will be available but not required to get up and running fast.

## How to use Graphity

1. Set up your DbContext as you would normally, ensuring it has been added to the DI container.
2. Add the [Graphity Nuget package](https://www.nuget.org/packages/Graphity) to your project using the Visual Studio Nuget Package manager or from the command line:

    ```
    dotnet add package graphiql
    ```

3. In your `ConfigureServices` method in `Startup`, add Graphity to your container:

    ```c#
    services.AddGraphity<YourDbContext>();
    ```

4. Add the Graphity middleware to your pipeline, this needs to happen before MVC if you are using it. Add this line to the `Startup.Configure` method:

    ```c#
    app.UseGraphity();
    ```

    This exposes the graph on the default endpoint of `/api/graph`. Supply a different value if you prefer another path.

5. Now you can call your graph with any GraphQL tool you choose. For example [Insomnia](https://insomnia.rest/) or [GraphiQL](https://electronjs.org/apps/graphiql).

## That's it?!

The idea behind Graphity is to be able to get up and running with minimal code. Of course you can configure the graph further by manually specifying exactly what you want to expose. For example:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options
        .ConfigureSet(ctx => ctx.Animals)
        .ConfigureSet(ctx => ctx.Countries, SetOption.IncludeAsFieldOnly);
});
```

With this code, no matter how many `DbSet`s you have in your context, the graph will only expose the ones configured here.

You can also apply some default filters to your sets. For example, perhaps you only want to query on rows where the `Active` column is set to `true`, that would look something like this:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options
        .ConfigureSet(ctx => ctx.Animals, defaultFilter: a => a.Active == true);
});
```

Or another example demonstrating the fluent interface:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.QueryName("AnimalsQuery");

    options.ConfigureSet(ctx => ctx.Animals)
        .FieldName("filteredAnimals")  //Name the field
        .Filter(a => a.Active == true) //Exclude all inactive animals
        .DefaultOrderBy(a => a.Name) //Add a default order to sort by name
        .ConfigureProperty(a => a.Id).Exclude() //Hide the Id column from the graph
        .ConfigureProperty(a => a.LivesInId).Exclude(); //Hide the LivesInId column from the graph

    options.ConfigureSet(ctx => ctx.Countries)
        .ConfigureProperty(c => c.Id).Exclude(); //Hide the Id column from the graph
});
```

## Authorisation

Authorisation comes in two parts. First the policies need to be defined, and secondly those policies need to be assigned to the query or individual fields.

Graphity comes with some default policies that you can use, or you can add your own custom policies. If you want to ensure a user has a particular role, use the `AddHasRolesAuthorisationPolicy` method:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.AddHasRolesAuthorisationPolicy("admin-policy", "admin");
};
```

If you want to ensure a user has a particular scope, use the `AddHasScopeAuthorisationPolicy` method:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.AddHasScopeAuthorisationPolicy("scope-policy", "scope1");
};
```

For a more complex requirement you have two options. You can implement your own `IAuthorizationPolicy` and add it to the store:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.options.AddAuthorisationPolicy<MyCustomAuthPolicy>("custom-policy");
};
```

Or you can use the `Func` policy that lets you use a simple method. For example, you could have a method as simple as this:

```c#
private static async Task<AuthorisationResult> WeekendOnlyPolicy()
{
    var isWeekend = DateTime.Today.DayOfWeek == DayOfWeek.Saturday ||
                    DateTime.Today.DayOfWeek == DayOfWeek.Sunday;

    //This isn't really an async method
    await Task.CompletedTask;

    return isWeekend 
        ? AuthorisationResult.Success() 
        : AuthorisationResult.Fail("This query can only be used on a Saturday or Sunday");
}
```

And add it to the store like this, note how it take a delegate to the `WeekendOnlyPolicy` method:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.AddFuncAuthorisationPolicy("weekendsOnly", WeekendOnlyPolicy);
};
```

Now you've defined your policies, you need to assign them. If you want to assign this policy to the entire query, add it as a global policy:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.AddAuthorisationPolicy<HasAdminRoleAuthorisationPolicy>("admin-policy");

    options.SetGlobalAuthorisationPolicy("admin-policy");
}
```

Or if you only want to protect a single field:

```c#
services.AddGraphity<AnimalContext>(options =>
{
    options.AddAuthorisationPolicy<HasAdminRoleAuthorisationPolicy>("admin-policy");

    options.ConfigureSet(ctx => ctx.Countries)
        .SetAuthorisationPolicy("admin-policy");
}
```

## Entity Framework Queries

Another aim of this project is to construct the Entity Framework queries to be as efficient as possible. One of the way we do that is to `Include` the relevant child entities and only `Select` the properties we need.

## Example Graph Queries

Starting off with a basic query, get all animals and the country where they live. Note that it is possible to rename the fields:

```graphql
{
  filteredAnimals {
    name
    livesIn {
      country: name
    }
  }
}
```

How about we only want animals that start with the letter 'S'. For that we could use the `where` parameter. That allows us to pass a list of clauses that will get "and"ed together:

```graphql
{
  filteredAnimals(where: [{path: "name", comparison: startsWith, value: "S"}]) {
    name
    livesIn {
      country: name
    }
  }
}
```

We also support some basic dotted dereferencing of non-enumerable child properties. This allows us to query all animals that don't live in France:

```graphql
{
  filteredAnimals(where: [{path: "livesIn.name", comparison: notEqual, value: "France"}]) {
    name
    livesIn {
      country: name
    }
  }
}
```

Or multiple clauses:

```graphql
{
  filteredAnimals(where: [{path: "name", comparison: startsWith, value: "C"},
                          {path: "numberOfLegs", comparison: greaterThan, value: "2"}]) {
    name
    livesIn {
      country: name
    }
  }
}
```

We can also write more complex filters in the query with the `filter` parameter. For example:

```graphql
{
  filteredAnimals(filter: "name = `Cat` or numberOfLegs < 4") {
    name
    livesIn {
      country: name
    }
  }
}
```

For a more comprehensive list of what is possible with the `filter` parameter, see the [wiki docs for `System.Linq.Dynamic.Core`](https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions). The only difference Graphity has is that you can specify strings using backticks as well as double quotes. This is because the filter values are already wrapped in quotes and are very awkward to write. So instead of having to escape the inner quotes like this:

```graphql
filter: "name = \"Cat\""
```

You can write

```graphql
filter: "name = `Cat`"
```

Perhaps we only want the first 3 animals:

```graphql
{
  filteredAnimals(take: 3) {
    name
    livesIn {
      country: name
    }
  }
}
```

Or the second batch of 3 animals:

```graphql
{
  filteredAnimals(skip: 3, take: 3) {
    name
    livesIn {
      country: name
    }
  }
}
```

But a `skip`/`take` is rarely a good idea without specifying an order, and we can order by the country the animal live:

```graphql
{
  filteredAnimals(skip: 3, take: 3, orderBy:{path:"livesIn.name"}) {
    name
    livesIn {
      country: name
    }
  }
}
```

## Samples

There are a few [sample projects in the repository](https://github.com/WiredUK/Graphity/tree/master/sample) if you would like to see Graphity in action.

1. **[ZeroConfiguration](https://github.com/WiredUK/Graphity/tree/master/sample/ZeroConfiguration)** - This project shows the bare minimum code you need to implement Graphity.
2. **[FullConfiguration](https://github.com/WiredUK/Graphity/tree/master/sample/FullConfiguration)** - This is a more complex configuration showing off a much more complete set of configuration options.
3. **[MvcWithAuthorisation](https://github.com/WiredUK/Graphity/tree/master/sample/MvcWithAuthorisation)** - A project with MVC that also demonstrates the authorisation aspect of Graphity.

## TODO

Here are some things I'd like to get working:

* *Advanced configuration*: The ability to further configure the graph, for example:
  * ~~Name the query.~~
  * ~~Name the fields.~~
  * ~~Name the types.~~
  * Name individual properties. (though this might make the dynamic expression building awkward which makes this low priority)
  * ~~Exclude properties from the graph.~~
* ~~*Add ordering*: Add argument to allow custom ordering.~~
* ~~*Skip & take*: To support pagination of queries.~~
* ~~*Authentication & authorisation*: To protect the query or individual fields.~~
* *Mutations*: Currently Graphity is read-only and a big part of GraphQL is the ability to send changes to your data store. However, I'd like to nail the auth component before tackling this one as making changes might be far more dangerous than reading data.
* *Custom mappings*: Allow a custom mapping to be injected that transforms the output into a different format. This would be applied after the data is retrieved from EF so could be very generic. As this would affect the graph, this needs some serious thought putting in and may not really be needed... Hmm, one to ponder. 

## Contributing

I am open to contributions but please open an issue and discuss it with me before you submit a pull request.

## Get in Contact

I love hearing from people who have tried out my projects, really makes it feel like a worthwhile endeavour. If you want to get in touch to say hi or comment about this (or any other projects) or even have any suggestions, please use the details in [my profile](https://github.com/WiredUK). 