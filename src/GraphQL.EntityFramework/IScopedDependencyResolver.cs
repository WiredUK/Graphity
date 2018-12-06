using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.EntityFramework
{
    public interface IScopedDependencyResolver : IDependencyResolver
    {
        IServiceScope CreateScope();
    }
}