using Microsoft.AspNetCore.Mvc;
using NLBAudit.AspNetCore.Controllers.Dto;
using NLBAudit.Core;
using NLBAudit.Core.Attributes;

namespace NLBAudit.AspNetCore.Controllers;

[ApiController]
[Route("api/[controller]")]
[NotAudited]
public class AuditLogController(IAuditingStore auditingStore) : ControllerBase
{
    [HttpGet]
    public async Task<AuditLogPagedResultDto> GetAuditLogs([FromQuery] AuditLogsRequestDto input)
    {
        var items = await auditingStore.FilterAsync(input.StartDate, input.EndDate, input.Path, input.HasException,
                                                    input.MinExecutionDuration, input.MaxExecutionDuration, input.SkipCount,
                                                    input.MaxResultCount, CancellationToken.None);
        return new AuditLogPagedResultDto(items.Count, items.Select(x => AuditLogListDto.FromAuditLogInfo(x.Value, x.Key)).ToList());
    }
}