using Microsoft.EntityFrameworkCore;

namespace ForestLib.Database;

public class ForestContext : DbContext
{
    public DbSet<RawMessage> RawMessages { get; set; }
    public DbSet<TmOperation> Operations { get; set; }
    public DbSet<Attack> Attacks { get; set; }
    public DbSet<Kingdom> Kingdoms { get; set; }
    public DbSet<Province> Provinces { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Settings.Instance.CreateConnectionStringBuilder().ConnectionString);
        options.EnableSensitiveDataLogging(true);
    }
}