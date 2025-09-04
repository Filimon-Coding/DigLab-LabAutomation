using Microsoft.EntityFrameworkCore;
using DigLabAPI.Models;

namespace DigLabAPI.Data;

public class DigLabDb : DbContext
{
    public DigLabDb(DbContextOptions<DigLabDb> opts) : base(opts) {}
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>()
         .HasIndex(o => o.LabNumber)
         .IsUnique();

        // SQLite has no native DateOnly/TimeOnly types; EF maps them to TEXT automatically.
        base.OnModelCreating(b);
    }
}
