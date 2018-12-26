using System;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Graphity.Options;
using Graphity.Ordering;
using Graphity.Where;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    internal class DynamicQuery<TContext> : ObjectGraphType<object>
        where TContext : DbContext
    {
        private readonly IScopedDependencyResolver _resolver;

        internal static IQueryOptions<TContext> QueryOptions { get; set; }

        public DynamicQuery(IScopedDependencyResolver resolver)
        {
            _resolver = resolver;

            Name = QueryOptions.Name;

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
                        new QueryArguments(GetDefaultArguments(QueryOptions.DefaultTake)),
                        (Func<ResolveFieldContext<object>, object>) (resolveContext =>
                            GetDataFromContext(dbSetProperty.Type, resolveContext, dbSetProperty)),
                        null
                    });
            }
        }

        private object GetDataFromContext(Type type, ResolveFieldContext<object> resolveContext,
            IDbSetConfiguration dbSetConfiguration)
        {
            var getDataMethod = typeof(DynamicQuery<TContext>)
                .GetMethod(nameof(GetTypedDataFromContext), BindingFlags.NonPublic | BindingFlags.Instance);

            // ReSharper disable once PossibleNullReferenceException
            getDataMethod = getDataMethod.MakeGenericMethod(type);

            using (var scope = _resolver.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();
                return getDataMethod.Invoke(this, new object[] {context, resolveContext, dbSetConfiguration});
            }
        }

        // ReSharper disable once UnusedMember.Local
        private IEnumerable<object> GetTypedDataFromContext<TEntity>(
            DbContext context,
            ResolveFieldContext resolveContext,
            IDbSetConfiguration dbSetConfiguration)
            where TEntity : class
        {
            IQueryable<TEntity> query = context.Set<TEntity>();

            if (dbSetConfiguration.FilterExpression != null)
            {
                query = query.Where((Expression<Func<TEntity, bool>>) dbSetConfiguration.FilterExpression);
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

            if (resolveContext.Arguments.ContainsKey("orderBy"))
            {
                var orderByExpression = resolveContext.GetArgument<OrderByExpression>("orderBy");
                query = ApplyCustomOrderBy(query, orderByExpression);
            }
            else if (dbSetConfiguration.DefaultOrderByExpression != null)
            {
                query = dbSetConfiguration.OrderByDirection == OrderByDirection.Ascending
                    ? query.OrderBy((Expression<Func<TEntity, object>>)dbSetConfiguration.DefaultOrderByExpression) 
                    : query.OrderByDescending((Expression<Func<TEntity, object>>)dbSetConfiguration.DefaultOrderByExpression);
            }

            if (resolveContext.Arguments.ContainsKey("skip"))
            {
                var skip = resolveContext.GetArgument<int>("skip");
                query = query.Skip(skip);
            }

            var take = resolveContext.GetArgument<int>("take");
            query = query.Take(take);

            var projectionExpression = (Expression<Func<TEntity, TEntity>>)GetProjectionExpression(typeof(TEntity), resolveContext.SubFields.Values);

            return query
                .Select(projectionExpression)
                .ToList();
        }

        private int _entityCounter;

        private Expression GetProjectionExpression(Type type, IEnumerable<Field> fields)
        {
            _entityCounter++;

            var parameterExp = Expression.Parameter(type, $"e{_entityCounter}");
            var newExpression = Expression.New(type);
            var memberInitExpression = Expression.MemberInit(
                newExpression,
                GetBindings(type, parameterExp, fields));

            return Expression.Lambda(memberInitExpression, parameterExp);
        }

        private IEnumerable<MemberBinding> GetBindings(Type type, Expression parameterExp,
            IEnumerable<Field> fields)
        {
            return fields
                .Where(subField => !subField.Name.StartsWith("__"))
                .Select(subField => GetBinding(type, parameterExp, subField))
                .ToList();
        }

        private MemberBinding GetBinding(Type type, Expression parameterExp, Field subField)
        {
            var mi = type.GetProperties()
                .Single(pi => pi.Name.Equals(subField.Name, StringComparison.OrdinalIgnoreCase));

            if (mi.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(mi.PropertyType))
            {
                return GetSelectMemberBinding(parameterExp, subField, mi);
            }

            if (mi.PropertyType.IsClass && mi.PropertyType != typeof(string))
            {
                return GetClassMemberBinding(parameterExp, subField, mi);
            }

            return GetDefaultMemberBinding(parameterExp, subField, mi);
        }

        private static MemberBinding GetDefaultMemberBinding(Expression parameterExp, Field subField, PropertyInfo mi)
        {
            var paramExpression = Expression.Property(parameterExp, subField.Name);
            return Expression.Bind(mi, paramExpression);
        }

        private MemberBinding GetClassMemberBinding(Expression parameterExp, Field subField, PropertyInfo mi)
        {
            var innerParameterExp = Expression.Property(parameterExp, subField.Name);
            var newExpression = Expression.New(mi.PropertyType);
            var memberInitExpression = Expression.MemberInit(
                newExpression,
                GetBindings(mi.PropertyType, innerParameterExp, subField.SelectionSet.Children.OfType<Field>()));

            return Expression.Bind(mi, memberInitExpression);
        }

        private MemberBinding GetSelectMemberBinding(Expression parameterExp, Field subField, PropertyInfo mi)
        {
            var selectParamExpression = Expression.Property(parameterExp, subField.Name);

            var enumerableType = mi.PropertyType.GetGenericArguments()[0];
            var child = GetProjectionExpression(enumerableType, subField.SelectionSet.Children.OfType<Field>());

            var selectExpression = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Select),
                new[] { enumerableType, enumerableType },
                selectParamExpression, child);

            var toListExpression = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.ToList),
                new[] { enumerableType },
                selectExpression);

            return Expression.Bind(mi, toListExpression);
        }

        private IEnumerable<QueryArgument> GetDefaultArguments(int defaultTake)
        {
            yield return new QueryArgument<ListGraphType<WhereExpressionType>>
            {
                Name = "where",
                Description = "Filter to apply in format ```{path, comparison, value}```."
            };

            yield return new QueryArgument<IntGraphType>
            {
                Name = "skip",
                Description = "The number of records to skip over"
            };

            yield return new QueryArgument<IntGraphType>
            {
                Name = "take",
                Description = $"The number of records to return, if omitted the default is {defaultTake}",
                DefaultValue = defaultTake
            };

            yield return new QueryArgument<IntGraphType>
            {
                Name = "take",
                Description = $"The number of records to return, if omitted the default is {defaultTake}",
                DefaultValue = defaultTake
            };

            yield return new QueryArgument<OrderByExpressionType>
            {
                Name = "orderBy",
                Description = "Specify the order in format ```{path, direction}```."
            };
        }

        private IQueryable<TEntity> ApplyCustomOrderBy<TEntity>(IQueryable<TEntity> source, OrderByExpression orderByExpression)
        {
            var parameterExp = Expression.Parameter(typeof(TEntity), "entity");
            var propertyExp = Expression.Property(parameterExp, orderByExpression.Path);

            var methodName = orderByExpression.Direction == OrderByDirection.Ascending
                ? "OrderBy"
                : "OrderByDescending";

            var orderByMethodGeneric = typeof(Queryable).GetMethods()
                .Single(mi => mi.Name == methodName && mi.GetParameters().Length == 2);

            var orderByMethod = orderByMethodGeneric.MakeGenericMethod(typeof(TEntity), propertyExp.Type);

            return (IQueryable<TEntity>) orderByMethod.Invoke(null, new object[] { source, Expression.Lambda(propertyExp, parameterExp)});
        }

    }
}