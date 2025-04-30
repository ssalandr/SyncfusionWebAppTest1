using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncfusionWebAppTest1.Data;
using SyncfusionWebAppTest1.Models;
using SyncfusionWebAppTest1.Services;
using System.Diagnostics;
using System.Linq.Expressions;

namespace WebApiAdaptor.Controllers;

/// <summary>
/// API controller for handling Syncfusion Grid operations using Entity Framework.
/// Provides endpoints for CRUD operations and supports filtering, sorting, and pagination
/// for the OrdersDetails entity.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class GridEFController : ControllerBase
{
    private readonly YourDbContext _context;

    /// <summary>
    /// Initializes a new instance of the GridEFController with the specified database context.
    /// </summary>
    /// <param name="context">The database context for accessing OrdersDetails data</param>
    public GridEFController(YourDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a paginated, sorted, and filtered list of OrdersDetails records.
    /// Handles OData-style query parameters for filtering, sorting, and pagination.
    /// </summary>
    /// <returns>A JSON object containing the result data and total count</returns>
    [HttpGet]
    public async Task<object> Get()
    {
        // Extract query parameters from the request
        var queryString = Request.Query;

        // Start with the base query for OrdersDetails
        var query = _context.OrdersDetails.AsQueryable();

        // Extract sorting and filtering parameters from the query string (OData format)
        string? sort = queryString["$orderby"];   // Get sorting parameter  
        string? filter = queryString["$filter"];  // Get filtering parameter

        // Log the filter string for debugging purposes
        Trace.WriteLine($"Filter: {filter}");

        // Parse the OData filter string into structured FilterGroup objects
        var filterGroups = FilterParser.ParseFilterString(filter ?? string.Empty);

        // Apply filters to the query using the FilterQueryBuilder
        query = FilterQueryBuilder<OrdersDetails>.BuildQuery(query, filterGroups);

        // Handle sorting based on OData $orderby parameter
        if (!string.IsNullOrEmpty(sort))
        {
            // Split multiple sort conditions (e.g., "OrderID asc, CustomerID desc")
            var sortConditions = sort.Split(',');
            foreach (var sortCondition in sortConditions)
            {
                // Parse the sort field and direction
                var sortParts = sortCondition.Trim().Split(' ');
                var sortBy = sortParts[0]; // Field name to sort by
                var sortOrder = sortParts.Length > 1 && sortParts[1].ToLower() == "desc"; // Check if descending order

                // Use expression trees to build dynamic sorting expressions
                var parameter = Expression.Parameter(typeof(OrdersDetails), "x");
                var property = Expression.Property(parameter, sortBy);
                var lambda = Expression.Lambda(property, parameter);

                // Apply the appropriate sort method based on the direction
                query = sortOrder
                    ? Queryable.OrderByDescending((IQueryable<OrdersDetails>)query, (dynamic)lambda)
                    : Queryable.OrderBy((IQueryable<OrdersDetails>)query, (dynamic)lambda);
            }
        }

        // Handle pagination using OData $skip and $top parameters
        int skip = Convert.ToInt32(queryString["$skip"]); // Number of records to skip
        int take = Convert.ToInt32(queryString["$top"]);  // Number of records to take

        // Get the total count for pagination information
        var totalRecordsCount = await query.CountAsync();

        // Apply pagination and execute the query
        var data = await query.Skip(skip).Take(take).ToListAsync();

        // Return both the paginated data and the total count
        return new { result = data, count = totalRecordsCount };
    }

    /// <summary>
    /// Inserts a new OrdersDetails record.
    /// </summary>
    /// <param name="newRecord">The new OrdersDetails record to insert</param>
    /// <returns>204 No Content response on success</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrdersDetails newRecord)
    {
        // Insert a new record into the OrdersDetails collection
        // Note: This is currently using an in-memory collection rather than the database context
        OrdersDetails.GetAllRecords().Insert(0, newRecord);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing OrdersDetails record.
    /// </summary>
    /// <param name="id">The ID of the record to update</param>
    /// <param name="order">The updated OrdersDetails data</param>
    /// <returns>204 No Content response on success</returns>
    [HttpPut]
    public async Task<IActionResult> Put(int id, [FromBody] OrdersDetails order)
    {
        // Find the existing order by ID
        // Note: This is currently using an in-memory collection rather than the database context
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

    /// <summary>
    /// Deletes an OrdersDetails record by ID.
    /// </summary>
    /// <param name="id">The ID of the record to delete</param>
    /// <returns>204 No Content response on success</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Find the order to remove by ID
        // Note: This is currently using an in-memory collection rather than the database context
        var orderToRemove = OrdersDetails.GetAllRecords().FirstOrDefault(order => order.OrderID == id);

        // If the order exists, remove it
        if (orderToRemove != null)
        {
            OrdersDetails.GetAllRecords().Remove(orderToRemove);
        }

        return NoContent();
    }
}