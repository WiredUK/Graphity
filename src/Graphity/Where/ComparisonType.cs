using System;
using GraphQL.Types;

namespace Graphity.Where
{
    public class ComparisonType : EnumerationGraphType
    {
        public ComparisonType()
        {
            Description = "Specifies the type of comparison to use in the ```where``` expression.";

            AddValue("equal", "Returns records when ```value``` is exactly equal to the value of the ```path``` property.\n\nThis comparison is only valid on *string*, *numeric* and *boolean* properties.", Comparison.Equal);
            AddValue("notEqual", "Returns records when ```value``` is not equal to the value of the ```path``` property.\n\nThis comparison is only valid on *string*, *numeric* and *boolean* properties.", Comparison.NotEqual);
            AddValue("contains", "Returns records when ```value``` is contained inside the the ```path``` property.\n\nThis comparison is only valid on *string* properties.", Comparison.Contains);
            AddValue("greaterThan", "Returns records when ```value``` is greater than the value of the ```path``` property.\n\nThis comparison is only valid on *numeric* properties.", Comparison.GreaterThan);
            AddValue("greaterThanOrEqual", "Returns records when ```value``` is greater than or equal to the value of the ```path``` property.\n\nThis comparison is only valid on *numeric* properties.", Comparison.GreaterThanOrEqual);
            AddValue("lessThan", "Returns records when ```value``` is less than the value of the ```path``` property.\n\nThis comparison is only valid on *numeric* properties.", Comparison.LessThan);
            AddValue("lessThanOrEqual", "Returns records when ```value``` is less than or equal to the value of the ```path``` property.\n\nThis comparison is only valid on *numeric* properties.", Comparison.LessThanOrEqual);
            AddValue("startsWith", "Returns records when ```value``` starts with the value of the ```path``` property.\n\nThis comparison is only valid on *string* properties.", Comparison.StartsWith);
            AddValue("endsWith", "Returns records when ```value``` ends with the value of the ```path``` property.\n\nThis comparison is only valid on *string* properties.", Comparison.EndsWith);
            AddValue("in", "Returns records when the value of the ```path``` property is contained within the items specified in ```value```. ```value``` must be a comma separated list of values.\n\nThis comparison is only valid on *string*, *numeric* and *boolean* properties.", Comparison.In);
        }
    }
}