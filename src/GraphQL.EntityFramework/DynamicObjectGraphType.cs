using GraphQL.Types;

namespace GraphQL.EntityFramework
{
    public class DynamicObjectGraphType<TEntity> : ObjectGraphType<TEntity>
    {
        public DynamicObjectGraphType()
        {
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

            }
        }
    }
}