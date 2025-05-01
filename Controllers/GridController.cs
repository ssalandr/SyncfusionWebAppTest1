using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncfusionWebAppTest1.Data;
using SyncfusionWebAppTest1.Models;
using SyncfusionWebAppTest1.Services;
using System.Diagnostics;

namespace WebApiAdaptor.Controllers;

[Route("api/[controller]")]

[ApiController]
/// GridEFInMemoryController
public class GridController : ControllerBase
{
    private readonly YourDbContext _context;

    public GridController(YourDbContext context)
    {
        _context = context;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<object> Get()
    {
        var queryString = Request.Query;
        var query = _context.OrdersDetails.AsQueryable();

        string? sort = queryString["$orderby"];   // Get sorting parameter  
        string? filter = queryString["$filter"]; // Get filtering parameter

        Trace.WriteLine($"Filter: {filter}");

        // Parse filters
        var filterGroups = FilterParser.ParseFilterString(filter ?? string.Empty);

        // Apply filters using Entity Framework
        query = FilterQueryBuilder<OrdersDetails>.BuildQuery(query, filterGroups);

        // Handle sorting
        if (!string.IsNullOrEmpty(sort))
        {
            var sortConditions = sort.Split(',');
            var orderedQuery = query;
            var isFirstSort = true;

            foreach (var sortCondition in sortConditions)
            {
                var sortParts = sortCondition.Trim().Split(' ');
                var sortBy = sortParts[0];
                var sortOrder = sortParts.Length > 1 && sortParts[1].ToLower() == "desc";

                if (isFirstSort)
                {
                    orderedQuery = sortOrder
                        ? orderedQuery.OrderByDescending(x => EF.Property<object>(x, sortBy))
                        : orderedQuery.OrderBy(x => EF.Property<object>(x, sortBy));
                    isFirstSort = false;
                }
                else
                {
                    orderedQuery = sortOrder
                        ? ((IOrderedQueryable<OrdersDetails>)orderedQuery).ThenByDescending(x => EF.Property<object>(x, sortBy))
                        : ((IOrderedQueryable<OrdersDetails>)orderedQuery).ThenBy(x => EF.Property<object>(x, sortBy));
                }
            }

            query = orderedQuery;
        }

        // Handle paging
        int skip = Convert.ToInt32(queryString["$skip"]);
        int take = Convert.ToInt32(queryString["$top"]);

        var totalRecordsCount = await query.CountAsync();
        var data = await query.Skip(skip).Take(take).ToListAsync();

        return new { result = data, count = totalRecordsCount };
    }

    private List<OrdersDetails> ApplyFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        if (condition.IsDate)
        {
            return ApplyDateFilter(data, condition);
        }
        else if (condition.IsText)
        {
            return ApplyTextFilter(data, condition);
        }
        else if (condition.IsNumber)
        {
            return ApplyNumericFilter(data, condition);
        }
        else if (condition.IsBoolean)
        {
            return ApplyBooleanFilter(data, condition);
        }

        return data;
    }

    private List<OrdersDetails> ApplyDateFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var dateValue = (DateTime?)condition.Value;
        var property = typeof(OrdersDetails).GetProperty(condition.Field);
        if (property == null || property.PropertyType != typeof(DateTime)) return data;

