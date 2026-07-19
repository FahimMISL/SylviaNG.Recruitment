using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class TalentPoolMapper
    {
        public static TalentPoolResponse ToResponse(this TalentPool entity)
        {
            return new TalentPoolResponse
            {
                TalentPoolId = entity.TalentPoolId,
                Name = entity.Name,
                CandidateCount = entity.Candidates.Count,
                CreatedAt = entity.CreatedAt
            };
        }

        public static TalentPoolLookupResponse ToLookupResponse(this TalentPool entity)
        {
            return new TalentPoolLookupResponse
            {
                TalentPoolId = entity.TalentPoolId,
                Name = entity.Name
            };
        }

        public static TalentPoolDetailResponse ToDetailResponse(this TalentPool entity)
        {
            return new TalentPoolDetailResponse
            {
                TalentPoolId = entity.TalentPoolId,
                Name = entity.Name,
                Candidates = entity.Candidates.Select(c => c.CandidateProfile.ToSummaryResponse()).ToList()
            };
        }

        public static TalentPoolBadgeResponse ToBadgeResponse(this TalentPoolCandidate entity)
        {
            return new TalentPoolBadgeResponse
            {
                TalentPoolId = entity.TalentPoolId,
                Name = entity.TalentPool.Name
            };
        }
    }
}
