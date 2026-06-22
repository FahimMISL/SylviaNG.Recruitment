using SylviaNG.Recruitment.Application.Features.OfferCompensations.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class OfferCompensationMapper
    {
        public static OfferCompensation ToEntity(this OfferCompensationCreateRequest request)
        {
            return new OfferCompensation
            {
                JobApplicationId = request.JobApplicationId,
                FitmentDataId = request.FitmentDataId,
                ComponentName = request.ComponentName,
                Amount = request.Amount,
                Currency = request.Currency,
                Frequency = request.Frequency,
                IsWithinPermittedRange = request.IsWithinPermittedRange,
                RequiresAdditionalApproval = request.RequiresAdditionalApproval,
            };
        }

        public static void ApplyUpdate(this OfferCompensation entity, OfferCompensationUpdateRequest request)
        {
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.FitmentDataId.HasValue) entity.FitmentDataId = request.FitmentDataId.Value;
            if (request.ComponentName is not null) entity.ComponentName = request.ComponentName;
            if (request.Amount.HasValue) entity.Amount = request.Amount.Value;
            if (request.Currency is not null) entity.Currency = request.Currency;
            if (request.Frequency is not null) entity.Frequency = request.Frequency;
            if (request.IsWithinPermittedRange.HasValue) entity.IsWithinPermittedRange = request.IsWithinPermittedRange.Value;
            if (request.RequiresAdditionalApproval.HasValue) entity.RequiresAdditionalApproval = request.RequiresAdditionalApproval.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static OfferCompensationResponse ToResponse(this OfferCompensation entity)
        {
            return new OfferCompensationResponse
            {
                OfferCompensationId = entity.OfferCompensationId,
                JobApplicationId = entity.JobApplicationId,
                FitmentDataId = entity.FitmentDataId,
                ComponentName = entity.ComponentName,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Frequency = entity.Frequency,
                IsWithinPermittedRange = entity.IsWithinPermittedRange,
                RequiresAdditionalApproval = entity.RequiresAdditionalApproval,
                IsActive = entity.IsActive,
            };
        }
    }
}
