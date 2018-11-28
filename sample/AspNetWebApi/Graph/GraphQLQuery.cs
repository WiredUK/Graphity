using System.Collections.Generic;

namespace AspNetWebApi.Graph
{

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public Dictionary<string, object> Variables { get; set; }
    }
}