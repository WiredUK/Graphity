using System;
using System.Linq.Expressions;
using Graphity.Ordering;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IDbSetConfigurationQueryOptions<TContext, TEntity> : IQueryOptions<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Set the field name.
        /// </summary>
        /// <param name="name">The name to use.</param>
        /// <returns></returns>
        IDbSetConfigurationQueryOptions<TContext, TEntity> FieldName(string name);

        /// <summary>
        /// Set the name for the type.
        /// </summary>
        /// <param name="name">The name to use.</param>
        /// <returns></returns>
        IDbSetConfigurationQueryOptions<TContext, TEntity> TypeName(string name);

        /// <summary>
        /// Assign an authorisation policy to this field.
        /// </summary>
        /// <param name="authorisationPolicy">The name of the authorisation policy.</param>
        /// <returns></returns>
        IDbSetConfigurationQueryOptions<TContext, TEntity> SetAuthorisationPolicy(string authorisationPolicy);

        /// <summary>
        /// Apply a default filter to the field.
        /// </summary>
        /// <param name="defaultFilter">The expression to use.</param>
        /// <returns></returns>
        IDbSetConfigurationQueryOptions<TContext, TEntity> Filter(
            Expression<Func<TEntity, bool>> defaultFilter);

        /// <summary>
        /// Apply a default ordering to the field.
        /// </summary>
        /// <param name="defaultOrderBy">The expression to use.</param>
        /// <param name="orderByDirection">The order direction to use.</param>
        /// <returns></returns>
        IDbSetConfigurationQueryOptions<TContext, TEntity> DefaultOrderBy(
            Expression<Func<TEntity, object>> defaultOrderBy,
            OrderByDirection orderByDirection = OrderByDirection.Ascending);

        /// <summary>
        /// Configure an individual property of the field.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression">The property to configure.</param>
        /// <returns></returns>
        IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> ConfigureProperty<TProperty>(
            Expression<Func<TEntity, TProperty>> propertyExpression);
    }
}