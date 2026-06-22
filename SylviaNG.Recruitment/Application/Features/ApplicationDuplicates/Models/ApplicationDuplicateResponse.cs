using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models
{
    public class ApplicationDuplicateResponse
    {
        public long ApplicationDuplicateId { get; set; }
        public long PrimaryApplicationId { get; set; }
        public long DuplicateApplicationId { get; set; }
        public string MatchField { get; set; } = string.Empty;
        public DuplicateResolutionEnum Resolution { get; set; }
        public long? ResolvedByUserId { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
