using RaidMonitor.Core.Models;
using System.Text.RegularExpressions;

namespace RaidMonitor.Core.Parsing;

public static class MdstatParser
{
    public static List<ArrayInfo> Parse(string mdstatContent)
    {
        var arrays = new List<ArrayInfo>();
        var lines = mdstatContent.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            var arrayMatch = Regex.Match(lines[i],
                @"^(md\d+)\s+:\s+(\w+)\s+(raid\d+|linear|multipath|faulty)\s+(.+)$");

            if (!arrayMatch.Success) continue;

            var array = new ArrayInfo
            {
                Name = arrayMatch.Groups[1].Value,
                State = arrayMatch.Groups[2].Value,
                RaidLevel = arrayMatch.Groups[3].Value,
                Disks = ParseDisks(arrayMatch.Groups[4].Value)
            };

            if (i + 2 < lines.Length)
                array.SyncProgress = ParseSyncProgress(lines[i + 2]);

            arrays.Add(array);
        }

        return arrays;
    }

    private static List<DiskInfo> ParseDisks(string diskSection)
    {
        var disks = new List<DiskInfo>();

        foreach (Match m in Regex.Matches(diskSection, @"(\w+)\[(\d+)\](\(F\)|\(S\))?"))
        {
            disks.Add(new DiskInfo
            {
                Device = m.Groups[1].Value,
                IsActive = m.Groups[3].Value != "(S)",
                IsFaulty = m.Groups[3].Value == "(F)"
            });
        }

        return disks;
    }

    private static SyncProgress? ParseSyncProgress(string line)
    {
        var match = Regex.Match(line,
            @"=\s+([\d.]+)%\s+\(.+\)\s+finish=([\d.]+min)\s+speed=(\S+)");

        if (!match.Success) return null;

        return new SyncProgress
        {
            Percent = double.Parse(match.Groups[1].Value),
            TimeRemaining = match.Groups[2].Value,
            Speed = match.Groups[3].Value
        };
    }
}