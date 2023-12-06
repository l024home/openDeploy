using Microsoft.EntityFrameworkCore;
using OpenDeploy.Domain.Models;

namespace OpenDeploy.SQLite;

/// <summary> OpenDeploy 数据上下文 </summary>
public class OpenDeployDbContext : DbContext
{
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<PublishRecord> PublishRecords { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=OpenDeploy.db");
}
