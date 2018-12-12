using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IDbSetConfigurationQueryOptions<TContext, TEntity> : IQueryOptions<TContext>
        where TContext : DbContext
    {
        IDbSetConfigurationQueryOptions<TContext, TEntity> FieldName(string name);
        IDbSetConfigurationQueryOptions<TContext, TEntity> TypeName(string name);

        IDbSetConfigurationQueryOptions<TContext, TEntity> Filter(
            Expression<Func<TEntity, bool>> defaultFilter);

        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> ConfigureProperty<TProperty>(
            Expression<Func<TEntity, TProperty>> propertyExpression);

        IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations { get; }
    }
}