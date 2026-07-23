using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class ApplicationSettingRepository : Repository<ApplicationSetting>, IApplicationSettingRepository
    {
        public ApplicationSettingRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<ApplicationSetting> GetSingletonAsync()
        {
            // Seeded via migration (ApplicationSettingId = 1), so this always resolves - the
            // FirstOrDefault fallback only guards a not-yet-migrated dev database.
            return await _dbSet.OrderBy(s => s.ApplicationSettingId).FirstOrDefaultAsync()
                ?? new ApplicationSetting { ApplicationSettingId = 1, MinimumProfileCompletenessPercentage = 0 };
        }
    }
}
