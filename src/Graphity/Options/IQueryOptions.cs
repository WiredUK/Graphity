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
    /// <summary>
    /// The interface for configuring Graphity
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IQueryOptions<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// The name of the query.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The default number to be used as the 'take' value.
        /// </summary>
        int DefaultTake { get; }

        /// <summary>
        /// The default authorisation policy to use for the entire query.
        /// </summary>
        string GlobalAuthorisationPolicy { get; }

        /// <summary>
        /// The assigned configurations for individual DbSets.
        /// </summary>
        IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations { get; }

        /// <summary>
        /// Sets the query name.
        /// </summary>
        /// <param name="name">The new query name.</param>
        /// <returns></returns>
        IQueryOptions<TContext> QueryName(string name);

        /// <summary>
        /// Sets the default number to be used as the 'take' value.
        /// </summary>
        /// <param name="defaultTake">The value to be used as the default 'take' value.</param>
        /// <returns></returns>
        IQueryOptions<TContext> SetDefaultTake(int defaultTake);

        /// <summary>
        /// Sets the global authorisation policy to apply to all queries.
        /// </summary>
        /// <param name="authorisationPolicy"></param>
        /// <returns></returns>
        IQueryOptions<TContext> SetGlobalAuthorisationPolicy(string authorisationPolicy);

        /// <summary>
        /// Add a custom authorisation policy to the store.
        /// </summary>
        /// <typeparam name="TAuthorisationPolicy">The type to be used</typeparam>
        /// <param name="name">The name of the policy used when assigning it to fields.</param>
        /// <returns></returns>
        IQueryOptions<TContext> AddAuthorisationPolicy<TAuthorisationPolicy>(string name)
            where TAuthorisationPolicy : IAuthorizationPolicy, new();

        /// <summary>
        /// Add a role requirement policy to the store.
        /// </summary>
        /// <param name="policyName">The name of the policy used when assigning it to fields.</param>
        /// <param name="roles">The roles used to authorise with.</param>
        /// <returns></returns>
        IQueryOptions<TContext> AddHasRolesAuthorisationPolicy(string policyName, params string[] roles);

        /// <summary>
        /// Add a scope requirement policy to the store. This is commonly used with Identity Server.
        /// </summary>
        /// <param name="policyName">The name of the policy used when assigning it to fields.</param>
        /// <param name="scopes">The scopes used to authorise with.</param>
        /// <returns></returns>
        IQueryOptions<TContext> AddHasScopeAuthorisationPolicy(string policyName, params string[] scopes);

        /// <summary>
        /// Add an authorisation policy to the store that uses a custom function.
        /// </summary>
        /// <param name="policyName">The name of the policy used when assigning it to fields.</param>
        /// <param name="authoriseFunc">The asynchronous function to call to authorise the request.</param>
        /// <returns></returns>
        IQueryOptions<TContext> AddFuncAuthorisationPolicy(string policyName, Func<Task<AuthorisationResult>> authoriseFunc);

        /// <summary>
        /// Add a generic claim check to the available authorisation policies.
        /// </summary>
        /// <param name="policyName">The name of the policy to be used when assigning it to fields.</param>
        /// <param name="claimType">The type of the claim (e.g. <see cref="ClaimTypes"/> for examples.</param>
        /// <param name="values">The claim values</param>
        /// <returns></returns>
        IQueryOptions<TContext> AddHasClaimAuthorisationPolicy(string policyName, string claimType, params string[] values);

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
        IDbSetConfigurationQueryOptions<TContext, TProperty> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression,
            string fieldName = null,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class;
    }
}