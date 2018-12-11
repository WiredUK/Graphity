# Graphity
An experimental .NET Core library that integrates GraphQL and Entity Framework Core with minimal effort.

The aim of this project is to provide a GraphQL endpoint by providing only the DbContext. Further configuration of the schema and queries will be available but not required to get up and running fast.

## How to use Graphity

1. Set up your DbContext as you would normally, ensuring it has been added to the DI container.
2. In your `ConfigureServices` method in `Startup`, add Graphity to your container:

       services.AddGraphity<YourDbContext>();

3. Add the Graphity middleware to your pipeline, this needs to happen before MVC if you are using it. Add this line to the `Startup.Configure` method:

       app.UseGraphity();

5. Now you can call your graph with any GraphQL tool you choose. For example [Insomnia](https://insomnia.rest/) or [GraphiQL](https://electronjs.org/apps/graphiql).

## That's it?!

The idea behind Graphity is to be able to get up and running with minimal code. Of course you can configure the graph further by manually specifying exactly what you want to expose. For example:

    services.AddGraphity<AnimalContext>(options =>
    {
        options
            .ConfigureSet(ctx => ctx.Animals)
            .ConfigureSet(ctx => ctx.Countries, SetOption.IncludeAsFieldOnly);
    });

With this code, no matter how many `DbSet`s you have in your context, the graph will only expose the ones configured here.

You can also apply some default filters to your sets. For example, perhaps you only want to query on rows where the `Active` column is set to `true`, that would look something like this:

    services.AddGraphity<AnimalContext>(options =>
    {
        options
            .ConfigureSet(ctx => ctx.Animals, defaultFilter: a => a.Active == true);
    });

Or another example demonstrating the fluent interface:

    services.AddGraphity<AnimalContext>(options =>
    {
        options.QueryName("AnimalsQuery");

        options.ConfigureSet(ctx => ctx.Animals)
            .FieldName("filteredAnimals")
            .FilterExpression(a => a.Id > 1);

        options.ConfigureSet(ctx => ctx.Countries);
    });

## Entity Framework Queries

Another aim of this project is to construct the Entity Framework queries to be as efficient as possible. One of the way we do that is to `Include` the relevant child entities and only `Select` the properties we need.

## TODO

Here are some things I'd like to get working:

* *Advanced configuration*: The ability to further configure the graph, for example:
  * ~~Name the query.~~
  * ~~Name the fields.~~
  * ~~Name the types.~~
  * Name individual properties. (though this might make the dynamic expression building awkward which makes this low priority)
  * Exclude properties from the graph.
* *Add unit testing*: Yeah, this should have been done already...

## Contributing

I am open to contributions but please open an issue and discuss it with me before you submit a pull request.
