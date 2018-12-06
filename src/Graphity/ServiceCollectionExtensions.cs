using System;
using System.Linq;
using GraphQL;
using GraphQL.Types;
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
        /// <param name="setupAction">Use this to configure the graph. Call <see cref="options.ConfigureSet"/> to exclude
        /// and exclude the DbSet properties.</param>
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

            setupAction(queryOptions);
            DynamicQuery<TContext>.QueryOptions = queryOptions;

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IScopedDependencyResolver>(serviceProvider => new ScopedFuncDependencyResolver(serviceProvider.GetRequiredService, serviceProvider.CreateScope));
            services.AddSingleton<ISchema, DynamicSchema<TContext>>();
            services.AddSingleton<DynamicQuery<TContext>>();
            services.AddSingleton<ObjectGraphType>();

            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()));

            foreach (var dbSetProperty in dbSetProperties)
            {
                var dbSetType = dbSetProperty.PropertyType.GenericTypeArguments[0];
                var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), dbSetType);

                services.AddSingleton(graphType,
                    Activator.CreateInstance(graphType, $"{dbSetType.Name}Type"));
            }

            return services;
        }
    }
}