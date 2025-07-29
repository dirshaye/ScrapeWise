// Main entry point for the ScrapeWise application
// Configures services, middleware pipeline, and starts the web server

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ScrapeWise.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add API controllers
builder.Services.AddControllers();

// Add SignalR for real-time web scraping job notifications
// Used in: ScrapingHub.cs, JobsController.cs, signalr-connection.js
// Purpose: Live updates during scraping operations
builder.Services.AddSignalR();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<MyUser, IdentityRole>(options => {
        options.SignIn.RequireConfirmedAccount = false; // Allow login without email confirmation
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true; // Ensure unique emails
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Configure Identity to use email as username
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
});

// Register application services for proper separation of concerns
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScrapeWise API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub for real-time scraping notifications
// Endpoint: /scrapingHub (used by signalr-connection.js)
// Provides: Job start, progress, completion, and error notifications
app.MapHub<ScrapingHub>("/scrapingHub");

// Map API routes
app.MapControllers();

// Legacy route redirects - replaces ScraperController
app.MapControllerRoute(
    name: "scraper_dashboard_legacy",
    pattern: "Scraper/Dashboard",
    defaults: new { controller = "Jobs", action = "Index" });

app.MapControllerRoute(
    name: "scraper_newjob_legacy", 
    pattern: "Scraper/NewJob",
    defaults: new { controller = "Jobs", action = "Create" });

app.MapControllerRoute(
    name: "scraper_details_legacy",
    pattern: "Scraper/JobDetails/{id?}",
    defaults: new { controller = "Jobs", action = "Details" });

app.MapControllerRoute(
    name: "scraper_results_legacy",
    pattern: "Scraper/Results/{jobId?}",
    defaults: new { controller = "Results", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Initialize database with seeding - separated from main application flow
await app.SeedDatabaseAsync();

app.Run();
