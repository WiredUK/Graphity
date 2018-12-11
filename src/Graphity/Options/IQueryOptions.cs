using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IQueryOptions<TContext>
        where TContext : DbContext
    {
        string Name { get; }
        IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations { get; }

        IQueryOptions<TContext> QueryName(string name);

        IDbSetConfigurationQueryOptions<TContext, TProperty> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression,
            string fieldName = null,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class;
    }
}