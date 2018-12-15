using System.Linq.Expressions;

namespace Graphity.Options
{
    public interface IPropertyConfiguration
    {
        bool Exclude { get; }
        MemberExpression PropertyExpression { get; }
        LambdaExpression FilterExpression { get; }
        string PropertyName { get; }
    }
}