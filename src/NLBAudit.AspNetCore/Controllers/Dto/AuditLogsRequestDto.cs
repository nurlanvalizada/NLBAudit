namespace NLBAudit.AspNetCore.Controllers.Dto;

public record AuditLogsRequestDto(
    DateTime StartDate,
    DateTime EndDate,
    string? Path,
    bool? HasException,
    int? MinExecutionDuration,
    int? MaxExecutionDuration,
    int SkipCount = 0,
    int MaxResultCount = 50);
