using Microsoft.AspNetCore.Mvc;

namespace InputService.Controllers;

[ApiController]
[Route("[controller]")]
public class InputServiceController : ControllerBase
{
    private const string _version = "1.00";

    [HttpGet("Version")]
    public object Version()
    {
        return new {Version = _version};
    }

    [HttpGet("Status")]
    public object Status()
    {
        return true;
    }

    [HttpGet("Start")]
    public object Start()
    {
        return true;
    }

    [HttpGet("Stop")]
    public object Stop()
    {
        return true;
    }
}