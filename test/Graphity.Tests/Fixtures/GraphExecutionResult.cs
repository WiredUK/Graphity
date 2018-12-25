using System.Collections.Generic;

namespace Graphity.Tests.Fixtures
{
    public class GraphExecutionResult<T>
    {
        public Dictionary<string, IEnumerable<T>> Data { get; set; }
        public IEnumerable<GraphError> Errors { get; set; }
    }
}