namespace RaidMonitor.Core.Models;

public class SmartInfo
{
    public string Device { get; set; } = "";
    public bool IsHealthy { get; set; }
    public int Temperature { get; set; }
    public int ReallocatedSectors { get; set; }
    public int PowerOnHours { get; set; }
    public string OverallHealth { get; set; } = ""; // PASSED / FAILED
}