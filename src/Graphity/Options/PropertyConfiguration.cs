using System.Linq.Expressions;

namespace Graphity.Options
{
    public class PropertyConfiguration : IPropertyConfiguration
    {
        public bool Exclude { get; internal set; }
        public Expression PropertyExpression { get; internal set; }

        public string PropertyName
        {
            get
            {
                var lambdaExpression = (LambdaExpression)PropertyExpression;
                var memberExpression = (MemberExpression)lambdaExpression.Body;
                return memberExpression.Member.Name;
            }
        }
    }
}