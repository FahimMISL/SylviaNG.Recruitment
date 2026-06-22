using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class InterviewScorecardMapper
    {
        public static InterviewScorecard ToEntity(this InterviewScorecardCreateRequest request)
        {
            return new InterviewScorecard
            {
                ScorecardName = request.ScorecardName,
                Description = request.Description,
                ScoringScale = request.ScoringScale,
            };
        }

        public static void ApplyUpdate(this InterviewScorecard entity, InterviewScorecardUpdateRequest request)
        {
            if (request.ScorecardName is not null) entity.ScorecardName = request.ScorecardName;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.ScoringScale is not null) entity.ScoringScale = request.ScoringScale;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static InterviewScorecardResponse ToResponse(this InterviewScorecard entity)
        {
            return new InterviewScorecardResponse
            {
                InterviewScorecardId = entity.InterviewScorecardId,
                ScorecardName = entity.ScorecardName,
                Description = entity.Description,
                ScoringScale = entity.ScoringScale,
                IsActive = entity.IsActive,
            };
        }
    }
}
