using Microsoft.EntityFrameworkCore;
using GraphQL.Types;

namespace Graphity
{
    public class DynamicSchema<TContext> : Schema
        where TContext : DbContext
    {
        public DynamicSchema(IScopedDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<DynamicQuery<TContext>>();
        }
    }
}