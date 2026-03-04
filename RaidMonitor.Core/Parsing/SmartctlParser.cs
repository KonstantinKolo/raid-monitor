using RaidMonitor.Core.Models;
using System.Text.RegularExpressions;

namespace RaidMonitor.Core.Parsing;

public static class SmartctlParser
{
    public static SmartInfo Parse(string device, string smartctlOutput)
    {
        var info = new SmartInfo { Device = device };

        if (smartctlOutput.Contains("SMART support is:     Unavailable"))
        {
            info.IsUsbEnclosure = true;
            info.LimitationNote = "Limited SMART data — USB enclosure detected";
            info.IsHealthy = true; // assume healthy if no explicit failure
        }

        foreach (var line in smartctlOutput.Split('\n'))
        {
            var healthMatch = Regex.Match(line,
                @"SMART overall-health self-assessment test result:\s+(\w+)");
            if (healthMatch.Success)
            {
                info.OverallHealth = healthMatch.Groups[1].Value;
                info.IsHealthy = info.OverallHealth == "PASSED";
            }

            var tempMatch = Regex.Match(line,
                @"194\s+Temperature_Celsius\s+\S+\s+\d+\s+\d+\s+\d+\s+\S+\s+\S+\s+(\d+)");
            if (tempMatch.Success)
                info.Temperature = int.Parse(tempMatch.Groups[1].Value);

            var scsiTempMatch = Regex.Match(line,
                @"Current Drive Temperature:\s+(\d+)");
            if (scsiTempMatch.Success)
                info.Temperature = int.Parse(scsiTempMatch.Groups[1].Value);

            var reallocMatch = Regex.Match(line,
                @"5\s+Reallocated_Sector_Ct\s+\S+\s+\d+\s+\d+\s+\d+\s+\S+\s+\S+\s+(\d+)");
            if (reallocMatch.Success)
                info.ReallocatedSectors = int.Parse(reallocMatch.Groups[1].Value);

            var pohMatch = Regex.Match(line,
                @"9\s+Power_On_Hours\s+\S+\s+\d+\s+\d+\s+\d+\s+\S+\s+\S+\s+(\d+)");
            if (pohMatch.Success)
                info.PowerOnHours = int.Parse(pohMatch.Groups[1].Value);

            var serialMatch = Regex.Match(line, @"Serial number:\s+(\S+)");
            if (serialMatch.Success)
                info.SerialNumber = serialMatch.Groups[1].Value;

            var capacityMatch = Regex.Match(line,
                @"User Capacity:\s+([\d,]+) bytes \[(.+?)\]");
            if (capacityMatch.Success)
                info.Capacity = capacityMatch.Groups[2].Value;
        }

        return info;
    }
}