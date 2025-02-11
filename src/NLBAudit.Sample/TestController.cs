using Microsoft.AspNetCore.Mvc;

namespace NLBAudit.Sample;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController : Controller
{
    [HttpGet]
    public async Task<object> GetTest(string name)
    {
        await Task.Delay(100);
        return new
        {
            Test = "Test",
            Name = name
        };
    }
    
    [HttpPost]
    public async Task<object> PostTest([FromBody] PostModel model)
    {
        await Task.Delay(100);
        return new { Test = "Test" };
    }
}

public record PostModel(string Name, string Email);