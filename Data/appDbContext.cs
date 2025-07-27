using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// The application's database context, including Identity and domain models.
/// </summary>
public class AppDbContext : IdentityDbContext<MyUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ScrapingJob> ScrapingJobs { get; set; }
    public DbSet<ScrapingResult> ScrapingResults { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // ScrapingJob: PK is ScrapingJobId
        modelBuilder.Entity<ScrapingJob>()
            .HasKey(j => j.ScrapingJobId);
        modelBuilder.Entity<ScrapingJob>()
            .HasOne(j => j.User)
            .WithMany(u => u.ScrapingJobs)
            .HasForeignKey(j => j.UserId);
        modelBuilder.Entity<ScrapingJob>()
            .HasMany(j => j.ScrapingResults)
            .WithOne(r => r.ScrapingJob)
            .HasForeignKey(r => r.ScrapingJobId);
        modelBuilder.Entity<ScrapingJob>()
            .HasMany(j => j.Tags)
            .WithMany(t => t.ScrapingJobs)
            .UsingEntity(j => j.ToTable("ScrapingJobTags"));

        modelBuilder.Entity<MyUser>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId);
    }
}
