using Microsoft.AspNetCore.Mvc;
using AIService.Interfaces;

namespace AIService.Controllers;

[ApiController]
[Route("[controller]")]
public class AIServiceController : ControllerBase
{
    private string _apiVersion = "1.00";
    private readonly IConsumerService _consumerService;

    public AIServiceController(IConsumerService consumerService)
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