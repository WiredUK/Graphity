using System.Linq.Expressions;

namespace Graphity.Options
{
    internal class PropertyConfiguration : IPropertyConfiguration
    {
        public bool Exclude { get; internal set; }
        internal MemberExpression PropertyExpression { private get; set; }

        public string PropertyName => PropertyExpression.Member.Name;
    }
}