using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Graphity.Authorisation;
using GraphQL.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    internal class QueryOptions<TContext> : IQueryOptions<TContext>
        where TContext : DbContext
    {
        public IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations { get; }
        public string Name { get; private set; }
        public int DefaultTake { get; private set; }
        public string GlobalAuthorisationPolicy { get; private set; }
        public IReadOnlyCollection<NamedAuthorisationPolicy> AuthorisationPolicies { get; }

        internal QueryOptions()
        {
            Name = $"{typeof(TContext).Name}Query";
            DefaultTake = 10;
            DbSetConfigurations = new List<IDbSetConfiguration>();
            AuthorisationPolicies = new List<NamedAuthorisationPolicy>();
        }

        public IQueryOptions<TContext> QueryName(string name)
        {
            Name = name;
            return this;
        }

        public IQueryOptions<TContext> SetDefaultTake(int defaultTake)
        {
            DefaultTake = defaultTake;
            return this;
        }

        public IQueryOptions<TContext> SetGlobalAuthorisationPolicy(string authorisationPolicy)
        {
            GlobalAuthorisationPolicy = authorisationPolicy;
            return this;
        }

        public IQueryOptions<TContext> AddAuthorisationPolicy<TAuthorisationPolicy>(string name)
            where TAuthorisationPolicy : IAuthorizationPolicy, new()
        {
            var newPolicy = new NamedAuthorisationPolicy(
                name,
                new TAuthorisationPolicy());

            ((List<NamedAuthorisationPolicy>)AuthorisationPolicies).Add(newPolicy);

            return this;
        }

        public IQueryOptions<TContext> AddHasClaimAuthorisationPolicy(string policyName, string claimType, params string[] values)
        {
            var newPolicy = new NamedAuthorisationPolicy(
                policyName,
                new HasClaimAuthorisationPolicy(claimType, values));

            ((List<NamedAuthorisationPolicy>)AuthorisationPolicies).Add(newPolicy);

            return this;
        }

        public IQueryOptions<TContext> AddHasRolesAuthorisationPolicy(string policyName, params string[] roles)
        {
            return AddHasClaimAuthorisationPolicy(policyName, ClaimTypes.Role, roles);
        }

        public IQueryOptions<TContext> AddHasScopeAuthorisationPolicy(string policyName, params string[] scopes)
        {
            return AddHasClaimAuthorisationPolicy(policyName, "Scope", scopes);
        }

        public IQueryOptions<TContext> AddFuncAuthorisationPolicy(string policyName, Func<Task<AuthorisationResult>> authoriseFunc)
        {
            var newPolicy = new NamedAuthorisationPolicy(
                policyName,
                new FuncAuthorisationPolicy(authoriseFunc));

            ((List<NamedAuthorisationPolicy>)AuthorisationPolicies).Add(newPolicy);

            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TProperty> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression,
            string fieldName = null,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class
        {
            var dbSetConfiguration = new DbSetConfiguration
            {
                Type = typeof(TProperty),
                SetOption = setOption,
                FieldName = fieldName,
                FilterExpression = defaultFilter
            };

            ((List<IDbSetConfiguration>)DbSetConfigurations).Add(dbSetConfiguration);

            return new DbSetConfigurationQueryOptions<TContext, TProperty>(this, dbSetConfiguration);
        }

    }
}