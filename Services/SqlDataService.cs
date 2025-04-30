using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SyncfusionWebAppTest1.Models;

namespace SyncfusionWebAppTest1.Services
{
    /// <summary>
    /// Service for executing SQL queries against a database with filtering, sorting, and pagination support.
    /// Uses SqlQueryBuilder to dynamically build SQL queries based on filter conditions.
    /// </summary>
    public class SqlDataService
    {
        private readonly string _connectionString;
        private readonly Dictionary<string, string> _columnMappings;

        /// <summary>
        /// Initializes a new instance of the SqlDataService with configuration settings.
        /// </summary>
        /// <param name="configuration">The application configuration containing the connection string</param>
        public SqlDataService(IConfiguration configuration)
        {
            // Get connection string from configuration
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            // Define column mappings between model properties and database columns
            _columnMappings = new Dictionary<string, string>
            {
                { "OrderID", "OrderID" },
                { "CustomerID", "CustomerID" },
                { "ShipCity", "ShipCity" },
                { "ShipCountry", "ShipCountry" },
                { "OrderDate", "OrderDate" },
                { "EmployeeID", "EmployeeID" },
                { "Verified", "Verified" }
            };
        }

        /// <summary>
        /// Retrieves filtered, sorted, and paginated data from the database.
        /// </summary>
        /// <typeparam name="T">The type of object to map database results to</typeparam>
        /// <param name="filterGroups">The filter conditions to apply</param>
        /// <param name="orderBy">Optional ordering expression</param>
        /// <param name="skip">Optional number of records to skip (for pagination)</param>
        /// <param name="take">Optional number of records to take (for pagination)</param>
        /// <returns>A tuple containing the list of data items and the total count of records matching the filter</returns>
        public async Task<(List<T> Data, int TotalCount)> GetFilteredData<T>(
            List<FilterGroup> filterGroups,
            string orderBy = null,
            int? skip = null,
            int? take = null) where T : class, new()
        {
            // Create SQL query builder for the specified entity type
            var builder = new SqlQueryBuilder<T>("OrdersDetails", _columnMappings);

            // Build the main query with filters, ordering, and pagination
            var (sql, parameters) = builder.BuildQuery(filterGroups, orderBy, skip, take);

            // Create a separate query to get total count for pagination
            var countSql = $"SELECT COUNT(*) FROM OrdersDetails";
            if (filterGroups != null && filterGroups.Any())
            {
                // Extract and reuse the WHERE clause from the main query
                countSql += " WHERE " + sql.Substring(sql.IndexOf("WHERE") + 6);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // First execute the count query to get total records
                using (var countCommand = new SqlCommand(countSql, connection))
                {
                    AddParameters(countCommand, parameters);
                    var totalCount = (int)await countCommand.ExecuteScalarAsync();

                    // Then execute the main query to get the data
                    using (var command = new SqlCommand(sql, connection))
                    {
                        AddParameters(command, parameters);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            // Map database results to objects of type T
                            var data = new List<T>();
                            while (await reader.ReadAsync())
                            {
                                var item = MapReaderToObject<T>(reader);
                                data.Add(item);
                            }

                            // Return both the data and total count for pagination support
                            return (data, totalCount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds SQL parameters to a command to prevent SQL injection.
        /// </summary>
        /// <param name="command">The SQL command to add parameters to</param>
        /// <param name="parameters">The list of parameters to add</param>
        private void AddParameters(SqlCommand command, List<SqlParameter> parameters)
        {
            foreach (var param in parameters)
            {
                // Handle null values by converting to DBNull
                command.Parameters.AddWithValue(param.Name, param.Value ?? DBNull.Value);
            }
        }

        /// <summary>
        /// Maps a database record to an object of type T using reflection.
        /// </summary>
        /// <typeparam name="T">The type of object to create and populate</typeparam>
        /// <param name="reader">The data reader containing the database record</param>
        /// <returns>A new object of type T populated with data from the reader</returns>
        private T MapReaderToObject<T>(SqlDataReader reader) where T : new()
        {
            var item = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // Only map properties that have a corresponding column mapping
                if (_columnMappings.TryGetValue(property.Name, out var columnName))
                {
                    // Get the value from the database record
                    var value = reader[columnName];

                    // Only set property if value is not null in the database
                    if (value != DBNull.Value)
                    {
                        property.SetValue(item, value);
                    }
                }
            }

            return item;
        }
    }
}