using Microsoft.AspNetCore.Mvc;
using ProjectileService.Interfaces;

namespace ProjectileService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectileServiceController : ControllerBase
{
    private string _apiVersion = "1.00";
    private IConsumerService _consumerService;

    public ProjectileServiceController(IConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {_apiVersion}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Service running = {_consumerService?.IsRunning ?? false}";
    }
}