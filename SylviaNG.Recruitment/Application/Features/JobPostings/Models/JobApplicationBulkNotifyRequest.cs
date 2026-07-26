using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>EP-09 US-076: re-send a notification for a chosen event to a batch of applications at once.</summary>
    public class JobApplicationBulkNotifyRequest
    {
        public List<long> JobApplicationIds { get; set; } = new();
        public RecruitmentEventEnum RecruitmentEvent { get; set; }
    }
}
