using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity.Options
{
    public class DbSetConfigurationQueryOptions<TContext, TProperty> : IDbSetConfigurationQueryOptions<TContext, TProperty>
        where TContext : DbContext
        where TProperty : class
    {
        private readonly QueryOptions<TContext> _options;
        private readonly DbSetConfiguration _dbSetConfiguration;

        internal DbSetConfigurationQueryOptions(QueryOptions<TContext> options, DbSetConfiguration dbSetConfiguration)
        {
            _options = options;
            _dbSetConfiguration = dbSetConfiguration;
        }

        public string Name => _options.Name;
        public ICollection<IDbSetConfiguration> DbSetConfigurations => _options.DbSetConfigurations;

        public DbSetConfigurationQueryOptions<TContext, TProperty> Filter(
            Expression<Func<TProperty, bool>> defaultFilter)
        {
            _dbSetConfiguration.FilterExpression = defaultFilter;
            return this;
        }

        public IQueryOptions<TContext> QueryName(string name)
        {
            return _options.QueryName(name);
        }

        public IDbSetConfigurationQueryOptions<TContext, TProperty> FieldName(string name)
        {
            _dbSetConfiguration.FieldName = name;
            return this;
        }

        public IDbSetConfigurationQueryOptions<TContext, TProperty> TypeName(string name)
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


        public IDbSetConfigurationQueryOptions<TContext, TProperty> FilterExpression(Expression<Func<TProperty, bool>> defaultFilter)
        {
            _dbSetConfiguration.FilterExpression = defaultFilter;
            return this;
        }
    }
}