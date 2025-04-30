using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using SyncfusionWebAppTest1.Models;
using System.Diagnostics;

namespace WebApiAdaptor.Controllers;

[Route("api/[controller]")]

[ApiController]
public class GridUrlController : ControllerBase
{
    // Action to retrieve orders
    [HttpPost]
    public object Post([FromBody] DataManagerRequest DataManagerRequest)
    {
        // Retrieve data from the data source (e.g., database).
        IQueryable<OrdersDetails> DataSource = OrdersDetails.GetAllRecords().AsQueryable();
        QueryableOperation queryableOperation = new QueryableOperation(); // Initialize QueryableOperation instance.

        // Handling Searching.
        if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
        {
            DataSource = queryableOperation.PerformSearching(DataSource, DataManagerRequest.Search);
        }

        // Handling Filtering.
        if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
        {
            // Handling filtering operation.
            foreach (var condition in DataManagerRequest.Where)
            {
                foreach (var predicate in condition.predicates)
                {
                    DataSource = queryableOperation.PerformFiltering(DataSource, DataManagerRequest.Where, predicate.Operator);
                }
            }
        }

        // Handling Sorting operation.
        if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
        {
            DataSource = queryableOperation.PerformSorting(DataSource, DataManagerRequest.Sorted);
        }


        // Handling paging operation.
        if (DataManagerRequest.Skip != 0)
        {
            DataSource = queryableOperation.PerformSkip(DataSource, DataManagerRequest.Skip);
        }
        if (DataManagerRequest.Take != 0)
        {
            DataSource = queryableOperation.PerformTake(DataSource, DataManagerRequest.Take);
        }

        // Get the total records count.
        int totalRecordsCount = DataSource.Count();

        // Return data based on the request.
        return new { result = DataSource, count = totalRecordsCount };
    }

    [HttpPost("Insert")]
    public void Insert([FromBody] CRUDModel<OrdersDetails> newRecord)
    {
        if (newRecord.Value != null)
        {
            OrdersDetails.GetAllRecords().Insert(0, newRecord.Value);
        }
    }

    [HttpPost("Update")]
    public void Update([FromBody] CRUDModel<OrdersDetails> updatedRecord)
    {
        var updatedOrder = updatedRecord.Value;
        if (updatedOrder != null)
        {
            var data = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == updatedOrder.OrderID);
            if (data != null)
            {
                // Update the existing record.
                data.OrderID = updatedOrder.OrderID;
                data.CustomerID = updatedOrder.CustomerID;
                data.ShipCity = updatedOrder.ShipCity;
                data.ShipCountry = updatedOrder.ShipCountry;
                // Update other properties similarly.
            }
        }
    }

    [HttpPost("Remove")]
    public void Remove([FromBody] CRUDModel<OrdersDetails> deletedRecord)
    {
        OrdersDetails? data = null;
        if (!string.IsNullOrEmpty(deletedRecord?.Key?.ToString()))
        {
            int orderId = int.Parse(deletedRecord.Key.ToString()!); 
            data = OrdersDetails.GetAllRecords().FirstOrDefault(orderData => orderData.OrderID == orderId);
        }

        if (data != null)
        {
            // Remove the record from the data collection.
            OrdersDetails.GetAllRecords().Remove(data);
        }
    }
}