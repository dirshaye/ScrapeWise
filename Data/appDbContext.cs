using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ScrapingJob> ScrapingJobs { get; set; }
    public DbSet<ScrapingResult> ScrapingResults { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ScrapingJobs)
            .WithOne(j => j.User)
            .HasForeignKey(j => j.UserId);

        modelBuilder.Entity<ScrapingJob>()
            .HasMany(j => j.ScrapingResults)
            .WithOne(r => r.ScrapingJob)
            .HasForeignKey(r => r.ScrapingJobId);

        // Many-to-Many relationship between ScrapingJob and Tag
        modelBuilder.Entity<ScrapingJob>()
            .HasMany(j => j.Tags)
            .WithMany(t => t.ScrapingJobs)
            .UsingEntity(j => j.ToTable("ScrapingJobTags"));
    }
}
