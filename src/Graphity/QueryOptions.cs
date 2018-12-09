using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    public class QueryOptions<TContext>
        where TContext : DbContext
    {
        internal List<DbSetConfiguration> DbSetConfigurations { get; set; }
        internal string Name { get; set; }

        internal QueryOptions()
        {
            Name = $"{typeof(TContext).Name}Query";
            DbSetConfigurations = new List<DbSetConfiguration>();
        }

        public QueryOptions<TContext> QueryName(string name)
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
        /// <returns></returns>
        public QueryOptions<TContext> ConfigureSet<TProperty>(Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression, SetOption setOption = SetOption.IncludeAsFieldAndChild)
            where TProperty : class
        {
            var memberExpression = (MemberExpression)dbSetExpression.Body;

            DbSetConfigurations.Add(new DbSetConfiguration
            {
                Type = typeof(TProperty),
                SetOption = setOption,
                FieldName = memberExpression.Member.Name
            });

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
        public QueryOptions<TContext> ConfigureSet<TProperty>(
            Expression<Func<TContext, DbSet<TProperty>>> dbSetExpression, 
            string fieldName, 
            SetOption setOption = SetOption.IncludeAsFieldAndChild, 
            Expression<Func<TProperty, bool>> defaultFilter = null)
            where TProperty : class
        {
            DbSetConfigurations.Add(new DbSetConfiguration
            {
                Type = typeof(TProperty),
                SetOption = setOption,
                FieldName = fieldName,
                FilterExpression = defaultFilter
            });

            return this;
        }
    }
}