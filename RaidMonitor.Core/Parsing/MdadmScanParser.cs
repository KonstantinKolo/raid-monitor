using RaidMonitor.Core.Models;
using System.Text.RegularExpressions;

namespace RaidMonitor.Core.Parsing;

public static class MdadmScanParser
{
    public static List<ArrayInfo> Parse(string scanOutput)
    {
        var arrays = new List<ArrayInfo>();

        foreach (var line in scanOutput.Split('\n'))
        {
            var match = Regex.Match(line,
                @"ARRAY\s+/dev/(?:md/|md)(\w+)\s+.*UUID=(\S+)");
            if (!match.Success) continue;

            var levelMatch = Regex.Match(line, @"level=(\S+)");

            arrays.Add(new ArrayInfo
            {
                Name = "md" + match.Groups[1].Value.TrimStart('m', 'd'),
                State = "offline",
                RaidLevel = levelMatch.Success ? levelMatch.Groups[1].Value : "unknown",
                Disks = []
            });
        }

        return arrays;
    }
}