using System.Collections;
using Graphity.Options;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Graphity
{
    public class DynamicObjectGraphType<TContext, TEntity> : ObjectGraphType<TEntity>
        where TContext : DbContext
    {
        public DynamicObjectGraphType(IDbSetConfiguration configuration)
        {
            Name = configuration.TypeName;

            var properties = typeof(TEntity)
                .GetProperties();

            foreach (var property in properties)
            {
                if (configuration.PropertyConfigurations.Any(pc => pc.PropertyName == property.Name && pc.Exclude))
                {
                    continue;
                }

                if (property.PropertyType == typeof(string))
                {
                    Field<StringGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(int))
                {
                    Field<IntGraphType>(property.Name);
                }
                else if(typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var type = property.PropertyType.GetGenericArguments()[0];

                    var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), type);
                    var listGraphType = typeof(ListGraphType<>).MakeGenericType(graphType);
                    Field(listGraphType, property.Name);
                }
                else if(DynamicQuery<TContext>.QueryOptions.CanBeAChild(property.PropertyType))
                {
                    var graphType = typeof(DynamicObjectGraphType<,>).MakeGenericType(typeof(TContext), property.PropertyType);
                    Field(graphType, property.Name);
                }
            }
        }
    }
}