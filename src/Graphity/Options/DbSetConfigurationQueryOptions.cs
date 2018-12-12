using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public class DbSetConfigurationQueryOptions<TContext, TEntity> : IDbSetConfigurationQueryOptions<TContext, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        private readonly QueryOptions<TContext> _options;
        private readonly DbSetConfiguration _dbSetConfiguration;

        internal DbSetConfigurationQueryOptions(QueryOptions<TContext> options, DbSetConfiguration dbSetConfiguration)
        {
            _options = options;
            _dbSetConfiguration = dbSetConfiguration;
        }

        public string Name => _options.Name;
        public IReadOnlyCollection<IDbSetConfiguration> DbSetConfigurations => _options.DbSetConfigurations;

        public IDbSetConfigurationQueryOptions<TContext, TEntity> Filter(
            Expression<Func<TEntity, bool>> defaultFilter)
        {
            _dbSetConfiguration.FilterExpression = defaultFilter;
            return this;
        }

        public IQueryOptions<TContext> QueryName(string name)
        {
            return _options.QueryName(name);
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

        public IDbSetConfigurationQueryOptions<TContext, TProperty1> ConfigureSet<TProperty1>(
            Expression<Func<TContext, DbSet<TProperty1>>> dbSetExpression, string fieldName,
            SetOption setOption = SetOption.IncludeAsFieldAndChild, 
            Expression<Func<TProperty1, bool>> defaultFilter = null) 
            where TProperty1 : class
        {
            return _options.ConfigureSet(dbSetExpression, fieldName, setOption, defaultFilter);
        }

        public IPropertyConfigurationQueryOptions<TContext, TEntity, TProperty> ConfigureProperty<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            var propertyConfiguration = new PropertyConfiguration
            {
                PropertyExpression = (MemberExpression)propertyExpression.Body
            };

            ((List<IPropertyConfiguration>) _dbSetConfiguration.PropertyConfigurations).Add(propertyConfiguration);

            return new PropertyConfigurationQueryOptions<TContext, TEntity, TProperty>(this, _dbSetConfiguration, propertyConfiguration);
        }

        public IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations =>
            _dbSetConfiguration.PropertyConfigurations;
    }
}