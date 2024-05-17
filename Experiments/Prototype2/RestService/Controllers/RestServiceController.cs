﻿using ClassLibrary;
using Microsoft.AspNetCore.Mvc;

namespace RestService.Controllers;

[ApiController]
[Route("[controller]")]
public class RestServiceController : ControllerBase
{
    private string _apiVersion = "1.00";

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {_apiVersion}";
    }

    [HttpPost("Test")]
    public ActionResult<MessageData> Test([FromBody] MessageData messageData)
    {
        return Ok(messageData);
    }
}