namespace Graphity.Options
{
    public interface IPropertyConfiguration
    {
        bool Exclude { get; }
        string PropertyName { get; }
    }
}