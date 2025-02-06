using Microsoft.AspNetCore.Mvc;

namespace NLBAudit.Sample;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : Controller
{
    [HttpGet]
    public async Task<object> GetTest()
    {
        return Task.FromResult(new { Test = "Test" });
    }
    
    [HttpPost]
    public async Task<object> PostTest([FromBody] PostModel model)
    {
        return Task.FromResult(new { Test = "Test" });
    }
}

public record PostModel(string Name, string Email);