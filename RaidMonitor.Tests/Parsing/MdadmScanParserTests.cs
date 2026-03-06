using RaidMonitor.Core.Parsing;
using Xunit;

namespace RaidMonitor.Tests.Parsing;

public class MdadmScanParserTests
{
    private const string ScanOutput = """
        ARRAY /dev/md/0 metadata=1.2 UUID=de3c16db:17786d6d:3e8a86d7:b0769425
        ARRAY /dev/md/1 metadata=1.2 UUID=ab1c23de:45fg67hi:89jk01lm:23no45pq
        """;

    [Fact]
    public void Parse_TwoArrays_ReturnsBoth()
    {
        var result = MdadmScanParser.Parse(ScanOutput);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Parse_Array_HasCorrectName()
    {
        var result = MdadmScanParser.Parse(ScanOutput);
        Assert.Equal("md0", result[0].Name);
    }

    [Fact]
    public void Parse_Array_HasOfflineState()
    {
        var result = MdadmScanParser.Parse(ScanOutput);
        Assert.Equal("offline", result[0].State);
    }

    [Fact]
    public void Parse_Array_HasUnknownRaidLevel()
    {
        var result = MdadmScanParser.Parse(ScanOutput);
        Assert.Equal("unknown", result[0].RaidLevel);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmpty()
    {
        var result = MdadmScanParser.Parse("");
        Assert.Empty(result);
    }
}