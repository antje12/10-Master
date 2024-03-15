using Microsoft.AspNetCore.Mvc;

namespace ProfileService.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldServiceController : ControllerBase
{
    private string _apiVersion = "1.00";

    [HttpGet("Version")]
    public object Version()
    {
        return $"Service version = {_apiVersion}";
    }
}