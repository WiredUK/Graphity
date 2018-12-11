using System;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Graphity.Options;
using Graphity.Where;
using GraphQL.Language.AST;
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
    }
}