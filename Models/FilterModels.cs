using System;

namespace SyncfusionWebAppTest1.Models
{
    public enum FilterOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        StartsWith,
        EndsWith,
        Contains,
        NotStartsWith,
        NotEndsWith,
        NotContains,
        IsNull,
        IsNotNull
    }

    public class FilterCondition
    {
        public string Field { get; set; }
        public FilterOperator Operator { get; set; }
        public object Value { get; set; }
        public bool IsDate { get; set; }
        public bool IsText { get; set; }
        public bool IsNumber { get; set; }
        public bool IsBoolean { get; set; }
    }

    public class FilterGroup
    {
        public List<FilterCondition> Conditions { get; set; } = new List<FilterCondition>();
        public bool IsOr { get; set; } = false; // Default to AND
    }
}