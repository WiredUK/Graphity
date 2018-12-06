using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    public class DynamicObjectGraphType<TContext, TEntity> : ObjectGraphType<TEntity>
        where TContext : DbContext
    {
        public DynamicObjectGraphType(string name)
        {
            Name = name;

            var properties = typeof(TEntity)
                .GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    Field<StringGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(int))
                {
                    Field<IntGraphType>(property.Name);
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