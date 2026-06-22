using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateSkillMapper
    {
        public static CandidateSkill ToEntity(this CandidateSkillCreateRequest request)
        {
            return new CandidateSkill
            {
                CandidateId = request.CandidateId,
                SkillName = request.SkillName,
                ProficiencyLevel = request.ProficiencyLevel,
            };
        }

        public static void ApplyUpdate(this CandidateSkill entity, CandidateSkillUpdateRequest request)
        {
            if (request.SkillName is not null) entity.SkillName = request.SkillName;
            if (request.ProficiencyLevel is not null) entity.ProficiencyLevel = request.ProficiencyLevel;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateSkillResponse ToResponse(this CandidateSkill entity)
        {
            return new CandidateSkillResponse
            {
                CandidateSkillId = entity.CandidateSkillId,
                CandidateId = entity.CandidateId,
                SkillName = entity.SkillName,
                ProficiencyLevel = entity.ProficiencyLevel,
                IsActive = entity.IsActive,
            };
        }
    }
}
