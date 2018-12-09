using System;
using System.Linq.Expressions;

namespace Graphity
{
    public interface IDbSetConfiguration
    {
        Type Type { get; }
        string TypeName { get; }
        string FieldName { get; }
        SetOption SetOption { get; }
        Expression FilterExpression { get; }
    }
}