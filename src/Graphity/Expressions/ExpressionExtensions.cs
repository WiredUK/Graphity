using System;
using System.Linq.Expressions;

namespace Graphity.Expressions
{
    internal static class ExpressionExtensions
    {
        public static MemberExpression GetPropertyExpression(
            this ParameterExpression parameterExpression,
            string propertyName)
        {
            MemberExpression propertyExp = null;

            //Step down the property hierarchy
            foreach (var member in propertyName.Split('.'))
            {
                propertyExp = Expression.PropertyOrField((Expression)propertyExp ?? parameterExpression, member);
            }

            if (propertyExp == null)
            {
                throw new ArgumentException($"Unable to resolve {propertyName} property", nameof(propertyName));
            }

            return propertyExp;
        }
    }
}