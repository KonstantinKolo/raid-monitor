namespace RaidMonitor.Core.Models;

public class ArrayInfo
{
    public string Name { get; set; } = "";
    public string State { get; set; } = "";
    public string RaidLevel { get; set; } = "";
    public List<DiskInfo> Disks { get; set; } = [];
    public SyncProgress? SyncProgress { get; set; } // null if not resyncing
    public EncryptionInfo? Encryption { get; set; }

    // Populated from mdadm --detail
    public string? Uuid { get; set; }
    public string? CreationTime { get; set; }
    public string? UpdateTime { get; set; }
    public string? ArraySize { get; set; }
    public string? ChunkSize { get; set; } // null for RAID1
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