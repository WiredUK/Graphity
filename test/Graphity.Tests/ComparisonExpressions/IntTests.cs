using System.Linq;
using Graphity.Tests.Fixtures;
using Graphity.Where;
using Xunit;

namespace Graphity.Tests.ComparisonExpressions
{
    //Int tests should cover Short and Long too
    public class IntTests : IClassFixture<ComparisonExpressionsFixture>
    {
        private readonly ComparisonExpressionsFixture _comparisonExpressionsFixture;

        public IntTests(ComparisonExpressionsFixture comparisonExpressionsFixture)
        {
            _comparisonExpressionsFixture = comparisonExpressionsFixture;
        }

        [Fact]
        public void Int_equal_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.Equal,
                "IntProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Single(result);
            Assert.Equal(2, result.First().IntProperty);
        }

        [Fact]
        public void Int_NotEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.NotEqual,
                "IntProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.All(result, parent => Assert.NotEqual(2, parent.IntProperty));
        }

        [Fact]
        public void Int_Contains_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.Contains, 
                    "IntProperty", 
                    "1"));
        }

        [Fact]
        public void Int_GreaterThan_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThan,
                "IntProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(3, result.First().IntProperty);
        }

        [Fact]
        public void Int_GreaterThanOrEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThanOrEqual,
                "IntProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal(2, result.First().IntProperty);
        }

        [Fact]
        public void Int_LessThan_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThan,
                "IntProperty",
                "3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.First().IntProperty);
            Assert.Equal(2, result.Last().IntProperty);
        }

        [Fact]
        public void Int_LessThanOrEqual_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThanOrEqual,
                "IntProperty",
                "3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(1, result.First().IntProperty);
            Assert.Equal(3, result.Last().IntProperty);
        }

        [Fact]
        public void Int_StartsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.StartsWith, 
                    "IntProperty", 
                    "1"));
        }

        [Fact]
        public void Int_EndsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() =>
                Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.EndsWith,
                    "IntProperty",
                    "1"));
        }

        [Fact]
        public void Int_In_returns_valid_response()
        {
            var expression = Expressions.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.In,
                "IntProperty",
                "2,3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);

            Assert.Equal(2, result.First().IntProperty);
            Assert.Equal(3, result.Last().IntProperty);
        }
    }
}
