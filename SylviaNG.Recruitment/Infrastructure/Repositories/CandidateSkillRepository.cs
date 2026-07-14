using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateSkillRepository : Repository<CandidateSkill>, ICandidateSkillRepository
    {
        public CandidateSkillRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<CandidateSkill>> GetAllByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet.Where(s => s.CandidateProfileId == candidateProfileId).ToListAsync();
        }
    }
}
