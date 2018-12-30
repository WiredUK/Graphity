using System;

namespace Graphity
{
    internal class GraphityException : Exception
    {
        public GraphityException(string message) : base(message)
        {
        }
    }
}