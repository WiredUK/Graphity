using Microsoft.EntityFrameworkCore;

namespace GraphQL.EntityFramework
{
    public class DynamicSchema<TContext> : GraphQL.Types.Schema
        where TContext : DbContext
    {
        public DynamicSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<DynamicQuery<TContext>>();
        }
    }
}