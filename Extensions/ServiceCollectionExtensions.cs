using Microsoft.AspNetCore.Identity;

/// <summary>
/// Extension methods for IServiceCollection to configure application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Seeds the database with initial users and roles.
    /// This method should be called after the application is built but before it runs.
    /// </summary>
    /// <param name="services">The service provider to get required services</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task SeedDatabaseAsync(this IServiceProvider services)
    {
        try 
        {
            await SeedData.InitializeAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
