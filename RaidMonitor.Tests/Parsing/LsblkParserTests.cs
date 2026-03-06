using RaidMonitor.Core.Parsing;
using Xunit;

namespace RaidMonitor.Tests.Parsing;

public class LsblkParserTests
{
    private const string LockedLuks = """
        {
          "blockdevices": [
            {
              "name": "md0",
              "type": "raid1",
              "fstype": "crypto_LUKS",
              "mountpoint": null
            }
          ]
        }
        """;

    private const string UnlockedLuks = """
    {
      "blockdevices": [
        {
          "name": "md0",
          "type": "raid1",
          "fstype": "crypto_LUKS",
          "mountpoint": null,
          "children": [
            {
              "name": "md0_crypt",
              "type": "crypt",
              "fstype": "ext4",
              "mountpoint": "/mnt/data"
            }
          ]
        }
      ]
    }
    """;

    private const string NoEncryption = """
        {
          "blockdevices": [
            {
              "name": "md0",
              "type": "raid1",
              "fstype": "ext4",
              "mountpoint": "/mnt/data"
            }
          ]
        }
        """;

    [Fact]
    public void Parse_LockedLuks_IsEncryptedTrue()
    {
        var result = LsblkParser.ParseEncryption("md0", LockedLuks);
        Assert.NotNull(result);
        Assert.True(result.IsEncrypted);
    }

    [Fact]
    public void Parse_LockedLuks_IsUnlockedFalse()
    {
        var result = LsblkParser.ParseEncryption("md0", LockedLuks);
        Assert.False(result!.IsUnlocked);
    }

    [Fact]
    public void Parse_UnlockedLuks_IsUnlockedTrue()
    {
        var result = LsblkParser.ParseEncryption("md0", UnlockedLuks);
        Assert.NotNull(result);
        Assert.True(result.IsUnlocked);
    }

    [Fact]
    public void Parse_UnlockedLuks_HasMappedName()
    {
        var result = LsblkParser.ParseEncryption("md0", UnlockedLuks);
        Assert.Equal("md0_crypt", result!.MappedName);
    }

    [Fact]
    public void Parse_UnlockedLuks_HasMountPoint()
    {
        var result = LsblkParser.ParseEncryption("md0", UnlockedLuks);
        Assert.Equal("/mnt/data", result!.MountPoint);
    }

    [Fact]
    public void Parse_NoEncryption_ReturnsNull()
    {
        var result = LsblkParser.ParseEncryption("md0", NoEncryption);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_WrongDevice_ReturnsNull()
    {
        var result = LsblkParser.ParseEncryption("md1", LockedLuks);
        Assert.Null(result);
    }
}