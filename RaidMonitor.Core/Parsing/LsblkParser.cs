using RaidMonitor.Core.Models;
using System.Text.Json;

namespace RaidMonitor.Core.Parsing;

public static class LsblkParser
{
    public static EncryptionInfo? ParseEncryption(string arrayName, string lsblkJson)
    {
        try
        {
            var doc = JsonDocument.Parse(lsblkJson);
            var devices = doc.RootElement.GetProperty("blockdevices");
            return SearchDevices(arrayName, devices);
        }
        catch { }
        return null;
    }

    private static EncryptionInfo? SearchDevices(string arrayName, JsonElement devices)
    {
        foreach (var device in devices.EnumerateArray())
        {
            var result = SearchDevice(arrayName, device);
            if (result != null) return result;
        }
        return null;
    }

    private static EncryptionInfo? SearchDevice(string arrayName, JsonElement device)
    {
        var name = device.TryGetProperty("name", out var n) ? n.GetString() : null;
        var fstype = device.TryGetProperty("fstype", out var ft) ? ft.GetString() : null;

        if (name == arrayName)
        {
            if (!IsCryptFstype(fstype)) return null;

            if (device.TryGetProperty("children", out var children))
                foreach (var child in children.EnumerateArray())
                {
                    var childType = child.TryGetProperty("type", out var ct) ? ct.GetString() : null;
                    var childMount = child.TryGetProperty("mountpoint", out var mp) ? mp.GetString() : null;
                    var childName = child.TryGetProperty("name", out var cn) ? cn.GetString() : null;

                    if (childType == "crypt")
                        return new EncryptionInfo
                        {
                            IsEncrypted = true,
                            Type = fstype,
                            MappedName = childName,
                            MountPoint = childMount,
                            IsUnlocked = true
                        };
                }

            return new EncryptionInfo
            {
                IsEncrypted = true,
                Type = fstype,
                IsUnlocked = false
            };
        }

        if (device.TryGetProperty("children", out var nested))
            return SearchDevices(arrayName, nested);

        return null;
    }

    private static bool IsCryptFstype(string? fstype) =>
        fstype is "crypto_LUKS" or "plain" or "bitlk" or "fvault2";
}