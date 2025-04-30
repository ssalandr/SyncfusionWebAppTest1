using Microsoft.AspNetCore.Mvc;
using SyncfusionWebAppTest1.Models;
using SyncfusionWebAppTest1.Services;
using System.Diagnostics;

namespace WebApiAdaptor.Controllers;

[Route("api/[controller]")]

[ApiController]
public class GridCopilotController : ControllerBase
{
    public class FilterCriteria
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }

    private List<FilterCriteria> ParseFilters(string filter)
    {
        var filters = new List<FilterCriteria>();

        if (string.IsNullOrEmpty(filter))
            return filters;

        var filterItems = filter.Split(new string[] { " and ", " or " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var filterItem in filterItems)
        {
            if (filterItem.Contains("substringof"))
            {
                var parts = filterItem.Split('(', ')', '\'');
                filters.Add(new FilterCriteria
                {
                    Field = parts[3],
                    Operator = "contains",
                    Value = parts[1]
                });
            }
            else if (filterItem.Contains("startswith"))
            {
                var parts = filterItem.Split('(', ')', '\'');
                filters.Add(new FilterCriteria
                {
                    Field = parts[3],
                    Operator = "startswith",
                    Value = parts[1]
                });
            }
            else if (filterItem.Contains("endswith"))
            {
                var parts = filterItem.Split('(', ')', '\'');
                filters.Add(new FilterCriteria
                {
                    Field = parts[3],
                    Operator = "endswith",
                    Value = parts[1]
                });
            }
            else if (filterItem.Contains("datetime"))
            {
                var parts = filterItem.Split(new[] { " ", "'", "datetime" }, StringSplitOptions.RemoveEmptyEntries);
                filters.Add(new FilterCriteria
                {
                    Field = parts[0],
                    Operator = parts[1],
                    Value = DateTime.Parse(parts[2])
                });
            }
            else
            {
                var parts = filterItem.Split(new[] { " ", "'" }, StringSplitOptions.RemoveEmptyEntries);
                filters.Add(new FilterCriteria
                {
                    Field = parts[0],
                    Operator = parts[1],
                    Value = parts.Length > 2 ? parts[2] : null
                });
            }
        }

        return filters;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<object> Get()
    {
        var queryString = Request.Query;
        var data = OrdersDetails.GetAllRecords().ToList();

        string? sort = queryString["$orderby"];   // Get sorting parameter  
        string? filter = queryString["$filter"]; // Get filtering parameter

        Trace.WriteLine($"Filter: {filter}");

        // Parse filters
        var filterGroups = FilterParser.ParseFilterString(filter);

        // Apply filters
        if (filterGroups.Any())
        {
            foreach (var group in filterGroups)
            {
                var filteredData = new List<OrdersDetails>();
                foreach (var condition in group.Conditions)
                {
                    var tempData = ApplyFilter(data, condition);
                    if (group.IsOr)
                    {
                        filteredData.AddRange(tempData);
                    }
                    else
                    {
                        data = tempData;
                    }
                }

                if (group.IsOr)
                {
                    data = filteredData.Distinct().ToList();
                }
            }
        }

        // Handle sorting
        if (!string.IsNullOrEmpty(sort))
        {
            var sortConditions = sort.Split(',');
            var orderedData = data.OrderBy(x => 0); // Start with a stable sort

            foreach (var sortCondition in sortConditions)
            {
                var sortParts = sortCondition.Trim().Split(' ');
                var sortBy = sortParts[0];
                var sortOrder = sortParts.Length > 1 && sortParts[1].ToLower() == "desc";

                switch (sortBy)
                {
                    case "OrderID":
                        orderedData = sortOrder ? orderedData.ThenByDescending(x => x.OrderID) : orderedData.ThenBy(x => x.OrderID);
                        break;
                    case "CustomerID":
                        orderedData = sortOrder ? orderedData.ThenByDescending(x => x.CustomerID) : orderedData.ThenBy(x => x.CustomerID);
                        break;
                    case "ShipCity":
                        orderedData = sortOrder ? orderedData.ThenByDescending(x => x.ShipCity) : orderedData.ThenBy(x => x.ShipCity);
                        break;
                    case "OrderDate":
                        orderedData = sortOrder ? orderedData.ThenByDescending(x => x.OrderDate) : orderedData.ThenBy(x => x.OrderDate);
                        break;
                }
            }

            data = [.. orderedData];
        }

        // Handle paging
        int skip = Convert.ToInt32(queryString["$skip"]);
        int take = Convert.ToInt32(queryString["$top"]);
        if (take != 0)
        {
            data = data.Skip(skip).Take(take).ToList();
        }

        int totalRecordsCount = data.Count;
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

        return data;
    }

    private List<OrdersDetails> ApplyDateFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var dateValue = (DateTime)condition.Value;
        return condition.Operator switch
        {
            FilterOperator.Equals => data.Where(x => x.OrderDate.Date == dateValue.Date).ToList(),
            FilterOperator.NotEquals => data.Where(x => x.OrderDate.Date != dateValue.Date).ToList(),
            FilterOperator.GreaterThan => data.Where(x => x.OrderDate > dateValue).ToList(),
            FilterOperator.GreaterThanOrEqual => data.Where(x => x.OrderDate >= dateValue).ToList(),
            FilterOperator.LessThan => data.Where(x => x.OrderDate < dateValue).ToList(),
            FilterOperator.LessThanOrEqual => data.Where(x => x.OrderDate <= dateValue).ToList(),
            FilterOperator.IsNull => data.Where(x => x.OrderDate == null).ToList(),
            FilterOperator.IsNotNull => data.Where(x => x.OrderDate != null).ToList(),
            _ => data
        };
    }

    private List<OrdersDetails> ApplyTextFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var value = condition.Value.ToString().ToLower();
        return condition.Operator switch
        {
            FilterOperator.Equals => data.Where(x => x.CustomerID.ToLower() == value).ToList(),
            FilterOperator.NotEquals => data.Where(x => x.CustomerID.ToLower() != value).ToList(),
            FilterOperator.StartsWith => data.Where(x => x.CustomerID.ToLower().StartsWith(value)).ToList(),
            FilterOperator.NotStartsWith => data.Where(x => !x.CustomerID.ToLower().StartsWith(value)).ToList(),
            FilterOperator.EndsWith => data.Where(x => x.CustomerID.ToLower().EndsWith(value)).ToList(),
            FilterOperator.NotEndsWith => data.Where(x => !x.CustomerID.ToLower().EndsWith(value)).ToList(),
            FilterOperator.Contains => data.Where(x => x.CustomerID.ToLower().Contains(value)).ToList(),
            FilterOperator.NotContains => data.Where(x => !x.CustomerID.ToLower().Contains(value)).ToList(),
            _ => data
        };
    }

    private List<OrdersDetails> ApplyNumericFilter(List<OrdersDetails> data, FilterCondition condition)
    {
        var value = Convert.ToDecimal(condition.Value);
        return condition.Operator switch
        {
            FilterOperator.Equals => data.Where(x => x.OrderID == value).ToList(),
            FilterOperator.NotEquals => data.Where(x => x.OrderID != value).ToList(),
            FilterOperator.GreaterThan => data.Where(x => x.OrderID > value).ToList(),
            FilterOperator.GreaterThanOrEqual => data.Where(x => x.OrderID >= value).ToList(),
            FilterOperator.LessThan => data.Where(x => x.OrderID < value).ToList(),
            FilterOperator.LessThanOrEqual => data.Where(x => x.OrderID <= value).ToList(),
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