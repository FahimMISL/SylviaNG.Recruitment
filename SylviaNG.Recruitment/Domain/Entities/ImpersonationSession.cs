namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-15/US-115: one SuperAdmin-initiated impersonation of another UserAccount (Admin/HR only -
/// candidate impersonation is out of scope for this pass). ExpiresAt is a hard 30-minute cap set
/// at StartAsync time; EndedAt is set either by an explicit EndAsync call or implicitly once
/// ExpiresAt passes - ImpersonationMiddleware checks both live on every request bearing the
/// impersonation token, not just the token's own JWT expiry.
/// </summary>
public class ImpersonationSession
{
    public long ImpersonationSessionId { get; set; }
    public long ActorUserAccountId { get; set; }
    public long TargetUserAccountId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public UserAccount ActorUserAccount { get; set; } = null!;
    public UserAccount TargetUserAccount { get; set; } = null!;
    public ICollection<ImpersonationLog> Logs { get; set; } = new List<ImpersonationLog>();
}
