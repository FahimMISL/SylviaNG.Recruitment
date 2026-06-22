using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewScorecardCriteriaMapper
    {
        public static InterviewScorecardCriteria ToEntity(this InterviewScorecardCriteriaCreateRequest request)
        {
            return new InterviewScorecardCriteria
            {
                InterviewScorecardId = request.InterviewScorecardId,
                CriteriaName = request.CriteriaName,
                Description = request.Description,
                Weight = request.Weight,
                MaxScore = request.MaxScore,
                SortOrder = request.SortOrder,
            };
        }

        public static void ApplyUpdate(this InterviewScorecardCriteria entity, InterviewScorecardCriteriaUpdateRequest request)
        {
            if (request.InterviewScorecardId.HasValue) entity.InterviewScorecardId = request.InterviewScorecardId.Value;
            if (request.CriteriaName is not null) entity.CriteriaName = request.CriteriaName;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.Weight.HasValue) entity.Weight = request.Weight.Value;
            if (request.MaxScore.HasValue) entity.MaxScore = request.MaxScore.Value;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewScorecardCriteriaResponse ToResponse(this InterviewScorecardCriteria entity)
        {
            return new InterviewScorecardCriteriaResponse
            {
                InterviewScorecardCriteriaId = entity.InterviewScorecardCriteriaId,
                InterviewScorecardId = entity.InterviewScorecardId,
                CriteriaName = entity.CriteriaName,
                Description = entity.Description,
                Weight = entity.Weight,
                MaxScore = entity.MaxScore,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive,
            };
        }
    }
}
