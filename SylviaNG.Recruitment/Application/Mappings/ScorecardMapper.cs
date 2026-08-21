using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>Manual mapping methods for Scorecard and ScorecardCriterion. No AutoMapper, matching ShortlistFilterMapper.</summary>
    public static class ScorecardMapper
    {
        public static ScorecardCriterion ToEntity(this ScorecardCriterionRequest request)
        {
            return new ScorecardCriterion
            {
                Name = request.Name,
                Weight = request.Weight,
                MaxScore = request.MaxScore,
                DisplayOrder = request.DisplayOrder,
            };
        }

        public static ScorecardCriterionResponse ToResponse(this ScorecardCriterion entity)
        {
            return new ScorecardCriterionResponse
            {
                ScorecardCriterionId = entity.ScorecardCriterionId,
                Name = entity.Name,
                Weight = entity.Weight,
                MaxScore = entity.MaxScore,
                DisplayOrder = entity.DisplayOrder,
            };
        }

        public static Scorecard ToEntity(this ScorecardCreateRequest request)
        {
            return new Scorecard
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                Criteria = request.Criteria.OrderBy(c => c.DisplayOrder).Select(c => c.ToEntity()).ToList(),
            };
        }

        public static ScorecardResponse ToResponse(this Scorecard entity)
        {
            return new ScorecardResponse
            {
                ScorecardId = entity.ScorecardId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                Criteria = entity.Criteria?
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => c.ToResponse())
                    .ToList() ?? new List<ScorecardCriterionResponse>(),
            };
        }

        public static ScorecardLookupResponse ToLookupResponse(this Scorecard entity)
        {
            return new ScorecardLookupResponse
            {
                ScorecardId = entity.ScorecardId,
                Name = entity.Name,
            };
        }
    }
}
