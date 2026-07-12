using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateSkillRepository : IRepository<CandidateSkill>
    {
        Task<List<CandidateSkill>> GetAllByCandidateProfileIdAsync(long candidateProfileId);
    }
}
