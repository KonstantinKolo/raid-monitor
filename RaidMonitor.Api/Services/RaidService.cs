using RaidMonitor.Core.Models;
using RaidMonitor.Core.Parsing;
using System.Linq;

namespace RaidMonitor.Api.Services;

public class RaidService
{
    public async Task<List<ArrayInfo>> GetArraysAsync()
    {
        var arrays = new List<ArrayInfo>();

        try
        {
            var mdstat = await File.ReadAllTextAsync("/proc/mdstat");
            arrays = MdstatParser.Parse(mdstat);
        }
        catch { }

        try
        {
            var scanOutput = await RunCommandAsync("/sbin/mdadm", "--examine --scan");
            var offlineArrays = MdadmScanParser.Parse(scanOutput);

            foreach (var offline in offlineArrays)
            {
                if (!arrays.Any(a => a.Name == offline.Name))
                    arrays.Add(offline);
            }
        }
        catch { }

        try
        {
            var lsblkJson = await RunCommandAsync("/usr/bin/lsblk", "-J -o NAME,TYPE,FSTYPE,MOUNTPOINT");
            foreach (var array in arrays)
                array.Encryption = LsblkParser.ParseEncryption(array.Name, lsblkJson);
        }
        catch { }

        return arrays;
    }

    public async Task<SmartInfo> GetSmartInfoAsync(string device)
    {
        var output = await RunCommandAsync("/usr/sbin/smartctl", $"-x /dev/{device}");
        if (output.Contains("Unknown USB bridge") || output.Contains("specify device type"))
            output = await RunCommandAsync("/usr/sbin/smartctl", $"-x -d scsi /dev/{device}");
        return SmartctlParser.Parse(device, output);
    }

    private static async Task<string> RunCommandAsync(string command, string args)
    {
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return output;
    }
}