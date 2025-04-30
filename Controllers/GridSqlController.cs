using Microsoft.AspNetCore.Mvc;
using SyncfusionWebAppTest1.Models;
using SyncfusionWebAppTest1.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiAdaptor.Controllers
{
    /// <summary>
    /// API controller for handling Syncfusion Grid operations using direct SQL queries.
    /// Provides endpoints for CRUD operations and supports filtering, sorting, and pagination
    /// for the OrdersDetails entity through SQL query generation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GridSqlController : ControllerBase
    {
        private readonly SqlDataService _sqlDataService;

        /// <summary>
        /// Initializes a new instance of the GridSqlController with the SqlDataService.
        /// </summary>
        /// <param name="sqlDataService">Service for executing SQL queries with filtering support</param>
        public GridSqlController(SqlDataService sqlDataService)
        {
            _sqlDataService = sqlDataService;
        }

        /// <summary>
        /// Retrieves a paginated, sorted, and filtered list of OrdersDetails records using direct SQL queries.
        /// Handles OData-style query parameters for filtering, sorting, and pagination.
        /// </summary>
        /// <returns>A JSON object containing the result data and total count</returns>
        [HttpGet]
        public async Task<object> Get()
        {
            // Extract query parameters from the request
            var queryString = Request.Query;

            // Extract OData-style parameters for sorting, filtering, and pagination
            string? sort = queryString["$orderby"];      // Sorting parameter (e.g., "OrderID desc")
            string? filter = queryString["$filter"];     // Filtering parameter (OData filter syntax)
            int skip = Convert.ToInt32(queryString["$skip"]);  // Number of records to skip for pagination
            int take = Convert.ToInt32(queryString["$top"]);   // Number of records to take per page

            // Log the filter string for debugging purposes
            Trace.WriteLine($"Filter: {filter}");

            // Parse the OData filter string into structured FilterGroup objects
            var filterGroups = FilterParser.ParseFilterString(filter ?? string.Empty);

            // Use the SqlDataService to execute a parameterized SQL query with the filters
            // This approach uses direct SQL generation rather than Entity Framework
            var (data, totalCount) = await _sqlDataService.GetFilteredData<OrdersDetails>(
                filterGroups,     // The parsed filter conditions
                sort,             // Sorting information
                skip,             // Number of records to skip
                take);            // Number of records to take

            // Return both the paginated data and the total count in the format expected by Syncfusion Grid
            return new { result = data, count = totalCount };
        }

        /// <summary>
        /// Inserts a new OrdersDetails record.
        /// </summary>
        /// <param name="newRecord">The new OrdersDetails record to insert</param>
        /// <returns>204 No Content response on success</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrdersDetails newRecord)
        {
            // Implementation for inserting new records
            // Note: This is a placeholder - actual SQL insert implementation would be needed
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
            // Implementation for updating records
            // Note: This is a placeholder - actual SQL update implementation would be needed
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
            // Implementation for deleting records
            // Note: This is a placeholder - actual SQL delete implementation would be needed
            return NoContent();
        }
    }
}