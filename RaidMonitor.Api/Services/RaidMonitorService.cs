using Microsoft.AspNetCore.SignalR;
using RaidMonitor.Api.Hubs;
using RaidMonitor.Core.Models;

namespace RaidMonitor.Api.Services;

public class RaidMonitorService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IHubContext<RaidHub> _hubContext;
    private List<ArrayInfo> _lastState = [];
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    public RaidMonitorService(IServiceProvider services, IHubContext<RaidHub> hubContext)
    {
        _services = services;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(PollInterval, stoppingToken);
            try
            {
                using var scope = _services.CreateScope();
                var raidService = scope.ServiceProvider.GetRequiredService<RaidService>();
                var current = await raidService.GetArraysAsync();

                if (HasChanged(current))
                {
                    _lastState = current;
                    await _hubContext.Clients.All.SendAsync("ArraysUpdated", current, stoppingToken);
                }
            }
            catch { }
        }
    }

    private bool HasChanged(List<ArrayInfo> current)
    {
        if (current.Count != _lastState.Count) return true;

        foreach (var array in current)
        {
            var prev = _lastState.FirstOrDefault(a => a.Name == array.Name);
            if (prev == null) return true;
            if (array.State != prev.State) return true;
            if (array.Disks.Count != prev.Disks.Count) return true;
            if (array.Disks.Any(d => !prev.Disks.Any(pd =>
                    pd.Device == d.Device &&
                    pd.IsActive == d.IsActive &&
                    pd.IsFaulty == d.IsFaulty)))
                return true;
        }

        return false;
    }
}
