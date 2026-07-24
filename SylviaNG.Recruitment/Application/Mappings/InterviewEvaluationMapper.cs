using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>Manual mapping methods for InterviewEvaluation. No AutoMapper, matching InterviewMapper.
    /// Assumes Scorecard.Criteria and Scores navigation properties are loaded by the caller.</summary>
    public static class InterviewEvaluationMapper
    {
        public static InterviewEvaluationResponse ToResponse(this InterviewEvaluation entity)
        {
            var criteriaById = entity.Scorecard.Criteria.ToDictionary(c => c.ScorecardCriterionId);

            var scoreResponses = entity.Scores
                .Select(s =>
                {
                    var criterion = criteriaById.GetValueOrDefault(s.ScorecardCriterionId);
                    return new InterviewEvaluationScoreResponse
                    {
                        ScorecardCriterionId = s.ScorecardCriterionId,
                        CriterionName = criterion?.Name ?? string.Empty,
                        Weight = criterion?.Weight ?? 0,
                        MaxScore = criterion?.MaxScore ?? 0,
                        Score = s.Score,
                    };
                })
                .OrderBy(s => criteriaById.GetValueOrDefault(s.ScorecardCriterionId)?.DisplayOrder ?? 0)
                .ToList();

            return new InterviewEvaluationResponse
            {
                InterviewEvaluationId = entity.InterviewEvaluationId,
                InterviewId = entity.InterviewId,
                EmployeeId = entity.EmployeeId,
                ScorecardId = entity.ScorecardId,
                ScorecardName = entity.Scorecard.Name,
                OverallComments = entity.OverallComments,
                SubmittedAt = entity.SubmittedAt,
                SubmittedByUserName = entity.SubmittedByUserName,
                Scores = scoreResponses,
                WeightedScore = DeriveWeightedScore(scoreResponses),
            };
        }

        /// <summary>Not persisted - Σ(Score/MaxScore × Weight) / Σ(Weight) × 100, robust regardless
        /// of whether the scorecard's weights sum to exactly 100 (US-066/068).</summary>
        private static decimal DeriveWeightedScore(List<InterviewEvaluationScoreResponse> scores)
        {
            var totalWeight = scores.Sum(s => s.Weight);
            if (totalWeight <= 0) return 0;

            var weightedSum = scores
                .Where(s => s.MaxScore > 0)
                .Sum(s => (s.Score / s.MaxScore) * s.Weight);

            return Math.Round(weightedSum / totalWeight * 100, 2);
        }
    }
}
