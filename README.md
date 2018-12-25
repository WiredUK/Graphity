# Graphity [![Nuget](https://img.shields.io/nuget/v/graphity.svg?style=for-the-badge)](https://www.nuget.org/packages/Graphity)
An experimental .NET Core library that integrates GraphQL and Entity Framework Core with minimal effort.

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
        .ConfigureProperty(a => a.Id).Exclude() //Hide the Id column from the graph
        .ConfigureProperty(a => a.LivesInId).Exclude(); //Hide the LivesInId column from the graph

    options.ConfigureSet(ctx => ctx.Countries)
        .ConfigureProperty(c => c.Id).Exclude(); //Hide the Id column from the graph
});
```

## Entity Framework Queries

Another aim of this project is to construct the Entity Framework queries to be as efficient as possible. One of the way we do that is to `Include` the relevant child entities and only `Select` the properties we need.

## TODO

Here are some things I'd like to get working:

* *Advanced configuration*: The ability to further configure the graph, for example:
  * ~~Name the query.~~
  * ~~Name the fields.~~
  * ~~Name the types.~~
  * Name individual properties. (though this might make the dynamic expression building awkward which makes this low priority)
  * ~~Exclude properties from the graph.~~
* *Add ordering*: Add argument to allow custom ordering.
* *Skip & take*: To support pagination of queries.
* *Authentication & authorisation*: Makes sense to protect your data right? Not sure how this would work but my current thought is to have some sort of pluggable module to allow a bunch of different mechanisms.
* *Mutations*: Currently Graphity is read-only and a big part of GraphQL is the ability to send changes to your data store. However, I'd like to nail the auth component before tackling this one as making changes might be far more dangerous than reading data.
* *Custom mappings*: Allow a custom mapping to be injected that transforms the output into a different format. This would be applied after the data is retrieved from EF so could be very generic. As this would affect the graph, this needs some serious thought putting in and may not really be needed... Hmm, one to ponder. 

## Contributing

I am open to contributions but please open an issue and discuss it with me before you submit a pull request.

## Get in Contact

I love hearing from people who have tried out my projects, really makes it feel like a worthwhile endeavour. If you want to get in touch to say hi or comment about this (or any other projects) or even have any suggestions, please use the details in [my profile](https://github.com/WiredUK). 