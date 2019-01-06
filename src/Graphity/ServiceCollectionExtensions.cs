using System;
using System.Collections.Generic;
using System.Linq;
using Graphity.Authorisation;
using Graphity.Options;
using Graphity.Ordering;
using Graphity.Where;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQL.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    /// <summary>
    /// The methods used to configure Graphity and add it to the DI container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Graphity with all context properties enabled
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static IServiceCollection AddGraphity<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return services.AddGraphity<TContext>(null);
        }

        /// <summary>
        /// Add Graphity with a user defined configuration
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="setupAction">Use this to configure the graph.</param>
        /// <returns></returns>
        public static IServiceCollection AddGraphity<TContext>(
            this IServiceCollection services, 
            Action<IQueryOptions<TContext>> setupAction)
            where TContext : DbContext
        {
            var contextService = services.SingleOrDefault(sd => sd.ImplementationType == typeof(TContext));

            if (contextService == null)
            {
                throw new GraphityException("Unable to configure Graphity when the context service hasn't been registered");
            }

            var queryOptions = new QueryOptions<TContext>();

            setupAction?.Invoke(queryOptions);
            DynamicQuery<TContext>.QueryOptions = queryOptions;

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IScopedDependencyResolver>(serviceProvider => new ScopedFuncDependencyResolver(serviceProvider.GetRequiredService, serviceProvider.CreateScope));
            services.AddSingleton<ISchema, DynamicSchema<TContext>>();
            services.AddSingleton<DynamicQuery<TContext>>();
            services.AddSingleton<ObjectGraphType>();
            services.AddSingleton<GuidGraphType>();

            //Where
            services.AddSingleton<WhereExpressionType>();
            services.AddSingleton<ComparisonType>();
            GraphTypeTypeRegistry.Register<Comparison, ComparisonType>();
            GraphTypeTypeRegistry.Register<WhereExpression, WhereExpressionType>();

            //Ordering
            services.AddSingleton<OrderByExpressionType>();
            services.AddSingleton<OrderByDirectionType>();
            GraphTypeTypeRegistry.Register<OrderByDirection, OrderByDirectionType>();
            GraphTypeTypeRegistry.Register<OrderByExpression, OrderByExpressionType>();

            foreach (var field in queryOptions.GetAllFields())
            {
                var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), field.Type);

                services.AddSingleton(
                    graphType,
                    Activator.CreateInstance(graphType, field, (Action<Type>) TypeRegistrar));
            }

            AddGraphQLAuth(services, queryOptions.AuthorisationPolicies);

            void TypeRegistrar(Type type) => services.AddSingleton(type);

            return services;
        }

        private static void AddGraphQLAuth(
            IServiceCollection services,
            IReadOnlyCollection<NamedAuthorisationPolicy> policies)
        {
            services.AddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.AddSingleton(provider =>
            {
                var authSettings = new AuthorizationSettings();

                if (policies != null && policies.Any())
                {
                    foreach (var policy in policies)
                    {
                        authSettings.AddPolicy(policy.Name, policy.AuthorizationPolicy);
                    }
                }

                return authSettings;
            });

            services.AddSingleton(new GraphQLSettings
            {
                BuildUserContext = ctx => new GraphQLUserContext
                {
                    User = ctx.User
                }
            });
        }
    }
}