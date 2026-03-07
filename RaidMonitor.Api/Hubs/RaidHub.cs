using Microsoft.AspNetCore.SignalR;
using RaidMonitor.Api.Services;

namespace RaidMonitor.Api.Hubs;

public class RaidHub : Hub
{
    private readonly RaidService _raidService;

    public RaidHub(RaidService raidService)
    {
        _raidService = raidService;
    }

    public override async Task OnConnectedAsync()
    {
        var arrays = await _raidService.GetArraysAsync();
        await Clients.Caller.SendAsync("ArraysUpdated", arrays);
    }

    public async Task GetArrays()
    {
        var arrays = await _raidService.GetArraysAsync();
        await Clients.Caller.SendAsync("ArraysUpdated", arrays);
    }
}
