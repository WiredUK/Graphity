using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    public class QueryOptions<TContext>
        where TContext : DbContext
    {
        internal List<DbSetConfiguration> DbSetConfigurations { get; set; }
        internal ServiceLifetime ServiceLifetime { get; set; }

        internal QueryOptions()
        {
            DbSetConfigurations = new List<DbSetConfiguration>();
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
            DbSetConfigurations.Add(new DbSetConfiguration
            {
                Type = typeof(TProperty),
                SetOption = setOption
            });

            return this;
        }

        internal IEnumerable<Type> GetFields()
        {
            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()));

            if (!DbSetConfigurations.Any())
            {
                return dbSetProperties.Select(pi => pi.PropertyType);
            }

            return DbSetConfigurations
                .Where(dsc => dsc.SetOption == SetOption.IncludeAsFieldAndChild || dsc.SetOption == SetOption.IncludeAsFieldOnly)
                .Select(dsc => dsc.Type);
        }

        public bool CanBeAChild(Type type)
        {
            return DbSetConfigurations
                .Any(dsc => dsc.Type == type &&
                            (dsc.SetOption == SetOption.IncludeAsChildOnly ||
                             dsc.SetOption == SetOption.IncludeAsFieldAndChild));
        }
    }
}