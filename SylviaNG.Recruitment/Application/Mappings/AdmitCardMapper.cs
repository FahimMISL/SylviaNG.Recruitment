using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class AdmitCardMapper
    {
        public static AdmitCard ToEntity(this AdmitCardCreateRequest request)
        {
            return new AdmitCard
            {
                ExamCandidateId = request.ExamCandidateId,
                ExamSeatPlanId = request.ExamSeatPlanId,
                FileUrl = request.FileUrl,
                DeliveryChannel = request.DeliveryChannel,
                DeliveryStatus = request.DeliveryStatus,
                DeliveredAt = request.DeliveredAt,
                FailureReason = request.FailureReason,
            };
        }

        public static void ApplyUpdate(this AdmitCard entity, AdmitCardUpdateRequest request)
        {
            if (request.ExamCandidateId.HasValue) entity.ExamCandidateId = request.ExamCandidateId.Value;
            if (request.ExamSeatPlanId.HasValue) entity.ExamSeatPlanId = request.ExamSeatPlanId.Value;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.DeliveryChannel.HasValue) entity.DeliveryChannel = request.DeliveryChannel.Value;
            if (request.DeliveryStatus.HasValue) entity.DeliveryStatus = request.DeliveryStatus.Value;
            if (request.DeliveredAt.HasValue) entity.DeliveredAt = request.DeliveredAt;
            if (request.FailureReason is not null) entity.FailureReason = request.FailureReason;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static AdmitCardResponse ToResponse(this AdmitCard entity)
        {
            return new AdmitCardResponse
            {
                AdmitCardId = entity.AdmitCardId,
                ExamCandidateId = entity.ExamCandidateId,
                ExamSeatPlanId = entity.ExamSeatPlanId,
                FileUrl = entity.FileUrl,
                DeliveryChannel = entity.DeliveryChannel,
                DeliveryStatus = entity.DeliveryStatus,
                DeliveredAt = entity.DeliveredAt,
                FailureReason = entity.FailureReason,
                IsActive = entity.IsActive,
            };
        }
    }
}
