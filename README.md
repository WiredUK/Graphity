# Graphity
An experimental .NET Core library that integrates GraphQL and Entity Framework Core with minimal effort.

The aim of this project is to provide a GraphQL endpoint by providing only the DbContext. Further configuration of the schema and queries will be available but not required to get up and running fast.

## How to use Graphity

1. Set up your DbContext as you would normally, ensuring it has been added to the DI container.
2. Add the [GraphQL Nuget package](https://www.nuget.org/packages/GraphQL/) to your project:

       dotnet add package GraphQL

3. Add a controller to receive the HTTP request, for example:

       [Route("api")]
       [ApiController]
       public class GraphController : ControllerBase
       {
           private readonly IDocumentExecuter _documentExecuter;
           private readonly ISchema _schema;

           public GraphController(IDocumentExecuter documentExecuter, ISchema schema, AnimalContext context)
           {
               _documentExecuter = documentExecuter;
               _schema = schema;
           }

           [Route("graph")]
           [HttpPost]
           public async Task<ActionResult<ExecutionResult>> GetGraph(GraphQLQuery query)
           {
               var result = await _documentExecuter.ExecuteAsync(options =>
               {
                   options.OperationName = query.OperationName;
                   options.Schema = _schema;
                   options.Query = query.Query;
                   options.Inputs = query.Variables == null ? null : new Inputs(query.Variables);
               }).ConfigureAwait(false);

               if (!(result.Errors?.Count > 0))
               {
                   return result;
               }

               return BadRequest(result);
           }
       }

4. In your `ConfigureServices` method in `Startup`, add Graphity to your container:

       services.AddGraphity<YourDbContext>();

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

* *Negate the need to set up a controller*: I'd like to inject Graphity directly into the pipeline and serve requests directly. This would remove the need for steps 2 and 3 from the instructions above.
* *Add multiple include levels*: Currently Graphity only lets you go a single level deep, I'd like to extend this to add the Entity Framework includes to any level based on what you have asked for in the query.
* *Add filtering*: Basically we need a where clause. This is currently in progress as I have the code from another project that I just need to drop in.
* *Advanced configuration*: The ability to further configure the graph, for example:
  * Name the fields.
  * Name individual properties.
  * Exclude properties from the graph.