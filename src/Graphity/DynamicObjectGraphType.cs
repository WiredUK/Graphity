using System;
using System.Collections;
using Graphity.Options;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GraphQL;

namespace Graphity
{
    internal class DynamicObjectGraphType<TContext, TEntity> : ObjectGraphType<TEntity>
        where TContext : DbContext
    {
        public DynamicObjectGraphType(IDbSetConfiguration configuration, Action<Type> typeRegistrar)
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
                else if (property.PropertyType == typeof(Guid))
                {
                    Field<NonNullGraphType<GuidGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(Guid?))
                {
                    Field<GuidGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(byte) ||
                         property.PropertyType == typeof(short) ||
                         property.PropertyType == typeof(int) ||
                         property.PropertyType == typeof(long))
                {
                    Field<NonNullGraphType<IntGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(byte?) ||
                         property.PropertyType == typeof(short?) ||
                         property.PropertyType == typeof(int?) ||
                         property.PropertyType == typeof(long?))
                {
                    Field<IntGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    Field<NonNullGraphType<BooleanGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(bool?))
                {
                    Field<BooleanGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(double) ||
                         property.PropertyType == typeof(float))
                {
                    Field<NonNullGraphType<FloatGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(double?) ||
                         property.PropertyType == typeof(float?))
                {
                    Field<FloatGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(decimal))
                {
                    Field<NonNullGraphType<DecimalGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    Field<DecimalGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    Field<NonNullGraphType<DateTimeGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    Field<DateTimeGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(DateTimeOffset))
                {
                    Field<NonNullGraphType<DateTimeOffsetGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(DateTimeOffset?))
                {
                    Field<DateTimeOffsetGraphType>(property.Name);
                }
                else if (property.PropertyType == typeof(TimeSpan))
                {
                    Field<NonNullGraphType<TimeSpanSecondsGraphType>>(property.Name);
                }
                else if (property.PropertyType == typeof(TimeSpan?))
                {
                    Field<TimeSpanSecondsGraphType>(property.Name);
                }
                else if (property.PropertyType.IsEnum)
                {
                    var enumGraphType = typeof(EnumerationGraphType<>).MakeGenericType(property.PropertyType);
                    var graphType = typeof(NonNullGraphType<>).MakeGenericType(enumGraphType);
                    Field(graphType, property.Name);
                    typeRegistrar(enumGraphType);
                }
                else if (property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericArguments().First().IsEnum)
                {
                    var enumType = property.PropertyType.GetGenericArguments().First();
                    var enumGraphType = typeof(EnumerationGraphType<>).MakeGenericType(enumType);
                    Field(enumGraphType, property.Name);
                    typeRegistrar(enumGraphType);
                }
                else if (property.PropertyType == typeof(byte[]))
                {
                    //Unsupported type, just ignore
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
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