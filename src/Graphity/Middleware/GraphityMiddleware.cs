using System;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Graphity.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GraphityMiddleware
    {
        // ReSharper disable once UnusedParameter.Local
        public GraphityMiddleware(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(HttpContext context,
            IDocumentExecuter documentExecuter,
            ISchema schema)
        {
            var query = await GetQuery(context);
            var result = await documentExecuter.ExecuteAsync(options =>
            {
                options.OperationName = query.OperationName;
                options.Schema = schema;
                options.Query = query.Query;
                options.Inputs = query.Variables == null ? null : new Inputs(query.Variables);
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                context.Response.StatusCode = 400;
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private static async Task<GraphQLQuery> GetQuery(HttpContext context)
        {
            var body = context.Request.Body;

            var request = context.Request;

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;

            if (bodyAsText == "")
            {
                bodyAsText = "{}";
            }

            return JsonConvert.DeserializeObject<GraphQLQuery>(bodyAsText);
        }
    }
}