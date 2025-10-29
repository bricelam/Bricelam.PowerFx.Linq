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
        var source = new List<Dictionary<string, object?>>
        {
            new() {
                ["Value"] = 1.0
            }
        };

        var result = source
            .AsQueryable()
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
        var source = new List<Dictionary<string, object?>>
        {
            new() {
                ["Value"] = null
            },
            new() {
                ["Value"] = 1.0
            }
        };

        var result = source
            .AsQueryable()
            .AddColumns(("IsBlank", "IsBlank(Value)"))
            .ToList();

        Assert.Collection(
            result,
            i =>
            {
                Assert.Null(i["Value"]);
                Assert.True((bool)i["IsBlank"]!);
            },
            i =>
            {
                Assert.Equal((double?)1.0, i["Value"]);
                Assert.False((bool)i["IsBlank"]!);
            });
    }

    [Fact]
    public void AddColumns_throws_when_dictionary_all_nulls()
    {
        var source = new List<Dictionary<string, object?>>
        {
            new() {
                ["Value"] = null
            },
            new() {
                ["Value"] = null
            }
        };

        var ex = Assert.Throws<PowerFxLinqException>(
            () => source
                .AsQueryable()
                .AddColumns(("IsBlank", "IsBlank(Value)"))
                .ToList());

        Assert.Equal("Cannot determine the dictionary value types of the query source.", ex.Message);
    }

    [Fact]
    public void AddColumns_throws_when_empty_and_dictionary()
    {
        var source = new List<Dictionary<string, object?>>();

        var ex = Assert.Throws<PowerFxLinqException>(
            () => source
                .AsQueryable()
                .AddColumns(("IsBlank", "IsBlank(Value)"))
                .ToList());

        Assert.Equal("Cannot determine the dictionary keys of the query source.", ex.Message);
    }
}
