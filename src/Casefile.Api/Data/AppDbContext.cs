using Microsoft.EntityFrameworkCore;

namespace Casefile.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SupportCaseRecord> Cases => Set<SupportCaseRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cases = modelBuilder.Entity<SupportCaseRecord>();
        cases.ToTable("cases");
        cases.HasKey(x => x.Id);
        cases.Property(x => x.Title).HasMaxLength(120).IsRequired();
        cases.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        cases.Property(x => x.RequesterEmail).HasMaxLength(254).IsRequired();
        cases.Property(x => x.Severity).HasConversion<string>().HasMaxLength(16);
        cases.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        cases.HasIndex(x => x.Status);
        cases.HasIndex(x => x.RequesterEmail);
    }
}
