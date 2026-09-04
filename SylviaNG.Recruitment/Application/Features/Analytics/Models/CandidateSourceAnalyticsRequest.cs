using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    /// <summary>EP-14 US-108 AC2: JobPostingId/EmploymentType/DateRange filter scope; DateFrom/DateTo
    /// filter on JobApplication.AppliedDate, same convention as RecruitmentFunnelRequest.</summary>
    public class CandidateSourceAnalyticsRequest
    {
        public long? JobPostingId { get; set; }
        public EmploymentTypeEnum? EmploymentType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
