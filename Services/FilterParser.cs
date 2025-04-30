using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SyncfusionWebAppTest1.Models;

namespace SyncfusionWebAppTest1.Services
{
    /// <summary>
    /// Parses OData-style filter strings into structured FilterGroup objects that can be used
    /// to build SQL queries or LINQ expressions. Handles various filter conditions including
    /// date, text, numeric, and boolean comparisons.
    /// </summary>
    public class FilterParser
    {
        /// <summary>
        /// Parses a filter string into a list of FilterGroup objects.
        /// </summary>
        /// <param name="filterString">The OData-style filter string to parse</param>
        /// <returns>A list of FilterGroup objects representing the parsed filter conditions</returns>
        public static List<FilterGroup> ParseFilterString(string filterString)
        {
            if (string.IsNullOrEmpty(filterString))
                return new List<FilterGroup>();

            var filterGroups = new List<FilterGroup>();
            var currentGroup = new FilterGroup();

            // Check for special case of NotEqual date condition
            // Format: ((Field le datetime'value') or (Field ge datetime'value'))
            var notEqualDateMatch = Regex.Match(filterString, @"\(\((\w+)\s+le\s+datetime'([^']+)'\)\s+or\s+\((\w+)\s+ge\s+datetime'([^']+)'\)\)");
            if (notEqualDateMatch.Success && notEqualDateMatch.Groups[1].Value == notEqualDateMatch.Groups[3].Value)
            {
                // This is a NotEqual date condition - extract field and date value
                var field = notEqualDateMatch.Groups[1].Value;
                var date1 = DateTime.Parse(notEqualDateMatch.Groups[4].Value).Subtract(new TimeSpan(1, 0, 0, 0));

                currentGroup.Conditions.Add(new FilterCondition
                {
                    Field = field,
                    Operator = FilterOperator.NotEquals,
                    Value = date1,
                    IsDate = true
                });
                filterGroups.Add(currentGroup);
                return filterGroups;
            }

            // Split the filter string by 'and' or 'or' operators while preserving the operators
            var parts = Regex.Split(filterString, @"\s+(and|or)\s+", RegexOptions.IgnoreCase);

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();

                // Skip empty parts
                if (string.IsNullOrEmpty(part))
                    continue;

                // If this is an operator (and/or), set the group's IsOr property accordingly
                if (part.ToLower() == "and" || part.ToLower() == "or")
                {
                    if (i > 0 && i < parts.Length - 1)
                    {
                        // The 'or' operator makes the group use OR logic; 'and' uses AND logic (default)
                        currentGroup.IsOr = part.ToLower() == "or";
                        continue;
                    }
                }

                // Parse individual condition parts
                var condition = ParseCondition(part);
                if (condition != null)
                {
                    currentGroup.Conditions.Add(condition);
                }
            }

            // Add the group to the list if it has any conditions
            if (currentGroup.Conditions.Any())
            {
                filterGroups.Add(currentGroup);
            }

            return filterGroups;
        }

        /// <summary>
        /// Parses a single condition string into a FilterCondition object.
        /// </summary>
        /// <param name="condition">The condition string to parse</param>
        /// <returns>A FilterCondition object representing the parsed condition, or null if parsing fails</returns>
        private static FilterCondition ParseCondition(string condition)
        {
            // Handle null checks (e.g., "Field eq null" or "Field ne null")
            if (condition.Contains("eq null") || condition.Contains("ne null"))
            {
                var parts = condition.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new FilterCondition
                {
                    Field = parts[0],
                    Operator = condition.Contains("eq null") ? FilterOperator.IsNull : FilterOperator.IsNotNull,
                    // Set type flags based on field name patterns or first character
                    IsDate = parts[0].Contains("Date"),
                    IsText = !parts[0].Contains("Date") && !char.IsDigit(parts[0][0]),
                    IsNumber = char.IsDigit(parts[0][0]),
                    IsBoolean = parts[0].ToLower() == "verified"
                };
            }

            // Handle date conditions (containing "datetime'")
            if (condition.Contains("datetime'"))
            {
                return ParseDateCondition(condition);
            }

            // Handle text conditions (containing "tolower" or "substringof")
            if (condition.Contains("tolower") || condition.Contains("substringof"))
            {
                return ParseTextCondition(condition);
            }

            // Handle boolean conditions (specific to "Verified" field)
            if (condition.Contains("Verified"))
            {
                var parts = condition.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new FilterCondition
                {
                    Field = parts[0],
                    Value = parts[1].ToLower(),
                    IsBoolean = true
                };
            }

            // Handle numeric conditions (default case)
            return ParseNumericCondition(condition);
        }

        /// <summary>
        /// Parses a date condition string into a FilterCondition object.
        /// </summary>
        /// <param name="condition">The date condition string to parse</param>
        /// <returns>A FilterCondition object representing the parsed date condition, or null if parsing fails</returns>
        private static FilterCondition ParseDateCondition(string condition)
        {
            // Match pattern: Field (gt|ge|lt|le|eq|ne) datetime'value'
            var match = Regex.Match(condition, @"(\w+)\s*(gt|ge|lt|le|eq|ne)\s*datetime'([^']+)'");
            if (!match.Success)
                return null;

            var field = match.Groups[1].Value;
            var op = match.Groups[2].Value;
            var dateStr = match.Groups[3].Value;

            return new FilterCondition
            {
                Field = field,
                Operator = GetDateOperator(op), // Convert OData operator to FilterOperator enum
                Value = DateTime.Parse(dateStr),
                IsDate = true
            };
        }

