using System;
using System.Linq;
using System.Linq.Expressions;
using Graphity.Where;

namespace Graphity.Expressions
{
    internal static class ComparisonExpressions
    {
        public static Expression<Func<T, bool>> GetComparisonExpression<T>(Comparison comparison, string propertyName,
            string propertyValue)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return Equal<T>(propertyName, propertyValue);
                case Comparison.NotEqual:
                    return NotEqual<T>(propertyName, propertyValue);
                case Comparison.Contains:
                    return Contains<T>(propertyName, propertyValue);
                case Comparison.GreaterThan:
                    return GreaterThan<T>(propertyName, propertyValue);
                case Comparison.GreaterThanOrEqual:
                    return GreaterThanOrEqual<T>(propertyName, propertyValue);
                case Comparison.LessThan:
                    return LessThan<T>(propertyName, propertyValue);
                case Comparison.LessThanOrEqual:
                    return LessThanOrEqual<T>(propertyName, propertyValue);
                case Comparison.StartsWith:
                    return StartsWith<T>(propertyName, propertyValue);
                case Comparison.EndsWith:
                    return EndsWith<T>(propertyName, propertyValue);
                case Comparison.In:
                    return In<T>(propertyName, propertyValue);

                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), "Invalid comparison type specified");
            }
        }

        private static Expression<Func<T, bool>> Equal<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);
            var method = propertyExp.Type.GetMethod("Equals", new[] {propertyExp.Type});
            var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
            // ReSharper disable once AssignNullToNotNullAttribute
            var methodExpression = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> NotEqual<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);
            var method = propertyExp.Type.GetMethod("Equals", new[] {propertyExp.Type});
            var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
            // ReSharper disable once AssignNullToNotNullAttribute
            var methodExpression = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(Expression.Not(methodExpression), parameterExp);
        }

        private static Expression<Func<T, bool>> Contains<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            if (propertyExp.Type != typeof(string))
            {
                throw new GraphityException(
                    $"The 'Contains' comparison cannot be made against a property of type {propertyExp.Type.Name}, did you mean to use 'In'?");
            }

            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var someValue = Expression.Constant(propertyValue, typeof(string));
            // ReSharper disable once AssignNullToNotNullAttribute
            var methodExpression = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> GreaterThan<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            Expression methodExpression;

            if (propertyExp.Type == typeof(bool))
            {
                throw new GraphityException(
                    $"The 'GreaterThan' comparison cannot be made against a property of type bool");
            }

            if (propertyExp.Type == typeof(string))
            {       
                //For strings, we need to use string.Compare(value) > 0
                var method = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                // ReSharper disable once AssignNullToNotNullAttribute
                var compareExpression = Expression.Call(null, method, propertyExp, someValue);
                methodExpression = Expression.GreaterThan(compareExpression, Expression.Constant(0));
            }
            else
            {
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                methodExpression = Expression.GreaterThan(propertyExp, someValue);
            }

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> GreaterThanOrEqual<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            Expression methodExpression;
            if (propertyExp.Type == typeof(bool))
            {
                throw new GraphityException(
                    $"The 'GreaterThanOrEqual' comparison cannot be made against a property of type bool");
            }

            if (propertyExp.Type == typeof(string))
            {
                //For strings, we need to use string.Compare(value) > 0
                var method = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                // ReSharper disable once AssignNullToNotNullAttribute
                var compareExpression = Expression.Call(null, method, propertyExp, someValue);
                methodExpression = Expression.GreaterThanOrEqual(compareExpression, Expression.Constant(0));
            }
            else
            {
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                methodExpression = Expression.GreaterThanOrEqual(propertyExp, someValue);
            }

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> LessThan<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            Expression methodExpression;

            if (propertyExp.Type == typeof(bool))
            {
                throw new GraphityException(
                    $"The 'LessThan' comparison cannot be made against a property of type bool");
            }

            if (propertyExp.Type == typeof(string))
            {
                //For strings, we need to use string.Compare(value) > 0
                var method = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                // ReSharper disable once AssignNullToNotNullAttribute
                var compareExpression = Expression.Call(null, method, propertyExp, someValue);
                methodExpression = Expression.LessThan(compareExpression, Expression.Constant(0));
            }
            else
            {
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                methodExpression = Expression.LessThan(propertyExp, someValue);
            }

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> LessThanOrEqual<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            Expression methodExpression;

            if (propertyExp.Type == typeof(bool))
            {
                throw new GraphityException(
                    $"The 'LessThanOrEqual' comparison cannot be made against a property of type bool");
            }

            if (propertyExp.Type == typeof(string))
            {
                //For strings, we need to use string.Compare(value) > 0
                var method = typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                // ReSharper disable once AssignNullToNotNullAttribute
                var compareExpression = Expression.Call(null, method, propertyExp, someValue);
                methodExpression = Expression.LessThanOrEqual(compareExpression, Expression.Constant(0));
            }
            else
            {
                var someValue = Expression.Constant(GetValueByType(propertyValue, propertyExp.Type), propertyExp.Type);
                methodExpression = Expression.LessThanOrEqual(propertyExp, someValue);
            }

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> StartsWith<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            if (propertyExp.Type != typeof(string))
            {
                throw new GraphityException(
                    $"The 'StartsWith' comparison cannot be made against a property of type {propertyExp.Type.Name}");
            }

            var method = typeof(string).GetMethod("StartsWith", new[] {typeof(string)});
            var someValue = Expression.Constant(propertyValue, typeof(string));
            // ReSharper disable once AssignNullToNotNullAttribute
            var methodExpression = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> EndsWith<T>(string propertyName, string propertyValue)
        {
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);

            if (propertyExp.Type != typeof(string))
            {
                throw new GraphityException(
                    $"The 'EndsWith' comparison cannot be made against a property of type {propertyExp.Type.Name}");
            }

            var method = typeof(string).GetMethod("EndsWith", new[] {typeof(string)});
            var someValue = Expression.Constant(propertyValue, typeof(string));
            // ReSharper disable once AssignNullToNotNullAttribute
            var methodExpression = Expression.Call(propertyExp, method, someValue);

            return Expression.Lambda<Func<T, bool>>(methodExpression, parameterExp);
        }

        private static Expression<Func<T, bool>> In<T>(string propertyName, string propertyValue)
        {
            var property = typeof(T).GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException(
                    $"Invalid property path specified, {propertyName} is not a property of type {typeof(T).Name}",
                    nameof(propertyName));
            }

            var type = property.PropertyType;
            var values = GetValuesByType(propertyValue, type);

            var arrayExp = Expression.Constant(values);
            var parameterExp = Expression.Parameter(typeof(T), "entity");
            var propertyExp = parameterExp.GetPropertyExpression(propertyName);
            var method = type.GetMethod("Equals", new[] {type});

            var innerParameterExp = Expression.Parameter(type, "innerValue");
            // ReSharper disable once AssignNullToNotNullAttribute
            // [This will never occur as the `Equals` method is available on all types]
            var methodExpression = Expression.Call(propertyExp, method, innerParameterExp);
            var innerLambdaExp = Expression.Lambda(methodExpression, innerParameterExp);
            var anyExpression = Expression.Call(typeof(Enumerable), "Any", new[] {type}, arrayExp, innerLambdaExp);

            return Expression.Lambda<Func<T, bool>>(anyExpression, parameterExp);
        }

        private static object GetValueByType(string input, Type type)
        {
            if (type == typeof(string))
            {
                return input;
            }

            //Integer types
            if (type == typeof(short))
            {
                return short.Parse(input);
            }

            if (type == typeof(int))
            {
                return int.Parse(input);
            }

            if (type == typeof(long))
            {
                return long.Parse(input);
            }

            //Floating point types
            if (type == typeof(decimal))
            {
                return decimal.Parse(input);
            }

            if (type == typeof(float))
            {
                return float.Parse(input);
            }

            if (type == typeof(double))
            {
                return double.Parse(input);
            }

            //Other
            if (type == typeof(bool))
            {
                return bool.Parse(input);
            }

            throw new ArgumentException("Valid types are string, short, int, long, decimal, float, double and bool",
                nameof(type));
        }

        private static object GetValuesByType(string input, Type type)
        {
            var parts = input.Split(',');

            //String type
            if (type == typeof(string))
            {
                return parts;
            }

            //Integer types
            if (type == typeof(short))
            {
                return parts.Select(short.Parse);
            }

            if (type == typeof(int))
            {
                return parts.Select(int.Parse);
            }

            if (type == typeof(long))
            {
                return parts.Select(long.Parse);
            }

            //Floating point types
            if (type == typeof(decimal))
            {
                return parts.Select(decimal.Parse);
            }

            if (type == typeof(float))
            {
                return parts.Select(float.Parse);
            }

            if (type == typeof(double))
            {
                return parts.Select(double.Parse);
            }

            //Other types
            if (type == typeof(bool))
            {
                return parts.Select(bool.Parse);
            }

            throw new ArgumentException("Valid types are string, short, int, long, decimal, float, double and bool",
                nameof(type));
        }

    }
}
