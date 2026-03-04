namespace RaidMonitor.Core.Models;

public class SmartInfo
{
    public string Device { get; set; } = "";
    public bool IsHealthy { get; set; }
    public int Temperature { get; set; }
    public int ReallocatedSectors { get; set; }
    public int PowerOnHours { get; set; }
    public string OverallHealth { get; set; } = "";
    public bool IsUsbEnclosure { get; set; }
    public string? SerialNumber { get; set; }
    public string? Capacity { get; set; }
    public string LimitationNote { get; set; } = "";
    public string? ProductName { get; set; }
    public string? FormFactor { get; set; }
    public string? LogicalUnitId { get; set; }
    public bool? ReadCacheEnabled { get; set; }
    public bool? WriteCacheEnabled { get; set; }
    public string? BlockSize { get; set; }
}