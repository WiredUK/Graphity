using System;

namespace Graphity
{
    public class GraphityException : Exception
    {
        public GraphityException(string message) : base(message)
        {
        }
    }
}