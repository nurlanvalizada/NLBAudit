namespace NLBAudit.AspNetCore.Extensions;

internal static class HttpContextExtensions
{
    internal static string GetFullUrl(this HttpContext context)
    {
        return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
    }
}