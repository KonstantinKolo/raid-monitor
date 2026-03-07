using RaidMonitor.Core.Models;
using RaidMonitor.Core.Parsing;
using Xunit;

namespace RaidMonitor.Tests.Parsing;

public class MdadmDetailParserTests
{
    private const string Raid1Detail = """
        /dev/md0:
                   Version : 1.2
             Creation Time : Mon Jan 15 12:34:56 2024
                Raid Level : raid1
                Array Size : 1953381440 (1863.02 GiB 2000.26 GB)
             Used Dev Size : 1953381440 (1863.02 GiB 2000.26 GB)
              Raid Devices : 2
             Total Devices : 2
               Persistence : Superblock is persistent

               Update Time : Tue Jan 16 08:22:11 2024
                     State : clean
            Active Devices : 2
           Working Devices : 2
            Failed Devices : 0
             Spare Devices : 0

        Consistency Policy : resync

                      Name : homeserver:0
                      UUID : a1b2c3d4:e5f6a7b8:c9d0e1f2:a3b4c5d6
                    Events : 18

            Number   Major   Minor   RaidDevice State
               0     8        0        0      active sync   /dev/sda
               1     8       16        1      active sync   /dev/sdb
        """;

    private const string Raid5Detail = """
        /dev/md1:
                   Version : 1.2
             Creation Time : Wed Feb 21 09:00:00 2024
                Raid Level : raid5
                Array Size : 3906762752 (3726.04 GiB 4000.78 GB)
             Used Dev Size : 1953381376 (1863.02 GiB 2000.26 GB)
              Raid Devices : 3
             Total Devices : 3
               Persistence : Superblock is persistent

               Update Time : Thu Feb 22 14:11:05 2024
                     State : clean
            Active Devices : 3
           Working Devices : 3
            Failed Devices : 0
             Spare Devices : 0

                Chunk Size : 512K

        Consistency Policy : resync

                      UUID : deadbeef:cafebabe:12345678:abcdef01
                    Events : 42
        """;

    [Fact]
    public void EnrichArray_ParsesUuid()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid1Detail);
        Assert.Equal("a1b2c3d4:e5f6a7b8:c9d0e1f2:a3b4c5d6", array.Uuid);
    }

    [Fact]
    public void EnrichArray_ParsesCreationTime()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid1Detail);
        Assert.Equal("Mon Jan 15 12:34:56 2024", array.CreationTime);
    }

    [Fact]
    public void EnrichArray_ParsesUpdateTime()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid1Detail);
        Assert.Equal("Tue Jan 16 08:22:11 2024", array.UpdateTime);
    }

    [Fact]
    public void EnrichArray_ParsesArraySize()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid1Detail);
        Assert.Equal("1863.02 GiB 2000.26 GB", array.ArraySize);
    }

    [Fact]
    public void EnrichArray_Raid1_HasNoChunkSize()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid1Detail);
        Assert.Null(array.ChunkSize);
    }

    [Fact]
    public void EnrichArray_Raid5_ParsesChunkSize()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, Raid5Detail);
        Assert.Equal("512K", array.ChunkSize);
    }

    [Fact]
    public void EnrichArray_EmptyOutput_LeavesFieldsNull()
    {
        var array = new ArrayInfo();
        MdadmDetailParser.EnrichArray(array, "");
        Assert.Null(array.Uuid);
        Assert.Null(array.CreationTime);
        Assert.Null(array.ArraySize);
    }
}
