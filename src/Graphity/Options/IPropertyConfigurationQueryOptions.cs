using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty>
        : IDbSetConfigurationQueryOptions<TContext,TEntity>
        where TContext : DbContext
    {
        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> Exclude();

        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> FilterProperty(
            Expression<Func<IEnumerable<TProperty>, bool>> propertyFilter);
    }
}