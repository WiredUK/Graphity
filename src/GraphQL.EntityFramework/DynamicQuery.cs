using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.EntityFramework
{
    public class DynamicQuery<TContext> : ObjectGraphType<object>
        where TContext : DbContext
    {
        private readonly IScopedDependencyResolver _resolver;

        internal static QueryOptions<TContext> QueryOptions { private get; set; }

        private IEnumerable<PropertyInfo> GetDbSets()
        {
            var dbSets = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()))
                .ToList();

            if (!QueryOptions.IncludeDbSets.Any())
            {
                return dbSets;
            }

            var includedDbSets = new List<PropertyInfo>();

            foreach (var includeDbSet in QueryOptions.IncludeDbSets)
            {
                if (!(includeDbSet.Body is MemberExpression member))
                {
                    continue;
                }

                var propInfo = member.Member as PropertyInfo;

                includedDbSets.Add(propInfo);
            }

            return includedDbSets;
        }

        public DynamicQuery(IScopedDependencyResolver resolver)
        {
            _resolver = resolver;

            Name = $"{typeof(TContext).Name}Query";
            
            foreach (var dbSetProperty in GetDbSets())
            {
                var dbSetType = dbSetProperty.PropertyType.GenericTypeArguments[0];
                var graphType = typeof(DynamicObjectGraphType<>).MakeGenericType(dbSetType);
                var listGraphType = typeof(ListGraphType<>).MakeGenericType(graphType);

                var genericFieldMethod = typeof(ObjectGraphType<object>)
                    .GetMethods()
                    .Single(mi => mi.Name == "Field" &&
                                  mi.IsGenericMethod &&
                                  mi.GetParameters().Length == 5);

                var fieldMethod = genericFieldMethod.MakeGenericMethod(listGraphType);

                fieldMethod.Invoke(this,
                    new object[]
                    {
                        dbSetProperty.Name,
                        $"{dbSetProperty.Name} of type {dbSetType.Name}",
                        new QueryArguments(),
                        (Func<ResolveFieldContext<object>, object>) (resolveContext =>
                            GetDataFromContext(dbSetType, resolveContext)),
                        null
                    });

            }
        }

        private object GetDataFromContext(Type type, ResolveFieldContext<object> resolveContext)
        {
            var getDataMethod = typeof(DynamicQuery<TContext>)
                .GetMethod("GetTypedDataFromContext", BindingFlags.NonPublic | BindingFlags.Static);

            // ReSharper disable once PossibleNullReferenceException
            getDataMethod = getDataMethod.MakeGenericMethod(type);

            using (var scope = _resolver.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();
                return getDataMethod.Invoke(null, new object[] { context, resolveContext });
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<T> GetTypedDataFromContext<T>(DbContext context, ResolveFieldContext resolveContext)
            where T : class
        {
            IQueryable<T> query = context.Set<T>();

            foreach (var subField in resolveContext.SubFields)
            {
                if (subField.Value.SelectionSet?.Children.Any() == true)
                {
                    var actualName = typeof(T)
                        .GetProperties()
                        .First(p => p.Name.Equals(subField.Key, StringComparison.OrdinalIgnoreCase))
                        .Name;

                    query = query.Include(actualName);
                }
            }

            //TODO: Invoke the where clause etc.

            return query.ToList();
        }
    }
}