using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ApplicationScreeningResultMapper
    {
        public static ApplicationScreeningResult ToEntity(this ApplicationScreeningResultCreateRequest request)
        {
            return new ApplicationScreeningResult
            {
                JobApplicationId = request.JobApplicationId,
                RelevanceScore = request.RelevanceScore,
                MatchedKeywordsJson = request.MatchedKeywordsJson,
                SkillTagsJson = request.SkillTagsJson,
                ExperienceBand = request.ExperienceBand,
                ScoreExplanation = request.ScoreExplanation,
                ScreenedAt = request.ScreenedAt,
            };
        }

        public static void ApplyUpdate(this ApplicationScreeningResult entity, ApplicationScreeningResultUpdateRequest request)
        {
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.RelevanceScore.HasValue) entity.RelevanceScore = request.RelevanceScore.Value;
            if (request.MatchedKeywordsJson is not null) entity.MatchedKeywordsJson = request.MatchedKeywordsJson;
            if (request.SkillTagsJson is not null) entity.SkillTagsJson = request.SkillTagsJson;
            if (request.ExperienceBand is not null) entity.ExperienceBand = request.ExperienceBand;
            if (request.ScoreExplanation is not null) entity.ScoreExplanation = request.ScoreExplanation;
            if (request.ScreenedAt.HasValue) entity.ScreenedAt = request.ScreenedAt.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ApplicationScreeningResultResponse ToResponse(this ApplicationScreeningResult entity)
        {
            return new ApplicationScreeningResultResponse
            {
                ApplicationScreeningResultId = entity.ApplicationScreeningResultId,
                JobApplicationId = entity.JobApplicationId,
                RelevanceScore = entity.RelevanceScore,
                MatchedKeywordsJson = entity.MatchedKeywordsJson,
                SkillTagsJson = entity.SkillTagsJson,
                ExperienceBand = entity.ExperienceBand,
                ScoreExplanation = entity.ScoreExplanation,
                ScreenedAt = entity.ScreenedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
