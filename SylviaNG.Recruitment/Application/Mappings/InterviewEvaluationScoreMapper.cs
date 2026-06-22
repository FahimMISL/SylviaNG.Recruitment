using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewEvaluationScoreMapper
    {
        public static InterviewEvaluationScore ToEntity(this InterviewEvaluationScoreCreateRequest request)
        {
            return new InterviewEvaluationScore
            {
                InterviewEvaluationId = request.InterviewEvaluationId,
                InterviewScorecardCriteriaId = request.InterviewScorecardCriteriaId,
                Score = request.Score,
                Notes = request.Notes,
            };
        }

        public static void ApplyUpdate(this InterviewEvaluationScore entity, InterviewEvaluationScoreUpdateRequest request)
        {
            if (request.InterviewEvaluationId.HasValue) entity.InterviewEvaluationId = request.InterviewEvaluationId.Value;
            if (request.InterviewScorecardCriteriaId.HasValue) entity.InterviewScorecardCriteriaId = request.InterviewScorecardCriteriaId.Value;
            if (request.Score.HasValue) entity.Score = request.Score.Value;
            if (request.Notes is not null) entity.Notes = request.Notes;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewEvaluationScoreResponse ToResponse(this InterviewEvaluationScore entity)
        {
            return new InterviewEvaluationScoreResponse
            {
                InterviewEvaluationScoreId = entity.InterviewEvaluationScoreId,
                InterviewEvaluationId = entity.InterviewEvaluationId,
                InterviewScorecardCriteriaId = entity.InterviewScorecardCriteriaId,
                Score = entity.Score,
                Notes = entity.Notes,
                IsActive = entity.IsActive,
            };
        }
    }
}
