using Microsoft.EntityFrameworkCore;
using GraphQL.Types;

namespace GraphQL.EntityFramework
{
    public class DynamicSchema<TContext> : Schema
        where TContext : DbContext
    {
        public DynamicSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<DynamicQuery<TContext>>();
        }
    }
}