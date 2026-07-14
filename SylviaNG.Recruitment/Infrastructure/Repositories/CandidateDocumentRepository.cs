using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateDocumentRepository : Repository<CandidateDocument>, ICandidateDocumentRepository
    {
        public CandidateDocumentRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<CandidateDocument>> GetAllByCandidateProfileIdAsync(long candidateProfileId)
        {
            return await _dbSet.Where(d => d.CandidateProfileId == candidateProfileId).ToListAsync();
        }
    }
}
