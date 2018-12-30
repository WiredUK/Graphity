namespace Graphity.Options
{
    /// <summary>
    /// The configuration of an individual field property.
    /// </summary>
    public interface IPropertyConfiguration
    {
        /// <summary>
        /// Exclude the property from the graph.
        /// </summary>
        bool Exclude { get; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        string PropertyName { get; }
    }
}