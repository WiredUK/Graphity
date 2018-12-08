using System;
using System.Linq;
using Graphity.Where;
using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Graphity with all context properties enabled
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
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
        public static IServiceCollection AddGraphity<TContext>(this IServiceCollection services, Action<QueryOptions<TContext>> setupAction)
            where TContext : DbContext
        {
            var contextService = services.SingleOrDefault(sd => sd.ImplementationType == typeof(TContext));

            if (contextService == null)
            {
                throw new GraphityException("Unable to configure Graphity when the context service hasn't been registered");
            }

            var queryOptions = new QueryOptions<TContext>
            {
                ServiceLifetime = contextService.Lifetime
            };

            setupAction?.Invoke(queryOptions);
            DynamicQuery<TContext>.QueryOptions = queryOptions;

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IScopedDependencyResolver>(serviceProvider => new ScopedFuncDependencyResolver(serviceProvider.GetRequiredService, serviceProvider.CreateScope));
            services.AddSingleton<ISchema, DynamicSchema<TContext>>();
            services.AddSingleton<DynamicQuery<TContext>>();
            services.AddSingleton<ObjectGraphType>();

            //Where
            services.AddSingleton<WhereExpressionType>();
            services.AddSingleton<ComparisonType>();
            GraphTypeTypeRegistry.Register<Comparison, ComparisonType>();
            GraphTypeTypeRegistry.Register<WhereExpression, WhereExpressionType>();

            foreach (var type in queryOptions.GetFields())
            {
                var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), type);

                services.AddSingleton(graphType,
                    Activator.CreateInstance(graphType, $"{type.Name}Type"));
            }

            return services;
        }
    }
}