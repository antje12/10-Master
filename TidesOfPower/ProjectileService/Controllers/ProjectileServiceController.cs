﻿using Microsoft.AspNetCore.Mvc;
using ProjectileService.Interfaces;

namespace ProjectileService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectileServiceController : ControllerBase
{
    private string _apiVersion = "1.00";
    private readonly IConsumerService _service;

    public ProjectileServiceController(IConsumerService service)
    {
        _service = service;
    }

    [HttpGet("Version")]
    public IActionResult Version()
    {
        return Ok($"Service version = {_apiVersion}");
    }

    [HttpGet("Status")]
    public IActionResult  Status()
    {
        return _service.IsRunning ? 
            Ok("Service running = true") :
            // Service Unavailable
            StatusCode(503, "Service running = false");
    }

    [HttpGet("Stop")]
    public IActionResult Stop()
    {
        _service.StopService();
        while (_service.IsRunning)
        {
            Thread.Sleep(100);
        }
        return Ok($"Service running = false");
    }
}