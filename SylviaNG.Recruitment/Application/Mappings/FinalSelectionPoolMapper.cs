using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class FinalSelectionPoolMapper
    {
        public static FinalSelectionPoolResponse ToResponse(this FinalSelectionPool entity)
        {
            return new FinalSelectionPoolResponse
            {
                FinalSelectionPoolId = entity.FinalSelectionPoolId,
                OfferLetterId = entity.OfferLetterId,
                JobApplicationId = entity.JobApplicationId,
                CandidateName = entity.JobApplication?.CandidateName ?? string.Empty,
                Designation = entity.OfferLetter?.Designation ?? string.Empty,
                BatchLabel = entity.BatchLabel,
                JoiningDate = entity.JoiningDate,
                HasJoined = entity.HasJoined,
                JoinedAt = entity.JoinedAt,
                EnteredPoolAt = entity.EnteredPoolAt,
                PreBoardingStatus = entity.PreBoardingSubmission?.Status.ToString(),
            };
        }
    }
}
