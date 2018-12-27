using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            ((List<NamedAuthorisationPolicy>)AuthorisationPolicies).Add(new NamedAuthorisationPolicy(
                name,
                new TAuthorisationPolicy()));
            return this;
        }

        /// <summary>
        /// Configure an individual DbSet for inclusion or exclusion. By default all DbSets are included. Manually including
        /// a single DbSet will then only include that item. 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="dbSetExpression"></param>
        /// <param name="setOption"></param>
        /// <param name="fieldName"></param>
        /// <param name="defaultFilter"></param>
        /// <returns></returns>
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