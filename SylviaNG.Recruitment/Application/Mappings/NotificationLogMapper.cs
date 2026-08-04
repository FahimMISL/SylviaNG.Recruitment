using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NotificationLogMapper
    {
        public static NotificationLogResponse ToResponse(this NotificationLog entity)
        {
            return new NotificationLogResponse
            {
                NotificationLogId = entity.NotificationLogId,
                RecruitmentEvent = entity.RecruitmentEvent,
                Channel = entity.Channel,
                RecipientType = entity.RecipientType,
                RecipientAddress = entity.RecipientAddress,
                RecipientName = entity.RecipientType == NotificationRecipientTypeEnum.Candidate
                    ? entity.JobApplication?.CandidateProfile?.FullName ?? entity.RecipientAddress
                    : "Admin / HR",
                RenderedSubject = entity.RenderedSubject,
                DeliveryStatus = entity.DeliveryStatus,
                SentAt = entity.SentAt,
                FailureReason = entity.FailureReason,
                IsRead = entity.IsRead,
                ReadAt = entity.ReadAt,
                JobApplicationId = entity.JobApplicationId,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
