using Microsoft.AspNetCore.Mvc;

namespace NLBAudit.AspNetCore.Tests.Setup;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetTest(int id)
    {
        return Ok(new { Message = "Test succeeded", Id = id });
    }
}