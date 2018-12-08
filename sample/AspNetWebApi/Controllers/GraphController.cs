using System.Threading.Tasks;
using AspNetWebApi.Data;
using Graphity;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace AspNetWebApi.Controllers
{
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
}
