using System.Linq.Expressions;

namespace Graphity.Options
{
    public interface IPropertyConfiguration
    {
        bool Exclude { get; }
        Expression PropertyExpression { get; }
        string PropertyName { get; }
    }
}