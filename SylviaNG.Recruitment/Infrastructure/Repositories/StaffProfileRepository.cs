using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class StaffProfileRepository : Repository<StaffProfile>, IStaffProfileRepository
    {
        public StaffProfileRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<StaffProfile?> GetByKeycloakSubjectIdAsync(string keycloakSubjectId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.KeycloakSubjectId == keycloakSubjectId);
        }
    }
}
