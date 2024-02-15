using Microsoft.AspNetCore.Mvc;
using InputService.Interfaces;

namespace InputService.Controllers;

[ApiController]
[Route("[controller]")]
public class InputServiceController : ControllerBase
{
    //https://github.com/confluentinc/confluent-kafka-dotnet/blob/master/examples/AvroSpecific/Program.cs
    private const string ApiVersion = "1.00";
    private readonly IConsumerService _inputService;

    public InputServiceController(IConsumerService inputService)
    {
        _inputService = inputService;
    }

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {ApiVersion}";
    }

    [HttpGet("Status")]
    public object Status()
    {
        return $"Service running = {_inputService?.IsRunning ?? false}";
    }
}