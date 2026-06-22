using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class NotificationLogMapper
    {
        public static NotificationLog ToEntity(this NotificationLogCreateRequest request)
        {
            return new NotificationLog
            {
                NotificationEventId = request.NotificationEventId,
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                Channel = request.Channel,
                Recipient = request.Recipient,
                Subject = request.Subject,
                Body = request.Body,
                DeliveryStatus = request.DeliveryStatus,
                SentAt = request.SentAt,
                DeliveredAt = request.DeliveredAt,
                FailureReason = request.FailureReason,
                RetryCount = request.RetryCount,
            };
        }

        public static void ApplyUpdate(this NotificationLog entity, NotificationLogUpdateRequest request)
        {
            if (request.NotificationEventId.HasValue) entity.NotificationEventId = request.NotificationEventId.Value;
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.Channel.HasValue) entity.Channel = request.Channel.Value;
            if (request.Recipient is not null) entity.Recipient = request.Recipient;
            if (request.Subject is not null) entity.Subject = request.Subject;
            if (request.Body is not null) entity.Body = request.Body;
            if (request.DeliveryStatus.HasValue) entity.DeliveryStatus = request.DeliveryStatus.Value;
            if (request.SentAt.HasValue) entity.SentAt = request.SentAt;
            if (request.DeliveredAt.HasValue) entity.DeliveredAt = request.DeliveredAt;
            if (request.FailureReason is not null) entity.FailureReason = request.FailureReason;
            if (request.RetryCount.HasValue) entity.RetryCount = request.RetryCount.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static NotificationLogResponse ToResponse(this NotificationLog entity)
        {
            return new NotificationLogResponse
            {
                NotificationLogId = entity.NotificationLogId,
                NotificationEventId = entity.NotificationEventId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                Channel = entity.Channel,
                Recipient = entity.Recipient,
                Subject = entity.Subject,
                Body = entity.Body,
                DeliveryStatus = entity.DeliveryStatus,
                SentAt = entity.SentAt,
                DeliveredAt = entity.DeliveredAt,
                FailureReason = entity.FailureReason,
                RetryCount = entity.RetryCount,
                IsActive = entity.IsActive,
            };
        }
    }
}
