using Microsoft.AspNetCore.Mvc;
using ProfileService.Interfaces;

namespace ProfileService.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldServiceController : ControllerBase
{
    private const string ApiVersion = "1.00";
    private readonly IConsumerService _consumerService;

    public WorldServiceController(IConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {ApiVersion}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Service running = {_consumerService?.IsRunning ?? false}";
    }
}