# SignalR Implementation in ScrapeWise

## Overview
SignalR is used throughout ScrapeWise to provide real-time updates during web scraping operations, enhancing user experience with live progress feedback.

## Configuration

### Program.cs Setup
```csharp
// Service Registration
builder.Services.AddSignalR();

// Hub Mapping
app.MapHub<ScrapingHub>("/scrapingHub");
```

## Implementation Components

### 1. Server-Side Hub (`Hubs/ScrapingHub.cs`)
- **Purpose**: Central hub for broadcasting scraping events
- **Methods**: 
  - `JoinJobGroup(int jobId)` - Subscribe to job updates
  - `LeaveJobGroup(int jobId)` - Unsubscribe from updates
  - `JobStarted()`, `JobProgress()`, `JobCompleted()`, `JobFailed()` - Event notifications

### 2. Controller Integration (`Controllers/JobsController.cs`)
- **Integration Point**: Injected `IHubContext<ScrapingHub>`
- **Usage**: Broadcasts events during scraping workflow:
  ```csharp
  // Job Started
  await _hubContext.Clients.All.SendAsync("JobStarted", job.ScrapingJobId, job.TargetUrl);
  
  // Progress Updates
  await _hubContext.Clients.All.SendAsync("JobProgress", job.ScrapingJobId, result);
  
  // Completion
  await _hubContext.Clients.All.SendAsync("JobCompleted", job.ScrapingJobId, totalResults);
  ```

### 3. Client-Side Connection (`wwwroot/js/signalr-connection.js`)
- **Purpose**: Manages browser-side SignalR connection
- **Features**:
  - Automatic reconnection
  - Event handling for job updates
  - Toast notifications
  - Real-time UI updates

### 4. Frontend Integration

#### Views Using SignalR:
- **`Views/Jobs/Create.cshtml`**: Real-time progress during job creation
- **`Views/Jobs/Index.cshtml`**: Live job status updates
- **`Views/Shared/_Layout.cshtml`**: Global SignalR scripts

#### SignalR Scripts Loading:
```html
<!-- SignalR Client Library -->
<script src="https://unpkg.com/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
<!-- ScrapeWise SignalR Connection -->
<script src="~/js/signalr-connection.js"></script>
```

## Real-Time Events Flow

1. **Job Creation** → SignalR broadcasts "JobStarted"
2. **Scraping Progress** → SignalR broadcasts "JobProgress" for each result
3. **Job Completion** → SignalR broadcasts "JobCompleted" with summary
4. **Error Handling** → SignalR broadcasts "JobFailed" with error details

## Benefits

- **Real-time feedback**: Users see scraping progress instantly
- **Better UX**: No need to refresh pages for updates
- **Error awareness**: Immediate notification of failures
- **Progress tracking**: Live updates on job status

## Technical Details

- **Connection Endpoint**: `/scrapingHub`
- **Transport**: WebSockets (with fallbacks)
- **Reconnection**: Automatic with exponential backoff
- **Groups**: Job-specific groups for targeted notifications
- **Error Handling**: Graceful degradation if connection fails
