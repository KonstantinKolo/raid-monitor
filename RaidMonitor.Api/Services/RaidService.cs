using RaidMonitor.Core.Models;
using RaidMonitor.Core.Parsing;

namespace RaidMonitor.Api.Services;

public class RaidService
{
    public async Task<List<ArrayInfo>> GetArraysAsync()
    {
        var mdstat = await File.ReadAllTextAsync("/proc/mdstat");
        return MdstatParser.Parse(mdstat);
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