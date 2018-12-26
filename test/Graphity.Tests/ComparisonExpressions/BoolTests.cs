using System.Linq;
using Graphity.Tests.Fixtures;
using Graphity.Where;
using Xunit;

namespace Graphity.Tests.ComparisonExpressions
{
    public class BoolTests : IClassFixture<ComparisonExpressionsFixture>
    {
        private readonly ComparisonExpressionsFixture _comparisonExpressionsFixture;

        public BoolTests(ComparisonExpressionsFixture comparisonExpressionsFixture)
        {
            _comparisonExpressionsFixture = comparisonExpressionsFixture;
        }

        [Fact]
        public void Bool_equal_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.Equal,
                "BoolProperty",
                "true");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.All(result, parent => Assert.True(parent.BoolProperty));
        }

        [Fact]
        public void Bool_NotEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.NotEqual,
                "BoolProperty",
                "true");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, parent => Assert.False(parent.BoolProperty));
        }

        [Fact]
        public void Bool_Contains_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.Contains, 
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_GreaterThan_returns_valid_response()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.GreaterThan,
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_GreaterThanOrEqual_returns_valid_response()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.GreaterThanOrEqual,
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_LessThan_returns_valid_response()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.LessThan,
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_LessThanOrEqual_returns_valid_response()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.LessThanOrEqual,
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_StartsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.StartsWith, 
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_EndsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.EndsWith,
                    "BoolProperty",
                    "true"));
        }

        [Fact]
        public void Bool_In_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.In,
                "BoolProperty",
                "true,false");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(5, result.Count);
        }
    }
}
