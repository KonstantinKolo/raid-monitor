namespace RaidMonitor.Core.Models;

public class ArrayInfo
{
    public string Name { get; set; } = "";
    public string State { get; set; } = "";
    public string RaidLevel { get; set; } = "";
    public List<DiskInfo> Disks { get; set; } = [];
    public SyncProgress? SyncProgress { get; set; } // null if not resyncing
    public EncryptionInfo? Encryption { get; set; }
}

public class DiskInfo
{
    public string Device { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsFaulty { get; set; }
}

public class SyncProgress
{
    public double Percent { get; set; }
    public string Speed { get; set; } = "";
    public string TimeRemaining { get; set; } = "";
}