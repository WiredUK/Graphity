using GraphQL.Types;

namespace Graphity.Where
{
    internal class WhereExpressionType : InputObjectGraphType<WhereExpression>
    {
        public WhereExpressionType()
        {
            Description = "The expression type used to specify custom filters.";

            Field(x => x.Path).Description("The property name to filter on.");
            Field(x => x.Comparison).Description("The type of comparison to apply to the property specified in the path.");
            Field(x => x.Value).Description("The value to use for the comparison. For compound comparisons, use a comma separated list, for example ```\"1,2,3\"```.");
        }
    }
}