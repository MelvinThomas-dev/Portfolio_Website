using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Models;

namespace Portfolio.Api.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<PageVisit> PageVisits => Set<PageVisit>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactSubmission>(entity =>
        {
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Email);
        });

        modelBuilder.Entity<PageVisit>(entity =>
        {
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.SessionId);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }
}
