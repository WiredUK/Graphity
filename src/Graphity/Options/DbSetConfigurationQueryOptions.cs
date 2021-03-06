﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Graphity.Authorisation;
using Graphity.Ordering;
using GraphQL.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    internal class DbSetConfigurationQueryOptions<TContext, TEntity> : IDbSetConfigurationQueryOptions<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly QueryOptions<TContext> _options;
        private readonly DbSetConfiguration _dbSetConfiguration;

        internal DbSetConfigurationQueryOptions(QueryOptions<TContext> options, DbSetConfiguration dbSetConfiguration)
        {
            _options = options;
            _dbSetConfiguration = dbSetConfiguration;
        }

        public string Name => _options.Name;
        public int DefaultTake => _options.DefaultTake;
        public string GlobalAuthorisationPolicy => _options.GlobalAuthorisationPolicy;

        public IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations => _options.DbSetConfigurations;

        public IDbSetConfigurationQueryOptions<TContext, TEntity> Filter(
            Expression<Func<TEntity, bool>> defaultFilter)
        {
            _dbSetConfiguration.FilterExpression = defaultFilter;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> DefaultOrderBy(
            Expression<Func<TEntity, object>> defaultOrderBy,
            OrderByDirection orderByDirection = OrderByDirection.Ascending)
        {
            _dbSetConfiguration.DefaultOrderByExpression = defaultOrderBy;
            _dbSetConfiguration.OrderByDirection = orderByDirection;
            return this;
        }

        public IQueryOptions<TContext> QueryName(string name)
        {
            return _options.QueryName(name);
        }

        public IQueryOptions<TContext> SetDefaultTake(int defaultTake)
        {
            return _options.SetDefaultTake(defaultTake);
        }

        public IQueryOptions<TContext> SetGlobalAuthorisationPolicy(string authorisationPolicy)
        {
            _options.SetGlobalAuthorisationPolicy(authorisationPolicy);
            return this;
        }

        public IQueryOptions<TContext> AddAuthorisationPolicy<TAuthorisationPolicy>(string name)
            where TAuthorisationPolicy : IAuthorizationPolicy, new()
        {
            _options.AddAuthorisationPolicy<TAuthorisationPolicy>(name);
            return this;
        }

        public IQueryOptions<TContext> AddHasRolesAuthorisationPolicy(string policyName, params string[] roles)
        {
            return _options.AddHasRolesAuthorisationPolicy(policyName, roles);
        }

        public IQueryOptions<TContext> AddHasScopeAuthorisationPolicy(string policyName, params string[] scopes)
        {
            return _options.AddHasScopeAuthorisationPolicy(policyName, scopes);
        }

        public IQueryOptions<TContext> AddFuncAuthorisationPolicy(string policyName, Func<Task<AuthorisationResult>> authoriseFunc)
        {
            return _options.AddFuncAuthorisationPolicy(policyName, authoriseFunc);
        }

        public IQueryOptions<TContext> AddHasClaimAuthorisationPolicy(string policyName, string claimType, params string[] values)
        {
            return _options.AddHasClaimAuthorisationPolicy(policyName, claimType, values);
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> FieldName(string name)
        {
            _dbSetConfiguration.FieldName = name;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> TypeName(string name)
        {
            _dbSetConfiguration.TypeName = name;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> SetAuthorisationPolicy(string authorisationPolicy)
        {
            _dbSetConfiguration.AuthorisationPolicy = authorisationPolicy;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TProperty1> ConfigureSet<TProperty1>(
            Expression<Func<TContext, DbSet<TProperty1>>> dbSetExpression, string fieldName,
            SetOption setOption = SetOption.IncludeAsFieldAndChild, 
            Expression<Func<TProperty1, bool>> defaultFilter = null) 
            where TProperty1 : class
        {
            return _options.ConfigureSet(dbSetExpression, fieldName, setOption, defaultFilter);
        }

        public IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> ConfigureProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyConfiguration = new PropertyConfiguration
            {
                PropertyExpression = (MemberExpression)propertyExpression.Body
            };

            ((List<IPropertyConfiguration>) _dbSetConfiguration.PropertyConfigurations).Add(propertyConfiguration);

            return new PropertyConfigurationQueryOptions<TContext, TEntity, TProperty>(this, _dbSetConfiguration, propertyConfiguration);
        }
    }
}