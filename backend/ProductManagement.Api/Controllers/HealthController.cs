namespace ProductManagement.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Simple health check endpoint to verify the service is callable.
    /// </summary>
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new { status = "Healthy", message = "Service is up and reachable" });
    }
}
