using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add API controllers
builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });
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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<ScrapingHub>("/scrapingHub");

// Map API routes
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed users on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any(u => u.Email == "admin@scrapewise.com"))
    {
        var admin = new User
        {
            UserName = "admin",
            Email = "admin@scrapewise.com",
            Password = "admin123",
            Role = "Admin",
            IsActive = true,
            Profile = new Profile { DisplayName = "Admin", AvatarUrl = "https://www.gravatar.com/avatar/?d=mp" }
        };
        db.Users.Add(admin);
    }
    if (!db.Users.Any(u => u.Email == "user@scrapewise.com"))
    {
        var user = new User
        {
            UserName = "user",
            Email = "user@scrapewise.com",
            Password = "user123",
            Role = "User",
            IsActive = true,
            Profile = new Profile { DisplayName = "User", AvatarUrl = "https://www.gravatar.com/avatar/?d=mp" }
        };
        db.Users.Add(user);
    }
    db.SaveChanges();
}

app.Run();
