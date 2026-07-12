using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class ApplicationStatusReasonResponse
    {
        public long ApplicationStatusReasonId { get; set; }
        public string Label { get; set; } = string.Empty;
        public ApplicationStatusEnum AppliesToStatus { get; set; }
        public int DisplayOrder { get; set; }
    }
}
