using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    internal interface IScopedDependencyResolver : IDependencyResolver
    {
        IServiceScope CreateScope();
    }
}