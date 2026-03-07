namespace RaidMonitor.Core.Models;

public class EncryptionInfo
{
    public bool IsEncrypted { get; set; }
    public string? Type { get; set; }
    public string? MappedName { get; set; }
    public string? MountPoint { get; set; }
    public bool IsUnlocked { get; set; }
}