        /// <summary>
        /// Parses a text condition string into a FilterCondition object.
        /// Handles startswith, endswith, contains, and equality operations.
        /// </summary>
        /// <param name="condition">The text condition string to parse</param>
        /// <returns>A FilterCondition object representing the parsed text condition, or null if parsing fails</returns>
        private static FilterCondition ParseTextCondition(string condition)
        {
            // Handle startswith operations: startswith(tolower(Field),'value')
            if (condition.Contains("startswith"))
            {
                var match = Regex.Match(condition, @"startswith\(tolower\((\w+)\),'([^']+)'\)");
                return new FilterCondition
                {
                    Field = match.Groups[1].Value,
                    Operator = condition.Contains("not") ? FilterOperator.NotStartsWith : FilterOperator.StartsWith,
                    Value = match.Groups[2].Value.ToLower(),
                    IsText = true
                };
            }
            // Handle endswith operations: endswith(tolower(Field),'value')
            else if (condition.Contains("endswith"))
            {
                var match = Regex.Match(condition, @"endswith\(tolower\((\w+)\),'([^']+)'\)");
                return new FilterCondition
                {
                    Field = match.Groups[1].Value,
                    Operator = condition.Contains("not") ? FilterOperator.NotEndsWith : FilterOperator.EndsWith,
                    Value = match.Groups[2].Value.ToLower(),
                    IsText = true
                };
            }
            // Handle contains operations: substringof('value',tolower(Field))
            else if (condition.Contains("substringof"))
            {
                var match = Regex.Match(condition, @"substringof\('([^']+)',tolower\((\w+)\)\)");
                return new FilterCondition
                {
                    Field = match.Groups[2].Value,
                    Operator = condition.Contains("not") ? FilterOperator.NotContains : FilterOperator.Contains,
                    Value = match.Groups[1].Value.ToLower(),
                    IsText = true
                };
            }
            // Handle equality operations: tolower(Field) eq 'value'
            else if (condition.Contains("eq"))
            {
                var match = Regex.Match(condition, @"tolower\((\w+)\)\s*eq\s*'([^']*)'");
                return new FilterCondition
                {
                    Field = match.Groups[1].Value,
                    Operator = FilterOperator.Equals,
                    Value = match.Groups[2].Value.ToLower(),
                    IsText = true
                };
            }

            return null;
        }

        /// <summary>
        /// Parses a numeric condition string into a FilterCondition object.
        /// </summary>
        /// <param name="condition">The numeric condition string to parse</param>
        /// <returns>A FilterCondition object representing the parsed numeric condition, or null if parsing fails</returns>
        private static FilterCondition ParseNumericCondition(string condition)
        {
            // Match pattern: Field (eq|ne|gt|ge|lt|le) value
            var match = Regex.Match(condition, @"(\w+)\s*(eq|ne|gt|ge|lt|le)\s*([\d.]+)");
            if (!match.Success)
                return null;

            var field = match.Groups[1].Value;
            var op = match.Groups[2].Value;
            var value = match.Groups[3].Value;

            return new FilterCondition
            {
                Field = field,
                Operator = GetNumericOperator(op), // Convert OData operator to FilterOperator enum
                Value = decimal.Parse(value),
                IsNumber = true
            };
        }

        /// <summary>
        /// Converts an OData date operator string to the corresponding FilterOperator enum value.
        /// </summary>
        /// <param name="op">The OData operator string (gt, ge, lt, le, eq, ne)</param>
        /// <returns>The corresponding FilterOperator enum value</returns>
        private static FilterOperator GetDateOperator(string op)
        {
            return op switch
            {
                "gt" => FilterOperator.GreaterThan,         // greater than
                "ge" => FilterOperator.GreaterThanOrEqual,  // greater than or equal
                "lt" => FilterOperator.LessThan,            // less than
                "le" => FilterOperator.LessThanOrEqual,     // less than or equal
                "eq" => FilterOperator.Equals,              // equals
                "ne" => FilterOperator.NotEquals,           // not equals
                _ => FilterOperator.Equals                  // default to equals
            };
        }

        /// <summary>
        /// Converts an OData numeric operator string to the corresponding FilterOperator enum value.
        /// </summary>
        /// <param name="op">The OData operator string (gt, ge, lt, le, eq, ne)</param>
        /// <returns>The corresponding FilterOperator enum value</returns>
        private static FilterOperator GetNumericOperator(string op)
        {
            return op switch
            {
                "gt" => FilterOperator.GreaterThan,         // greater than
                "ge" => FilterOperator.GreaterThanOrEqual,  // greater than or equal
                "lt" => FilterOperator.LessThan,            // less than
                "le" => FilterOperator.LessThanOrEqual,     // less than or equal
                "eq" => FilterOperator.Equals,              // equals
                "ne" => FilterOperator.NotEquals,           // not equals
                _ => FilterOperator.Equals                  // default to equals
            };
        }
    }
}