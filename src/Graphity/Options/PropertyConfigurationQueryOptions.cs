using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Graphity.Ordering;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    internal class PropertyConfigurationQueryOptions<TContext, TEntity, TProperty>
        : IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly DbSetConfigurationQueryOptions<TContext, TEntity> _options;
        private readonly DbSetConfiguration _dbSetConfiguration;
        private readonly PropertyConfiguration _propertyConfiguration;

        internal PropertyConfigurationQueryOptions(DbSetConfigurationQueryOptions<TContext, TEntity> options, DbSetConfiguration dbSetConfiguration, PropertyConfiguration propertyConfiguration)
        {
            _options = options;
            _dbSetConfiguration = dbSetConfiguration;
            _propertyConfiguration = propertyConfiguration;
        }

        public string Name => _options.Name;
        public int DefaultTake => _options.DefaultTake;

        public IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations => _options.DbSetConfigurations;

        public IQueryOptions<TContext> QueryName(string name)
        {
            return _options.QueryName(name);
        }

        public IQueryOptions<TContext> SetDefaultTake(int defaultTake)
        {
            return _options.SetDefaultTake(defaultTake);
        }

        public IDbSetConfigurationQueryOptions<TContext, TProperty1> ConfigureSet<TProperty1>(
            Expression<Func<TContext, DbSet<TProperty1>>> dbSetExpression, string fieldName,
            SetOption setOption = SetOption.IncludeAsFieldAndChild,
            Expression<Func<TProperty1, bool>> defaultFilter = null)
            where TProperty1 : class
        {
            return _options.ConfigureSet(dbSetExpression, fieldName, setOption, defaultFilter);
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> FieldName(string name)
        {
            _dbSetConfiguration.FieldName = name;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> TypeName(string name)
        {
            _dbSetConfiguration.TypeName = name;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> Filter(Expression<Func<TEntity, bool>> defaultFilter)
        {
            _dbSetConfiguration.FilterExpression = defaultFilter;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TEntity> DefaultOrderBy(
            Expression<Func<TEntity, object>> defaultOrderBy,
            OrderByDirection orderByDirection = OrderByDirection.Ascending)
        {
            _dbSetConfiguration.DefaultOrderByExpression = defaultOrderBy;
            _dbSetConfiguration.OrderByDirection = orderByDirection;
            return this;
        }

        public IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty1> ConfigureProperty<TProperty1>(Expression<Func<TEntity, TProperty1>> propertyExpression)
        {
            return _options.ConfigureProperty(propertyExpression);
        }

        public IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> Exclude()
        {
            _propertyConfiguration.Exclude = true;
            return this;
        }
    }
}