        return condition.Operator switch
        {
            FilterOperator.Equals => data.Where(x => ((DateTime?)property.GetValue(x))?.Date == dateValue?.Date).ToList(),
            FilterOperator.NotEquals => data.Where(x => ((DateTime?)property.GetValue(x))?.Date != dateValue?.Date).ToList(),
            FilterOperator.GreaterThan => data.Where(x => ((DateTime?)property.GetValue(x)) > dateValue).ToList(),
            FilterOperator.GreaterThanOrEqual => data.Where(x => ((DateTime?)property.GetValue(x)) >= dateValue).ToList(),
            FilterOperator.LessThan => data.Where(x => ((DateTime?)property.GetValue(x)) < dateValue).ToList(),
            FilterOperator.LessThanOrEqual => data.Where(x => ((DateTime?)property.GetValue(x)) <= dateValue).ToList(),
            FilterOperator.IsNull => data.Where(x => property.GetValue(x) == null).ToList(),
            FilterOperator.IsNotNull => data.Where(x => property.GetValue(x) != null).ToList(),
            _ => data
        };
    }

    private List<OrdersDetails> ApplyTextFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var value = condition.Value?.ToString()?.ToLower(); // Ensure null safety
        var property = typeof(OrdersDetails).GetProperty(condition.Field);
        if (property == null || property.PropertyType != typeof(string)) return data;

        return condition.Operator switch
        {
            FilterOperator.Equals => data.Where(x => string.Equals((string?)property.GetValue(x), value, StringComparison.OrdinalIgnoreCase)).ToList(),
            FilterOperator.NotEquals => data.Where(x => !string.Equals((string?)property.GetValue(x), value, StringComparison.OrdinalIgnoreCase)).ToList(),
            FilterOperator.StartsWith => data.Where(x => ((string?)property.GetValue(x))?.StartsWith(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true).ToList(),
            FilterOperator.NotStartsWith => data.Where(x => ((string?)property.GetValue(x))?.StartsWith(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) != true).ToList(),
            FilterOperator.EndsWith => data.Where(x => ((string?)property.GetValue(x))?.EndsWith(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true).ToList(),
            FilterOperator.NotEndsWith => data.Where(x => ((string?)property.GetValue(x))?.EndsWith(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) != true).ToList(),
            FilterOperator.Contains => data.Where(x => ((string?)property.GetValue(x))?.Contains(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) == true).ToList(),
            FilterOperator.NotContains => data.Where(x => ((string?)property.GetValue(x))?.Contains(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) != true).ToList(),
            _ => data
        };
    }

    private List<OrdersDetails> ApplyNumericFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var value = Convert.ToDecimal(condition.Value);
        var property = typeof(OrdersDetails).GetProperty(condition.Field);
        if (property == null) return data;

        // Handle different numeric types
        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
        {
            return condition.Operator switch
            {
                FilterOperator.Equals => data.Where(x => (int?)property.GetValue(x) == (int)value).ToList(),
                FilterOperator.NotEquals => data.Where(x => (int?)property.GetValue(x) != (int)value).ToList(),
                FilterOperator.GreaterThan => data.Where(x => (int?)property.GetValue(x) > (int)value).ToList(),
                FilterOperator.GreaterThanOrEqual => data.Where(x => (int?)property.GetValue(x) >= (int)value).ToList(),
                FilterOperator.LessThan => data.Where(x => (int?)property.GetValue(x) < (int)value).ToList(),
                FilterOperator.LessThanOrEqual => data.Where(x => (int?)property.GetValue(x) <= (int)value).ToList(),
                _ => data
            };
        }
        else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
        {
            return condition.Operator switch
            {
                FilterOperator.Equals => data.Where(x => (double?)property.GetValue(x) == (double)value).ToList(),
                FilterOperator.NotEquals => data.Where(x => (double?)property.GetValue(x) != (double)value).ToList(),
                FilterOperator.GreaterThan => data.Where(x => (double?)property.GetValue(x) > (double)value).ToList(),
                FilterOperator.GreaterThanOrEqual => data.Where(x => (double?)property.GetValue(x) >= (double)value).ToList(),
                FilterOperator.LessThan => data.Where(x => (double?)property.GetValue(x) < (double)value).ToList(),
                FilterOperator.LessThanOrEqual => data.Where(x => (double?)property.GetValue(x) <= (double)value).ToList(),
                _ => data
            };
        }
        else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
        {
            return condition.Operator switch
            {
                FilterOperator.Equals => data.Where(x => (decimal?)property.GetValue(x) == value).ToList(),
                FilterOperator.NotEquals => data.Where(x => (decimal?)property.GetValue(x) != value).ToList(),
                FilterOperator.GreaterThan => data.Where(x => (decimal?)property.GetValue(x) > value).ToList(),
                FilterOperator.GreaterThanOrEqual => data.Where(x => (decimal?)property.GetValue(x) >= value).ToList(),
                FilterOperator.LessThan => data.Where(x => (decimal?)property.GetValue(x) < value).ToList(),
                FilterOperator.LessThanOrEqual => data.Where(x => (decimal?)property.GetValue(x) <= value).ToList(),
                _ => data
            };
        }

        return data;
    }

    private List<OrdersDetails> ApplyBooleanFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var property = typeof(OrdersDetails).GetProperty(condition.Field);
        if (property == null || property.PropertyType != typeof(bool)) return data;

        var conditionValue = condition.Value?.ToString()?.ToLower(); // Ensure null safety
        return conditionValue switch
        {
            "true" => data.Where(x => (bool?)property.GetValue(x) == true).ToList(),
            "false" => data.Where(x => (bool?)property.GetValue(x) == false).ToList(),
            _ => data
        };
    }

    // POST: api/Orders
    [HttpPost]
    /// <summary>
    /// Inserts a new data item into the data collection.
    /// </summary>
    /// <param name="value">It holds new record detail which is need to be inserted.</param>
    /// <returns>Returns void</returns>
    public async Task<IActionResult> Post([FromBody] OrdersDetails newRecord)
    {
        // Insert a new record into the OrdersDetails model
        OrdersDetails.GetAllRecords().Insert(0, newRecord);

        return NoContent();
    }

    // PUT: api/Orders/5
    [HttpPut]
    /// <summary>
    /// Update a existing data item from the data collection.
    /// </summary>
    /// <param name="order">It holds updated record detail which is need to be updated.</param>
    /// <returns>Returns void</returns>
    public async Task<IActionResult> Put(int id, [FromBody] OrdersDetails order)
    {
        // Find the existing order by ID
        var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(o => o.OrderID == id);
        if (existingOrder != null)
        {
            // If the order exists, update its properties
            existingOrder.OrderID = order.OrderID;
            existingOrder.CustomerID = order.CustomerID;
            existingOrder.ShipCity = order.ShipCity;
        }

        return NoContent();
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    /// <summary>
    /// Remove a specific data item from the data collection.
    /// </summary>
    /// <param name="id">It holds specific record detail id which is need to be removed.</param>
    /// <returns>Returns void</returns>
    public async Task<IActionResult> Delete(int id)
    {
        // Find the order to remove by ID
        var orderToRemove = OrdersDetails.GetAllRecords().FirstOrDefault(order => order.OrderID == id);
        // If the order exists, remove it
        if (orderToRemove != null)
        {
            OrdersDetails.GetAllRecords().Remove(orderToRemove);
        }

        return NoContent();
    }
}