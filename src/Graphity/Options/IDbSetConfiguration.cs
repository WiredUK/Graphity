using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Graphity.Ordering;

namespace Graphity.Options
{
    public interface IDbSetConfiguration
    {
        Type Type { get; }
        string TypeName { get; }
        string FieldName { get; }
        SetOption SetOption { get; }
        LambdaExpression FilterExpression { get; }
        LambdaExpression DefaultOrderByExpression { get; }
        OrderByDirection OrderByDirection { get; }
        IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations { get; }
    }
}