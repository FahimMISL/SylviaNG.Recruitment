using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CompanyBrandingRepository : Repository<CompanyBranding>, ICompanyBrandingRepository
    {
        public CompanyBrandingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<CompanyBranding?> GetByCompanyIdAsync(long? companyId)
        {
            return await _dbSet.Where(b => b.CompanyId == companyId)
                .OrderBy(b => b.CompanyBrandingId)
                .FirstOrDefaultAsync();
        }
    }
}
