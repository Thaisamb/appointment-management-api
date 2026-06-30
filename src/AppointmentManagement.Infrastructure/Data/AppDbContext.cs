using Microsoft.EntityFrameworkCore;
using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceIssuer> InvoiceIssuers => Set<InvoiceIssuer>();
    public DbSet<InvoiceIssuance> InvoiceIssuances => Set<InvoiceIssuance>();
    public DbSet<FinancialResponsible> FinancialResponsibles => Set<FinancialResponsible>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTimestamps()
    {
        var utcNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedAt") is { } createdAt)
                    createdAt.CurrentValue = utcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                if (entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt") is { } updatedAt)
                    updatedAt.CurrentValue = utcNow;
            }
        }
    }
}