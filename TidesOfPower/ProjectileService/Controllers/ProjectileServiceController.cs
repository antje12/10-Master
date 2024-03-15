using Microsoft.AspNetCore.Mvc;
using ProjectileService.Interfaces;

namespace ProjectileService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectileServiceController : ControllerBase
{
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private const string ApiVersion = "1.00";
    private readonly IConsumerService _consumerService;

    public ProjectileServiceController(IConsumerService consumerService)
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