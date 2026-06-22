using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class JobPostingChannel : Audit
{
    public long JobPostingChannelId { get; set; }
    public long JobPostingId { get; set; }
    public PublishChannelEnum Channel { get; set; }
    public ChannelPublishStatusEnum PublishStatus { get; set; } = ChannelPublishStatusEnum.Pending;
    public string? ExternalReferenceId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? FailureReason { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
}
