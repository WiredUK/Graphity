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
        int DefaultTake { get; }

        IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations { get; }

        IQueryOptions<TContext> QueryName(string name);
        IQueryOptions<TContext> SetDefaultTake(int defaultTake);

        IDbSetConfigurationQueryOptions<TContext, TProperty> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression,
            string fieldName = null,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class;
    }
}