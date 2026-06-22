using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models
{
    public class JobPostingChannelResponse
    {
        public long JobPostingChannelId { get; set; }
        public long JobPostingId { get; set; }
        public PublishChannelEnum Channel { get; set; }
        public ChannelPublishStatusEnum PublishStatus { get; set; }
        public string? ExternalReferenceId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? FailureReason { get; set; }
        public bool IsActive { get; set; }
    }
}
