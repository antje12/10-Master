using Microsoft.AspNetCore.Mvc;
using PhysicsService.Interfaces;

namespace PhysicsService.Controllers;

[ApiController]
[Route("[controller]")]
public class PhysicsServiceController : ControllerBase
{
    private const string ApiVersion = "1.00";
    private readonly IConsumerService _consumerService;

    public PhysicsServiceController(IConsumerService consumerService)
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