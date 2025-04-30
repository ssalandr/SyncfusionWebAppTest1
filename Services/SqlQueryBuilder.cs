using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SyncfusionWebAppTest1.Models;

namespace SyncfusionWebAppTest1.Services
{
    /// <summary>
    /// Builds SQL queries dynamically based on filter conditions.
    /// Allows for parameterized queries to prevent SQL injection and supports
    /// pagination, ordering, and complex filtering with AND/OR logic.
    /// </summary>
    /// <typeparam name="T">The entity type the SQL query is targeting</typeparam>
    public class SqlQueryBuilder<T> where T : class
    {
        private readonly string _tableName;
        private readonly Dictionary<string, string> _columnMappings;
        private readonly List<SqlParameter> _parameters;
        private int _parameterCounter;

        /// <summary>
        /// Initializes a new instance of the SqlQueryBuilder class.
        /// </summary>
        /// <param name="tableName">The database table name to query</param>
        /// <param name="columnMappings">Optional mappings from property names to database column names</param>
        public SqlQueryBuilder(string tableName, Dictionary<string, string> columnMappings = null)
        {
            _tableName = tableName;
            _columnMappings = columnMappings ?? new Dictionary<string, string>();
            _parameters = new List<SqlParameter>();
            _parameterCounter = 0;
        }

        /// <summary>
        /// Builds a complete SQL query with WHERE, ORDER BY, and pagination clauses.
        /// </summary>
        /// <param name="filterGroups">Filter conditions to apply in the WHERE clause</param>
        /// <param name="orderBy">Optional ORDER BY clause</param>
        /// <param name="skip">Optional number of rows to skip for pagination</param>
        /// <param name="take">Optional number of rows to take for pagination</param>
        /// <returns>A tuple containing the SQL query string and a list of parameters</returns>
        public (string Sql, List<SqlParameter> Parameters) BuildQuery(List<FilterGroup> filterGroups, string orderBy = null, int? skip = null, int? take = null)
        {
            var sqlBuilder = new StringBuilder();

            // Start with basic SELECT statement
            sqlBuilder.Append($"SELECT * FROM {_tableName}");

            // Add WHERE clause if filters are provided
            if (filterGroups != null && filterGroups.Any())
            {
                sqlBuilder.Append(" WHERE ");
                BuildWhereClause(sqlBuilder, filterGroups);
            }

            // Add ORDER BY clause if specified
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlBuilder.Append($" ORDER BY {orderBy}");
            }

            // Add pagination with OFFSET/FETCH if specified
            if (skip.HasValue || take.HasValue)
            {
                sqlBuilder.Append($" OFFSET {skip ?? 0} ROWS");
                if (take.HasValue)
                {
                    sqlBuilder.Append($" FETCH NEXT {take} ROWS ONLY");
                }
            }

            return (sqlBuilder.ToString(), _parameters);
        }

        /// <summary>
        /// Builds the WHERE clause of the SQL query based on filter groups.
        /// </summary>
        /// <param name="sqlBuilder">The StringBuilder to append the WHERE clause to</param>
        /// <param name="filterGroups">The filter groups to process</param>
        private void BuildWhereClause(StringBuilder sqlBuilder, List<FilterGroup> filterGroups)
        {
            var conditions = new List<string>();

            foreach (var group in filterGroups)
            {
                var groupConditions = new List<string>();

                // Process each condition in the group
                foreach (var condition in group.Conditions)
                {
                    groupConditions.Add(BuildCondition(condition));
                }

                // Determine if conditions within this group use AND or OR logic
                var groupOperator = group.IsOr ? " OR " : " AND ";

                // Wrap group in parentheses and join conditions with appropriate operator
                conditions.Add($"({string.Join(groupOperator, groupConditions)})");
            }

            // Join all groups with AND operator (each group is treated as a separate condition set)
            sqlBuilder.Append(string.Join(" AND ", conditions));
        }

        /// <summary>
        /// Builds a single SQL condition based on the provided filter condition.
        /// </summary>
        /// <param name="condition">The filter condition to convert to SQL</param>
        /// <returns>A SQL condition string with parameterized values</returns>
        private string BuildCondition(FilterCondition condition)
        {
            // Get the database column name (possibly mapped from property name)
            var columnName = GetColumnName(condition.Field);

            // Create a unique parameter name
            var parameterName = $"@p{_parameterCounter++}";
            var parameterValue = condition.Value;

            // Add parameter to the list for later binding
            _parameters.Add(new SqlParameter(parameterName, parameterValue));

            // Build the appropriate SQL condition based on the operator
            return condition.Operator switch
            {
                // Equality operators
                FilterOperator.Equals => $"{columnName} = {parameterName}",
                FilterOperator.NotEquals => $"{columnName} <> {parameterName}",

                // Comparison operators
                FilterOperator.GreaterThan => $"{columnName} > {parameterName}",
                FilterOperator.GreaterThanOrEqual => $"{columnName} >= {parameterName}",
                FilterOperator.LessThan => $"{columnName} < {parameterName}",
                FilterOperator.LessThanOrEqual => $"{columnName} <= {parameterName}",

                // String pattern matching operators
                FilterOperator.Contains => $"{columnName} LIKE '%' + {parameterName} + '%'",
                FilterOperator.StartsWith => $"{columnName} LIKE {parameterName} + '%'",
                FilterOperator.EndsWith => $"{columnName} LIKE '%' + {parameterName}",

                // Null checking operators
                FilterOperator.IsNull => $"{columnName} IS NULL",
                FilterOperator.IsNotNull => $"{columnName} IS NOT NULL",

                // Handle unsupported operators
                _ => throw new ArgumentException($"Unsupported operator: {condition.Operator}")
            };
        }

        /// <summary>
        /// Gets the database column name for a given property name, using mappings if available.
        /// </summary>
        /// <param name="propertyName">The property name to get the column name for</param>
        /// <returns>The corresponding database column name</returns>
        private string GetColumnName(string propertyName)
        {
            return _columnMappings.TryGetValue(propertyName, out var columnName)
                ? columnName  // Use mapped column name if available
                : propertyName;  // Default to property name if no mapping exists
        }
    }

    /// <summary>
    /// Represents a SQL parameter with a name and value for parameterized queries.
    /// Used to prevent SQL injection by separating query structure from data values.
    /// </summary>
    public class SqlParameter
    {
        /// <summary>
        /// Gets the name of the SQL parameter (e.g., @p0, @p1)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the SQL parameter
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the SqlParameter class.
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        public SqlParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}