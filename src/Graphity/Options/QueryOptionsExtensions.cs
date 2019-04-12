using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    internal static class QueryOptionsExtensions
    {
        private static readonly Type[] DbTypes = { typeof(DbSet<>), typeof(DbQuery<>) };

        internal static IEnumerable<IDbSetConfiguration> GetFields<TContext>(this IQueryOptions<TContext> options)
            where TContext : DbContext
        {

            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             DbTypes.Any(dbt => dbt.IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition())));

            if (!options.DbSetConfigurations.Any())
            {
                return dbSetProperties.Select(pi => new DbSetConfiguration
                {
                    SetOption = SetOption.IncludeAsFieldAndChild,
                    Type = pi.PropertyType.GetGenericArguments()[0],
                    TypeName = pi.Name,
                    FieldName = pi.Name,
                    IsQuery = typeof(DbQuery<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition())
                });
            }

            return options.DbSetConfigurations
                .Where(dsc => dsc.SetOption == SetOption.IncludeAsFieldAndChild ||
                              dsc.SetOption == SetOption.IncludeAsFieldOnly);
        }

        internal static IEnumerable<IDbSetConfiguration> GetAllFields<TContext>(this IQueryOptions<TContext> options)
            where TContext : DbContext
        {
            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             DbTypes.Any(dbt => dbt.IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition())));

            if (!options.DbSetConfigurations.Any())
            {
                return dbSetProperties.Select(pi => new DbSetConfiguration
                {
                    SetOption = SetOption.IncludeAsFieldAndChild,
                    Type = pi.PropertyType.GetGenericArguments()[0],
                    FieldName = pi.Name
                });
            }

            return options.DbSetConfigurations;
        }

        internal static bool CanBeAChild<TContext>(this IQueryOptions<TContext> options, Type type)
            where TContext : DbContext
        {
            if (!options.DbSetConfigurations.Any())
            {
                return true;
            }

            return options.DbSetConfigurations
                .Any(dsc => dsc.Type == type &&
                            (dsc.SetOption == SetOption.IncludeAsChildOnly ||
                             dsc.SetOption == SetOption.IncludeAsFieldAndChild));
        }

    }
}