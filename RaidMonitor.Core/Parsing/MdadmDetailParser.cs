using RaidMonitor.Core.Models;
using System.Text.RegularExpressions;
using System.Linq;

namespace RaidMonitor.Core.Parsing;

public static class MdadmDetailParser
{
    public static void EnrichArray(ArrayInfo array, string detailOutput)
    {
        foreach (var line in detailOutput.Split('\n').Select(l => l.Trim()))
        {
            var uuidMatch = Regex.Match(line, @"^UUID\s*:\s+(\S+)");
            if (uuidMatch.Success)
                array.Uuid = uuidMatch.Groups[1].Value;

            var creationMatch = Regex.Match(line, @"^Creation Time\s*:\s+(.+)");
            if (creationMatch.Success)
                array.CreationTime = creationMatch.Groups[1].Value.Trim();

            var updateMatch = Regex.Match(line, @"^Update Time\s*:\s+(.+)");
            if (updateMatch.Success)
                array.UpdateTime = updateMatch.Groups[1].Value.Trim();

            var sizeMatch = Regex.Match(line, @"^Array Size\s*:\s+\d+\s+\((.+?)\)");
            if (sizeMatch.Success)
                array.ArraySize = sizeMatch.Groups[1].Value.Trim();

            var chunkMatch = Regex.Match(line, @"^Chunk Size\s*:\s+(\S+)");
            if (chunkMatch.Success)
                array.ChunkSize = chunkMatch.Groups[1].Value;
        }
    }
}
