using Microsoft.AspNetCore.Mvc;
using RaidMonitor.Api.Services;

namespace RaidMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArraysController : ControllerBase
{
    private readonly RaidService _raidService;

    public ArraysController(RaidService raidService)
    {
        _raidService = raidService;
    }

    [HttpGet]
    public async Task<IActionResult> GetArrays()
    {
        var arrays = await _raidService.GetArraysAsync();
        return Ok(arrays);
    }

    [HttpGet("{arrayName}/disks/{device}/smart")]
    public async Task<IActionResult> GetSmartInfo(string device)
    {
        var smart = await _raidService.GetSmartInfoAsync(device);
        return Ok(smart);
    }
}