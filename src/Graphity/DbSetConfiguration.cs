using System;
using System.Linq.Expressions;

namespace Graphity
{
    internal class DbSetConfiguration
    {
        private string _fieldName;
        private string _typeName;

        public Type Type { get; set; }

        public string TypeName
        {
            get => _typeName ?? $"{Type.Name}Type";
            set => _typeName = value;
        }

        public string FieldName
        {
            get => _fieldName ?? Type.Name;
            set => _fieldName = value;
        }

        public SetOption SetOption { get; set; }

        public Expression FilterExpression { get; set; }
    }
}