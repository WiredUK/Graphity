namespace Graphity
{
    /// <summary>
    /// The visibility of a DbSet.
    /// </summary>
    public enum SetOption
    {
        /// <summary>
        /// The DbSet will only present as a root level field.
        /// </summary>
        IncludeAsFieldOnly,

        /// <summary>
        /// The DbSet will only present as a child af another DbSet and not a field.
        /// </summary>
        IncludeAsChildOnly,

        /// <summary>
        /// The DbSet will present as both a field an as children of other DbSets.
        /// </summary>
        IncludeAsFieldAndChild
    }
}