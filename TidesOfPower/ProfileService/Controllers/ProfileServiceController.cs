using Microsoft.AspNetCore.Mvc;

namespace ProfileService.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldServiceController : ControllerBase
{
    private const string ApiVersion = "1.00";

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {ApiVersion}";
    }
}