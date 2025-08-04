# API Documentation

## Overview

ScrapeWise provides a comprehensive RESTful API for managing web scraping operations, built with ASP.NET Core 9.0.

## Base URL
```
Production: https://scrapewise-app-x2w3nky2.azurewebsites.net/api
Local: https://localhost:7139/api
```

## Authentication

All API endpoints require authentication via ASP.NET Core Identity.

### Headers
```http
Authorization: Bearer <your-jwt-token>
Content-Type: application/json
```

## Endpoints

### Jobs API

#### GET /api/jobs
Get all scraping jobs for the authenticated user.

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Example Job",
      "url": "https://example.com",
      "status": "Completed",
      "createdAt": "2025-08-04T10:00:00Z",
      "lastRunAt": "2025-08-04T11:00:00Z"
    }
  ],
  "totalCount": 1
}
```

#### POST /api/jobs
Create a new scraping job.

**Request Body:**
```json
{
  "name": "My Scraping Job",
  "url": "https://target-website.com",
  "selectors": [
    {
      "name": "title",
      "selector": "h1",
      "attribute": "text"
    }
  ],
  "schedule": {
    "frequency": "daily",
    "time": "09:00"
  }
}
```

#### GET /api/jobs/{id}
Get a specific scraping job by ID.

#### PUT /api/jobs/{id}
Update an existing scraping job.

#### DELETE /api/jobs/{id}
Delete a scraping job.

### Results API

#### GET /api/jobs/{jobId}/results
Get scraping results for a specific job.

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10)
- `format` (optional): Response format (json, csv, xml)

#### GET /api/jobs/{jobId}/results/{resultId}
Get a specific scraping result.

### Stats API

#### GET /api/stats/dashboard
Get dashboard statistics.

**Response:**
```json
{
  "totalJobs": 15,
  "activeJobs": 3,
  "completedJobs": 12,
  "totalResults": 1250,
  "successRate": 94.5,
  "lastWeekStats": {
    "jobsCreated": 5,
    "jobsCompleted": 8,
    "dataPoints": 450
  }
}
```

### Tags API

#### GET /api/tags
Get all available tags.

#### POST /api/tags
Create a new tag.

#### PUT /api/tags/{id}
Update a tag.

#### DELETE /api/tags/{id}
Delete a tag.

## Error Handling

The API uses standard HTTP status codes and returns consistent error responses:

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request data",
    "details": [
      {
        "field": "url",
        "message": "URL is required"
      }
    ]
  }
}
```

### Status Codes
- `200` - Success
- `201` - Created
- `400` - Bad Request
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not Found
- `500` - Internal Server Error

## Rate Limiting

API requests are rate-limited to prevent abuse:
- **Authenticated users**: 1000 requests per hour
- **Anonymous users**: 100 requests per hour

## WebSocket Events (SignalR)

Real-time updates are provided via SignalR:

### Connection
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/scrapingHub")
    .build();
```

### Events
- `JobStarted` - When a scraping job begins
- `JobCompleted` - When a scraping job finishes
- `JobFailed` - When a scraping job encounters an error
- `ProgressUpdate` - Real-time progress updates

## SDKs and Examples

### JavaScript/Node.js Example
```javascript
const response = await fetch('/api/jobs', {
    headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
    }
});
const jobs = await response.json();
```

### C# Example
```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync("/api/jobs");
var jobs = await response.Content.ReadFromJsonAsync<JobResponse>();
```

## Swagger Documentation

Interactive API documentation is available at:
- **Production**: https://scrapewise-app-x2w3nky2.azurewebsites.net/swagger
- **Local**: https://localhost:7139/swagger
