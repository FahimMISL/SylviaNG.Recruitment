using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class TalentPoolCandidateMapper
    {
        public static TalentPoolCandidate ToEntity(this TalentPoolCandidateCreateRequest request)
        {
            return new TalentPoolCandidate
            {
                TalentPoolId = request.TalentPoolId,
                CandidateId = request.CandidateId,
                Rank = request.Rank,
            };
        }

        public static void ApplyUpdate(this TalentPoolCandidate entity, TalentPoolCandidateUpdateRequest request)
        {
            if (request.TalentPoolId.HasValue) entity.TalentPoolId = request.TalentPoolId.Value;
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.Rank.HasValue) entity.Rank = request.Rank.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static TalentPoolCandidateResponse ToResponse(this TalentPoolCandidate entity)
        {
            return new TalentPoolCandidateResponse
            {
                TalentPoolCandidateId = entity.TalentPoolCandidateId,
                TalentPoolId = entity.TalentPoolId,
                CandidateId = entity.CandidateId,
                Rank = entity.Rank,
                IsActive = entity.IsActive,
            };
        }
    }
}
