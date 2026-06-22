using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models
{
    public class JobPostingChannelCreateRequest
    {
        public long JobPostingId { get; set; }
        public PublishChannelEnum Channel { get; set; }
        public string? ExternalReferenceId { get; set; }
    }
}
