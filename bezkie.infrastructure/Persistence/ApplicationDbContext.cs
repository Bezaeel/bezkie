using bezkie.core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace bezkie.infrastructure.Persistence;

public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }
    DbSet<User> Users { get; }
    DbSet<RSVP> RSVPs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Book> Books => Set<Book>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RSVP> RSVPs => Set<RSVP>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
