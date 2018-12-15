using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Graphity
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}