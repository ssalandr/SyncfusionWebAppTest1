using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SyncfusionWebAppTest1.Models;

namespace SyncfusionWebAppTest1.Services
{
    /// <summary>
    /// Provides functionality to dynamically build Entity Framework queries based on filter conditions.
    /// Allows for complex filtering with support for AND/OR logic groups and various comparison operators.
    /// </summary>
    /// <typeparam name="T">The entity type to query against</typeparam>
    public class FilterQueryBuilder<T> where T : class
    {
        /// <summary>
        /// Builds a dynamic query by applying filter groups to the base query.
        /// </summary>
        /// <param name="query">The base IQueryable to apply filters to</param>
        /// <param name="filterGroups">A collection of filter groups to apply</param>
        /// <returns>An IQueryable with all specified filters applied</returns>
        public static IQueryable<T> BuildQuery(IQueryable<T> query, List<FilterGroup> filterGroups)
        {
            // Return the original query if no filters are specified
            if (filterGroups == null || !filterGroups.Any())
                return query;

            foreach (var group in filterGroups)
            {
                if (group.IsOr)
                {
                    // Handle OR conditions - combine multiple conditions with OR logic
                    var orExpressions = new List<Expression<Func<T, bool>>>();
                    foreach (var condition in group.Conditions)
                    {
                        orExpressions.Add(BuildExpression(condition));
                    }
                    query = query.Where(CombineOrExpressions(orExpressions));
                }
                else
                {
                    // Handle AND conditions - each condition is applied with AND logic
                    foreach (var condition in group.Conditions)
                    {
                        var expression = BuildExpression(condition);
                        query = query.Where(expression);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// Builds an Expression based on a single filter condition.
        /// </summary>
        /// <typeparam name="T">The entity type the expression is for</typeparam>
        /// <param name="condition">The filter condition to convert to an Expression</param>
        /// <returns>A lambda expression representing the filter condition</returns>
        private static Expression<Func<T, bool>> BuildExpression(FilterCondition condition)
        {
            // Create a parameter expression for the entity
            var parameter = Expression.Parameter(typeof(T), "x");

            // Get the property to filter on
            var property = Expression.Property(parameter, condition.Field);

            // Create a constant for the value to compare with
            var value = Expression.Constant(condition.Value);

            // Build the appropriate expression based on the operator
            Expression body;
            switch (condition.Operator)
            {
                case FilterOperator.Equals:
                    body = Expression.Equal(property, value);
                    break;
                case FilterOperator.NotEquals:
                    body = Expression.NotEqual(property, value);
                    break;
                case FilterOperator.GreaterThan:
                    body = Expression.GreaterThan(property, value);
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    body = Expression.GreaterThanOrEqual(property, value);
                    break;
                case FilterOperator.LessThan:
                    body = Expression.LessThan(property, value);
                    break;
                case FilterOperator.LessThanOrEqual:
                    body = Expression.LessThanOrEqual(property, value);
                    break;
                case FilterOperator.Contains:
                    // For string Contains operation, we need to use the string.Contains method
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    if (containsMethod == null)
                        throw new InvalidOperationException("The 'Contains' method could not be found.");
                    body = Expression.Call(property, containsMethod, value);
                    break;
                case FilterOperator.StartsWith:
                    // For string StartsWith operation
                    var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    if (startsWithMethod == null)
                        throw new InvalidOperationException("The 'StartsWith' method could not be found.");
                    body = Expression.Call(property, startsWithMethod, value);
                    break;
                case FilterOperator.EndsWith:
                    // For string EndsWith operation
                    var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    if (endsWithMethod == null)
                        throw new InvalidOperationException("The 'EndsWith' method could not be found.");
                    body = Expression.Call(property, endsWithMethod, value);
                    break;
                case FilterOperator.IsNull:
                    body = Expression.Equal(property, Expression.Constant(null));
                    break;
                case FilterOperator.IsNotNull:
                    body = Expression.NotEqual(property, Expression.Constant(null));
                    break;
                default:
                    throw new ArgumentException($"Unsupported operator: {condition.Operator}");
            }

            // Create and return a lambda expression from the body
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Combines multiple expressions with OR logic.
        /// </summary>
        /// <typeparam name="T">The entity type the expressions are for</typeparam>
        /// <param name="expressions">The list of expressions to combine with OR</param>
        /// <returns>A single expression that combines all expressions with OR logic</returns>
        private static Expression<Func<T, bool>> CombineOrExpressions(List<Expression<Func<T, bool>>> expressions)
        {
            // If no expressions provided, return a default "true" expression
            if (!expressions.Any())
                return x => true;

            // Create a parameter for the combined expression
            var parameter = Expression.Parameter(typeof(T), "x");

            // Combine all expressions with OR logic
            // First, we convert each expression to an invocation that uses our parameter
            // Then we aggregate them with OrElse operations
            var body = expressions.Select(e => Expression.Invoke(e, parameter))
                                .Aggregate((Expression)Expression.Constant(false),
                                         (left, right) => Expression.OrElse(left, right));

            // Create and return the final lambda expression
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}