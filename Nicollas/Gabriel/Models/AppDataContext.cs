using Microsoft.EntityFrameworkCore;

namespace Gabriel.Models;


public class AppDbContext : DbContext
{
    public DbSet<ConsumoDeAgua> ConsumoDeAguas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Nicollas_Gabriel.db");
    }
}