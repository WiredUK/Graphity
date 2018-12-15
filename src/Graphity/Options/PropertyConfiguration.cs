using System.Linq.Expressions;

namespace Graphity.Options
{
    internal class PropertyConfiguration : IPropertyConfiguration
    {
        public bool Exclude { get; internal set; }
        public MemberExpression PropertyExpression { get; internal set; }
        public LambdaExpression FilterExpression { get; internal set; }

        public string PropertyName => PropertyExpression.Member.Name;
    }
}