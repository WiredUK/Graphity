using System.Collections.Generic;

namespace Graphity
{
    internal class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}