using Microsoft.AspNetCore.Identity;

namespace ScrapeWise.Extensions;

/// <summary>
/// Extension methods for WebApplication to handle application initialization tasks.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures and seeds the application database.
    /// This method handles database initialization separately from the main Program.cs flow.
    /// </summary>
    /// <param name="app">The web application instance</param>
    /// <returns>The web application for method chaining</returns>
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try 
        {
            await SeedData.InitializeAsync(scope.ServiceProvider);
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw; // Re-throw to prevent application startup with incomplete data
        }
        
        return app;
    }
}
