using Microsoft.EntityFrameworkCore;
using System;

public class ApplicationDbContext : DbContext
{
    public DbSet<Location> Locations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=Test;User Id=jjtest;Password=test1234;",
            x => x.UseNetTopologySuite());
    }
}

