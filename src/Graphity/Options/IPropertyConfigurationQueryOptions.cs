using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty>
        : IDbSetConfigurationQueryOptions<TContext,TEntity>
        where TContext : DbContext
    {
        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> Exclude();
    }
}