using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    internal static class QueryOptionsExtensions
    {
        internal static IEnumerable<IDbSetConfiguration> GetFields<TContext>(this IQueryOptions<TContext> options)
            where TContext : DbContext
        {
            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()));

            if (!options.DbSetConfigurations.Any())
            {
                return dbSetProperties.Select(pi => new DbSetConfiguration
                {
                    SetOption = SetOption.IncludeAsFieldAndChild,
                    Type = pi.PropertyType,
                    TypeName = pi.Name,
                    FieldName = pi.PropertyType.Name
                });
            }

            return options.DbSetConfigurations
                .Where(dsc => dsc.SetOption == SetOption.IncludeAsFieldAndChild || 
                              dsc.SetOption == SetOption.IncludeAsFieldOnly);
        }

        internal static bool CanBeAChild<TContext>(this IQueryOptions<TContext> options, Type type)
            where TContext : DbContext
        {
            return options.DbSetConfigurations
                .Any(dsc => dsc.Type == type &&
                            (dsc.SetOption == SetOption.IncludeAsChildOnly ||
                             dsc.SetOption == SetOption.IncludeAsFieldAndChild));
        }

    }
}