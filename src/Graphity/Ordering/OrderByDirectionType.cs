using GraphQL.Types;

namespace Graphity.Ordering
{
    internal class OrderByDirectionType : EnumerationGraphType
    {
        public OrderByDirectionType()
        {
            Description = "Specifies the direction of ordering.";

            AddValue("ascending", "Order by ascending", OrderByDirection.Ascending);
            AddValue("descending", "Order by descending", OrderByDirection.Descending);
        }
    }
}