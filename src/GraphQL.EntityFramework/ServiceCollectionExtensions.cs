using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGraphQL<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddTransient<IDocumentExecuter, DocumentExecuter>();

            services.AddScoped<IDependencyResolver>(serviceProvider => new FuncDependencyResolver(serviceProvider.GetRequiredService));
            services.AddScoped<ISchema, DynamicSchema<TContext>>();
            services.AddScoped<DynamicQuery<TContext>>();

            services.AddSingleton<ObjectGraphType>();

            var dbSetProperties = typeof(TContext)
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                             typeof(DbSet<>).IsAssignableFrom(pi.PropertyType.GetGenericTypeDefinition()));

            //var assemblyName = typeof(TContext).Assembly.GetName();

            foreach (var dbSetProperty in dbSetProperties)
            {
                var dbSetType = dbSetProperty.PropertyType.GenericTypeArguments[0];
                var graphType = typeof(DynamicObjectGraphType<>).MakeGenericType(dbSetType);

                services.AddSingleton(graphType);
            }

            return services;
        }
    }
}