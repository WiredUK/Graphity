using System;
using System.Linq.Expressions;
using Graphity.Ordering;
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

        IDbSetConfigurationQueryOptions<TContext, TEntity> DefaultOrderBy(
            Expression<Func<TEntity, object>> defaultOrderBy,
            OrderByDirection orderByDirection = OrderByDirection.Ascending);

        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> ConfigureProperty<TProperty>(
            Expression<Func<TEntity, TProperty>> propertyExpression);
    }
}