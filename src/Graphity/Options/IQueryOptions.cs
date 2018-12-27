using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GraphQL.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public interface IQueryOptions<TContext>
        where TContext : DbContext
    {
        string Name { get; }
        int DefaultTake { get; }
        string GlobalAuthorisationPolicy { get; }

        IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations { get; }

        IQueryOptions<TContext> QueryName(string name);
        IQueryOptions<TContext> SetDefaultTake(int defaultTake);

        IQueryOptions<TContext> SetGlobalAuthorisationPolicy(string authorisationPolicy);
        IQueryOptions<TContext> AddAuthorisationPolicy<TAuthorisationPolicy>(string name)
            where TAuthorisationPolicy : IAuthorizationPolicy, new();

        IDbSetConfigurationQueryOptions<TContext, TProperty> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression,
            string fieldName = null,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class;
    }
}