using Microsoft.AspNetCore.Mvc;

namespace ScrapeWise.Controllers;

/// <summary>
/// Simple test API controller to verify Swagger is working
/// </summary>
[Route("api/test")]
[ApiController]
public class TestApiController : ControllerBase
{
    /// <summary>
    /// Simple test endpoint to verify API registration
    /// </summary>
    /// <returns>Test message</returns>
    [HttpGet]
    public IActionResult GetTest()
    {
        return Ok(new { message = "API is working!", timestamp = DateTime.Now });
    }

    /// <summary>
    /// Test endpoint with parameter
    /// </summary>
    /// <param name="id">Test ID</param>
    /// <returns>Test response with ID</returns>
    [HttpGet("{id}")]
    public IActionResult GetTest(int id)
    {
        return Ok(new { id = id, message = $"Test endpoint with ID: {id}" });
    }
}
