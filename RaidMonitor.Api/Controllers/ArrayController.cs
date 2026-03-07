using Microsoft.AspNetCore.Mvc;
using RaidMonitor.Api.Services;
using System.Text.RegularExpressions;

namespace RaidMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArraysController : ControllerBase
{
    private static readonly Regex ValidDevice = new(@"^[a-z][a-z0-9]+$", RegexOptions.Compiled);

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
        if (!ValidDevice.IsMatch(device))
            return BadRequest("Invalid device name.");

        var smart = await _raidService.GetSmartInfoAsync(device);
        return Ok(smart);
    }
}