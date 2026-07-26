using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class PreBoardingSubmissionRepository : Repository<PreBoardingSubmission>, IPreBoardingSubmissionRepository
    {
        public PreBoardingSubmissionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PreBoardingSubmission?> GetByFinalSelectionPoolIdWithDetailsAsync(long finalSelectionPoolId)
        {
            return await _dbSet
                .Include(s => s.Nominees)
                .FirstOrDefaultAsync(s => s.FinalSelectionPoolId == finalSelectionPoolId);
        }
    }
}
