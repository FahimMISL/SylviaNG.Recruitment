namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-15/US-115: append-only audit trail of requests made while an ImpersonationSession was
/// active, stamped by ImpersonationMiddleware on every authenticated request bearing an
/// impersonation token.
/// </summary>
public class ImpersonationLog
{
    public long ImpersonationLogId { get; set; }
    public long ImpersonationSessionId { get; set; }
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public ImpersonationSession Session { get; set; } = null!;
}
