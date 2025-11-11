using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Bricelam.PowerFx.Linq.Test;

sealed class TestDbContext : DbContext
{
    readonly SqliteConnection _connection;

    public TestDbContext(SqliteConnection connection)
        => _connection = connection;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(_connection);

    public DbSet<TestEntity> Entities { get; set; }
}
