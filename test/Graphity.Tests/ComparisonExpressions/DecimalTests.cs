using System;
using System.Linq;
using Graphity.Tests.Fixtures;
using Graphity.Where;
using Xunit;

namespace Graphity.Tests.ComparisonExpressions
{
    //Decimal tests should cover Float and Double too
    public class DecimalTests : IClassFixture<ComparisonExpressionsFixture>
    {
        private readonly ComparisonExpressionsFixture _comparisonExpressionsFixture;

        public DecimalTests(ComparisonExpressionsFixture comparisonExpressionsFixture)
        {
            _comparisonExpressionsFixture = comparisonExpressionsFixture;
        }

        [Fact]
        public void Decimal_equal_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.Equal,
                "DecimalProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Single(result);
            Assert.Equal(2, result.First().DecimalProperty);
        }

        [Fact]
        public void Decimal_NotEqual_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.NotEqual,
                "DecimalProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.All(result, parent => Assert.NotEqual(2, parent.DecimalProperty));
        }

        [Fact]
        public void Decimal_Contains_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.Contains, 
                    "DecimalProperty", 
                    "1"));
        }

        [Fact]
        public void Decimal_GreaterThan_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThan,
                "DecimalProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(3, result.First().DecimalProperty);
        }

        [Fact]
        public void Decimal_GreaterThanOrEqual_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.GreaterThanOrEqual,
                "DecimalProperty",
                "2");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal(2, result.First().DecimalProperty);
        }

        [Fact]
        public void Decimal_LessThan_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThan,
                "DecimalProperty",
                "3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.First().DecimalProperty);
            Assert.Equal(2, result.Last().DecimalProperty);
        }

        [Fact]
        public void Decimal_LessThanOrEqual_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.LessThanOrEqual,
                "DecimalProperty",
                "3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(1, result.First().DecimalProperty);
            Assert.Equal(3, result.Last().DecimalProperty);
        }

        [Fact]
        public void Decimal_StartsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() => 
                Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.StartsWith, 
                    "DecimalProperty", 
                    "1"));
        }

        [Fact]
        public void Decimal_EndsWith_throws_exception()
        {
            Assert.Throws<GraphityException>(() =>
                Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                    Comparison.EndsWith,
                    "DecimalProperty",
                    "1"));
        }

        [Fact]
        public void Decimal_In_returns_valid_response()
        {
            var expression = Where.ComparisonExpressions.GetComparisonExpression<ComparisonExpressionsFixture.Parent>(
                Comparison.In,
                "DecimalProperty",
                "2,3");

            var result = _comparisonExpressionsFixture.Parents
                .Where(expression.Compile())
                .ToList();

            Assert.Equal(2, result.Count);

            Assert.Equal(2, result.First().DecimalProperty);
            Assert.Equal(3, result.Last().DecimalProperty);
        }
    }
}
