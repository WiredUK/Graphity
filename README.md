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

Yeah, like I said above the idea is to be able to get up and running with minimal code. Of course you can configure the graph further by manually specifying exactly what you want to expose. For example:

    services.AddGraphity<AnimalContext>(options =>
    {
        options
            .ConfigureSet(ctx => ctx.Animals)
            .ConfigureSet(ctx => ctx.Countries, SetOption.IncludeAsFieldOnly);
    });

With this code, no matter how many `DbSet`s you have in your context, the graph will only expose the ones mentioned here.

## TODO

Here are some things I'd like to get working:

* *Add multiple include levels*: Currently Graphity only lets you go a single level deep, I'd like to extend this to add the Entity Framework includes to any level based on what you have asked for in the query.
* *Advanced configuration*: The ability to further configure the graph, for example:
  * Name the fields.
  * Name individual properties.
  * Exclude properties from the graph.

## Contributing

I am open to contributions but please open an issue and discuss it with me before you submit a pull request.