using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewEvaluationMapper
    {
        public static InterviewEvaluation ToEntity(this InterviewEvaluationCreateRequest request)
        {
            return new InterviewEvaluation
            {
                InterviewId = request.InterviewId,
                PanelistUserId = request.PanelistUserId,
                OverallScore = request.OverallScore,
                Recommendation = request.Recommendation,
                Commentary = request.Commentary,
                SubmittedAt = request.SubmittedAt,
            };
        }

        public static void ApplyUpdate(this InterviewEvaluation entity, InterviewEvaluationUpdateRequest request)
        {
            if (request.InterviewId.HasValue) entity.InterviewId = request.InterviewId.Value;
            if (request.PanelistUserId.HasValue) entity.PanelistUserId = request.PanelistUserId.Value;
            if (request.OverallScore.HasValue) entity.OverallScore = request.OverallScore.Value;
            if (request.Recommendation.HasValue) entity.Recommendation = request.Recommendation.Value;
            if (request.Commentary is not null) entity.Commentary = request.Commentary;
            if (request.SubmittedAt.HasValue) entity.SubmittedAt = request.SubmittedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewEvaluationResponse ToResponse(this InterviewEvaluation entity)
        {
            return new InterviewEvaluationResponse
            {
                InterviewEvaluationId = entity.InterviewEvaluationId,
                InterviewId = entity.InterviewId,
                PanelistUserId = entity.PanelistUserId,
                OverallScore = entity.OverallScore,
                Recommendation = entity.Recommendation,
                Commentary = entity.Commentary,
                SubmittedAt = entity.SubmittedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
