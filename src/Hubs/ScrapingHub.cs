using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

/// <summary>
/// SignalR hub for real-time updates on scraping jobs.
/// Used to notify clients about job status, results, and errors.
/// 
/// USAGE LOCATIONS:
/// - JobsController.cs: Sends job status notifications during scraping
/// - signalr-connection.js: Client-side connection and event handling
/// - Views/Jobs/Create.cshtml: Real-time progress display
/// - Views/Jobs/Index.cshtml: Live job status updates
/// 
/// ENDPOINTS:
/// - /scrapingHub: Main hub endpoint (configured in Program.cs)
/// 
/// REAL-TIME EVENTS:
/// - JobStarted: When scraping begins
/// - JobProgress: During scraping (new results found)
/// - JobCompleted: When scraping finishes successfully
/// - JobFailed: When scraping encounters errors
/// </summary>
public class ScrapingHub : Hub
{
    /// <summary>
    /// Adds the current connection to a group for a specific job.
    /// </summary>
    public async Task JoinJobGroup(int jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }

    /// <summary>
    /// Removes the current connection from a group for a specific job.
    /// </summary>
    public async Task LeaveJobGroup(int jobId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }

    /// <summary>
    /// Notifies clients in the job group that a job has started.
    /// </summary>
    public async Task JobStarted(int jobId, string url)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobStarted", jobId, url);
    }

    /// <summary>
    /// Notifies clients in the job group that a job has completed.
    /// </summary>
    public async Task JobCompleted(int jobId, int resultsCount)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobCompleted", jobId, resultsCount);
    }

    /// <summary>
    /// Notifies clients in the job group that a job has failed.
    /// </summary>
    public async Task JobFailed(int jobId, string error)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobFailed", jobId, error);
    }

    /// <summary>
    /// Sends a new scraping result to clients in the job group.
    /// </summary>
    public async Task NewResult(int jobId, string extractedText)
    {
        await Clients.Group($"job_{jobId}").SendAsync("NewResult", jobId, extractedText);
    }
} 