using GraphQL.Types;

namespace Graphity.Ordering
{
    internal class OrderByExpressionType : InputObjectGraphType<OrderByExpression>
    {
        public OrderByExpressionType()
        {
            Description = "The expression type used to specify custom filters.";

            Field(x => x.Path)
                .Description("The property name to order on.");

            Field(x => x.Direction, nullable: true)
                .Description("The direction of ordering. The default is ascending.");
        }
    }
}