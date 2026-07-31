using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class FitmentDataRepository : Repository<FitmentData>, IFitmentDataRepository
    {
        public FitmentDataRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<FitmentData?> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.JobApplicationId == jobApplicationId);
        }
    }
}
