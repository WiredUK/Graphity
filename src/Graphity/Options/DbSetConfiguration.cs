using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Graphity.Ordering;

namespace Graphity.Options
{
    internal class DbSetConfiguration : IDbSetConfiguration
    {
        private string _fieldName;
        private string _typeName;

        public DbSetConfiguration()
        {
            PropertyConfigurations = new List<IPropertyConfiguration>();
        }

        public Type Type { get; internal set; }

        public string TypeName
        {
            get => _typeName ?? $"{Type.Name}Type";
            internal set => _typeName = value;
        }

        public string FieldName
        {
            get => _fieldName ?? Type.Name;
            internal set => _fieldName = value;
        }

        public SetOption SetOption { get; internal set; }
        public string AuthorisationPolicy { get; internal set; }

        public LambdaExpression FilterExpression { get; internal set; }
        public LambdaExpression DefaultOrderByExpression { get; internal set; }
        public OrderByDirection OrderByDirection { get; internal set; }

        public IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations { get; }
        public bool IsQuery { get; internal set; }
    }
}