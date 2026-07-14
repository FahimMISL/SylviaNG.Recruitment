using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateCertificationRepository : Repository<CandidateCertification>, ICandidateCertificationRepository
    {
        public CandidateCertificationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<CandidateCertification>> GetAllByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet.Where(c => c.CandidateProfileId == candidateProfileId).ToListAsync();
        }
    }
}
