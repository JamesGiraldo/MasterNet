using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("GetDemo")]
    public IActionResult GetDemo() => Ok("Hello World from DemoController get demo");
}