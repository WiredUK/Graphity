using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Graphity.Options
{
    public interface IDbSetConfiguration
    {
        Type Type { get; }
        string TypeName { get; }
        string FieldName { get; }
        SetOption SetOption { get; }
        LambdaExpression FilterExpression { get; }
        IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations { get; }
    }
}