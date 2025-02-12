namespace NLBAudit.AspNetCore.Controllers.Dto;

public record AuditLogPagedResultDto(int TotalCount, IReadOnlyList<AuditLogListDto> AuditLogs);