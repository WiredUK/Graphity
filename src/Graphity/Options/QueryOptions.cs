using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public class QueryOptions<TContext> : IQueryOptions<TContext>
        where TContext : DbContext
    {
        public ICollection<IDbSetConfiguration> DbSetConfigurations { get; }
        public string Name { get; private set; }

        internal QueryOptions()
        {
            Name = $"{typeof(TContext).Name}Query";
            DbSetConfigurations = new List<IDbSetConfiguration>();
        }

        public IQueryOptions<TContext> QueryName(string name)
        {
            Name = name;
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

            DbSetConfigurations.Add(dbSetConfiguration);

            return new DbSetConfigurationQueryOptions<TContext, TProperty>(this, dbSetConfiguration);
        }
    }
}