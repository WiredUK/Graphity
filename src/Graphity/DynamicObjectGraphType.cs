using GraphQL.Types;

namespace Graphity
{
    public class DynamicObjectGraphType<TEntity> : ObjectGraphType<TEntity>
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
                else
                {
                    var graphType = typeof(DynamicObjectGraphType<>).MakeGenericType(property.PropertyType);
                    Field(graphType, property.Name);
                }
            }
        }
    }
}