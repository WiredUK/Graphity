using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Graphity.Ordering;

namespace Graphity.Options
{
    /// <summary>
    /// The interface for configuring an individual DbSet field.
    /// </summary>
    public interface IDbSetConfiguration
    {
        /// <summary>
        /// The type of the DbSet.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// The name of the type.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// The name of the field.
        /// </summary>
        string FieldName { get; }

        /// <summary>
        /// The visibility option of the set.
        /// </summary>
        SetOption SetOption { get; }

        /// <summary>
        /// Configure an authorisation policy to apply to this field.
        /// </summary>
        string AuthorisationPolicy { get; }

        /// <summary>
        /// The expression used to filter the data.
        /// </summary>
        LambdaExpression FilterExpression { get; }

        /// <summary>
        /// The expression used to order the set.
        /// </summary>
        LambdaExpression DefaultOrderByExpression { get; }

        /// <summary>
        /// The default ordering direction used by <see cref="DefaultOrderByExpression"/>.
        /// </summary>
        OrderByDirection OrderByDirection { get; }

        /// <summary>
        /// The configurations applied to individual properties.
        /// </summary>
        IReadOnlyCollection<IPropertyConfiguration> PropertyConfigurations { get; }
    }
}