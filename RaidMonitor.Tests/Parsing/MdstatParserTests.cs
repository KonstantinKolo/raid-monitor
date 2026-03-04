using RaidMonitor.Core.Parsing;
using Xunit;

namespace RaidMonitor.Tests.Parsing;

public class MdstatParserTests
{
    private const string HealthyArray = """
        Personalities : [raid1]
        md0 : active raid1 sda[0] sdb[1]
              1953381440 blocks super 1.2 [2/2] [UU]

        unused devices: <none>
        """;

    private const string DegradedArray = """
        Personalities : [raid1]
        md0 : active raid1 sda[0] sdb[1](F)
              1953381440 blocks super 1.2 [2/1] [U_]

        unused devices: <none>
        """;

    private const string ResyncingArray = """
        Personalities : [raid1]
        md0 : active raid1 sda[0] sdb[1]
              1953381440 blocks super 1.2 [2/2] [UU]
              [=>.................]  resync = 8.5% (166420480/1953381440) finish=142.3min speed=209808K/sec

        unused devices: <none>
        """;

    [Fact]
    public void Parse_HealthyArray_ReturnsOneArray()
    {
        var result = MdstatParser.Parse(HealthyArray);
        Assert.Single(result);
    }

    [Fact]
    public void Parse_HealthyArray_HasCorrectName()
    {
        var result = MdstatParser.Parse(HealthyArray);
        Assert.Equal("md0", result[0].Name);
    }

    [Fact]
    public void Parse_HealthyArray_HasCorrectRaidLevel()
    {
        var result = MdstatParser.Parse(HealthyArray);
        Assert.Equal("raid1", result[0].RaidLevel);
    }

    [Fact]
    public void Parse_HealthyArray_HasTwoActiveDisks()
    {
        var result = MdstatParser.Parse(HealthyArray);
        Assert.Equal(2, result[0].Disks.Count);
        Assert.All(result[0].Disks, d => Assert.True(d.IsActive));
    }

    [Fact]
    public void Parse_HealthyArray_HasNoSyncProgress()
    {
        var result = MdstatParser.Parse(HealthyArray);
        Assert.Null(result[0].SyncProgress);
    }

    [Fact]
    public void Parse_DegradedArray_HasFaultyDisk()
    {
        var result = MdstatParser.Parse(DegradedArray);
        Assert.Contains(result[0].Disks, d => d.IsFaulty);
    }

    [Fact]
    public void Parse_ResyncingArray_HasSyncProgress()
    {
        var result = MdstatParser.Parse(ResyncingArray);
        Assert.NotNull(result[0].SyncProgress);
    }

    [Fact]
    public void Parse_ResyncingArray_HasCorrectPercent()
    {
        var result = MdstatParser.Parse(ResyncingArray);
        Assert.Equal(8.5, result[0].SyncProgress!.Percent);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyList()
    {
        var result = MdstatParser.Parse("");
        Assert.Empty(result);
    }
}