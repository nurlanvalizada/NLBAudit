using Microsoft.AspNetCore.Mvc;

namespace NLBAudit.AspNetCore.Mvc.Extensions;

public static class McOptionsExtensions
{
    public static void AddAuditingFilter(this MvcOptions options)
    {
        options.Filters.Add(typeof(MvcAuditActionFilter));
    }
}