using System;
using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.EntityFramework
{
    public class DynamicQuery<TContext> : ObjectGraphType<object>
        where TContext : DbContext
    {
        public DynamicQuery(TContext ctx)
        {
            Name = $"{typeof(TContext).Name}Query";

            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()));
            foreach (var dbSetProperty in dbSetProperties)
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
                            GetDataFromContext(ctx, dbSetType, resolveContext)),
                        null
                    });

            }
        }

        private object GetDataFromContext(TContext context, Type type, ResolveFieldContext<object> resolveContext)
        {
            var genericMethod = typeof(TContext).GetMethod("Set");
            var method = genericMethod.MakeGenericMethod(type);

            var queryable = method.Invoke(context, null);

            //TODO: Invoke the includes, where clause etc.

            var genericToListMethod = typeof(Enumerable).GetMethod("ToList");
            var toListMethod = genericToListMethod.MakeGenericMethod(type);

            return toListMethod.Invoke(null, new[] {queryable});
        }
    }
}