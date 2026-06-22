using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class JobPostingChannelMapper
    {
        public static JobPostingChannel ToEntity(this JobPostingChannelCreateRequest request)
        {
            return new JobPostingChannel
            {
                JobPostingId = request.JobPostingId,
                Channel = request.Channel,
                ExternalReferenceId = request.ExternalReferenceId
            };
        }

        public static void ApplyUpdate(this JobPostingChannel entity, JobPostingChannelUpdateRequest request)
        {
            if (request.PublishStatus.HasValue) entity.PublishStatus = request.PublishStatus.Value;
            if (request.ExternalReferenceId is not null) entity.ExternalReferenceId = request.ExternalReferenceId;
            if (request.PublishedAt.HasValue) entity.PublishedAt = request.PublishedAt;
            if (request.FailureReason is not null) entity.FailureReason = request.FailureReason;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static JobPostingChannelResponse ToResponse(this JobPostingChannel entity)
        {
            return new JobPostingChannelResponse
            {
                JobPostingChannelId = entity.JobPostingChannelId,
                JobPostingId = entity.JobPostingId,
                Channel = entity.Channel,
                PublishStatus = entity.PublishStatus,
                ExternalReferenceId = entity.ExternalReferenceId,
                PublishedAt = entity.PublishedAt,
                FailureReason = entity.FailureReason,
                IsActive = entity.IsActive
            };
        }
    }
}
