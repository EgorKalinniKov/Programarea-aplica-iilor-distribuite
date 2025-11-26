using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Root()
    {
        return Ok(new
        {
            service = "API Gateway",
            version = "1.0.0",
            status = "running"
        });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            gateway = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}

