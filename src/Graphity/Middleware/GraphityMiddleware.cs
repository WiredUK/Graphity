using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Graphity.Authorisation;
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Graphity.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class GraphityMiddleware
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        private readonly GraphQLSettings _settings;
        private readonly IEnumerable<IValidationRule> _validationRules;

        // ReSharper disable once UnusedParameter.Local
        public GraphityMiddleware(RequestDelegate next,
            IDocumentExecuter documentExecuter,
            ISchema schema,
            GraphQLSettings settings,
            IEnumerable<IValidationRule> validationRules)
        {
            _documentExecuter = documentExecuter;
            _schema = schema;
            _settings = settings;

            _validationRules = validationRules;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task InvokeAsync(HttpContext context
            )
        {
            var query = GetQuery(context);
            var result = await _documentExecuter.ExecuteAsync(options =>
            {
                options.SetFieldMiddleware = false;

                options.UserContext = _settings.BuildUserContext(context);
                options.ValidationRules = _validationRules;

                options.OperationName = query.OperationName;
                options.Schema = _schema;
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