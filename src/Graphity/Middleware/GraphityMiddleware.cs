using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Graphity.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class GraphityMiddleware
    {
        // ReSharper disable once UnusedParameter.Local
        public GraphityMiddleware(RequestDelegate next)
        {
        }

        // ReSharper disable once UnusedMember.Global
        public async Task InvokeAsync(HttpContext context,
            IDocumentExecuter documentExecuter,
            ISchema schema)
        {
            var query = GetQuery(context);
            var result = await documentExecuter.ExecuteAsync(options =>
            {
                options.OperationName = query.OperationName;
                options.Schema = schema;
                options.Query = query.Query;
                options.ExposeExceptions = true;
                options.Inputs = query.Variables == null ? null : new Inputs(query.Variables);
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                context.Response.StatusCode = 400;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private static GraphQLQuery GetQuery(HttpContext context)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(context.Request.Body))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<GraphQLQuery>(jsonTextReader);
            }
        }
    }
}