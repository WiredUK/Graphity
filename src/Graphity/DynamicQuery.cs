using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Graphity.Options;
using Graphity.Where;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    public class DynamicQuery<TContext> : ObjectGraphType<object>
        where TContext : DbContext
    {
        private readonly IScopedDependencyResolver _resolver;

        internal static IQueryOptions<TContext> QueryOptions { get; set; }

        public DynamicQuery(IScopedDependencyResolver resolver)
        {
            _resolver = resolver;

            Name = QueryOptions.Name;

            var whereArgument = new QueryArgument<ListGraphType<WhereExpressionType>>
            {
                Name = "where",
                Description = "Filter to apply in format ```{path, comparison, value}```."
            };

            foreach (var dbSetProperty in QueryOptions.GetFields())
            {
                var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), dbSetProperty.Type);
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
                        dbSetProperty.FieldName,
                        $"{dbSetProperty.FieldName} of type {dbSetProperty.TypeName}",
                        new QueryArguments(whereArgument),
                        (Func<ResolveFieldContext<object>, object>) (resolveContext =>
                            GetDataFromContext(dbSetProperty.Type, resolveContext, dbSetProperty)),
                        null
                    });
            }
        }

        private object GetDataFromContext(Type type, ResolveFieldContext<object> resolveContext, IDbSetConfiguration dbSetConfiguration)
        {
            var getDataMethod = typeof(DynamicQuery<TContext>)
                .GetMethod("GetTypedDataFromContext", BindingFlags.NonPublic | BindingFlags.Static);

            // ReSharper disable once PossibleNullReferenceException
            getDataMethod = getDataMethod.MakeGenericMethod(type);

            using (var scope = _resolver.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();
                return getDataMethod.Invoke(null, new object[] { context, resolveContext, dbSetConfiguration });
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<object> GetTypedDataFromContext<TEntity>(
            DbContext context, 
            ResolveFieldContext resolveContext,
            IDbSetConfiguration dbSetConfiguration)
            where TEntity : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>();

            foreach (var subField in resolveContext.SubFields)
            {
                if (subField.Value.SelectionSet?.Children.Any() == true)
                {
                    var actualName = typeof(TEntity)
                        .GetProperties()
                        .First(p => p.Name.Equals(subField.Key, StringComparison.OrdinalIgnoreCase))
                        .Name;

                    query = query.Include(actualName);
                }
            }

            if (dbSetConfiguration.FilterExpression != null)
            {
                query = query.Where((Expression<Func<TEntity, bool>>)dbSetConfiguration.FilterExpression);
            }

            if (resolveContext.Arguments.ContainsKey("where"))
            {
                var whereExpressions = resolveContext.GetArgument<WhereExpression[]>("where");

                foreach (var whereExpression in whereExpressions)
                {
                    var expression = ComparisonExpressions.GetComparisonExpression<TEntity>(
                        whereExpression.Comparison,
                        whereExpression.Path, 
                        whereExpression.Value);

                    query = query.Where(expression);
                }
            }

            return query
                .Select(GetProjectionExpression<TEntity>(resolveContext))
                .ToList();
        }

        private static Expression<Func<TEntity, TEntity>> GetProjectionExpression<TEntity>(ResolveFieldContext resolveContext)
        {
            var parameterExp = Expression.Parameter(typeof(TEntity), "entity");
            var newExpression = Expression.New(typeof(TEntity));
            var memberInitExpression = Expression.MemberInit(
                newExpression, 
                GetBindings<TEntity>(parameterExp, resolveContext));

            return Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, parameterExp);
        }

        private static IEnumerable<MemberBinding> GetBindings<TEntity>(ParameterExpression parameterExp, ResolveFieldContext resolveContext)
        {
            foreach (var subField in resolveContext.SubFields)
            {
                var paramExpression = Expression.Property(parameterExp, subField.Value.Name);
                var mi = typeof(TEntity).GetProperties()
                    .Single(pi => pi.Name.Equals(subField.Value.Name, StringComparison.OrdinalIgnoreCase));

                yield return Expression.Bind(mi, paramExpression);
            }

        }
    }
}