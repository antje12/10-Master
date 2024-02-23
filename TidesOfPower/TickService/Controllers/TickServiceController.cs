using Microsoft.AspNetCore.Mvc;
using TickService.Interfaces;

namespace TickService.Controllers;

[ApiController]
[Route("[controller]")]
public class TickServiceController : ControllerBase
{
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private const string ApiVersion = "1.00";
    private readonly IConsumerService _consumerService;

    public TickServiceController(IConsumerService consumerService)
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