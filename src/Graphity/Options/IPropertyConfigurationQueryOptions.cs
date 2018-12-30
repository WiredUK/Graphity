using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    /// <summary>
    /// The interface for configuring an individual property.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public interface IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty>
        : IDbSetConfigurationQueryOptions<TContext,TEntity>
        where TContext : DbContext
    {
        /// <summary>
        /// Configure an individual property to be excluded from the graph.
        /// </summary>
        /// <returns></returns>
        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> Exclude();
    }
}