namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models
{
    public class ImpersonationLogUpdateRequest
    {
        public string? Reason { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
