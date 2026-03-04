using RaidMonitor.Core.Parsing;
using Xunit;

namespace RaidMonitor.Tests.Parsing;

public class SmartctlParserTests
{
    private const string HealthyDrive = """
        SMART overall-health self-assessment test result: PASSED

        ID# ATTRIBUTE_NAME          FLAG     VALUE WORST THRESH TYPE      UPDATED  RAW_VALUE
          5 Reallocated_Sector_Ct   0x0033   100   100   036    Pre-fail  Always       0
          9 Power_On_Hours          0x0032   098   098   000    Old_age   Always       8760
        194 Temperature_Celsius     0x0022   064   052   000    Old_age   Always       36
        """;

    private const string FailingDrive = """
        SMART overall-health self-assessment test result: FAILED!

        ID# ATTRIBUTE_NAME          FLAG     VALUE WORST THRESH TYPE      UPDATED  RAW_VALUE
          5 Reallocated_Sector_Ct   0x0033   062   062   036    Pre-fail  Always       300
          9 Power_On_Hours          0x0032   072   072   000    Old_age   Always       24000
        194 Temperature_Celsius     0x0022   040   035   000    Old_age   Always       55
        """;

    [Fact]
    public void Parse_HealthyDrive_IsHealthyTrue()
    {
        var result = SmartctlParser.Parse("sda", HealthyDrive);
        Assert.True(result.IsHealthy);
    }

    [Fact]
    public void Parse_HealthyDrive_CorrectTemperature()
    {
        var result = SmartctlParser.Parse("sda", HealthyDrive);
        Assert.Equal(36, result.Temperature);
    }

    [Fact]
    public void Parse_HealthyDrive_ZeroReallocatedSectors()
    {
        var result = SmartctlParser.Parse("sda", HealthyDrive);
        Assert.Equal(0, result.ReallocatedSectors);
    }

    [Fact]
    public void Parse_HealthyDrive_CorrectPowerOnHours()
    {
        var result = SmartctlParser.Parse("sda", HealthyDrive);
        Assert.Equal(8760, result.PowerOnHours);
    }

    [Fact]
    public void Parse_FailingDrive_IsHealthyFalse()
    {
        var result = SmartctlParser.Parse("sda", FailingDrive);
        Assert.False(result.IsHealthy);
    }

    [Fact]
    public void Parse_FailingDrive_HighReallocatedSectors()
    {
        var result = SmartctlParser.Parse("sda", FailingDrive);
        Assert.Equal(300, result.ReallocatedSectors);
    }

    [Fact]
    public void Parse_FailingDrive_CorrectTemperature()
    {
        var result = SmartctlParser.Parse("sda", FailingDrive);
        Assert.Equal(55, result.Temperature);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsDefaultValues()
    {
        var result = SmartctlParser.Parse("sda", "");
        Assert.Equal("sda", result.Device);
        Assert.False(result.IsHealthy);
        Assert.Equal(0, result.Temperature);
    }

    private const string UsbEnclosureDrive = """
    /dev/sda: Unknown USB bridge [0x2109:0x0561 (0x002)]
    Product:              Unionsine0USB3.0
    Serial number:        ED2025070500E1
    User Capacity:        2,000,398,934,016 bytes [2.00 TB]
    SMART support is:     Unavailable - device lacks SMART capability.
    Current Drive Temperature:     0 C
    """;

    [Fact]
    public void Parse_UsbEnclosure_IsUsbEnclosureTrue()
    {
        var result = SmartctlParser.Parse("sda", UsbEnclosureDrive);
        Assert.True(result.IsUsbEnclosure);
    }

    [Fact]
    public void Parse_UsbEnclosure_HasLimitationNote()
    {
        var result = SmartctlParser.Parse("sda", UsbEnclosureDrive);
        Assert.False(string.IsNullOrEmpty(result.LimitationNote));
    }

    [Fact]
    public void Parse_UsbEnclosure_ParsesSerialNumber()
    {
        var result = SmartctlParser.Parse("sda", UsbEnclosureDrive);
        Assert.Equal("ED2025070500E1", result.SerialNumber);
    }

    [Fact]
    public void Parse_UsbEnclosure_ParsesCapacity()
    {
        var result = SmartctlParser.Parse("sda", UsbEnclosureDrive);
        Assert.Equal("2.00 TB", result.Capacity);
    }

    [Fact]
    public void Parse_UsbEnclosure_AssumedHealthy()
    {
        var result = SmartctlParser.Parse("sda", UsbEnclosureDrive);
        Assert.True(result.IsHealthy);
    }
}