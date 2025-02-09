using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace NLBAudit.AspNetCore.Tests.Setup;

public class TestApplicationFactory : WebApplicationFactory<TestStartup>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<TestStartup>();
                       webBuilder.UseTestServer();
                   });
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(GetProjectRootBySearching());
    }
    
    private static string GetProjectRootBySearching()
    {
        var dir = Directory.GetCurrentDirectory();
        
        while (!Directory.Exists(Path.Combine(dir, "NLBAudit.AspNetCore.Tests")))
        {
            dir = Directory.GetParent(dir)?.FullName ?? throw new DirectoryNotFoundException("Could not locate test project directory.");
        }
        
        return dir;
    }
}