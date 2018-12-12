using System.Linq.Expressions;

namespace Graphity.Options
{
    public interface IPropertyConfiguration
    {
        bool Exclude { get; }
        MemberExpression PropertyExpression { get; }
        string PropertyName { get; }
    }
}