using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IDbSetConfigurationQueryOptions<TContext, TProperty> : IQueryOptions<TContext>
        where TContext : DbContext
    {
        IDbSetConfigurationQueryOptions<TContext, TProperty> FieldName(string name);
        IDbSetConfigurationQueryOptions<TContext, TProperty> TypeName(string name);

        IDbSetConfigurationQueryOptions<TContext, TProperty> FilterExpression(
            Expression<Func<TProperty, bool>> defaultFilter);
    }
}