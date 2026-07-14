using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ApplicationStatusReasonRepository : Repository<ApplicationStatusReason>, IApplicationStatusReasonRepository
    {
        public ApplicationStatusReasonRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<List<ApplicationStatusReason>> GetActiveByStatusAsync(ApplicationStatusEnum status)
        {
            return await _dbSet
                .Where(r => r.AppliesToStatus == status && r.IsActive)
                .OrderBy(r => r.DisplayOrder)
                .ToListAsync();
        }
    }
}
