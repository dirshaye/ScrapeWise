using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<ConfigProfile> ConfigProfiles { get; set; }
    public DbSet<ScrapingJob> ScrapingJobs { get; set; }
    public DbSet<ScrapingResult> ScrapingResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.ConfigProfile)
            .WithOne(cp => cp.User)
            .HasForeignKey<ConfigProfile>(cp => cp.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ScrapingJobs)
            .WithOne(j => j.User);

        modelBuilder.Entity<ScrapingJob>()
            .HasMany(j => j.ScrapingResults)
            .WithOne(r => r.ScrapingJob);
    }
}
