using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public DemoController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet("get-demo")]
    public IActionResult GetDemo() => Ok("Hello World from DemoController get demo");

    [HttpGet("ambient-demo")]
    public IActionResult AmbientDemo()
    {
        var environment = _configuration.GetValue<string>("Environment");
        var isDevelopment = _environment.EnvironmentName;

        return Ok(new { environment, isDevelopment });
    }
}