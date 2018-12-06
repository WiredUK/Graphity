using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    public interface IScopedDependencyResolver : IDependencyResolver
    {
        IServiceScope CreateScope();
    }
}