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
        public static IServiceCollection AddGraphQL<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            return services.AddGraphQL<TContext>(null);

        }

        public static IServiceCollection AddGraphQL<TContext>(this IServiceCollection services, Action<QueryOptions<TContext>> setupAction)
            where TContext : DbContext
        {
            var queryOptions = new QueryOptions<TContext>();
            setupAction(queryOptions);
            DynamicQuery<TContext>.QueryOptions = queryOptions;

            services.AddTransient<IDocumentExecuter, DocumentExecuter>();

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
                var graphType = typeof(DynamicObjectGraphType<>).MakeGenericType(dbSetType);

                services.AddSingleton(graphType,
                    Activator.CreateInstance(graphType, $"{dbSetType.Name}Type"));
            }

            return services;
        }
    }
}