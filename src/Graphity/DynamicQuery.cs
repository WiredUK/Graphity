using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Graphity.Where;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    public class DynamicQuery<TContext> : ObjectGraphType<object>
        where TContext : DbContext
    {
        private readonly IScopedDependencyResolver _resolver;

        internal static QueryOptions<TContext> QueryOptions { get; set; }

        public DynamicQuery(IScopedDependencyResolver resolver)
        {
            _resolver = resolver;

            Name = $"{typeof(TContext).Name}Query";

            var whereArgument = new QueryArgument<ListGraphType<WhereExpressionType>>
            {
                Name = "where",
                Description = "Filter to apply in format ```{path, comparison, value}```."
            };

            foreach (var dbSetProperty in QueryOptions.GetFields())
            {
                var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), dbSetProperty);
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
                        $"{dbSetProperty.Name} of type {dbSetProperty.Name}",
                        new QueryArguments(whereArgument),
                        (Func<ResolveFieldContext<object>, object>) (resolveContext =>
                            GetDataFromContext(dbSetProperty, resolveContext)),
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

            if (resolveContext.Arguments.ContainsKey("where"))
            {
                var whereExpressions = resolveContext.GetArgument<WhereExpression[]>("where");

                foreach (var whereExpression in whereExpressions)
                {
                    var expression = ComparisonExpressions.GetComparisonExpression<T>(
                        whereExpression.Comparison,
                        whereExpression.Path, 
                        whereExpression.Value);

                    query = query.Where(expression);
                }
            }

            return query.ToList();
        }
    }
}