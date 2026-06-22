using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models
{
    public class JobPostingChannelUpdateRequest
    {
        public ChannelPublishStatusEnum? PublishStatus { get; set; }
        public string? ExternalReferenceId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? FailureReason { get; set; }
        public bool? IsActive { get; set; }
    }
}
