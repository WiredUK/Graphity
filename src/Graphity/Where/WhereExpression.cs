// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Graphity.Where
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class WhereExpression
    {
        public string Path { get; set; }
        public Comparison Comparison { get; set; }
        public string Value { get; set; }
    }
}