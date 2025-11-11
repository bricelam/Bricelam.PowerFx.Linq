using Bricelam.PowerFx.Linq.Test;
using Microsoft.Data.Sqlite;

namespace Bricelam.PowerFx.Linq;

public class PowerFxQueryableTests
{
    [Fact]
    public void AddColumns_works()
    {
        var source = Queryable.AsQueryable(
            [
                new { Value = 1.0 }
            ]);

        var result = source
            .AddColumns(("Next", "Value + 1"))
            .ToList();

        var i = Assert.Single(result);
        Assert.Equal(2, i.Count);
        Assert.Equal(1.0, i["Value"]);
        Assert.Equal(2.0, i["Next"]);
    }

    [Fact]
    public void AddColumns_works_again()
    {
        var source = Queryable.AsQueryable(
            [
                new { Value = 1.0 }
            ]);

        var result = source
            .AddColumns(("Next", "Value + 1"))
            .AddColumns(("Previous", "Value - 1"))
            .ToList();

        var i = Assert.Single(result);
        Assert.Equal(3, i.Count);
        Assert.Equal(1.0, i["Value"]);
        Assert.Equal(2.0, i["Next"]);
        Assert.Equal(0.0, i["Previous"]);
    }

    [Fact]
    public void AddColumns_works_when_dictionary()
    {
        var source = Queryable.AsQueryable(
            [
                new Dictionary<string, object?> {
                    ["Value"] = 1.0
                }
            ]);

        var result = source
            .AddColumns(("Next", "Value + 1"))
            .ToList();

        var i = Assert.Single(result);
        Assert.Equal(2, i.Count);
        Assert.Equal(1.0, i["Value"]);
        Assert.Equal(2.0, i["Next"]);
    }

    [Fact]
    public void AddColumns_works_when_dictionary_with_nulls()
    {
        var source = Queryable.AsQueryable(
            new List<Dictionary<string, object?>>
            {
                new() {
                    ["Value"] = null
                },
                new() {
                    ["Value"] = 1.0
                }
            });

        var result = source
            .AddColumns(("IsBlank", "IsBlank(Value)"))
            .AddColumns(("Default", "Coalesce(Value, 0.0)"))
            .ToList();

        Assert.Collection(
            result,
            i =>
            {
                Assert.Null(i["Value"]);
                Assert.True((bool)i["IsBlank"]!);
                Assert.Equal(0.0, i["Default"]!);
            },
            i =>
            {
                Assert.Equal((double?)1.0, i["Value"]);
                Assert.False((bool)i["IsBlank"]!);
                Assert.Equal(1.0, i["Default"]!);
            });
    }

    [Fact]
    public void AddColumns_throws_when_dictionary_all_nulls()
    {
        var source = Queryable.AsQueryable(
            new List<Dictionary<string, object?>>
            {
                new() {
                    ["Value"] = null
                },
                new() {
                    ["Value"] = null
                }
            });

        var result = source
            .AddColumns(("IsBlank", "IsBlank(Value)"))
            .AddColumns(("Default", "Coalesce(Value, 0.0)"))
            .ToList();

        Assert.All(
            result,
            i =>
            {
                Assert.Null(i["Value"]);
                Assert.True((bool)i["IsBlank"]!);
                Assert.Equal(0.0, i["Default"]!);
            });
    }

    [Fact]
    public void AddColumns_throws_when_empty_and_dictionary()
    {
        var source = Queryable.AsQueryable(
            new List<Dictionary<string, object?>>());

        var ex = Assert.Throws<PowerFxLinqException>(
            () => source
                .AddColumns(("IsBlank", "IsBlank(Value)"))
                .ToList());

        Assert.Equal("Cannot determine the dictionary keys of the query source.", ex.Message);
    }

    [Fact]
    public void ShowColumns_works()
    {
        var source = Queryable.AsQueryable(
            [
                new
                {
                    Value = 1.0,
                    IsBlank = false
                }
            ]);

        var result = source
            .ShowColumns("Value")
            .ToList();

        var i = Assert.Single(result);
        var column = Assert.Single(i);
        Assert.Equal("Value", column.Key);
        Assert.Equal(1.0, column.Value);
    }

    [Fact]
    public void ShowColumns_works_when_dictionary()
    {
        var source = Queryable.AsQueryable(
            [
                new Dictionary<string, object?> {
                    ["Value"] = 1.0,
                    ["IsBlank"] = false
                }
            ]);

        var result = source
            .ShowColumns("Value")
            .ToList();

        var i = Assert.Single(result);
        var column = Assert.Single(i);
        Assert.Equal("Value", column.Key);
        Assert.Equal(1.0, column.Value);
    }

    [Fact]
    public void ShowColumns_works_when_EF_and_AddColumns()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        using var db = new TestDbContext(connection);
        db.Database.EnsureCreated();
        db.Entities.Add(new TestEntity { Value = 1.0 });
        db.SaveChanges();

        var result = db.Entities
            .AddColumns(("Next", "Value + 1"))
            .ShowColumns("Next")
            .ToList();

        var i = Assert.Single(result);
        var column = Assert.Single(i);
        Assert.Equal("Next", column.Key);
        Assert.Equal(2.0, column.Value);
    }

    [Fact]
    public void DropColumns_works()
    {
        var source = Queryable.AsQueryable(
            [
                new
                {
                    Value = 1.0,
                    IsBlank = false
                }
            ]);

        var result = source
            .DropColumns("IsBlank")
            .ToList();

        var i = Assert.Single(result);
        var column = Assert.Single(i);
        Assert.Equal("Value", column.Key);
        Assert.Equal(1.0, column.Value);
    }

    [Fact]
    public void DropColumns_works_when_dictionary()
    {
        var source = Queryable.AsQueryable(
            [
                new Dictionary<string, object?> {
                    ["Value"] = 1.0,
                    ["IsBlank"] = false
                }
            ]);

        var result = source
            .DropColumns("IsBlank")
            .ToList();

        var i = Assert.Single(result);
        var column = Assert.Single(i);
        Assert.Equal("Value", column.Key);
        Assert.Equal(1.0, column.Value);
    }
}
