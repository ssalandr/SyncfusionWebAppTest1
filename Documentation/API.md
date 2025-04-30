# API Documentation

## Overview
This document provides detailed information about the API endpoints in the SyncfusionWebAppTest1 application.

## Base URL
```
https://localhost:5001/api/
```

## Endpoints

### Weather Forecast
- **GET** `/WeatherForecast`
  - Returns weather forecast data
  - Response: Array of WeatherForecast objects
  - Example:
    ```json
    [
      {
        "date": "2024-04-30",
        "temperatureC": 25,
        "temperatureF": 77,
        "summary": "Warm"
      }
    ]
    ```

### Grid Data
- **GET** `/GridEFInMemory`
  - Returns grid data with filtering and sorting
  - Query Parameters:
    - `$filter`: Filter conditions
    - `$orderby`: Sort conditions
    - `$skip`: Number of records to skip
    - `$top`: Number of records to return
  - Response: Object containing data and total count
  - Example:
    ```json
    {
      "result": [...],
      "count": 100
    }
    ```

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid request parameters"
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An error occurred while processing your request"
}
```

## Authentication
Currently, the API does not require authentication. Future versions will implement JWT authentication.

## Rate Limiting
- Maximum 100 requests per minute per IP address
- Rate limit headers included in responses:
  - `X-RateLimit-Limit`: Maximum requests per minute
  - `X-RateLimit-Remaining`: Remaining requests
  - `X-RateLimit-Reset`: Time until limit reset

## Best Practices
1. Always check response status codes
2. Implement proper error handling
3. Use appropriate HTTP methods
4. Follow RESTful conventions
5. Implement proper validation 