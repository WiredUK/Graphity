using System.Linq;
using Graphity.Tests.Fixtures;
using Graphity.Where;
using Xunit;

namespace Graphity.Tests.ComparisonExpressions
{
    public class StringTests : IClassFixture<ComparisonExpressionsFixture>
    {
        private readonly ComparisonExpressionsFixture _comparisonExpressionsFixture;

        public StringTests(ComparisonExpressionsFixture comparisonExpressionsFixture)
        {
            _comparisonExpressionsFixture = comparisonExpressionsFixture;
        }

        [Fact]
        public void String_equal_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.Equal,
                "StringProperty",
                "string2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Single(result);
            Assert.Equal("string2", result.First().StringProperty);
        }

        [Fact]
        public void String_NotEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.NotEqual,
                "StringProperty",
                "string2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.All(result, parent => Assert.NotEqual("string2", parent.StringProperty));
        }

        [Fact]
        public void String_Contains_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.Contains,
                "StringProperty",
                "ring2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Single(result);
            Assert.Equal("string2", result.First().StringProperty);
        }

        [Fact]
        public void String_GreaterThan_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThan,
                "StringProperty",
                "string2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal("string3", result.First().StringProperty);
        }

        [Fact]
        public void String_GreaterThanOrEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThanOrEqual,
                "StringProperty",
                "string2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal("string2", result.First().StringProperty);
        }

        [Fact]
        public void String_LessThan_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThan,
                "StringProperty",
                "string3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("string1", result.First().StringProperty);
            Assert.Equal("string2", result.Last().StringProperty);
        }

        [Fact]
        public void String_LessThanOrEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThanOrEqual,
                "StringProperty",
                "string3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal("string1", result.First().StringProperty);
            Assert.Equal("string3", result.Last().StringProperty);
        }

        [Fact]
        public void String_StartsWith_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.StartsWith,
                "StringProperty",
                "str");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void String_EndsWith_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.EndsWith,
                "StringProperty",
                "ring2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Single(result);

            var item = result.First();
            Assert.Equal("string2", item.StringProperty);
        }

        [Fact]
        public void String_In_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.In,
                "StringProperty",
                "string2,string3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);

            Assert.Equal("string2", result.First().StringProperty);
            Assert.Equal("string3", result.Last().StringProperty);
        }
    }
}
