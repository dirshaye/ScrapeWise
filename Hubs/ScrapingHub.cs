using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ScrapingHub : Hub
{
    public async Task JoinJobGroup(int jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }

    public async Task LeaveJobGroup(int jobId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"job_{jobId}");
    }

    public async Task JobStarted(int jobId, string url)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobStarted", jobId, url);
    }

    public async Task JobCompleted(int jobId, int resultsCount)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobCompleted", jobId, resultsCount);
    }

    public async Task JobFailed(int jobId, string error)
    {
        await Clients.Group($"job_{jobId}").SendAsync("JobFailed", jobId, error);
    }

    public async Task NewResult(int jobId, string extractedText)
    {
        await Clients.Group($"job_{jobId}").SendAsync("NewResult", jobId, extractedText);
    }
} 